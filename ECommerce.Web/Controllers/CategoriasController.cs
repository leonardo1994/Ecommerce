using ECommerce.Web.Areas.Admin.Models;
using ECommerce.Web.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ECommerce.Web.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Categorias
        public ActionResult Index()
        {
            var categorias = _db.Categorias.ToList().Where(c => c.Publicar == "S");
            IList<Categoria> listCategorias = new List<Categoria>();
            foreach (var item in categorias)
            {
                var subCategorias = _db.SubCategorias.Where(c => c.CategoriaId == item.Id && c.Publicar == "S").ToList();
                foreach (var itemSub in subCategorias)
                {
                    itemSub.CategoriaId = item.Id;
                    itemSub.Categoria = item;
                }
                item.SubCategorias = subCategorias;
                listCategorias.Add(item);
            }
            return View(categorias);
        }

        public ActionResult Index2()
        {
            var categorias = _db.Categorias.ToList().Where(c => c.Publicar == "S");
            IList<Categoria> listCategorias = new List<Categoria>();
            foreach (var item in categorias)
            {
                var subCategorias = _db.SubCategorias.Where(c => c.CategoriaId == item.Id && c.Publicar == "S").ToList();
                foreach (var itemSub in subCategorias)
                {
                    itemSub.CategoriaId = item.Id;
                    itemSub.Categoria = item;
                }
                item.SubCategorias = subCategorias;
                listCategorias.Add(item);
            }
            return View(categorias);
        }
    }
}