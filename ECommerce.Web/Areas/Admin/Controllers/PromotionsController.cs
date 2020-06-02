using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ECommerce.Web.Areas.Admin.Helpers;
using ECommerce.Web.Areas.Admin.Models;
using ECommerce.Web.Models;
using Microsoft.AspNet.Identity;

namespace ECommerce.Web.Areas.Admin.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static List<PromocaoItem> _promotionItems = new List<PromocaoItem>();

        [Authorize(Roles = "Admin")]
        // GET: Admin/Promotions
        public ActionResult Index()
        {
            _promotionItems.Clear();
            return View(_db.Promocoes.ToList());
        }

        [Authorize(Roles = "Admin")]
        // GET: Admin/Promotions/Create
        public ActionResult Create()
        {
            _promotionItems.Clear();
            LoadViewBag();
            return View(new Promocao());
        }

        // POST: Admin/Promotions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Promocao promotion, HttpPostedFileBase fileImage)
        {
            if (_db.Promocoes.Any(c => c.Codigo == promotion.Codigo))
                ModelState.AddModelError("", "Código já cadastro, considere informar outro");

            if (!ModelState.IsValid)
            {
                promotion.PromocaoItems = _promotionItems;
                LoadViewBag();
                return View("Create", promotion);
            }
            promotion.Imagem = this.SaveImage(fileImage, promotion.Imagem);
            if (string.IsNullOrEmpty(promotion.Imagem) && promotion.TipoExibicao != TipoDeExibicao.CupomDesconto)
            {
                promotion.PromocaoItems = _promotionItems;
                ModelState.AddModelError("", "Imagem não informada");
                LoadViewBag();
                return View("Create", promotion);
            }

            //Se for todos itens então eu mesmo preencho a lista automáticamente.
            if (promotion.TodosItens)
            {
                var produto = (from p in _db.ProdutosMontados.Where(c => c.Publica == 1 && c.ProdutoId.HasValue && c.ProdutoTamanhoId.HasValue && c.ProdutoId.HasValue && c.TabelaPrecos.Any())
                               select new { p.Id, TabelaPreco = p.TabelaPrecos.FirstOrDefault(c => c.OptLoja == 1) }).Distinct().ToList();
                foreach (var item in produto)
                {
                    _promotionItems.Add(new PromocaoItem()
                    {
                        ProdutoMontadoId = item.Id,
                        TabelaDePrecoId = item.TabelaPreco.Id
                    });
                }
            }

            if (promotion.PromocaoItems == null)
                promotion.PromocaoItems = new List<PromocaoItem>();

            foreach (var promotionItem in _promotionItems)
            {
                promotionItem.ProdutoMontado = null;
                promotionItem.TabelaDePreco = null;
                promotion.PromocaoItems.Add(promotionItem);
            }

            _db.Promocoes.Add(promotion);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        // GET: Admin/Promotions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoadViewBag();
            var promotion = _db.Promocoes.Find(id);
            if (!promotion.TodosItens)
                _promotionItems = promotion.PromocaoItems.ToList();
            return View(promotion);
        }

        // POST: Admin/Promotions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Promocao promotion, HttpPostedFileBase fileImage)
        {
            if (!string.IsNullOrEmpty(promotion.Codigo) && (_db.Promocoes.Any(c => c.Codigo == promotion.Codigo && c.Id != promotion.Id)))
                ModelState.AddModelError("", "Código já cadastro, considere informar outro");

            if (!ModelState.IsValid)
            {
                promotion.PromocaoItems = _promotionItems;
                LoadViewBag();
                return View(promotion);
            }
            if (fileImage != null)
                promotion.Imagem = this.SaveImage(fileImage, promotion.Imagem);
            if (string.IsNullOrEmpty(promotion.Imagem))
            {
                promotion.PromocaoItems = _promotionItems;
                LoadViewBag();
                return View(promotion);
            }

            _db.PromocaoItems.RemoveRange(_db.PromocaoItems.Where(c => c.PromocaoId == promotion.Id));

            //Se for todos itens então eu mesmo preencho a lista automáticamente.
            if (promotion.TodosItens)
            {
                var produto = (from p in _db.ProdutosMontados.Where(c => c.Publica == 1 && c.ProdutoId.HasValue && c.ProdutoTamanhoId.HasValue && c.ProdutoId.HasValue && c.TabelaPrecos.Any())
                               select new { p.Id, TabelaPreco = p.TabelaPrecos.FirstOrDefault(c => c.OptLoja == 1) }).Distinct().ToList();
                foreach (var item in produto)
                {
                    _promotionItems.Add(new PromocaoItem()
                    {
                        ProdutoMontadoId = item.Id,
                        TabelaDePrecoId = item.TabelaPreco.Id
                    });
                }
            }

            foreach (var promotionItem in _promotionItems)
            {
                promotionItem.Id = 0;
                promotionItem.Promocao = null;
                promotionItem.ProdutoMontado = null;
                promotionItem.TabelaDePreco = null;
                promotionItem.PromocaoId = promotion.Id;
                _db.PromocaoItems.Add(promotionItem);
            }

            _db.Entry(promotion).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        // GET: Admin/Promotions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promocao promotion = _db.Promocoes.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        [Authorize(Roles = "Admin")]
        // POST: Admin/Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _promotionItems.Clear();
            Promocao promotion = _db.Promocoes.Find(id);
            _db.Promocoes.Remove(promotion);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Pega a tabela de preço do produto
        /// </summary>
        /// <param name="productId">Código do item</param>
        /// <returns>Retorna formato Json a tabela de preço</returns>
        [Authorize(Roles = "Admin")]
        public JsonResult TablePricesOfProduct(string productId)
        {
            return new JsonResult()
            {
                Data = _db.TabelaPrecos.Where(tp => tp.ProdutoMontadoId == productId && tp.OptLoja.Value == 1).ToList(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// Carrega a view onde contém o grid com as informações dos itens da promoção.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult PromotionItems()
        {
            LoadViewBag();
            return PartialView(_promotionItems);
        }

        /// <summary>
        /// Inserir um item da promoção.
        /// </summary>
        /// <param name="promotionItem">Item da promoção</param>
        /// <returns>Lista atualizada dos itens da promoção</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult InsertItemPromotion(PromocaoItem promotionItem)
        {
            if (promotionItem.ProdutoMontadoId == null) return Json(new { success = false, message = "Item não informado!" });
            if (promotionItem.TabelaDePrecoId == 0) return Json(new { success = false, message = "Tabela de preço não informada!" });
            if (_promotionItems.Any(
                item => item.ProdutoMontadoId == promotionItem.ProdutoMontadoId && item.TabelaDePrecoId == promotionItem.TabelaDePrecoId))
                return Json(new { success = false, message = "Item já existe na lista!" });
            promotionItem.ProdutoMontado = _db.ProdutosMontados.Find(promotionItem.ProdutoMontadoId);
            promotionItem.TabelaDePreco = _db.TabelaPrecos.Find(promotionItem.TabelaDePrecoId);
            _promotionItems.Add(promotionItem);
            return Json(new { success = true });
        }

        /// <summary>
        /// Remove um item da promoção
        /// </summary>
        /// <param name="id">index do item na lista</param>
        /// <returns>Retorna a lista atualizada</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void DeleteItemPromotion(int id)
        {
            var item = _promotionItems[id];
            _promotionItems.Remove(item);
        }

        /// <summary>
        /// Retorna todas as tabelas de preço do item.
        /// </summary>
        /// <param name="productId">Item que será pesquisado</param>
        /// <returns>Retorna a lista da tabela de preço em formato JSON.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult GetTablePrices(string productId)
        {
            var result = _db.TabelaPrecos.Where(c => c.ProdutoMontadoId == productId && c.OptLoja.Value == 1).ToList();
            var list = new List<object>();
            foreach (var tablePrice in result)
            {
                if (tablePrice.Valor != null)
                    list.Add(new { id = tablePrice.Id, code = tablePrice.Codigo + " - " + tablePrice.Descricao + " " + Convert.ToDecimal(tablePrice.Valor.Value).ToString("C") });
            }
            return Json(list);
        }

        public PartialViewResult SlidePromotion(TipoDeExibicao tipoExibicao = 0)
        {
            var promotions = _db.Promocoes.Where(c => c.Ativo && c.TipoExibicao == tipoExibicao && c.Publicar).ToList();
            var remove = new List<Promocao>();
            // Faço um loop para remover promoções que estão fora do periodo.
            foreach (var promotion in promotions)
            {
                if (!VerificaPromocao(promotion))
                    remove.Add(promotion);
            }

            foreach (var promotion in remove)
            {
                promotions.Remove(promotion);
            }

            return PartialView(promotions);
        }

        public static bool VerificaPromocao(Promocao promotion)
        {
            if (promotion == null) return false;

            var dateInicial = new DateTime(promotion.DataInicial.Year, promotion.DataInicial.Month, promotion.DataInicial.Day,
                promotion.HoraInicial.Hour, promotion.HoraInicial.Minute, promotion.HoraInicial.Second);

            var dateFinal = new DateTime(promotion.DataFinal.Year, promotion.DataFinal.Month, promotion.DataFinal.Day,
                    promotion.HoraFinal.Hour, promotion.HoraFinal.Minute, promotion.HoraFinal.Second);

            return DateTime.Now >= dateInicial && DateTime.Now <= dateFinal;
        }

        public PartialViewResult CupomDesconto()
        {
            var cupom = _db.Promocoes.FirstOrDefault(c => c.Ativo && c.TipoExibicao == TipoDeExibicao.CupomDesconto && c.Publicar);
            if (cupom == null || !VerificaPromocao(cupom)) return PartialView(null);
            {
                if (!Request.IsAuthenticated) return PartialView(cupom);
                if (!cupom.NovosUsuarios) return PartialView(cupom);
                var userId = HttpContext.User.Identity.GetUserId();
                var temPedidos = _db.Clientes.AsNoTracking().FirstOrDefault(c => c.ApplicationUserId == userId)?.Pedidos?.Any();
                return temPedidos.HasValue ? PartialView(null) : PartialView(cupom);
            }
        }

        private void LoadViewBag()
        {
            var products = (from p in _db.ProdutosMontados
                            where p.Publica == 1 && p.Status == "A" && p.CodTipo == "PA"
                            orderby p.Descricao
                            select new { p.Id, Descricao = p.Id + " - " + p.Descricao }).ToList();
            ViewBag.ProdutoMontadoId = new SelectList(products, "Id", "Descricao");

            var tabelas = (from t in _db.TabelaPrecos.Where(c => c.Id == 0)
                           where t.OptLoja == 1
                           select new { t.Id, Descricao = t.Codigo + " - " + t.Descricao + " - " + t.Valor.Value }).ToList();

            ViewBag.TabelaDePrecoId = new SelectList(tabelas, "Id", "Descricao");
        }
    }
}