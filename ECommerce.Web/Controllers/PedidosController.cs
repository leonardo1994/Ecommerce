using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ECommerce.Web.Models;
using ECommerce.Web.Models.EntPedido;
using Microsoft.AspNet.Identity;
using ECommerce.Web.Models.EntCarrinho;
using System;
using System.Collections.Generic;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Constants;
using Uol.PagSeguro.Exception;

namespace ECommerce.Web.Controllers
{
    [Authorize(Roles = "Admin,Cliente")]
    public class PedidosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var pedidos = db.Pedidos.Include(p => p.Cliente).Where(c => c.Cliente.ApplicationUserId == userId);

            return View(pedidos.ToList());
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pedido pedido = db.Pedidos.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();

            var cliente = db.Clientes.FirstOrDefault(c => c.ApplicationUserId == userId);
            var user = db.Users.Find(userId);

            var urlSenderEmail = "<a href='/Account/SendEmail?userId=" + userId + @"&returnUrl=\Pedidos\Create'>Clique aqui para enviar novamente.</a>";

            var mensagens = !user.EmailConfirmed ? "Você precisa confirmar seu e-mail antes de continuar " + urlSenderEmail : "";

            ModelState.AddModelError("", mensagens);

            if (cliente == null)
            {
                return RedirectToAction("Create", "Clientes", new { returnUrl = Request.Url.AbsoluteUri });
            }

            var carrinho = new Carrinho(User.Identity.GetUserId(), Request.AnonymousID);

            ViewBag.TotalItens = carrinho.ObterQuantidadeItens();

            var cep = Session["cep"] as string;

            var comprimento = carrinho.ItemCarrinhos.Max(c => c.ProdutoMontado.Comprimento);
            var altura = carrinho.ItemCarrinhos.Sum(c => c.ProdutoMontado.Altura * c.QuantidadeTotalItem);
            var largura = carrinho.ItemCarrinhos.Max(c => c.ProdutoMontado.Largura);
            var peso = carrinho.ItemCarrinhos.Sum(c => c.ProdutoMontado.PesoBruto * c.QuantidadeTotalItem);

            //Informações do Frete
            ServicoCorreio.cServico[] cepCorreio = new ServicoCorreio.CorreiosController().PrazoPreco(cep ?? cliente.Cep, Convert.ToDecimal(comprimento), Convert.ToDecimal(largura), Convert.ToDecimal(altura), Convert.ToDecimal(peso)).Data as ServicoCorreio.cServico[];

            ViewBag.PrazoEntrega = cepCorreio[0].PrazoEntrega;

            var valorFrete = Convert.ToDouble(cepCorreio[0].Valor);

