using System.Web.Mvc;
using System.Web.Routing;

namespace ECommerce.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              "DetalheProduto",
              "Produto/{ProdutoId}/{ProdutoTamanhoId}/{ProdutoCorId}",
              new { controller = "ProdutoVitrine", action = "Details" },
              new { ProdutoId = "", ProdutoTamanhoId = "", ProdutoCorId = "" }
          );

            routes.MapRoute(
              "Promocao",
              "Promocao/{id}",
              new { controller = "ProdutoVitrine", action = "Promocao" },
              new { id = @"\d+" }
          );

            routes.MapRoute(
              "ClienteAtualizar",
              "Atualizar/",
              new { controller = "Clientes", action = "Edit" }
          );

            routes.MapRoute(
              "ClienteCadastrar",
              "Cadastrar/",
              new { controller = "Clientes", action = "Create" }
          );

            routes.MapRoute(
              "ClienteDetalhe",
              "Visualizar/",
              new { controller = "Clientes", action = "Details" }
          );

            routes.MapRoute(
              "Detalhe",
              "Produto/{id}",
              new { controller = "ProdutoVitrine", action = "DetailsMontadoId" },
              new { id = @"\d+" }
          );

            routes.MapRoute(
              "Categoria",
              "{controller}/{action}/{subCategoriaId}/{categoriaId}",
              new { controller = "ProdutoVitrine", action = "Categoria" },
              new { subCategoriaId = "", categoriaId = "" }
          );

            routes.MapRoute(
                name: "Pedido",
                url: "Checkout/",
                defaults: new { controller = "Pedidos", action = "Create" }
            );

            routes.MapRoute(
                name: "Carrinho",
                url: "Carrinho/",
                defaults: new { controller = "Carrinho", action = "CarrinhoDeCompras" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
