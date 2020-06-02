using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ECommerce.Web.Areas.Admin.Models;
using ECommerce.Web.Models;
using ECommerce.Web.Areas.Admin.Controllers;

namespace ECommerce.Web.HtmlHelpers
{
    /// <summary>
    /// Class responsável em montar o produto da vitrine. 
    /// </summary>
    public class ProdutoHelpers
    {
        private readonly StringBuilder _resultado = new StringBuilder();
        private TagBuilder _tagDivPanel;
        private TagBuilder _tagDivHeading;
        private TagBuilder _tagDivPanelBody;
        private TagBuilder _tagDivPanelFooter;
        private TagBuilder _tagDivDescricao;
        private TagBuilder _tagDivValor;
        private TagBuilder _tagDivTipos;
        private TagBuilder _tagDivCores;
        private TagBuilder _tagDivImagem;
        private TagBuilder _tagDivFormulario;
        private TagBuilder _tagDivButton;
        private TagBuilder _tagImg;
        private TagBuilder _tagCor;
        private TagBuilder _tagDescricao;
        private TagBuilder _tagValor;
        private TagBuilder _tagTipos;
        private TagBuilder _tagFormulario;
        private TagBuilder _tagInputProdutoId;
        private TagBuilder _tagInputProdutoTamanhoId;
        private TagBuilder _tagInputProdutoCorId;
        private TagBuilder _tagInputTabelaPrecoId;
        private TagBuilder _tagButton;
        private ApplicationDbContext _db;
        private Estoque _estoque = new Estoque();

