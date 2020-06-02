using System.Linq;
using System.Web.Mvc;
using ECommerce.Web.Areas.Admin.Models;
using ECommerce.Web.Models;
using ECommerce.Web.Models.EntCarrinho;
using Microsoft.AspNet.Identity;
using System;

//using mercadopago;

namespace ECommerce.Web.Controllers
{
    public class CarrinhoController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Carrinho
        public ActionResult CarrinhoDeCompras()
        {
            var cvm = new CarrinhoViewModel { Carrinho = ObterCarrinho() };
            return View(cvm);
        }

        // Adicionar no Carrinho
        public RedirectToRouteResult AdicionarId(string id)
        {
            var produto = _db.ProdutosMontados
                .FirstOrDefault(p => p.Id == id);

            try
            {
                if (produto != null) ObterCarrinho().AdicionarItem(produto);
                TempData["MensagemCarrinhoOk"] = "Item adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["MensagemCarrinho"] = ex.Message;
            }

            return RedirectToAction("CarrinhoDeCompras");
        }

        // Adicionar no Carrinho
        public ActionResult Adicionar(ProdutoMontado produtoMontado, int tabelaPrecoId)
        {
            var produto = _db.ProdutosMontados
                .FirstOrDefault(p => p.ProdutoId == produtoMontado.ProdutoId
                    && p.ProdutoTamanhoId == produtoMontado.ProdutoTamanhoId
                    && p.ProdutoCorId == produtoMontado.ProdutoCorId);

            produto.TabelaPrecos = _db.TabelaPrecos.Where(c => c.Id == tabelaPrecoId).ToList();

            if (!produto.TabelaPrecos.Any())
                return Redirect(Request.UrlReferrer.AbsoluteUri);

            try
            {
                if (produto != null) ObterCarrinho().AdicionarItem(produto);
                TempData["MensagemCarrinhoOk"] = "Item adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["MensagemCarrinho"] = ex.Message;
            }

            return RedirectToAction("CarrinhoDeCompras");
        }

        public RedirectToRouteResult RemoverQtd(string produtoMontadoId)
        {
            var produto = _db.ProdutosMontados.FirstOrDefault(p => p.Id == produtoMontadoId);
            if (produto != null)
            {
                ObterCarrinho().RemoverQtd(produto);
            }

            return RedirectToAction("CarrinhoDeCompras");
        }

        // Remover do Carrinho
        public RedirectToRouteResult Remover(string produtoMontadoId)
        {
            var produto = _db.ProdutosMontados.FirstOrDefault(p => p.Id == produtoMontadoId);
            if (produto != null)
            {
                ObterCarrinho().RemoverItem(produto);
            }

            return RedirectToAction("CarrinhoDeCompras");
        }

        public RedirectToRouteResult CupomDesconto(string cupomDesconto)
        {
            try
            {
                ObterCarrinho().CupomPromocional(cupomDesconto);
                TempData["MensagemCarrinhoOK"] = "Cupom validado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["MensagemCarrinho"] = ex.Message;
            }

            return RedirectToAction("CarrinhoDeCompras");
        }

        // Obter Carrinho
        public Carrinho ObterCarrinho()
        {
            var userId = User.Identity.GetUserId();

            if (userId != null)
            {
                var user = _db.Users.Find(userId);
                var itens = _db.ItemCarrinhos.Where(d => d.AnonymousId == Request.AnonymousID).ToList();
                if (user == null)
                {
                    _db.ItemCarrinhos.RemoveRange(itens);
                    return new Carrinho(userId, Request.AnonymousID);
                }

                foreach (var item in itens)
                {
                    item.AnonymousId = null;
                    item.ApplicationUserId = userId;
                    _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            else
            {
                var itens = _db.ItemCarrinhos.AsNoTracking().Where(d => d.AnonymousId == Request.AnonymousID).ToList();
                foreach (var item in itens)
                {
                    if (item.Status == Status.Desalocado)
                    {
                        var local = _db.Parametros.FirstOrDefault().local_loja;
                        var movEstoque = _db.Estoques.First(c => c.ProdutoMontadoId == item.ProdutoMontadoId && c.LocalId == local);
                        movEstoque.SaldoAlocado += 1;
                        item.Status = Status.Alocado;
                        _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        _db.Entry(movEstoque).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
            }

            var carrinho = new Carrinho(userId, Request.AnonymousID);
            return carrinho;
        }

        public void VerificarCarrinho()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                var carrinho = ObterCarrinho().ItemCarrinhos;
                foreach (var car in carrinho)
                {
                    if (car.Status == Status.Alocado)
                    {
                        var local = _db.Parametros.First().local_loja;
                        var movEstoque = _db.Estoques.First(c => c.ProdutoMontadoId == car.ProdutoMontadoId && c.LocalId == local);
                        movEstoque.SaldoAlocado -= car.QuantidadeTotalItem;
                        var itemCar = _db.ItemCarrinhos.Find(car.Id);
                        itemCar.Status = Status.Desalocado;
                        _db.Entry(itemCar).State = System.Data.Entity.EntityState.Modified;
                        _db.Entry(movEstoque).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
            }
        }

        public void AnaliseCarrinho()
        {
            // Verifico itens que contém no carrinho de cliente logados com mais de 3 dias.
            var itens = _db.ItemCarrinhos.Where(c => (c.Data - DateTime.Now).TotalDays >= 3 && !string.IsNullOrEmpty(c.ApplicationUserId));

            // Verifico itens que contém no carrinho no qual foi criado de forma anonima e tem mais de 1 dia.
            var itensAnonymouns = _db.ItemCarrinhos.Where(c => (c.Data - DateTime.Now).TotalMinutes >= 5 && !string.IsNullOrEmpty(c.AnonymousId));

            foreach (var itemA in itens)
            {
                _db.ItemCarrinhos.Remove(itemA);
                if (itemA.Status == Status.Alocado)
                {
                    var local = _db.Parametros.FirstOrDefault().local_loja;
                    var estoque = _db.Estoques.FirstOrDefault(c => c.ProdutoMontadoId == itemA.ProdutoMontadoId && c.LocalId == local);
                    estoque.SaldoAlocado -= itemA.QuantidadeTotalItem;
                    _db.Entry(estoque).State = System.Data.Entity.EntityState.Modified;
                }
            }

            foreach (var itemA in itensAnonymouns)
            {
                _db.ItemCarrinhos.Remove(itemA);
                if (itemA.Status == Status.Alocado)
                {
                    var local = _db.Parametros.FirstOrDefault().local_loja;
                    var estoque = _db.Estoques.FirstOrDefault(c => c.ProdutoMontadoId == itemA.ProdutoMontadoId && c.LocalId == local);
                    estoque.SaldoAlocado -= itemA.QuantidadeTotalItem;
                    _db.Entry(estoque).State = System.Data.Entity.EntityState.Modified;
                }
            }

            _db.SaveChanges();
        }

        public PartialViewResult BotaoCarrinho()
        {
            return PartialView("_BotaoCarrinho", ObterCarrinho().ObterQuantidadeItens());
        }
    }
}