            if (string.IsNullOrEmpty(cep))
            {
                return View(new Pedido
                {
                    ValorFrete = valorFrete,
                    Bairro = cliente.Bairro,
                    Cep = cliente.Cep,
                    Cidade = cliente.Cidade,
                    ClienteId = cliente.Id,
                    Endereco = cliente.Rua,
                    Uf = cliente.Estado,
                    ValorPedido = carrinho.ObterValorTotal(),
                    Numero = cliente.Numero
                });
            }
            else
            {   
                return View(new Pedido
                {
                    ValorFrete = valorFrete,
                    Cep = cep,
                    ClienteId = cliente.Id,
                    ValorPedido = carrinho.ObterValorTotal()
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pedido pedido, string FormaPagamento)
        {
            var userId = User.Identity.GetUserId();
            var cliente = db.Clientes.FirstOrDefault(c => c.ApplicationUserId == userId);

            var user = db.Users.Find(userId);
            var Mensagens = !user.EmailConfirmed ? "Você precisa confirmar seu e-mail antes de continuar" : "";

            ModelState.AddModelError("", Mensagens);

            if (!string.IsNullOrEmpty(Mensagens))
            {
                pedido.ClienteId = cliente.Id;
                return View(pedido);
            }

            // Iniciando a sessão com PagSeguro
            // homologação: D07D86F6F5D7472182A2979A69D36AFA
            // Produção: 79C4FEC17D444783947521D2645E2DE5
            // v89194894581853426399@sandbox.pagseguro.com.br
            //cobranca@famara.com.br
            AccountCredentials autenticacao = new AccountCredentials("cobranca@famara.com.br", "D07D86F6F5D7472182A2979A69D36AFA");

            //Informando o Endereço de entrega
            Address endereco = new Address()
            {
                Country = "BRA",
                State = pedido.Uf,
                City = pedido.Cidade,
                District = pedido.Bairro,
                PostalCode = pedido.Cep,
                Street = pedido.Endereco,
                Number = pedido.Numero,
                Complement = pedido.Complemento
            };

            // Definindo os paramêtros para transação
            var carrinho = new Carrinho(User.Identity.GetUserId(), Request.AnonymousID);

            ViewBag.TotalItens = carrinho?.ObterQuantidadeItens();

            var comprimento = carrinho.ItemCarrinhos.Max(c => c.ProdutoMontado.Comprimento);
            var altura = carrinho.ItemCarrinhos.Sum(c => c.ProdutoMontado.Altura * c.QuantidadeTotalItem);
            var largura = carrinho.ItemCarrinhos.Max(c => c.ProdutoMontado.Largura);
            var peso = carrinho.ItemCarrinhos.Sum(c => c.ProdutoMontado.PesoBruto * c.QuantidadeTotalItem);

            var cupomDesconto = carrinho.ItemCarrinhos.First().Promocao;
            pedido.PromocaoId = cupomDesconto?.Id;

            //Informações do Frete
            ServicoCorreio.cServico[] cep = new ServicoCorreio.CorreiosController().PrazoPreco(pedido.Cep, Convert.ToDecimal(comprimento), Convert.ToDecimal(largura), Convert.ToDecimal(altura), Convert.ToDecimal(peso)).Data as ServicoCorreio.cServico[];
            var frete = new Shipping()
            {
                Address = endereco,
                Cost = Convert.ToDecimal(cep[0].Valor),
                ShippingType = ShippingType.Sedex
            };

            // Dados do comprador
            var comprador = new Sender(cliente.Nome, cliente.Email, new Phone(cliente.DddFixo, cliente.TelefoneFixo));
            var cpfComprador = new SenderDocument(Documents.GetDocumentByType("CPF"), cliente.Cpf);
            comprador.Documents.Add(cpfComprador);

            // Número de referência ao pagseguro
            var numeroReferencia = Guid.NewGuid().ToString();
            pedido.RefereciaPagSeguro = numeroReferencia;

            var requisicaoPagamento = new PaymentRequest()
            {
                Currency = "BRL",
                Shipping = frete,
                Sender = comprador,
                Reference = numeroReferencia
            };

            var itensCarrinho = carrinho.ItemCarrinhos.ToList();

            foreach (var item in itensCarrinho)
            {
                requisicaoPagamento.Items.Add(new Item(item.ProdutoMontadoId, item.ProdutoMontado.Descricao, item.QuantidadeTotalItem, Convert.ToDecimal(item.ValorUnitario)));
            }

            var url = "";

            requisicaoPagamento.NotificationURL = "http://www.famaraonline.com.br/PagSeguro/Notificacao";
            // try 
            try
            {
                url = requisicaoPagamento.Register(autenticacao).AbsoluteUri;
            }
            catch (PagSeguroServiceException ex)
            {
                ViewBag.TotalItens = carrinho.ObterQuantidadeItens();

                ModelState.AddModelError("", "Não foi possível processar o pedido tente novamente.");

                foreach (var item in ex.Errors)
                {
                    ModelState.AddModelError("", item.Message);
                }

                return View(pedido);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(pedido);
            }

            pedido.Id = (Convert.ToInt32(db.Pedidos.Max(c => c.Id)) + 1).ToString().PadLeft(6, '0');

            pedido.DataEmissao = DateTime.Now;
            pedido.data_alteracao = DateTime.Now;
            pedido.data_gravacao = DateTime.Now;

            pedido.ValorFrete = double.Parse(frete.Cost.Value.ToString());
            pedido.ValorPedido = Convert.ToDouble(carrinho.ObterValorTotal());
            pedido.ValorPedido1 = pedido.ValorPedido;

            pedido.ClienteId = cliente.Id;
            pedido.CpfCliente = cliente.Cpf;

            pedido.CpfCliente = cliente.Cpf;

            var local = db.Parametros.FirstOrDefault().local_loja;

            var itensPedido = itensCarrinho.Select(item => new ItensPedido()
            {
                ClienteId = cliente.Id,
                CodTabelaPreco = item.TabelaPreco.Codigo,
                TabelaPrecoId = item.TabelaPrecoId,
                ProdutoMontadoId = item.ProdutoMontadoId,
                Quantidade = item.QuantidadeTotalItem,
                ValorTotal = item.ValorTotalItem,
                ValorTotal1 = item.ValorTotalItem,
                CpfCliente = cliente.Cpf,
                DescricaoProduto = item.ProdutoMontado.Descricao,
                ValorUnitario = item.ValorUnitario,
                ItemPedido = (itensCarrinho.IndexOf(item) + 1).ToString().PadLeft(3, '0'),
                PedidoId = pedido.Id,
                LocalId = local,
                data_entrega = DateTime.Now,
                data_emissao = DateTime.Now,
                data_alteracao = DateTime.Now,
                data_gravacao = DateTime.Now
            }).ToList();

            pedido.aprovacao_financeiro = "N";
            pedido.DataEmissaoPagSeguro = DateTime.Now;
            pedido.UrlPagamento = url;
            pedido.StatusCode = "1";
            pedido.StatusDescricao = "Aguardando pagamento";

            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Pedidos.Add(pedido);
                    db.SaveChanges();

                    db.ItensPedido.AddRange(itensPedido);
                    db.SaveChanges();

                    var itens = db.ItemCarrinhos.Where(c => c.ApplicationUserId == cliente.ApplicationUserId).ToList();
                    foreach (var item in itens)
                    {
                        var produto = db.ProdutosMontados.Find(item.ProdutoMontadoId);
                        var grade = db.Produtos.Find(produto.ProdutoId);
                        produto.QtdVendida += 1;
                        grade.QtdVendida += 1;

                        db.Entry(produto).State = EntityState.Modified;
                        db.SaveChanges();

                        db.Entry(grade).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    db.ItemCarrinhos.RemoveRange(itens);
                    db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    ViewBag.TotalItens = carrinho.ObterQuantidadeItens();

                    ModelState.AddModelError("", "Não foi possível processar o pedido tente novamente.");
                    return View(pedido);
                }
            }

            return Redirect(url);
            //return RedirectToAction("Index", "Pedidos");
        }

        public ActionResult CancelamentoPedido(string pedidoId)
        {
            var pedido = db.Pedidos.Find(pedidoId);

            if (!string.IsNullOrEmpty(pedido.CodigoTransacao))
            {
                if (new PagSeguroController().Cancelamento(pedido.CodigoTransacao))
                {
                    CancelarPedido(pedido);
                }
                else
                {
                    ModelState.AddModelError("", "Não foi possível cancelar o Pedido: " + pedido.Id + " tente novamente mais tarde.");
                }
            }
            else
            {
                CancelarPedido(pedido);
            }

            var userId = User.Identity.GetUserId();
            var pedidos = db.Pedidos.Include(p => p.Cliente).Where(c => c.Cliente.ApplicationUserId == userId);

            return RedirectToAction("Index", "Pedidos");
        }

        private void CancelarPedido(Pedido pedido)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    pedido.StatusCode = "7";
                    pedido.StatusDescricao = "Cancelado";
                    foreach (var itemPedido in pedido.ItensPedido)
                    {
                        var estoque = db.Estoques.FirstOrDefault(c => c.ProdutoMontadoId == itemPedido.ProdutoMontadoId && c.LocalId == itemPedido.LocalId);
                        estoque.SaldoAlocado -= itemPedido.Quantidade;
                        db.Entry(estoque).State = EntityState.Modified;
                    }
                    db.Entry(pedido).State = EntityState.Modified;
                    db.SaveChanges();
                    transaction.Commit();
                    ModelState.AddModelError("", "Pedido " + pedido.Id + "Cancelado com sucesso");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Não foi possível cancelar o Pedido: " + pedido.Id + " tente novamente mais tarde.");
                }
           }
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
}