        /// <summary>
        /// Monta o panel completo do produto
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        public MvcHtmlString PanelProdutoFor(Produto produto)
        {
            // Panel
            _tagDivPanel = new TagBuilder("div");
            _tagDivPanel.AddCssClass("panel");
            _tagDivPanel.AddCssClass(Convert.ToString(produto.Id));
            _tagDivPanel.MergeAttribute("Id", Convert.ToString(produto.Id));
            _tagDivPanel.MergeAttribute("control-value", Convert.ToString(produto.Id));

            // Panel Heading
            _tagDivHeading = new TagBuilder("div");
            _tagDivHeading.AddCssClass("panel-heading");
            _tagDivHeading.InnerHtml = produto.Descricao;

            // Panel Body
            _tagDivPanelBody = new TagBuilder("div");
            _tagDivPanelBody.AddCssClass("panel-body");

            // Panel Footer
            _tagDivPanelFooter = new TagBuilder("div");
            _tagDivPanelFooter.AddCssClass("panel-footer");

            // Descrição do Produto
            _tagDivDescricao = new TagBuilder("div");
            _tagDivDescricao.MergeAttribute("Id", "div-descricao-produto");
            _tagDescricao = new TagBuilder("label");
            _tagDescricao.MergeAttribute("Id", "label-descricao-produto");
            _tagDescricao.InnerHtml = produto.ProdutosMontado.FirstOrDefault(C => C.Publica == 1).Descricao1 == "" ? produto.Descricao : produto.ProdutosMontado.FirstOrDefault().Descricao1;
            var tagBr = new TagBuilder("br");
            _tagDivDescricao.InnerHtml = _tagDescricao.ToString();
            _tagDivDescricao.InnerHtml += tagBr.ToString();
            //_tagDivPanelFooter.InnerHtml = _tagDivDescricao.ToString();

            // Valor do Produto
            _tagDivValor = new TagBuilder("div");
            _tagDivValor.MergeAttribute("Id", "div-valor-produto");
            var _tagValorOrigem = new TagBuilder("label");
            _tagValorOrigem.MergeAttribute("Id", "origemlabel-valor-produto" + produto.Id);
            _tagValor = new TagBuilder("label");
            _tagValor.MergeAttribute("Id", "label-valor-produto" + produto.Id);
            _tagValor.AddCssClass("valor-produto");
            _tagValorOrigem.AddCssClass("text-muted valor-origem");

            produto.ProdutosMontado = produto.ProdutosMontado
                .Where(c => c.ProdutoTamanhoId.HasValue
                && c.ProdutoCorId.HasValue
                && c.ProdutoId.HasValue
                && c.ProdutoTamanho.ProdutoCores.Any() 
                && c.ProdutoCor.ProdutoImagens.Any()).ToList();

            var promocaoItem = produto
                .ProdutosMontado.FirstOrDefault(c => c.Publica == 1)
                .PromocaoItems
                .FirstOrDefault(c => c.Promocao.Ativo && c.Promocao.TipoExibicao != TipoDeExibicao.CupomDesconto);

            var valorAtual = produto
                .ProdutosMontado
                .First(c => c.Publica == 1)
                .TabelaPrecos
                .LastOrDefault(c => c.OptLoja == 1).Valor.Value;

            if (promocaoItem != null && PromotionsController.VerificaPromocao(promocaoItem.Promocao))
            {
                var valorPromocional = promocaoItem.TabelaDePreco.Valor.Value;
                valorPromocional = promocaoItem.Promocao.TipoValor == TipoValor.Porcentual ?
                    valorPromocional - ((valorPromocional / 100) * promocaoItem.Promocao.Valor) :
                    (valorPromocional - promocaoItem.Promocao.Valor);

                var desconto = promocaoItem.Promocao.TipoValor == TipoValor.Porcentual ?
                    promocaoItem.Promocao.Valor :
                    Convert.ToInt32((valorPromocional / promocaoItem.Promocao.Valor) * 100);

                _tagValorOrigem.SetInnerText("De: " + promocaoItem.TabelaDePreco.Valor.Value.ToString("C"));

                _tagValor.SetInnerText("Por: " + valorPromocional.ToString("C"));
                _tagDivPanelBody.InnerHtml = "<span class= 'valor-desconto text-center' title='Promoção válida apenas para tamanho e cor selecionado!'>" + desconto + "% <br/>Off</span>";
            }
            else
            {
                _tagValor.InnerHtml = valorAtual.ToString("C");
            }

            _tagDivValor.InnerHtml += _tagValorOrigem + "<br/>";
            _tagDivValor.InnerHtml += _tagValor.ToString();
            _tagDivPanelFooter.InnerHtml += _tagDivValor;

            var disponibilidade = _estoque.AvaliaDisponibilidade(produto.ProdutoTamanhos.First().ProdutoCores.First().ProdutosMontado.First().Id) > 0;

            if (disponibilidade)
            {
                //Formulário para envio ao carrinho
                Formulario(produto, "ProdutoVitrine", "Details", "Comprar", "btn-success");
            }
            else
            {
                //Formulário para envio de e-mail
                Formulario(produto, "ProdutoVitrine", "Details", "Produto indisponível avise-me !", "btn-warning open-modal", disponibilidade: true);
            }

            // Tamanhos do produto
            _tagDivPanelFooter.InnerHtml += Tipos(produto, "", "", 0, 0, false);


            // Cores do Produto
            _tagDivPanelFooter.InnerHtml += _tagDivCores == null ? "" : _tagDivCores.ToString() ?? "";

            _tagDivPanelFooter.InnerHtml += _tagDivFormulario.ToString();

            _tagDivPanel.InnerHtml += _tagDivHeading.ToString();
            _tagDivPanel.InnerHtml += _tagDivPanelBody.ToString();
            _tagDivPanel.InnerHtml += _tagDivPanelFooter.ToString();

            _resultado.Append(_tagDivPanel);
            return MvcHtmlString.Create(Convert.ToString(_resultado));
        }

        /// <summary>
        /// Método responsável de devolver Nome do Produto 
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="cssClass"></param>
        /// <param name="idElemento"></param>
        /// <param name="nameElemento"></param>
        /// <returns></returns>
        public MvcHtmlString NomeProduto(Produto produto, string cssClass, string idElemento, string nameElemento)
        {
            TagBuilder elementoNomeProduto = new TagBuilder("label");
            elementoNomeProduto.MergeAttribute("Id", idElemento);
            elementoNomeProduto.MergeAttribute("name", nameElemento);
            elementoNomeProduto.MergeAttribute("control-produto-Id", produto.Id.ToString());
            elementoNomeProduto.AddCssClass(cssClass);
            elementoNomeProduto.SetInnerText(produto.Descricao);

            return MvcHtmlString.Create(elementoNomeProduto.ToString());
        }

