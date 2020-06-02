using System.Collections.Generic;
using System.Linq;
using ECommerce.Web.Areas.Admin.Models;
using System;
using ECommerce.Web.Areas.Admin.Controllers;

namespace ECommerce.Web.Models.EntCarrinho
{
    public class Carrinho : IDisposable
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly string _userId;
        private readonly string _userAnonymonusId;
        private readonly Estoque _estoque;

        public Carrinho(string userId, string userAnonymounsId)
        {
            _userId = userId;
            _userAnonymonusId = userAnonymounsId;
            _estoque = new Estoque();
            AtualizaCarrinho();
        }

        //Adicionar
        public void AdicionarItem(ProdutoMontado produtoMontado)
        {
            ItemCarrinho item;
            if (_userId == null)
                item = _db.ItemCarrinhos
                .FirstOrDefault(p => p.ProdutoMontado.Id == produtoMontado.Id && p.AnonymousId == _userAnonymonusId);
            else
                item = _db.ItemCarrinhos
                    .FirstOrDefault(p => p.ProdutoMontado.Id == produtoMontado.Id && p.ApplicationUserId == _userId);

            if (_estoque.AvaliaDisponibilidade(produtoMontado.Id) <= 0)
                throw new Exception("Item sem disponibilidade no estoque");

            var promocao = _db.PromocaoItems.FirstOrDefault(c => c.ProdutoMontadoId == produtoMontado.Id && c.Promocao.Ativo);

            if (promocao != null && !PromotionsController.VerificaPromocao(promocao.Promocao))
                promocao = null;

            if (item == null)
            {
                item = new ItemCarrinho
                {
                    PromocaoId = promocao?.PromocaoId,
                    Data = DateTime.Now,
                    ProdutoMontadoId = produtoMontado.Id,
                    QuantidadeTotalItem = 1,
                    ApplicationUserId = _userId,
                    AnonymousId = _userAnonymonusId,
                    TabelaPrecoId = produtoMontado.TabelaPrecos.First().Id,
                    Status = Status.Alocado
                };
                _db.ItemCarrinhos.Add(item);
            }
            else
            {
                item.QuantidadeTotalItem += 1;
                _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            var localLoja = _db.Parametros.First().local_loja;
            var movEstoque = _db.Estoques.First(c => c.ProdutoMontadoId == produtoMontado.Id && c.LocalId == localLoja);
            movEstoque.SaldoAlocado += 1;
            _db.Entry(movEstoque).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

        // Menos quantidade
        public void RemoverQtd(ProdutoMontado produtoMontado)
        {
            ItemCarrinho item;

            if (_userId == null)
                item = _db.ItemCarrinhos
                .FirstOrDefault(p => p.ProdutoMontado.Id == produtoMontado.Id && p.AnonymousId == _userAnonymonusId);
            else
                item = _db.ItemCarrinhos
                    .FirstOrDefault(p => p.ProdutoMontado.Id == produtoMontado.Id && p.ApplicationUserId == _userId);

            item.QuantidadeTotalItem -= 1;
            _db.Entry(item).State = System.Data.Entity.EntityState.Modified;

            var movEstoque = _db.Estoques.First(c => c.ProdutoMontadoId == produtoMontado.Id);
            movEstoque.SaldoAlocado -= 1;

            _db.SaveChanges();
        }

        //Remover
        public void RemoverItem(ProdutoMontado produtoMontado)
        {
            ItemCarrinho item;

            if (_userId == null)
                item = _db.ItemCarrinhos
                .FirstOrDefault(p => p.ProdutoMontado.Id == produtoMontado.Id && p.AnonymousId == _userAnonymonusId);
            else
                item = _db.ItemCarrinhos
                    .FirstOrDefault(p => p.ProdutoMontado.Id == produtoMontado.Id && p.ApplicationUserId == _userId);

            _db.ItemCarrinhos.Remove(item);

            var movEstoque = _db.Estoques.First(c => c.ProdutoMontadoId == produtoMontado.Id);
            movEstoque.SaldoAlocado -= item.QuantidadeTotalItem;
            _db.Entry(movEstoque).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

        public void CupomPromocional(string cupom)
        {
            if (_userId != null)
                if (_db.Pedidos.Where(c => c.Promocao.Codigo == cupom && c.Cliente.ApplicationUserId == _userId).Any())
                    throw new Exception("Este cupom não é mais válido");

            var cupomDesconto = _db.Promocoes.FirstOrDefault(c => c.Ativo && c.Codigo == cupom);

            if (cupomDesconto == null)
                throw new Exception("Cupom não encontrada!");

            if (!PromotionsController.VerificaPromocao(cupomDesconto))
                throw new Exception("Este cupom não é mais válido");

            IQueryable<ItemCarrinho> itens;

            if (_userId == null)
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.AnonymousId == _userAnonymonusId);
            else
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.ApplicationUserId == _userId);

            foreach (var itemCarrinho in itens)
            {
                if (cupomDesconto.TodosItens)
                {
                    itemCarrinho.PromocaoId = cupomDesconto.Id;
                    _db.Entry(itemCarrinho).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    bool control = false;

                    var itemP = cupomDesconto.PromocaoItems.FirstOrDefault(c => c.ProdutoMontadoId == itemCarrinho.ProdutoMontadoId);
                    if (itemP != null)
                    {
                        control = true;
                        itemCarrinho.PromocaoId = cupomDesconto.Id;
                        itemCarrinho.TabelaPrecoId = itemP.TabelaDePrecoId;
                        itemCarrinho.TabelaPreco = itemP.TabelaDePreco;
                        _db.Entry(itemCarrinho).State = System.Data.Entity.EntityState.Modified;
                    }

                    if (!control) throw new Exception("Os itens no carrinho não são válidos para este cupom");
                }
            }

            _db.SaveChanges();
        }

        //Obter Valor Total
        public double ObterValorTotal()
        {
            IList<ItemCarrinho> itens;

            if (_userId == null)
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.AnonymousId == _userAnonymonusId).ToList();
            else
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.ApplicationUserId == _userId).ToList();

