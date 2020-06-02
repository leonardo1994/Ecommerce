using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ECommerce.Web.Areas.Admin.Models;
using ECommerce.Web.Models;
using Microsoft.AspNet.Identity;
using System;

namespace ECommerce.Web.Controllers
{
    public class AvaliacaoProdutosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AvaliacaoProdutos
        public ActionResult Index(string produtoMontadoId)
        {
            var avaliacaoProduto = (from avaliacao in db.AvaliacaoProduto.AsNoTracking()
                                    join cliente in db.Clientes.AsNoTracking() on avaliacao.ApplicationUserId equals cliente.ApplicationUserId
                                    where avaliacao.ProdutoMontadoId == produtoMontadoId && cliente.ApplicationUserId != null && avaliacao.Publica == Recomendaria.Sim
                                    select new AvaliacaoViewModel()
                                    {
                                        Titulo = avaliacao.Titulo,
                                        Cliente = cliente.Nome,
                                        Data = avaliacao.Data,
                                        Nota = avaliacao.Nota,
                                        Opniao = avaliacao.Opniao,
                                        Recomendaria = avaliacao.Recomendaria,
                                        Tipo = avaliacao.Tipo
                                    });
            return View(avaliacaoProduto.AsNoTracking().ToList());
        }

        // GET: AvaliacaoProdutos/Create
        public ActionResult Create(string produtoMotadoId, string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            var userId = User.Identity.GetUserId();
            var result = db.AvaliacaoProduto.AsNoTracking().Any(c => c.ProdutoMontadoId == produtoMotadoId && c.ApplicationUserId == userId);
            if (result)
                return View("JaAvaliou");
            return View(new AvaliacaoProduto
            {
                ApplicationUserId = userId,
                ProdutoMontadoId = produtoMotadoId,
                Data = DateTime.Now
            });
        }

        // POST: AvaliacaoProdutos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AvaliacaoProduto avaliacaoProduto)
        {
            var produto = db.ProdutosMontados.AsNoTracking().FirstOrDefault(c => c.Id == avaliacaoProduto.ProdutoMontadoId);

            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                avaliacaoProduto.ApplicationUserId = userId;
                db.AvaliacaoProduto.Add(avaliacaoProduto);
                db.SaveChanges();
                return RedirectToAction("Details", "ProdutoVitrine", new { produtoId = produto.ProdutoId, ProdutoCorId = produto.ProdutoCorId, ProdutoTamanhoId = produto.ProdutoTamanhoId });
            }
            return RedirectToAction("Details", "ProdutoVitrine", new { produtoId = produto.ProdutoId, ProdutoCorId = produto.ProdutoCorId, ProdutoTamanhoId = produto.ProdutoTamanhoId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class AvaliacaoViewModel
    {
        public string Titulo { get; set; }
        public DateTime Data { get; set; }
        public string Cliente { get; set; }
        public int Nota { get; set; }
        public Recomendaria Recomendaria { get; set; }
        public string Opniao { get; set; }
        public Tipo Tipo { get; set; }
        public Recomendaria Publica { get; set; }
    }
}