        /// <summary>
        /// Método responsáve de devolver Descrição do Produto
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="cssClass"></param>
        /// <param name="idElemento"></param>
        /// <param name="nameElemento"></param>
        /// <returns></returns>
        public MvcHtmlString DescricaoProduto(Produto produto, string cssClass, string idElemento, string nameElemento)
        {
            TagBuilder elementoDescricaoProduto = new TagBuilder("label");
            elementoDescricaoProduto.MergeAttribute("Id", idElemento);
            elementoDescricaoProduto.MergeAttribute("name", nameElemento);
            elementoDescricaoProduto.MergeAttribute("control-produto-Id", produto.Id.ToString());
            elementoDescricaoProduto.AddCssClass(cssClass);
            elementoDescricaoProduto.SetInnerText(produto.Descricao);

            return MvcHtmlString.Create(elementoDescricaoProduto.ToString());
        }

        /// <summary>
        /// Método responsáve de devolver Valor do Produto
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="cssClass"></param>
        /// <param name="idElemento"></param>
        /// <param name="nameElemento"></param>
        /// <returns></returns>
        public MvcHtmlString ValorProduto(Produto produto, string cssClass, string idElemento, string nameElemento, string produtoMontadoId)
        {
            TagBuilder elementoValorProduto = new TagBuilder("label");
            elementoValorProduto.MergeAttribute("Id", idElemento + produto.Id);
            elementoValorProduto.MergeAttribute("name", nameElemento);
            elementoValorProduto.MergeAttribute("control-produto-Id", produto.Id.ToString());
            elementoValorProduto.AddCssClass("valor-" + produto.Id);
            elementoValorProduto.AddCssClass(cssClass);

            using (_db = new ApplicationDbContext())
            {
                var promocao = _db.PromocaoItems.FirstOrDefault(c => c.ProdutoMontadoId == produtoMontadoId && c.Promocao.Ativo);

                if (promocao != null && PromotionsController.VerificaPromocao(promocao.Promocao))
                {
                    var valorPromocional = promocao.TabelaDePreco.Valor.Value;
                    valorPromocional = promocao.Promocao.TipoValor == TipoValor.Porcentual ?
                        valorPromocional - ((valorPromocional / 100) * promocao.Promocao.Valor) :
                        (valorPromocional - promocao.Promocao.Valor);

                    elementoValorProduto.SetInnerText("Por: " + valorPromocional.ToString("C"));
                }
                else
                {
                    elementoValorProduto.SetInnerText(produto.ProdutosMontado.First(c => c.Id == produtoMontadoId).TabelaPrecos.LastOrDefault(c => c.OptLoja == 1).Valor.Value.ToString("C"));
                }
            }

            return MvcHtmlString.Create(elementoValorProduto.ToString());
        }

