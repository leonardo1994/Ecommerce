using System.Linq;
using System.Web.Mvc;
using ECommerce.Web.Models;
using System.Collections.Generic;
using ECommerce.Web.Areas.Admin.Models;
using System.Data.Entity;
using ECommerce.Web.Areas.Admin.Controllers;
using System.Web.Script.Serialization;
using ECommerce.Web.Models.EntCarrinho;
using System;
using PagedList;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ECommerce.Web.Controllers
{
    public class ProdutoVitrineController : Controller, IController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private static SubCategoria _ultimaPesquisa = null;

        public ActionResult Index(int? pagina, string categoriaId, string subCategoriaId, string search, int? promocaoId)
        {
            int paginaTamanho = 12;
            int paginaNumero = (pagina ?? 1);

            var produtos = (!string.IsNullOrEmpty(subCategoriaId)) ?
                (from grade in _db.Produtos.AsNoTracking()
                 join tamanho in _db.ProdutoTamanhos.AsNoTracking() on grade.Id equals tamanho.ProdutoId
                 join cor in _db.ProdutoCores.AsNoTracking() on tamanho.Id equals cor.ProdutoTamanhoId
                 join imagem in _db.ProdutoImagens.AsNoTracking() on cor.Id equals imagem.ProdutoCorId
                 join produto in _db.ProdutosMontados.AsNoTracking() on grade.Id equals produto.ProdutoId
                 where produto.ProdutoCorId == cor.Id && produto.ProdutoTamanhoId == tamanho.Id && produto.Publica == 1 &&
                         grade.CategoriaId == categoriaId && grade.SubCategoriaId == subCategoriaId &&
                         (
                            promocaoId.HasValue && produto.PromocaoItems.Any(c => c.PromocaoId == promocaoId)
                            ||
                            !promocaoId.HasValue
                         )
                 select new
                 {
                     grade,
                     tamanho,
                     cor,
                     imagem
                 })
            : (from grade in _db.Produtos.AsNoTracking()
               join tamanho in _db.ProdutoTamanhos.AsNoTracking() on grade.Id equals tamanho.ProdutoId
               join cor in _db.ProdutoCores.AsNoTracking() on tamanho.Id equals cor.ProdutoTamanhoId
               join imagem in _db.ProdutoImagens.AsNoTracking() on cor.Id equals imagem.ProdutoCorId
               join produto in _db.ProdutosMontados.AsNoTracking() on grade.Id equals produto.ProdutoId
               where produto.ProdutoCorId == cor.Id && produto.ProdutoTamanhoId == tamanho.Id && produto.Publica == 1 &&
                   (
                       (!string.IsNullOrEmpty(categoriaId) && grade.CategoriaId == categoriaId)
                       ||
                       string.IsNullOrEmpty(categoriaId)
                   ) &&
                         (
                            promocaoId.HasValue && produto.PromocaoItems.Any(c => c.PromocaoId == promocaoId)
                            ||
                            !promocaoId.HasValue
                         )
               select new
               {
                   grade,
                   tamanho,
                   cor,
                   imagem
               });

            IList<Produto> listProduto = new List<Produto>();

            if (!string.IsNullOrEmpty(search))
            {
                ViewBag.Search = search;

                var arraySearch = search.Split(' ');

                // Para cada posição do array filtra dentro da lista anterior.
                for (int i = 0; i < arraySearch.Length; i++)
                {
                    var valor = arraySearch[i].Trim();

                    if (string.IsNullOrEmpty(valor.Trim()))
                        continue;

                    produtos = produtos.Where(c => c.grade.Categoria.Descricao.Contains(valor)
                    || c.grade.Codigo.Contains(valor)
                    || c.grade.Descricao.Contains(valor)
                    || c.grade.SubCategoria.Descricao.Contains(valor)
                    || c.cor.Cor.Descricao.Contains(valor)
                    || c.tamanho.Tamanho.Descricao.Contains(valor)
                    || c.cor.ProdutosMontado.Any(p => p.Id.Contains(valor)));
                }
            }

            var resultado = produtos.GroupBy(c => c.cor)
                            .GroupBy(c => c.Key.ProdutoTamanho)
                            .GroupBy(c => c.Key.Produto)
                            .OrderBy(c => c.Key.Descricao)
                            .ToPagedList(paginaNumero, paginaTamanho);

            ViewBag.HasPreviousPage = resultado.HasPreviousPage;
            ViewBag.PageCount = resultado.PageCount;
            ViewBag.PageNumber = resultado.PageNumber;
            ViewBag.HasNextPage = resultado.HasNextPage;

            foreach (var groupGrade in resultado)
            {
                var produto = groupGrade.Key;
                produto.ProdutoTamanhos.Clear();
                if (promocaoId.HasValue)
                    produto.ProdutosMontado = produto.ProdutosMontado.Where(c => c.PromocaoItems.Any(d => d.PromocaoId == promocaoId)).ToList();

                if (!string.IsNullOrEmpty(search))
                {
                    var arraySearch = search.Split(' ');
                    foreach (var item in arraySearch)
                    {
                        produto.ProdutosMontado = produto
                        .ProdutosMontado
                        .Where(c => c.Id.Contains(item) || c.Descricao.Contains(item)).ToList();
                    }
                }

                foreach (var tamanhos in groupGrade)
                {
                    var tamanho = tamanhos.Key;
                    tamanho.ProdutoCores.Clear();

                    if (promocaoId.HasValue)
                        tamanho.ProdutosMontado = tamanho.ProdutosMontado.Where(c => c.PromocaoItems.Any(d => d.PromocaoId == promocaoId)).ToList();

                    if (!string.IsNullOrEmpty(search))
                    {
                        var arraySearch = search.Split(' ');
                        foreach (var item in arraySearch)
                        {
                            produto.ProdutosMontado = produto
                            .ProdutosMontado
                            .Where(c => c.Id.Contains(item) || c.Descricao.Contains(item)).ToList();
                        }
                    }

                    produto.ProdutoTamanhos.Add(tamanho);
                    foreach (var cores in tamanhos)
                    {
                        var cor = cores.Key;
                        cor.ProdutoImagens.Clear();

                        if (promocaoId.HasValue)
                            cor.ProdutosMontado = cor.ProdutosMontado.Where(c => c.PromocaoItems.Any(d => d.PromocaoId == promocaoId)).ToList();

                        if (!string.IsNullOrEmpty(search))
                        {
                            var arraySearch = search.Split(' ');
                            foreach (var item in arraySearch)
                            {
                                produto.ProdutosMontado = produto
                                .ProdutosMontado
                                .Where(c => c.Id.Contains(item) || c.Descricao.Contains(item)).ToList();
                            }
                        }

                        tamanho.ProdutoCores.Add(cor);
                        foreach (var item in cores)
                        {
                            cor.ProdutoImagens.Add(item.imagem);
                        }
                    }
                }
                listProduto.Add(produto);
            }

            if (!string.IsNullOrEmpty(categoriaId))
            {
                var categoria = _db.Categorias.Find(categoriaId);

                ViewBag.CategoriaId = categoriaId;
                ViewBag.Icone = categoria.Icone;

                if (!string.IsNullOrEmpty(subCategoriaId))
                {
                    var subCategoria = categoria.SubCategorias.FirstOrDefault(c => c.Id == subCategoriaId);
                    ViewBag.SubCategoriaId = subCategoriaId;
                    ViewBag.SubCategoriaDescricao = subCategoria.Descricao;
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                return View("Search", listProduto);
            }

            if (promocaoId.HasValue)
            {
                var promocao = _db.Promocoes.Find(promocaoId.Value);
                ViewBag.PromocaoId = promocaoId.Value;
                ViewBag.PromocaoImagem = promocao.Imagem;
                return View("Promocao", listProduto);
            }

            if (!string.IsNullOrEmpty(categoriaId) && string.IsNullOrEmpty(subCategoriaId))
                return View("Categoria", listProduto);
            if (!string.IsNullOrEmpty(categoriaId) && !string.IsNullOrEmpty(subCategoriaId))
                return View("SubCategoria", listProduto);

            return View(listProduto);
        }

        public ActionResult MaisVendido()
        {
            var produtos = (from grade in _db.Produtos.AsNoTracking()
                            join tamanho in _db.ProdutoTamanhos.AsNoTracking() on grade.Id equals tamanho.ProdutoId
                            join cor in _db.ProdutoCores.AsNoTracking() on tamanho.Id equals cor.ProdutoTamanhoId
                            join imagem in _db.ProdutoImagens.AsNoTracking() on cor.Id equals imagem.ProdutoCorId
                            join produto in _db.ProdutosMontados.AsNoTracking() on grade.Id equals produto.ProdutoId
                            where produto.ProdutoCorId == cor.Id && produto.ProdutoTamanhoId == tamanho.Id && produto.Publica == 1
                            select new
                            {
                                grade,
                                tamanho,
                                cor,
                                imagem
                            }).GroupBy(c => c.cor).GroupBy(c => c.Key.ProdutoTamanho).GroupBy(c => c.Key.Produto).OrderByDescending(c => c.Key.QtdVendida).ToPagedList(1, 8);

            ViewBag.HasPreviousPage = produtos.HasPreviousPage;
            ViewBag.PageCount = produtos.PageCount;
            ViewBag.PageNumber = produtos.PageNumber;
            ViewBag.HasNextPage = produtos.HasNextPage;

            IList<Produto> listProduto = new List<Produto>();

            foreach (var groupGrade in produtos)
            {
                var produto = groupGrade.Key;
                produto.ProdutoTamanhos.Clear();
                foreach (var tamanhos in groupGrade)
                {
                    var tamanho = tamanhos.Key;
                    tamanho.ProdutoCores.Clear();
                    produto.ProdutoTamanhos.Add(tamanho);
                    foreach (var cores in tamanhos)
                    {
                        var cor = cores.Key;
                        cor.ProdutoImagens.Clear();
                        tamanho.ProdutoCores.Add(cor);
                        foreach (var item in cores)
                        {
                            cor.ProdutoImagens.Add(item.imagem);
                        }
                    }
                }
                listProduto.Add(produto);
            }

            return View(listProduto);
        }

        public ActionResult MaisVisitado()
        {
            var produtos = (from grade in _db.Produtos.AsNoTracking()
                            join tamanho in _db.ProdutoTamanhos.AsNoTracking() on grade.Id equals tamanho.ProdutoId
                            join cor in _db.ProdutoCores.AsNoTracking() on tamanho.Id equals cor.ProdutoTamanhoId
                            join imagem in _db.ProdutoImagens.AsNoTracking() on cor.Id equals imagem.ProdutoCorId
                            join produto in _db.ProdutosMontados.AsNoTracking() on grade.Id equals produto.ProdutoId
                            where produto.ProdutoCorId == cor.Id && produto.ProdutoTamanhoId == tamanho.Id && produto.Publica == 1
                            select new
                            {
                                grade,
                                tamanho,
                                cor,
                                imagem
                            }).GroupBy(c => c.cor).GroupBy(c => c.Key.ProdutoTamanho).GroupBy(c => c.Key.Produto).OrderByDescending(c => c.Key.QtdVisitada).ToPagedList(1, 8);

            ViewBag.HasPreviousPage = produtos.HasPreviousPage;
            ViewBag.PageCount = produtos.PageCount;
            ViewBag.PageNumber = produtos.PageNumber;
            ViewBag.HasNextPage = produtos.HasNextPage;

            IList<Produto> listProduto = new List<Produto>();

            foreach (var groupGrade in produtos)
            {
                var produto = groupGrade.Key;
                produto.ProdutoTamanhos.Clear();
                foreach (var tamanhos in groupGrade)
                {
                    var tamanho = tamanhos.Key;
                    tamanho.ProdutoCores.Clear();
                    produto.ProdutoTamanhos.Add(tamanho);
                    foreach (var cores in tamanhos)
                    {
                        var cor = cores.Key;
                        cor.ProdutoImagens.Clear();
                        tamanho.ProdutoCores.Add(cor);
                        foreach (var item in cores)
                        {
                            cor.ProdutoImagens.Add(item.imagem);
                        }
                    }
                }
                listProduto.Add(produto);
            }

            return View(listProduto);
        }

        public ActionResult Categoria(string subCategoriaId, string categoriaId)
        {
            if (categoriaId == null)
                return View("Index");
            var subcategoria = _db.SubCategorias.AsNoTracking().Where(c => c.Id == subCategoriaId && c.CategoriaId == categoriaId).Include(p => p.Produtos).FirstOrDefault();
            subcategoria.Categoria = _db.Categorias.Find(categoriaId);
            subcategoria.CategoriaId = subcategoria.Categoria.Id;
            var produtos = subcategoria.Produtos.Where(c => c.ProdutoTamanhos.Any(d => d.ProdutoCores.Any(e => e.ProdutoImagens.Any())) && c.CategoriaId == categoriaId && c.SubCategoriaId == subCategoriaId).ToList();
            subcategoria.Produtos = produtos;
            _ultimaPesquisa = subcategoria;
            return View("Categoria", subcategoria);
        }

        public List<Produto> ListaProdutoVitrine(List<Produto> produto)
        {
            var estoque = _db.Estoques.Where(p => p.ProdutoMontado.ProdutoId != null).ToList();
            var produtosVitrine = new List<Produto>();

            var montarProduto = new Produto();

            foreach (var itemProduto in produto)
            {
                montarProduto.Codigo = itemProduto.Codigo;
                montarProduto.Descricao = itemProduto.Descricao;

                foreach (var itemProdutoTamanho in itemProduto.ProdutoTamanhos)
                {
                    if (
                        !estoque.ToList()
                            .Exists(
                                p =>
                                    p.ProdutoMontado.ProdutoTamanhoId == itemProdutoTamanho.Id &&
                                    p.ProdutoMontado.Publica == 1)) continue;
                    {
                        var verificarEstoque = from i in _db.Estoques
                                               where itemProdutoTamanho.Id == i.ProdutoMontado.ProdutoTamanhoId
                                               select new { result = (i.MovJaneiro + i.MovFevereiro + i.MovMarco + i.MovAbril + i.MovMaio + i.MovJunho + i.MovJulho + i.MovAgosto + i.MovSetembro + i.MovOutubro + i.MovNovembro + i.MovDezembro) + i.MovAnterior };

                        var totalEstoque = verificarEstoque.Sum(p => p.result);
                        if (totalEstoque == 0) continue;
                        {
                            new List<ProdutoTamanho>().Add(itemProdutoTamanho);

                            var produtoCor = new List<ProdutoCor>();
                            foreach (var itemProdutoCor in itemProdutoTamanho.ProdutoCores)
                            {
                                if (
                                    !estoque.ToList()
                                        .Exists(
                                            p =>
                                                p.ProdutoMontado.ProdutoCorId == itemProdutoCor.Id &&
                                                p.ProdutoMontado.Publica == 1)) continue;
                                {
                                    var verificarEstoque2 = from i in _db.Estoques
                                                            where itemProdutoCor.Id == i.ProdutoMontado.ProdutoCorId
                                                            select new { result = (i.MovJaneiro + i.MovFevereiro + i.MovMarco + i.MovAbril + i.MovMaio + i.MovJunho + i.MovJulho + i.MovAgosto + i.MovSetembro + i.MovOutubro + i.MovNovembro + i.MovDezembro) + i.MovAnterior };

                                    var totalEstoque2 = verificarEstoque2.Sum(p => p.result);
                                    if (totalEstoque2 == 0) continue;
                                    produtoCor.Add(itemProdutoCor);
                                    var list = new List<ProdutoTamanho>();
                                    if (montarProduto.ProdutoTamanhos == null) continue;
                                    list.AddRange(montarProduto.ProdutoTamanhos);
                                    var indexProdutoTamanho = list.IndexOf(itemProdutoTamanho);
                                    montarProduto.ProdutoTamanhos.ToList()[indexProdutoTamanho].ProdutoCores = produtoCor;
                                }
                            }
                        }
                    }
                }
                if (montarProduto.ProdutoTamanhos != null)
                    produtosVitrine.Add(montarProduto);
            }
            return produtosVitrine;
        }

        public JsonResult ObterCores(int tamanhoId)
        {
            var cores = _db.ProdutoCores.AsNoTracking().Where(c => c.ProdutoTamanhoId == tamanhoId && c.ProdutoImagens.Any()).ToList();

            var query = (from p in cores
                         where p.ProdutosMontado.First().Publica == 1
                         select new
                         {
                             Cor = p.Cor.Hexadecimal,
                             Cor2 = p.Cor.HexadecimalCombinacao ?? p.Cor.Hexadecimal,
                             ImagemCor = p.Cor.Imagem,
                             ProdutoCorId = p.Id,
                             p.ProdutoTamanho.ProdutoId,
                             p.ProdutoTamanhoId,
                             publicado = p.ProdutosMontado.First().Publica
                         }).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Método responsável, em retornar imagens referente a Cor selececionada.
        /// </summary>
        /// <param name="corId"></param>
        /// <returns></returns>
        public JsonResult ObterImagem(int corId)
        {
            var estoque = new Estoque();

            var query = (from i in _db.ProdutoImagens.Where(c => c.ProdutoCorId == corId).ToList()
                         select new
                         {
                             porcent = AvaliacaoPorcentual(i.ProdutoCor.ProdutosMontado.First()).ToHtmlString(),
                             notaEstrelas = NotaEstrelas(i.ProdutoCor.ProdutosMontado.First()).ToHtmlString(),
                             codReferencia = Convert.ToInt32(i.ProdutoCor.ProdutosMontado.First().Id),
                             i.ProdutoCorId,
                             i.ProdutoCor.ProdutoTamanho.ProdutoId,
                             i.ProdutoCor.ProdutoTamanhoId,
                             i.Imagem,
                             i.Id,
                             disponibilidade = estoque.AvaliaDisponibilidade(i.ProdutoCor.ProdutosMontado.First().Id) > 0,
                             quantidadeEstoque = ((i.ProdutoCor.ProdutosMontado.First().TetoEstoque >=
                             estoque.AvaliaDisponibilidade(i.ProdutoCor.ProdutosMontado.First().Id)) ? estoque.AvaliaDisponibilidade(i.ProdutoCor.ProdutosMontado.First().Id)
                             : i.ProdutoCor.ProdutosMontado.First().TetoEstoque),
                             i.Imagem2,
                             i.TipoArquivo,
                             ProdutoMontadoId = i.ProdutoCor.ProdutosMontado.First().Id
                         }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
            //
        }

        private MvcHtmlString AvaliacaoPorcentual(ProdutoMontado produtoMontado)
        {
            if (produtoMontado.AvaliacaoProduto.Any())
            {
                var sim = produtoMontado.AvaliacaoProduto.Count(c => c.Recomendaria == Recomendaria.Sim);
                var total = produtoMontado.AvaliacaoProduto.Count;
                var porcent = (sim / total) * 100;
                return MvcHtmlString.Create("<p><h2><span class='text-success'>" + porcent + "%</span></h2> dos clientes recomendam este produto.</p>");
            }
            else
            {
                return MvcHtmlString.Create("<a href='#avaliacaoProduto' class='btn btn-success btn-sm'>Avaliação de produtos</a>");
            }

        }

        private MvcHtmlString NotaEstrelas(ProdutoMontado produtoMontado)
        {
            var nota = produtoMontado.AvaliacaoProduto.Any() ? produtoMontado.AvaliacaoProduto.Average(c => c.Nota) : 0;

            StringBuilder estrelas = new StringBuilder();

            for (int i = 0; i < 5; i++)
            {
                if (i < nota)
                {
                    estrelas.Append("<i class='glyphicon glyphicon-star' style='font-size: 15px; color:#ffd800;'></i>");
                }
                else
                {
                    estrelas.Append("<i class='glyphicon glyphicon-star' style='font-size: 15px; color:gray;'></i>");
                }

            }

            estrelas.Append("<span>(" + produtoMontado.AvaliacaoProduto.Count + ")</span>");

            return MvcHtmlString.Create(estrelas.ToString());
        }

        public JsonResult ObterValor(int ProdutoCorId)
        {
            var produto = _db.ProdutoCores.AsNoTracking().FirstOrDefault(c => c.Id == ProdutoCorId);
            if (produto == null) return Json("");
            var promocao = produto.ProdutosMontado.First().PromocaoItems.FirstOrDefault(c => c.Promocao.Ativo && c.Promocao.TipoExibicao != TipoDeExibicao.CupomDesconto);
            var tabelaPreco = produto.ProdutosMontado.First().TabelaPrecos.LastOrDefault(c => c.OptLoja != null && c.OptLoja.Value == 1);

            if (promocao != null && PromotionsController.VerificaPromocao(promocao.Promocao))
            {
                var valorPromocional = promocao.TabelaDePreco.Valor.Value;
                valorPromocional = promocao.Promocao.TipoValor == TipoValor.Porcentual ?
                    valorPromocional - ((valorPromocional / 100) * promocao.Promocao.Valor) :
                    (valorPromocional - promocao.Promocao.Valor);

                var porcentualDesconto = promocao.Promocao.TipoValor == TipoValor.Porcentual ?
                    promocao.Promocao.Valor :
                    Convert.ToInt32((valorPromocional / promocao.Promocao.Valor) * 100);

                return Json(new { ValueOrigem = promocao.TabelaDePreco.Valor.Value.ToString("C"), Value = valorPromocional.ToString("C"), promocao.ProdutoMontado.ProdutoId, isPromocao = true, desconto = porcentualDesconto, TabelaDePrecoId = promocao.TabelaDePrecoId }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Value = tabelaPreco.Valor.Value.ToString("C"), produto.ProdutosMontado.First().ProdutoId, isPromocao = false, TabelaDePrecoId = tabelaPreco.Id }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Details(int produtoId, int ProdutoCorId, int ProdutoTamanhoId)
        {
            var produto = _db.ProdutosMontados.FirstOrDefault(c => c.ProdutoId == produtoId && c.ProdutoCorId == ProdutoCorId && c.ProdutoTamanhoId == ProdutoTamanhoId);
            if (produto == null) return RedirectToAction("Index", "Home");
            produto.FichaTecnica = _db.FichaTecnica.Find(produto.FichaTecnicaId);
            var disponibilidade = new Estoque().AvaliaDisponibilidade(produto.Id);
            if (produto.TetoEstoque >= disponibilidade)
                ViewBag.QuantidadeEstoque = disponibilidade;
            else
                ViewBag.QuantidadeEstoque = produto.TetoEstoque;

            produto.QtdVisitada += 1;
            produto.Produto.QtdVisitada += 1;

            _db.Database.ExecuteSqlCommand($"Update CadProd set QtdVisitada = {produto.QtdVisitada} where cod_produto ='{produto.Id}'");
            _db.Database.ExecuteSqlCommand($"Update ProdutoGrade set QtdVisitada = {produto.Produto.QtdVisitada} where id = {produto.ProdutoId}");

            return View(produto);
        }

        public ActionResult DetailsMontadoId(string id)
        {
            var produto = _db.ProdutosMontados.FirstOrDefault(c => c.Id == id);
            var disponibilidade = new Estoque().AvaliaDisponibilidade(produto.Id);
            if (produto.TetoEstoque >= disponibilidade)
                ViewBag.QuantidadeEstoque = disponibilidade;
            else
                ViewBag.QuantidadeEstoque = produto.TetoEstoque;

            produto.QtdVisitada += 1;
            produto.Produto.QtdVisitada += 1;
            _db.Entry(produto).State = EntityState.Modified;
            _db.Entry(produto.Produto).State = EntityState.Modified;
            _db.SaveChanges();

            return View("Details", produto);
        }

        /// <summary>
        /// Método que recebe o código da promoção para exibição dos itens.
        /// </summary>
        /// <param name="disposing"></param>
        public ActionResult Promocao(int id)
        {
            var itensPromocao = _db.Promocoes.Find(id).PromocaoItems.ToList();
            return View("Promocao", itensPromocao);
        }

        /// <summary>
        /// Método responsável em realizar pesquisa dinamica na base de dados.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public PartialViewResult Search(string search, int? pagina = 1)
        {
            if (string.IsNullOrWhiteSpace(search)) return PartialView("TableSearch", new List<ViewSearch>());

            var serialize = new JavaScriptSerializer();

            // Transforma a expressão em um array.
            var arraySearch = search.Split(' ');

            var searchIndex1 = arraySearch[0].Trim();

            // Realiza um pesquisa na base pela primeira expressão do array
            var createResult = (from p in _db.ProdutosMontados
                                join pt in _db.ProdutoTamanhos on p.ProdutoTamanhoId equals pt.Id
                                join pc in _db.ProdutoCores on pt.Id equals pc.ProdutoTamanhoId
                                join pi in _db.ProdutoImagens.Take(1) on pc.Id equals pi.ProdutoCorId
                                join tp in _db.TabelaPrecos on p.Id equals tp.ProdutoMontadoId
                                join pm in _db.PromocaoItems.Where(pm => pm.Promocao.Ativo == true
                                ) on p.Id equals pm.ProdutoMontadoId into pmi
                                from pm in pmi.DefaultIfEmpty()
                                where p.Publica == 1 && p.ProdutoId != null &&
                                      (p.Produto.Descricao.Contains(searchIndex1) ||
                                      p.Id.Contains(searchIndex1) ||
                                      pt.Tamanho.Descricao.Contains(searchIndex1) ||
                                      pc.Cor.Descricao.Contains(searchIndex1) ||
                                      p.Categoria.Descricao.Contains(searchIndex1) ||
                                      p.SubCategoria.Descricao.Contains(searchIndex1))
                                select new
                                {
                                    Codigo = p.Id,
                                    Descricao = p.Produto.Descricao,
                                    Tamanho = pt.Tamanho.Descricao,
                                    Cor = pc.Cor.Descricao,
                                    Imagem = pi.Imagem,
                                    Valor = tp.Valor,
                                    ProdutoCorId = pc.Id
                                });

            // Para cada posição do array filtra dentro da lista anterior.
            for (int i = 1; i < arraySearch.Length; i++)
            {
                var valor = arraySearch[i].Trim();

                if (string.IsNullOrEmpty(valor.Trim()))
                    continue;

                createResult = createResult.Where(c => c.Codigo.Contains(valor)
                || c.Cor.Contains(valor)
                || c.Descricao.Contains(valor)
                || c.Tamanho.Contains(valor));
            }

            // passa a pesquisa para um classe modelo.
            var firstResult = createResult.Distinct().OrderBy(c => c.Descricao).ToPagedList(pagina.Value, 20);
            var result = (from p in firstResult
                          select new ViewSearch
                          {
                              Codigo = p.Codigo,
                              Cor = p.Cor,
                              Descricao = p.Descricao,
                              Imagem = p.Imagem,
                              Tamanho = p.Tamanho,
                              Valor = serialize.Deserialize<ValorPreco>(serialize.Serialize(ObterValor(p.ProdutoCorId).Data)).Value
                          });

            ViewBag.HasPreviousPage = firstResult.HasPreviousPage;
            ViewBag.PageCount = firstResult.PageCount;
            ViewBag.PageNumber = firstResult.PageNumber;
            ViewBag.HasNextPage = firstResult.HasNextPage;

            // Retorna o valor em Json.
            return PartialView("TableSearch", result);
        }

        public PartialViewResult EnviarEmail(string ProdutoMontadoId)
        {
            string email = "";
            if (HttpContext.User.Identity.IsAuthenticated)
                email = HttpContext.User.Identity.Name;
            return PartialView(new AvisoDisponibilidade() { Email = email, ProdutoMontadoId = ProdutoMontadoId });
        }

        [HttpPost]
        public JsonResult EnviarEmail(AvisoDisponibilidade aviso)
        {
            if (ModelState.IsValid)
            {
                if (_db.AvisoDisponibilidade.Any(c => c.Email == aviso.Email && c.ProdutoMontadoId == aviso.ProdutoMontadoId))
                    return Json(new Retorno() { Mensagem = "Já existe uma solicitação, aguarde nossa notificação. Obrigado !" }, JsonRequestBehavior.AllowGet);
                _db.AvisoDisponibilidade.Add(aviso);
                _db.SaveChanges();
                return Json(new Retorno() { Mensagem = "Assim que tivermos o produto disponível em nosso estoque, comunicaremos você. Obrigado !" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new Retorno() { Mensagem = "Não foi possível processar tente novamente." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AvisoDisponibilidadeTemplate()
        {
            return View("Template");
        }

        public void AvisoDisponibilidade()
        {
            var produtos = _db.AvisoDisponibilidade.ToList();
            var estoque = new Estoque();
            foreach (var item in produtos)
            {
                var result = estoque.AvaliaDisponibilidade(item.ProdutoMontadoId) > 0;
                if (result)
                {
                    // Plug in your email service here to send an email.
                    const string @from = "famaraonline@famara.com.br"; // E-mail de remetente cadastrado no painel
                    var to = item.Email;   // E-mail do destinatário
                    const string user = "famaraonline@famara.com.br"; // Usuário de autenticação do servidor SMTP
                    const string pass = "LojaVirtual2017"; // Senha de autenticação do servidor SMTP

                    WebClient wc = new WebClient();
                    wc.Encoding = System.Text.Encoding.UTF8;

                    //Obtendo o conteúdo do template
                    string sTemplate = wc.DownloadString(
                        "http://www.famaraonline.com.br/ProdutoVitrine/AvisoDisponibilidadeTemplate");

                    var subject = "Famara Online | Produto disponível " + item.ProdutoMontado.Descricao;

                    var body = subject;
                    body = sTemplate.Replace("##Produdo##", item.ProdutoMontado.Descricao);
                    body = body.Replace("##CodProduto##", item.ProdutoMontado.Id);
                    body = body.Replace("##Imagem##", @"http://www.famaraonline.com.br" + item.ProdutoMontado.ProdutoCor.ProdutoImagens.First().Imagem);

                    var msg = new MailMessage(from, to, subject, body)
                    {
                        From = new MailAddress(@from, "Farama Online - Loja Virtual"),
                        IsBodyHtml = true
                    };

                    using (var smtp = new SmtpClient("email-ssl.com.br", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(user, pass);
                        try
                        {
                            smtp.Send(msg);
                            _db.AvisoDisponibilidade.Remove(item);
                            _db.SaveChanges();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class ValorPreco
    {
        public string Value { get; set; }
        public int ProdutoId { get; set; }
        public bool isPromocao { get; set; }
    }

    public class ViewSearch
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Tamanho { get; set; }
        public string Cor { get; set; }
        public string Imagem { get; set; }
        public string Valor { get; set; }
    }

    public class Retorno
    {
        public string Mensagem { get; set; }
    }
}