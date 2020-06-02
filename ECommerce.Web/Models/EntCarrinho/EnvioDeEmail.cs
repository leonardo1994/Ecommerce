using System;
using Hangfire;

namespace ECommerce.Web.Models.EntCarrinho
{
    public static class EnvioEmail
    {
        public static void JobEmail()
        {
            BackgroundJob.Enqueue((() => SendEmail()));
        }

        public static void SendEmail()
        {
            Console.WriteLine("enviou");
        }
    }
}