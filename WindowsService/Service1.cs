using System;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Threading;

namespace WindowsService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Thread t = new Thread(new ThreadStart(MonitoraSite));
            t.Start();
        }

        private void MonitoraSite()
        {
            while (true)
            {
                try
                {
                    #region Controle de Status Pag Seguro
                    //Dispara uma requisição para atualizar os Pedidos Status do PagSeguro.

                    //Classe que irá fazer a requisição GET.
                    ((HttpWebRequest)WebRequest.Create(@"http://www.famaraonline.com.br/PagSeguro/AtualizaStatus")).Method = "GET";

                    //Obtém resposta do servidor.
                    using (HttpWebResponse response = (HttpWebResponse)((HttpWebRequest)WebRequest.Create(@"http://www.famaraonline.com.br/PagSeguro/AtualizaStatus")).GetResponse())
                    {

                    }

                    #endregion

                    #region Aviso de Disponibilidade
                    //Dispara uma requisição para atualizar os Pedidos Status do PagSeguro.
                    string uriAtualizaStatus = @"http://www.famaraonline.com.br/ProdutoVitrine/AvisoDisponibilidade";

                    //Classe que irá fazer a requisição GET.
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriAtualizaStatus);

                    //Método do webrequest.
                    request.Method = "GET";

                    //Obtém resposta do servidor.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {

                    }
                    #endregion

                    #region Verifica Carrinho
                    //Dispara uma requisição para atualizar os Pedidos Status do PagSeguro.
                    string uriAtualizaCarrinho = @"http://www.famaraonline.com.br/Carrinho/AnaliseCarrinho";

                    //Classe que irá fazer a requisição GET.
                    HttpWebRequest requestCarrinho = (HttpWebRequest)WebRequest.Create(uriAtualizaCarrinho);

                    //Método do webrequest.
                    requestCarrinho.Method = "GET";

                    //Obtém resposta do servidor.
                    using (HttpWebResponse requestC = (HttpWebResponse)requestCarrinho.GetResponse())
                    {

                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    // Cria o nome do arquivo com ano, mês, dia, hora minuto e segundo

                    string nomeArquivo = @"c:\log" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

                    // Cria um novo arquivo e devolve um StreamWriter para ele

                    StreamWriter writer = new StreamWriter(nomeArquivo);

                    // Agora é só sair escrevendo

                    writer.WriteLine(ex.Message + " Inner: " + ex.InnerException?.Message);

                    // Não esqueça de fechar o arquivo ao terminar

                    writer.Close();
                }
                // Executa o serviço a cada 5 min.
                Thread.Sleep(1000);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
