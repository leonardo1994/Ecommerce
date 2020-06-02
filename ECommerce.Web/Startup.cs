using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(ECommerce.Web.Startup))]
namespace ECommerce.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // Configure o HangFire para usar o SqlServer para o armazenamento persistente de tarefas.
            // Definir a sondagem para cada minuto.

            //GlobalConfiguration.Configuration.UseSqlServerStorage(
            //    @"Data Source = 177.105.119.175; Initial Catalog = LOJA_VIRTUAL; User ID = sa; Password = Sql@2>0<1*4",
            //    new SqlServerStorageOptions
            //    {
            //        // Definido como true para que o HangFire crie seu esquema
            //        PrepareSchemaIfNecessary = true,
            //        // Definido como falso e executar HangFire.sql para criar seu sql.
            //        // PrepareSchemaIfNecessary = false,
            //        QueuePollInterval = TimeSpan.FromSeconds(1)
            //    });

            //// Habilitar o painel do HangFire para exibir informações sobre o status do trabalho e do servidor
            //app.UseHangfireDashboard();

            //// Assegura o processamento de trabalhos HangFire no aplicativo da Web ASP.NET usando OWIN + IIS
            //app.UseHangfireServer();
        }
    }
}