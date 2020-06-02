using ECommerce.Web.Models;
using ECommerce.Web.Models.EntPedido;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Xml;
using Uol.PagSeguro.Constants;
using Uol.PagSeguro.Domain;
using Uol.PagSeguro.Exception;

namespace ECommerce.Web.Controllers
{
    [EnableCors(origins: "https://sandbox.pagseguro.uol.com.br", headers: "*", methods: "*")]
    public class PagSeguroController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public PagSeguroController()
        {
        }

        public ActionResult Notificacao()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Notificacao(FormCollection formCollection)
        {
            AccountCredentials autenticacao = new AccountCredentials("cobranca@famara.com.br", "D07D86F6F5D7472182A2979A69D36AFA");

            string notificationType = Request.Form["notificationType"];
            string notificationCode = Request.Form["notificationCode"];

            if (notificationType == "transaction")
            {
                ProcessarPedido(notificationCode);
            }

            return View();
        }

        public ActionResult Finalizado(string transaction_id)
        {
            ProcessarPedido(transaction_id);
            return RedirectToAction("Index", "Pedidos");
        }

        /// <summary>
        /// Pesquisa pela referencia e retorna a transacao
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        public string ConsultaReferencia(string referencia)
        {
            //uri de consulta da transação.
            string uri = "https://ws.sandbox.pagseguro.uol.com.br/v2/transactions?email=cobranca@famara.com.br&token=D07D86F6F5D7472182A2979A69D36AFA&reference=" + referencia;

            //Classe que irá fazer a requisição GET.
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            //Método do webrequest.
            request.Method = "GET";

            //String que vai armazenar o xml de retorno.
            string xmlString = null;

            //Obtém resposta do servidor.
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                //Cria stream para obter retorno.
                using (Stream dataStream = response.GetResponseStream())
                {
                    //Lê stream.
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        //Xml convertido para string.
                        xmlString = reader.ReadToEnd();

                        //Cria xml document para facilitar acesso ao xml.
                        XmlDocument xmlDoc = new XmlDocument();

                        //Carrega xml document através da string com XML.
                        xmlDoc.LoadXml(xmlString);

                        //Busca elemento status do XML.
                        var status = xmlDoc.GetElementsByTagName("code")[0];

                        //Fecha reader.
                        reader.Close();

                        //Fecha stream.
                        dataStream.Close();

                        return status?.InnerText;
                    }
                }
            }
        }

        public void AtualizaStatus()
        {
            var pedido = db.Pedidos.Where(p => new[] { "0", "1" }.Contains(p.StatusCode)).ToList();

            foreach (var item in pedido)
            {
                if (string.IsNullOrEmpty(item.CodigoTransacao))
                    ProcessarPedido(ConsultaReferencia(item.RefereciaPagSeguro), item.RefereciaPagSeguro);
                else
                    ProcessarPedido(item.CodigoTransacao);
            }
        }

        public bool Cancelamento(string code)
        {
            try
            {
                //uri de consulta da transação.
                string uri = "https://ws.sandbox.pagseguro.uol.com.br/v2/transactions/cancels?email=cobranca@famara.com.br&token=D07D86F6F5D7472182A2979A69D36AFA&transactionCode=" + code;

                //Classe que irá fazer a requisição GET.
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

                //Método do webrequest.
                request.Method = "POST";

                //String que vai armazenar o xml de retorno.
                string xmlString = null;

                var status = "";

                //Obtém resposta do servidor.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //Cria stream para obter retorno.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        //Lê stream.
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            //Xml convertido para string.
                            xmlString = reader.ReadToEnd();

                            //Cria xml document para facilitar acesso ao xml.
                            XmlDocument xmlDoc = new XmlDocument();

                            //Carrega xml document através da string com XML.
                            xmlDoc.LoadXml(xmlString);

                            //Busca elemento status do XML.
                            status = xmlDoc.GetElementsByTagName("result")[0].InnerText;

                            //Fecha reader.
                            reader.Close();

                            //Fecha stream.
                            dataStream.Close();
                        }
                    }
                }

                return status == "OK";
            }
            catch (Exception)
            {

                return false;
            }
        }

        public void ProcessarPedido(string transaction_id, string reference_id = "")
        {
            var formPagamento = 0;
            var tipoPagamento = 0;
            string status = null;
            string code = null;
            string reference = null;
            var installmentcount = 0;
            string linkPagamento = null;

            if (!string.IsNullOrEmpty(transaction_id))
            {
                var url = @"https://ws.sandbox.pagseguro.uol.com.br/v2/transactions/" + transaction_id + "?email=cobranca@famara.com.br&token=D07D86F6F5D7472182A2979A69D36AFA";

                //Classe que irá fazer a requisição GET.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                //Método do webrequest.
                request.Method = "GET";

                //String que vai armazenar o xml de retorno.
                string xmlString = null;

                //Obtém resposta do servidor.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //Cria stream para obter retorno.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        //Lê stream.
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            //Xml convertido para string.
                            xmlString = reader.ReadToEnd();

                            //Cria xml document para facilitar acesso ao xml.
                            XmlDocument xmlDoc = new XmlDocument();

                            //Carrega xml document através da string com XML.
                            xmlDoc.LoadXml(xmlString);

                            //Busca elemento status do XML.
                            var tagStatus = xmlDoc.GetElementsByTagName("status")[0];

                            if (tagStatus != null)
                                status = tagStatus.InnerText;

                            var tagCode = xmlDoc.GetElementsByTagName("code")[0];

                            if (tagCode != null)
                                code = tagCode.InnerText;

                            var tagReference = xmlDoc.GetElementsByTagName("reference")[0];

                            if (tagReference != null)
                                reference = tagReference.InnerText;

                            var tagInstallmentCount = xmlDoc.GetElementsByTagName("installmentCount")[0];

                            if (tagInstallmentCount != null)
                                installmentcount = Convert.ToInt32(tagInstallmentCount.InnerText);

                            var tagPaymentMethod = xmlDoc.GetElementsByTagName("paymentMethod")[0];

                            if (tagPaymentMethod != null && tagPaymentMethod.ChildNodes.Count > 0)
                            {
                                tipoPagamento = Convert.ToInt32(tagPaymentMethod.ChildNodes.Item(1).InnerText);
                                formPagamento = Convert.ToInt32(tagPaymentMethod.ChildNodes.Item(0).InnerText);
                            }

                            var tagPaymentLink = xmlDoc.GetElementsByTagName("paymentLink")[0];

                            if (tagPaymentLink != null)
                                linkPagamento = tagPaymentLink.InnerText;

                            //Fecha reader.
                            reader.Close();

                            //Fecha stream.
                            dataStream.Close();
                        }
                    }
                }
            }

            string transactionCode = transaction_id;

            Pedido pedido = new Pedido();

            pedido = db.Pedidos.FirstOrDefault(c => c.RefereciaPagSeguro == (reference ?? reference_id));

            if (string.IsNullOrEmpty(pedido.CodigoTransacao))
            {
                //if ((DateTime.Now - item.DataEmissao).Days >= 3)
                if ((DateTime.Now - pedido.DataEmissaoPagSeguro.Value).TotalMinutes >= 5)
                {
                    if ((string.IsNullOrEmpty(pedido.CodigoTransacao)) || Cancelamento(code))
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
                            }
                            catch (Exception)
                            {
                                transaction.Rollback();
                            }
                        }
                        return;
                    }
                }
            }

            pedido.CodigoTransacao = transactionCode;

            pedido.LinkPagamento = linkPagamento;

            pedido.Parcelamento = installmentcount == 1 ? string.Format("{0} vez", installmentcount) :
                string.Format("{0} vezes", installmentcount);

            if (string.IsNullOrEmpty(status))
                pedido.StatusCode = "0";
            else
                pedido.StatusCode = status;

            pedido.StatusDescricao = new PagSeguroStatus()[Convert.ToInt32(pedido.StatusCode)];

            pedido.PagSeguroPgtoCodigo = tipoPagamento.ToString();

            pedido.PagSeguroPgtoDescricao = new PagSeguroFormaPagamento()[Convert.ToInt32(tipoPagamento)];

            if (formPagamento == 1)
            {
                pedido.FormaPagamentoId = "18";
                pedido.des_formapag = "CARTÃO DE CRÉDITO";
            }
            else if (formPagamento == 2)
            {
                pedido.FormaPagamentoId = "01";
                pedido.des_formapag = "BOLETO BANCARIO";
            }

            if (pedido.StatusCode == "3")
            {
                pedido.aprovacao_financeiro = "A";

                // Plug in your email service here to send an email.
                const string @from = "famaraonline@famara.com.br"; // E-mail de remetente cadastrado no painel
                var to = pedido.Cliente.Email;   // E-mail do destinatário
                const string user = "famaraonline@famara.com.br"; // Usuário de autenticação do servidor SMTP
                const string pass = "LojaVirtual2017"; // Senha de autenticação do servidor SMTP

                WebClient wc = new WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;

                //Obtendo o conteúdo do template
                string sTemplate = wc.DownloadString(
                    "http://www.famaraonline.com.br/PagSeguro/Template");

                var subject = "Famara Online | Pedido " + pedido.Id;

                var body = subject;
                body = sTemplate.Replace("##Cliente##", pedido.Cliente.Nome);
                body = body.Replace("##Pedido##", pedido.Id);
                body = body.Replace("##Transacao##", pedido.CodigoTransacao);

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
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            db.Entry(pedido).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult ReprocessarPagamento(string pedidoId)
        {
            var pedido = db.Pedidos.Find(pedidoId);

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

            var comprimento = pedido.ItensPedido.Max(c => c.ProdutoMontado.Comprimento);
            var altura = pedido.ItensPedido.Sum(c => c.ProdutoMontado.Altura * c.Quantidade);
            var largura = pedido.ItensPedido.Max(c => c.ProdutoMontado.Largura);
            var peso = pedido.ItensPedido.Sum(c => c.ProdutoMontado.PesoBruto * c.Quantidade);

            //Informações do Frete
            ServicoCorreio.cServico[] cep = new ServicoCorreio.CorreiosController().PrazoPreco(pedido.Cep, Convert.ToDecimal(comprimento), Convert.ToDecimal(largura), Convert.ToDecimal(altura), Convert.ToDecimal(peso)).Data as ServicoCorreio.cServico[];
            var frete = new Shipping()
            {
                Address = endereco,
                Cost = Convert.ToDecimal(cep[0].Valor),
                ShippingType = ShippingType.Sedex
            };

            // Dados do comprador
            var comprador = new Sender(pedido.Cliente.Nome, pedido.Cliente.Email, new Phone(pedido.Cliente.DddFixo, pedido.Cliente.TelefoneFixo));
            var cpfComprador = new SenderDocument(Documents.GetDocumentByType("CPF"), pedido.Cliente.Cpf);
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

            var itensPedido = pedido.ItensPedido;

            foreach (var item in itensPedido)
            {
                // Passando os itens do carrinho para classe do PagSeguro. 
                requisicaoPagamento.Items.Add(new Item(item.ProdutoMontadoId, item.ProdutoMontado.Descricao, Convert.ToInt32(item.Quantidade), Convert.ToDecimal(item.TabelaPreco.Valor.Value)));
            }

            var url = "";

            requisicaoPagamento.NotificationURL = "http://www.famaraonline.com.br/PagSeguro/Notificacao";

            try
            {
                url = requisicaoPagamento.Register(autenticacao).AbsoluteUri;
            }
            catch (PagSeguroServiceException ex)
            {
                ModelState.AddModelError("", "Não foi possível processar o pedido tente novamente.");

                foreach (var item in ex.Errors)
                {
                    ModelState.AddModelError("", item.Message);
                }

                return View("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index");
            }

            return Redirect(url);
        }

        public ActionResult Template()
        {
            return View();
        }
    }

    public class PagSeguroStatus : Dictionary<int, string>
    {
        public PagSeguroStatus()
        {
            Add(0, "Transação não concluída");
            Add(1, "Aguardando pagamento");
            Add(2, "Em análise");
            Add(3, "Pago");
            Add(4, "Disponível");
            Add(5, "Em disputa");
            Add(6, "Devolvido");
            Add(7, "Cancelado");
        }
    }

    public class PagSeguroFormaPagamento : Dictionary<int, string>
    {
        public PagSeguroFormaPagamento()
        {
            Add(101, "Cartão de crédito Visa");
            Add(102, "Cartão de Crédito MasterCard");
            Add(103, "Cartão de Crédito American Express");
            Add(104, "Cartão de Crédito Diners");
            Add(105, "Cartão de Crédito Hipercard");
            Add(106, "Cartão de Crédito Aura");
            Add(107, "Cartão de Crédito Elo");
            Add(108, "Cartão de Crédito PLENOCard");
            Add(109, "Cartão de Crédito PersonalCard");
            Add(110, "Cartão de Crédito JCB");
            Add(111, "Cartão de Crédito Discover");
            Add(112, "Cartão de Crédito BrasilCard");
            Add(113, "Cartão de Crédito FORTBRASIL");
            Add(114, "Cartão de Crédito CARDBAN");
            Add(115, "Cartão de Crédito VALECARD");
            Add(116, "Cartão de Crédito Cabal");
            Add(117, "Cartão de Crédito Mais!");
            Add(118, "Cartão de Crédito Avista");
            Add(119, "Cartão de Crédito GRANDCARD");
            Add(120, "Cartão de Crédito Sorocred");
            Add(201, "Boleto Bradesco");
            Add(202, "Boleto Santander");
            Add(301, "Débito online Bradesco");
            Add(302, "Débito online Itáu");
            Add(303, "Débito online Unibanco");
            Add(304, "Débito online Banco do Brasil");
            Add(305, "Débito online Banco Real");
            Add(306, "Débito online Banrisul");
            Add(307, "Débito online HSBC");
            Add(401, "Saldo PagSeguro");
            Add(501, "Oi Paggo");
            Add(701, "Depósito em conta - Banco do Brasil");
            Add(702, "Depósito em conta - HSBC");
        }
    }
}