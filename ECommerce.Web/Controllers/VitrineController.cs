using ECommerce.Web.Areas.Admin.Models;
using ECommerce.Web.Models;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using PagedList;

namespace ECommerce.Web.Controllers
{
    public class VitrineController : Controller
    {
        /// <summary>
        /// Conexão com banco de dados.
        /// </summary>
        private readonly ApplicationDbContext _db;

        public VitrineController()
        {
            _db = new ApplicationDbContext();
        }

        /// <summary>
        /// Carrega Vitrine de produto do site
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? pageNumber, int? totalPage)
        {
            pageNumber = (pageNumber == null) ? 1 : pageNumber.Value;
            totalPage = (totalPage == null) ? 1 : totalPage.Value;
            var grades = CarregaGrade(pageNumber.Value, totalPage.Value);

            return View(grades);
        }

        /// <summary>
        /// Processa grade de produtos.
        /// </summary>
        /// <param name="pageNumber">Especifica número da página</param>
        /// /// <param name="totalPage">Total de páginas que será retornada</param>
        /// <returns></returns>
        private IList<Produto> CarregaGrade(int pageNumber, int totalPage)
        {
            var grades = _db.Produtos.AsNoTracking().OrderBy(c => c.Descricao).ToPagedList(pageNumber, totalPage);

            var resultado = (from g in grades
                             join pt in _db.ProdutoTamanhos.AsNoTracking() on g.Id equals pt.ProdutoId
                             join pc in _db.ProdutoCores.AsNoTracking() on pt.Id equals pc.ProdutoTamanhoId
                             join pi in _db.ProdutoImagens.AsNoTracking() on pc.Id equals pi.ProdutoCorId
                             join p in _db.ProdutosMontados.AsNoTracking() on g.Id equals p.ProdutoId
                             where p.ProdutoTamanhoId == pt.Id && p.ProdutoCorId == pc.Id && p.Publica == 1
                             select new
                             {
                                 GId = g.Id,
                                 PtId = pt.Id,
                                 PcId = pc.Id,
                                 g.Descricao,
                                 Tamanho = pt.Tamanho.Descricao,
                                 Cor = pc.Cor.Descricao,
                                 pc.Cor.Hexadecimal,
                                 pc.Cor.HexadecimalCombinacao,
                                 CorImagem = pc.Cor.Imagem,
                                 pi.Imagem
                             }).OrderBy(c => c.Descricao);

            // Informações da paginção do conteúdo.

            TempData["FirstItemOnPage"] = grades.FirstItemOnPage;
            TempData["HasNextPage"] = grades.HasNextPage;
            TempData["HasPreviousPage"] = grades.HasPreviousPage;
            TempData["IsFirstPage"] = grades.IsFirstPage;
            TempData["IsLastPage"] = grades.IsLastPage;
            TempData["LastItemOnPage"] = grades.LastItemOnPage;
            TempData["PageCount"] = grades.PageCount;
            TempData["PageNumber"] = grades.PageNumber;
            TempData["PageSize"] = grades.PageSize;
            TempData["TotalItemCount"] = grades.TotalItemCount;

            // Prepara estrutura da grade.
            IList<Produto> grade = new List<Produto>();

            // Grupo de grades
            var gGrade = resultado.GroupBy(c => new { c.GId, c.Descricao });

            foreach (var g in gGrade)
            {
                var produto = new Produto()
                {
                    Id = g.Key.GId,
                    Descricao = g.Key.Descricao
                };

                // Grupo de tamanhos da grade
                var gTamanhos = g.GroupBy(c => new { c.PtId, c.Tamanho });

                // Inicializa coleção de tamanhos da grade.
                if (produto.ProdutoTamanhos == null)
                    produto.ProdutoTamanhos = new List<ProdutoTamanho>();

                foreach (var t in gTamanhos)
                {
                    var tamanho = new ProdutoTamanho()
                    {
                        Id = t.Key.PtId,
                        Tamanho = new Tamanho() { Descricao = t.Key.Tamanho }
                    };

                    // Grupo de cores.
                    var gCores = t.GroupBy(c => new { c.PcId, c.Cor, c.Hexadecimal, c.HexadecimalCombinacao, c.CorImagem });

                    // Inicializa coleção de cores do tamanho
                    if (tamanho.ProdutoCores == null)
                        tamanho.ProdutoCores = new List<ProdutoCor>();

                    foreach (var c in gCores)
                    {
                        var cor = new ProdutoCor()
                        {
                            Id = c.Key.PcId,
                            Cor = new Cor()
                            {
                                Descricao = c.Key.Cor,
                                Hexadecimal = c.Key.Hexadecimal,
                                HexadecimalCombinacao = c.Key.HexadecimalCombinacao,
                                Imagem = c.Key.CorImagem
                            },
                            ProdutoImagens = new List<ProdutoImagem>()
                            {
                                { new ProdutoImagem() { Imagem = c.First().Imagem } }
                            }
                        };
                        tamanho.ProdutoCores.Add(cor);
                    }
                    produto.ProdutoTamanhos.Add(tamanho);
                }
                grade.Add(produto);
            }
            return grade;
        }
    }
}