            if (!itens.Any()) return 0;

            return itens.Sum(e => e.ValorTotalItem);
        }

        public int ObterQuantidadeItens()
        {
            IQueryable<ItemCarrinho> itens;

            if (_userId == null)
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.AnonymousId == _userAnonymonusId);
            else
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.ApplicationUserId == _userId);

            var qtd = 0;
            if (itens.Any())
                qtd = itens.Sum(c => c.QuantidadeTotalItem);

            return qtd;
        }

        //Limpar Carrinho
        public void LimparCarrinho()
        {
            IQueryable<ItemCarrinho> itens;

            if (_userId == null)
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.AnonymousId == _userAnonymonusId);
            else
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.ApplicationUserId == _userId);

            _db
                .ItemCarrinhos
                .RemoveRange(itens);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        //Itens Carrinho
        public IEnumerable<ItemCarrinho> ItemCarrinhos => GetItens();

        public void AtualizaCarrinho()
        {
            if (_userId != null)
            {
                var user = _db.Users.Find(_userId);
                var itens = _db.ItemCarrinhos.Where(d => d.AnonymousId == _userAnonymonusId).ToList();
                if (user == null)
                {
                    _db.ItemCarrinhos.RemoveRange(itens);
                }

                foreach (var item in itens)
                {
                    item.AnonymousId = null;
                    item.ApplicationUserId = _userId;
                    _db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            else
            {
                var itens = _db.ItemCarrinhos.AsNoTracking().Where(d => d.AnonymousId == _userAnonymonusId).ToList();
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
        }

        private IEnumerable<ItemCarrinho> GetItens()
        {
            IQueryable<ItemCarrinho> itens;

            if (_userId == null)
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.AnonymousId == _userAnonymonusId);
            else
                itens = _db.ItemCarrinhos.Include("ProdutoMontado")
                .Where(c => c.ApplicationUserId == _userId);

            return itens.ToList();
        }
    }
}