        /// <summary>
        /// Método responsável em devolver todos os Tipos do Produto
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="nomeElemento"></param>
        /// <param name="cssClass"></param>
        /// <param name="tipoId"></param>
        /// <param name="corId"></param>
        /// <param name="apenasTipos"></param>
        /// <returns></returns>
        public MvcHtmlString Tipos(Produto produto, string nomeElemento, string cssClass, int tipoId, int corId, bool apenasTipos)
        {
            IList<ProdutoTamanho> produtoTamanho = produto.ProdutoTamanhos.Where(c => c.ProdutoCores.Any(s => s.ProdutoImagens.Any())).ToList();

            IList<ProdutoTamanho> pT = new List<ProdutoTamanho>();

            foreach (var item in produtoTamanho)
            {
                IList<ProdutoCor> produtoCores = new List<ProdutoCor>();
                foreach (var item2 in item.ProdutoCores)
                    if (item2.ProdutoImagens.Any())
                        produtoCores.Add(item2);
                item.ProdutoCores = produtoCores;
                if (item.ProdutoCores.Any())
                    pT.Add(item);
            }

            produto.ProdutoTamanhos = pT;

            _tagDivTipos = new TagBuilder("div");
            _tagDivTipos.MergeAttribute("Id", produto.Id.ToString());
            _tagDivTipos.AddCssClass("tipos " + produto.Id);
            _tagDivTipos.MergeAttribute("control-produto-Id", produto.Id.ToString());

            var tamanhos = produto.ProdutoTamanhos.ToList();

            if (tamanhos.Count == 0)
            {
                if (produto.ProdutoTamanhos.Any())
                {
                    Cores(produto.ProdutoTamanhos.First(), null, null, 0, true);
                }
                return MvcHtmlString.Create(_tagDivTipos.ToString());
            }

            foreach (ProdutoTamanho tiposProduto in tamanhos)
            {
                _tagTipos = new TagBuilder("span");
                _tagTipos.MergeAttribute("Id", Convert.ToString(tiposProduto.Id));
                _tagTipos.MergeAttribute("name", tiposProduto.Tamanho.Descricao);
                _tagTipos.AddCssClass("tipo");
                _tagTipos.MergeAttribute("control-value", Convert.ToString(tiposProduto.Id));
                _tagTipos.MergeAttribute("control-tipo-produto-Id", Convert.ToString(tiposProduto.Id));
                _tagTipos.AddCssClass(cssClass);
                _tagTipos.InnerHtml = tiposProduto.Tamanho.Descricao;

                if (tiposProduto.Id == tipoId && tipoId != 0)
                {
                    Cores(tiposProduto, "", "", corId, true);

                    _tagTipos.AddCssClass("Selected");
                    _tagTipos.MergeAttribute("control-active", "true");
                    _tagTipos.AddCssClass("btn btn-sm btn-primary");
                }
                else
                {
                    var firstOrDefault = tamanhos.FirstOrDefault();
                    if (firstOrDefault != null && firstOrDefault.Id == tiposProduto.Id && tipoId == 0)
                    {
                        Cores(tiposProduto, "", "", corId, true);

                        _tagTipos.AddCssClass("Selected");
                        _tagTipos.MergeAttribute("control-active", "true");
                        _tagTipos.AddCssClass("btn btn-sm btn-primary");
                    }
                    else
                    {
                        _tagTipos.MergeAttribute("control-active", "false");
                        _tagTipos.AddCssClass("btn btn-sm btn-default");
                    }
                }

                _tagDivTipos.InnerHtml += Convert.ToString(_tagTipos);
            }
            return MvcHtmlString.Create(_tagDivTipos.ToString());
        }

        /// <summary>
        /// Método responsável em devolver todas as cores referente ao Tipo do produto
        /// </summary>
        /// <param name="produtoTipo"></param>
        /// <param name="nameElemento"></param>
        /// <param name="cssClass"></param>
        /// <param name="corId"></param>
        /// <param name="imagens"></param>
        /// <returns></returns>
        public MvcHtmlString Cores(ProdutoTamanho produtoTipo, string nameElemento, string cssClass, int corId, bool imagens)
        {
            var cores = produtoTipo.ProdutoCores.Where(c => c.ProdutoImagens.Any());
            produtoTipo.ProdutoCores = cores.ToList();

            if (corId == 0)
            {
                _tagDivCores = new TagBuilder("div");
                _tagDivCores.InnerHtml += _tagCor;
                _tagDivPanelBody.InnerHtml += Imagem(0, 1, produtoTipo.ProdutoCores.First()).ToHtmlString();
            }

            _tagDivCores = new TagBuilder("div");
            _tagDivCores.AddCssClass(cssClass);
            _tagDivCores.MergeAttribute("Id", produtoTipo?.Produto?.Id.ToString());
            _tagDivCores.AddCssClass("cores");
            _tagDivCores.MergeAttribute("name", nameElemento);

            var produtoCores = produtoTipo.ProdutoCores.ToList();

            var result = produtoCores;

            foreach (var produtoCor in result)
            {
                _tagCor = new TagBuilder("span");
                _tagCor.MergeAttribute("Id", produtoCor.Id.ToString());
                _tagCor.MergeAttribute("control-value", Convert.ToString(produtoCor.Id));
                if (result.IndexOf(produtoCor) == 0)
                {
                    var backgroundCor = "";

                    if (string.IsNullOrEmpty(produtoCor.Cor.Imagem))
                    {
                        backgroundCor = "linear-gradient(to left," + produtoCor.Cor.Hexadecimal + "," + (produtoCor.Cor.HexadecimalCombinacao ?? produtoCor.Cor.Hexadecimal) + ")";
                    }
                    else
                    {
                        backgroundCor = "url('" + produtoCor.Cor.Imagem + "'); background-size: 100% 100%;";
                    }

                    _tagCor.MergeAttribute("style", "background: " + backgroundCor);

                    if (corId == 0)
                        _tagCor.AddCssClass("selected");
                    else if (produtoCor.Id == corId)
                        _tagCor.AddCssClass("selected");

                    _tagCor.AddCssClass("btn btn-default btn-sm");
                    _tagCor.MergeAttribute("control-active", "true");
                    _tagCor.MergeAttribute("onclick", "Cores(" + Convert.ToString(produtoCor.Id) + ");");

                    // Imagem do Produto
                    if (corId != 0 && imagens)
                    {
                        if (_tagDivPanelBody == null)
                            _tagDivPanelBody = new TagBuilder("div");
                        _tagDivPanelBody.InnerHtml += Imagem(0, 1, produtoCor).ToHtmlString();
                    }

                }
                else
                {
                    if (produtoCor.Id == corId)
                        _tagCor.AddCssClass("selected");

                    _tagCor.AddCssClass("btn btn-default btn-sm");

                    var backgroundCor = "";

                    if (string.IsNullOrEmpty(produtoCor.Cor.Imagem))
                    {
                        backgroundCor = "linear-gradient(to left," + produtoCor.Cor.Hexadecimal + "," + (produtoCor.Cor.HexadecimalCombinacao ?? produtoCor.Cor.Hexadecimal) + ")";
                    }
                    else
                    {
                        backgroundCor = "url(" + produtoCor.Cor.Imagem + "); background-size: 100% 100%;";
                    }

                    _tagCor.MergeAttribute("style", "background: " + backgroundCor);
                    _tagCor.MergeAttribute("control-active", "false");
                    _tagCor.MergeAttribute("onclick", "Cores(" + Convert.ToString(produtoCor.Id) + ");");
                }
                _tagDivCores.InnerHtml += _tagCor;
            }

            return MvcHtmlString.Create(_tagDivCores.ToString());
        }

