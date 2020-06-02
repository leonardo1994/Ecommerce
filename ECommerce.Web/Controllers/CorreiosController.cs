using ECommerce.Web.Models.EntCarrinho;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ECommerce.Web.ServicoCorreio
{
    /// <summary>
    /// Serviços disponíveis 
    ///41106  PAC sem contrato
    ///40010  SEDEX sem contrato
    ///40045  SEDEX a Cobrar, sem contrato
    ///40215  SEDEX 10, sem contrato
    ///40290  SEDEX Hoje, sem contrato
    ///40096  SEDEX com contrato
    ///40436  SEDEX com contrato
    ///40444  SEDEX com contrato
    ///81019  e-SEDEX, com contrato
    ///41068  PAC com contrato
    /// </summary>
    public class CorreiosController : Controller
    {
        private CalcPrecoPrazoWSSoapClient _servico;
        private const string _cdServico = "04014";
        private const string _cepOrigem = "06365150";

        public JsonResult Prazo(string cepDestino)
        {
            _servico = new CalcPrecoPrazoWSSoapClient();

            var resultado = _servico.CalcPrazo(_cdServico, _cepOrigem, cepDestino);

            if (resultado.Servicos.Length > 0)
                return Json(resultado.Servicos, JsonRequestBehavior.AllowGet);
            return Json("");
        }

        public JsonResult PrazoPreco(string cepDestino, decimal comprimento, decimal largura, decimal altura, decimal pesoBruto)
        {
            try
            {
                Session["Cep"] = cepDestino;
            }
            catch (System.Exception)
            {
            }

            _servico = new CalcPrecoPrazoWSSoapClient();

            var resultado = _servico.CalcPrecoPrazo("", "", _cdServico, _cepOrigem, cepDestino, pesoBruto.ToString(), 1, comprimento, altura, largura, 0, "N", 0, "N");

            if (resultado.Servicos.Length > 0)
                return Json(resultado.Servicos, JsonRequestBehavior.AllowGet);
            return Json("");
        }

        public JsonResult PrazoCep(string cepDestino)
        {
            var carrinho = new Carrinho(User.Identity.GetUserId(), Request.AnonymousID);

            if (!carrinho.ItemCarrinhos.Any()) return Json("");
            var comprimento = carrinho.ItemCarrinhos.Max(c => c.ProdutoMontado.Comprimento);
            var altura = carrinho.ItemCarrinhos.Sum(c => c.ProdutoMontado.Altura * c.QuantidadeTotalItem);
            var largura = carrinho.ItemCarrinhos.Max(c => c.ProdutoMontado.Largura);
            var pesoBruto = carrinho.ItemCarrinhos.Sum(c => c.ProdutoMontado.PesoBruto * c.QuantidadeTotalItem);

            try
            {
                Session["Cep"] = cepDestino;
            }
            catch (System.Exception)
            {
            }

            _servico = new CalcPrecoPrazoWSSoapClient();

            var resultado = _servico.CalcPrecoPrazo("", "", _cdServico, _cepOrigem, cepDestino, pesoBruto.ToString(), 1, Convert.ToDecimal(comprimento), Convert.ToDecimal(altura), Convert.ToDecimal(largura), 0, "N", 0, "N");

            if (resultado.Servicos.Length > 0)
                return Json(resultado.Servicos, JsonRequestBehavior.AllowGet);
            return Json("");
        }

        public JsonResult RastrearCodigo(string codigo)
        {
            var resultado = new ServicoCorreioRastreamento.buscaEventosRequest("","", "L", "", "101", codigo);
            return Json(resultado.objetos, JsonRequestBehavior.AllowGet);
        }
    }
}