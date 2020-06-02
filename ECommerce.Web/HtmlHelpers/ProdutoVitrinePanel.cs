using System.Web.Mvc;
using ECommerce.Web.Areas.Admin.Models;

namespace ECommerce.Web.HtmlHelpers
{
    public static class ProdutoVitrinePanel
    {
        public static MvcHtmlString PainelVitrineProdutoFor(this HtmlHelper htmlHelper, Produto produto)
        {
            ProdutoHelpers pm = new ProdutoHelpers();
            return pm.PanelProdutoFor(produto);
        }

        public static MvcHtmlString TiposProdutoFor(this HtmlHelper htmlHelper, Produto produto, string nomeElemento, string cssClass, int tipoID, int corID)
        {
            ProdutoHelpers pm = new ProdutoHelpers();
            return pm.Tipos(produto, nomeElemento, cssClass, tipoID, corID, true);
        }

        public static MvcHtmlString NomeProduto(this HtmlHelper htmlHelper, Produto produto, string cssClass, string idElemento, string nameElemento)
        {
            ProdutoHelpers pm = new ProdutoHelpers();
            return pm.NomeProduto(produto, cssClass, idElemento, nameElemento);
        }

        public static MvcHtmlString DescricaoProduto(this HtmlHelper htmlHelper, Produto produto, string cssClass, string idElemento, string nameElemento)
        {
            ProdutoHelpers pm = new ProdutoHelpers();
            return pm.DescricaoProduto(produto, cssClass, idElemento, nameElemento);
        }

        public static MvcHtmlString ValorProduto(this HtmlHelper htmlHelper, Produto produto, string cssClass, string idElemento, string nameElemento, string codProdutoMontado)
        {
            ProdutoHelpers pm = new ProdutoHelpers();
            return pm.ValorProduto(produto, cssClass, idElemento, nameElemento, codProdutoMontado);
        }

        public static MvcHtmlString CoresProduto(this HtmlHelper htmlHelper, ProdutoTamanho produtoTipos, string cssClass, string idElemento, string nameElemento, int corID, bool imagens)
        {
            ProdutoHelpers pm = new ProdutoHelpers();
            return pm.Cores(produtoTipos, nameElemento, cssClass, corID, imagens);
        }

        public static MvcHtmlString ImagemProduto(this HtmlHelper htmlHelper, int produtoCorID, int quantidadeDeImagens)
        {
            ProdutoHelpers pm = new ProdutoHelpers();
            return pm.Imagem(produtoCorID, quantidadeDeImagens, null);
        }

        public static MvcHtmlString FormularioProduto(this HtmlHelper htmlHelper, Produto produto, string controller, string action, string descricaoButton, string cssClass, int produtoTamanhoId = 0, int produtoCorId = 0, bool disponibilidade = false)
        {
            ProdutoHelpers pm = new ProdutoHelpers();
            return pm.Formulario(produto, controller, action, descricaoButton, cssClass, produtoTamanhoId, produtoCorId, disponibilidade);
        }

        public static MvcHtmlString Numeracao(this HtmlHelper htmlHelper, ProdutoTamanho tamanho, string cssClass, string nameElemento)
        {
            return new ProdutoHelpers().Numeracao(tamanho, cssClass, nameElemento);
        }

        public static MvcHtmlString IconPromocao(this HtmlHelper htmlHelper, Produto produto, string produtoMontadoId)
        {
            return new ProdutoHelpers().IconPromotion(produto, produtoMontadoId);
        }
    }
}