        /// <summary>
        /// Retorna as imagens de acordo com a cor passada.
        /// </summary>
        /// <param name="produtoCorId"></param>
        /// <param name="quantIdadeImagem"></param>
        /// <param name="produtoCor"></param>
        /// <returns></returns>
        public MvcHtmlString Imagem(int produtoCorId, int quantIdadeImagem, ProdutoCor produtoCor)
        {
            _tagDivImagem = new TagBuilder("div");

            ICollection<ProdutoImagem> imagem;
            if (quantIdadeImagem == 0)
            {
                using (_db = new ApplicationDbContext())
                {
                    imagem = produtoCorId != 0 ? _db.ProdutoImagens.AsNoTracking().Where(i => i.ProdutoCorId == produtoCorId).ToList() : produtoCor.ProdutoImagens.ToList();
                }
            }
            else
            {
                imagem = produtoCorId != 0 ? _db.ProdutoImagens.AsNoTracking().Where(i => i.ProdutoCorId == produtoCorId).Take(quantIdadeImagem).ToList() : produtoCor.ProdutoImagens.Take(quantIdadeImagem).ToList();
            }

            foreach (var item in imagem)
            {
                _tagImg = new TagBuilder("img");
                _tagImg.MergeAttribute("Id", Convert.ToString(item.Id));
                _tagImg.AddCssClass(produtoCor == null
                    ? Convert.ToString(produtoCorId)
                    : Convert.ToString(produtoCor.ProdutoTamanho.ProdutoId));
                _tagImg.AddCssClass("img-responsive img-secund");
                _tagImg.MergeAttribute("src", item.TipoArquivo == TipoArquivo.Imagem ? item.Imagem : "/img/no-video-img.png");
                _tagImg.MergeAttribute("alt", "");
                _tagImg.MergeAttribute("tipo", item.TipoArquivo.ToString());
                _tagImg.MergeAttribute("url", item.Imagem);
                _tagImg.MergeAttribute("src-other", item.Imagem2);
                if (produtoCor != null)
                    _tagImg.MergeAttribute("onClick", "DispararBotao(" + produtoCor.ProdutoTamanho.ProdutoId + ")");
                _tagDivImagem.InnerHtml += _tagImg.ToString();
            }

            return MvcHtmlString.Create(_tagDivImagem.ToString());
        }

        /// <summary>
        /// Monta formulário com botão de compra.
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="descricaoButton"></param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public MvcHtmlString Formulario(Produto produto, string controller, string action, string descricaoButton, string cssClass, int produtoTamanhoId = 0, int produtoCodId = 0, bool disponibilidade = false)
        {
            IList<ProdutoTamanho> produtoTamanho = produto.ProdutoTamanhos.Where(c => c.ProdutoCores.Any(s => s.ProdutoImagens.Any())).ToList();

            foreach (var item in produtoTamanho)
            {
                IList<ProdutoCor> produtoCores = new List<ProdutoCor>();
                foreach (var item2 in item.ProdutoCores)
                {
                    if (item2.ProdutoImagens.Any())
                        produtoCores.Add(item2);
                }
                item.ProdutoCores = produtoCores;
            }

            produto.ProdutoTamanhos = produtoTamanho;

            var tamanhoId = produtoTamanhoId == 0 ? produto.ProdutoTamanhos.FirstOrDefault().Id : produtoTamanhoId;
            var corId = produtoCodId == 0 ? produto.ProdutoTamanhos.FirstOrDefault().ProdutoCores.FirstOrDefault().Id : produtoCodId;
            var produtoMontado = produto.ProdutosMontado.FirstOrDefault(c => c.ProdutoTamanhoId == tamanhoId && c.ProdutoCorId == corId);
            var tabelaPromocao = produtoMontado.PromocaoItems.FirstOrDefault(c => c.Promocao.Ativo)?.TabelaDePrecoId;
            var tabelaPrecoId = tabelaPromocao ?? produtoMontado.TabelaPrecos.LastOrDefault().Id;

            _tagDivFormulario = new TagBuilder("div");
            _tagDivFormulario.AddCssClass(produto.Id.ToString());
            _tagDivFormulario.MergeAttribute("Id", "div-form-produto");
            _tagFormulario = new TagBuilder("form");
            _tagFormulario.MergeAttribute("Id", Convert.ToString(produto.Id));
            _tagFormulario.MergeAttribute("method", "get");
            _tagFormulario.MergeAttribute("action", "/" + controller + "/" + action);

            _tagInputProdutoId = new TagBuilder("input");
            _tagInputProdutoId.MergeAttribute("type", "hIdden");
            _tagInputProdutoId.MergeAttribute("name", "ProdutoId");
            _tagInputProdutoId.MergeAttribute("Id", "ProdutoId");
            _tagInputProdutoId.MergeAttribute("value", Convert.ToString(produto.Id));
            _tagFormulario.InnerHtml += _tagInputProdutoId.ToString();

            _tagInputProdutoTamanhoId = new TagBuilder("input");
            _tagInputProdutoTamanhoId.MergeAttribute("name", "ProdutoTamanhoId");
            _tagInputProdutoTamanhoId.MergeAttribute("Id", "ProdutoTamanhoId");
            _tagInputProdutoTamanhoId.MergeAttribute("value", Convert.ToString(tamanhoId));
            _tagInputProdutoTamanhoId.MergeAttribute("type", "hIdden");
            _tagFormulario.InnerHtml += _tagInputProdutoTamanhoId.ToString();

            _tagInputProdutoCorId = new TagBuilder("input");
            _tagInputProdutoCorId.MergeAttribute("name", "ProdutoCorId");
            _tagInputProdutoCorId.MergeAttribute("Id", "ProdutoCorId");
            _tagInputProdutoCorId.MergeAttribute("type", "hIdden");
            _tagInputProdutoCorId.MergeAttribute("value", Convert.ToString(corId));
            _tagFormulario.InnerHtml += _tagInputProdutoCorId.ToString();

            _tagInputTabelaPrecoId = new TagBuilder("input");
            _tagInputTabelaPrecoId.MergeAttribute("name", "tabelaPrecoId");
            _tagInputTabelaPrecoId.MergeAttribute("Id", "tabelaPrecoId");
            _tagInputTabelaPrecoId.MergeAttribute("type", "hIdden");
            _tagInputTabelaPrecoId.MergeAttribute("value", Convert.ToString(tabelaPrecoId));
            _tagFormulario.InnerHtml += _tagInputTabelaPrecoId.ToString();

            if (!disponibilidade)
            {
                // Button de compra
                _tagDivButton = new TagBuilder("div");
                _tagDivButton.MergeAttribute("Id", "div-button-comprar" + produto.Id);
                _tagButton = new TagBuilder("input");
                _tagButton.MergeAttribute("Id", "btn-comprar");
                _tagButton.MergeAttribute("name", produto.Id.ToString());
                _tagButton.MergeAttribute("value", descricaoButton);
                _tagButton.AddCssClass("btn");
                _tagButton.AddCssClass(cssClass);
                _tagButton.MergeAttribute("type", "submit");
                _tagButton.MergeAttribute("onClick", "DisplayWait();");
                _tagDivButton.InnerHtml = _tagButton.ToString();
                _tagFormulario.InnerHtml += _tagDivButton.ToString();
                _tagDivFormulario.InnerHtml += _tagFormulario.ToString();

                return MvcHtmlString.Create(_tagDivFormulario.ToString());
            }
            else
            {
                // Button de compra
                _tagDivButton = new TagBuilder("div");
                _tagDivButton.MergeAttribute("Id", "div-button-comprar" + produto.Id);
                _tagButton = new TagBuilder("a");
                _tagButton.MergeAttribute("Id", "btn-comprar");
                _tagButton.MergeAttribute("name", produto.Id.ToString());
                _tagButton.InnerHtml = descricaoButton;
                _tagButton.AddCssClass("btn");
                _tagButton.AddCssClass(cssClass);
                _tagButton.MergeAttribute("onClick", "DisplayWait();");
                _tagButton.MergeAttribute("href", "/ProdutoVitrine/EnviarEmail?ProdutoMontadoId=" + produtoMontado.Id);
                _tagDivButton.InnerHtml = _tagButton.ToString();
                _tagFormulario.InnerHtml += _tagDivButton.ToString();
                _tagDivFormulario.InnerHtml += _tagFormulario.ToString();

                return MvcHtmlString.Create(_tagDivFormulario.ToString());
            }
        }

        /// <summary>
        /// Retorna as numerações dos tamanhos.
        /// </summary>
        /// <param name="tamanhoId"></param>
        /// <param name="cssClass"></param>
        /// <param name="nameElemento"></param>
        /// <returns></returns>
        public MvcHtmlString Numeracao(ProdutoTamanho tamanho, string cssClass, string nameElemento)
        {
            var divNumeracao = new TagBuilder("div");
            divNumeracao.MergeAttribute("id", Convert.ToString(tamanho.ProdutoId));
            divNumeracao.AddCssClass("numeracao" + tamanho.ProdutoId);
            divNumeracao.AddCssClass(cssClass);
            divNumeracao.MergeAttribute("name", nameElemento);
            //divNumeracao.InnerHtml = Convert.ToString(tamanho.Tamanho.Referencia);

            return MvcHtmlString.Create(divNumeracao.ToString());
        }

        public MvcHtmlString IconPromotion(Produto produto, string produtoMontadoId)
        {
            var promocaoItem = produto.ProdutosMontado.First(c => c.Id == produtoMontadoId)?.PromocaoItems.FirstOrDefault(c => c.Promocao.Ativo);
            var valorAtual = produto.ProdutosMontado.First(c => c.Id == produtoMontadoId).TabelaPrecos.LastOrDefault().Valor.Value;

            if (promocaoItem != null && PromotionsController.VerificaPromocao(promocaoItem.Promocao))
            {
                var valorPromocional = promocaoItem.TabelaDePreco.Valor.Value;
                valorPromocional = promocaoItem.Promocao.TipoValor == TipoValor.Porcentual ?
                    valorPromocional - ((valorPromocional / 100) * promocaoItem.Promocao.Valor) :
                    (valorPromocional - promocaoItem.Promocao.Valor);

                var desconto = promocaoItem.Promocao.TipoValor == TipoValor.Porcentual ?
                    promocaoItem.Promocao.Valor :
                    Convert.ToInt32((valorPromocional / promocaoItem.Promocao.Valor) * 100);

                return MvcHtmlString.Create("<span class= 'valor-desconto text-center' title='Promoção válida apenas para tamanho e cor selecionado!'>" + desconto.ToString("0.##") + "% <br/>Off</span>");
            }

            return MvcHtmlString.Create("");
        }
    }
}