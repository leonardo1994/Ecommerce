using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ECommerce.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using static ECommerce.Web.Controllers.ManageController;

namespace ECommerce.Web.Controllers
{
    [Authorize(Roles = "Admin,Cliente")]
    public class ClientesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public ActionResult DetailsPartial(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = _db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View("_DetailsPartial", cliente.GetClienteViewModels());
        }

        public ActionResult Details()
        {
            var userId = User.Identity.GetUserId();
            var cliente = _db.Clientes.FirstOrDefault(c => c.ApplicationUserId == userId);

            if (cliente == null)
            {
                return RedirectToAction("Create");
            }
            return View(cliente.GetClienteViewModels());
        }

        public ActionResult Create()
        {
            return View(new ClienteViewModels()
            {
                ApplicationUserId = HttpContext.User.Identity.GetUserId(),
                Email = HttpContext.User.Identity.GetUserName()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Cpf,DataNascimento,Sexo,DddFixo,TelefoneFixo,DddCelular,TelefoneCelular,Cep,Rua,Numero,Complemento,Bairro,Cidade,Estado,Email,ApplicationUserId, Ibge")] ClienteViewModels cliente, string returnUrl)
        {
            if(!ValidaNome(cliente.Nome))
                ModelState.AddModelError("", "Nome inválido");
            if (!ValidaDDD(cliente.DddCelular))
                ModelState.AddModelError("", "DDD do celular inválido");
            if (!ValidaDDD(cliente.DddFixo))
                ModelState.AddModelError("", "DDD do telefone inválido");
            if (!ValidaTelCelular(cliente.TelefoneCelular))
                ModelState.AddModelError("", "Telefone celular inválido");
            if (!ValidaTelFixo(cliente.TelefoneFixo))
                ModelState.AddModelError("", "Telefone fixo inválido");
            if (!ValidaDtNasc(cliente.DataNascimento))
                ModelState.AddModelError("", "Data de nascimento inválida");

            if (!ModelState.IsValid) return View(cliente);

            cliente.Id = (Convert.ToInt32(_db.Clientes.Max(c => c.Id)) + 1).ToString().PadLeft(5, '0');
            cliente.Email = HttpContext.User.Identity.GetUserName();
            var clienteEF = cliente.GetCliente();
            clienteEF.DataCadastro = DateTime.Now;
            _db.Clientes.Add(clienteEF);
            _db.SaveChanges();
            if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
            ViewBag.StatusMessage = "Cadastro atualizado.";
            return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.CadastroAtualizado });
        }

        public ActionResult Edit(string returnUrl)
        {
            var userId = User.Identity.GetUserId();
            var cliente = _db.Clientes.FirstOrDefault(c => c.ApplicationUserId == userId);

            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente.GetClienteViewModels());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Cpf,DataNascimento,Sexo,DddFixo,TelefoneFixo,DddCelular,TelefoneCelular,Cep,Rua,Numero,Complemento,Bairro,Cidade,Estado,Email,ApplicationUserId, Ibge")] ClienteViewModels cliente, string returnUrl)
        {
            if (!ValidaNome(cliente.Nome))
                ModelState.AddModelError("", "Nome inválido");
            if (!ValidaDDD(cliente.DddCelular))
                ModelState.AddModelError("", "DDD do celular inválido");
            if (!ValidaDDD(cliente.DddFixo))
                ModelState.AddModelError("", "DDD do telefone inválido");
            if (!ValidaTelCelular(cliente.TelefoneCelular))
                ModelState.AddModelError("", "Telefone celular inválido");
            if (!ValidaTelFixo(cliente.TelefoneFixo))
                ModelState.AddModelError("", "Telefone fixo inválido");
            if (!ValidaDtNasc(cliente.DataNascimento))
                ModelState.AddModelError("", "Data de nascimento inválida");

            if (!ModelState.IsValid)
            {
                return View(cliente);
            };

            cliente.Email = HttpContext.User.Identity.GetUserName();
            var clienteEF = cliente.GetCliente();
            _db.Entry(clienteEF).State = EntityState.Modified;
            _db.SaveChanges();
            ViewBag.StatusMessage = "Cadastro atualizado.";
            if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.CadastroAtualizado });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ValidaDDD(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            var ddds = new string[]
            {
                "11", "12", "13", "14", "15", "16", "17", "18", "19",
                "22", "21", "24", "27", "28",
                "31", "32", "33", "34", "35", "37", "38",
                "41", "42", "43", "44", "46", "47", "48", "49",
                "51", "53", "54", "55",
                "61", "62", "63", "64", "65", "66", "67", "68", "69",
                "71", "73", "74", "75", "77", "79",
                "81", "82", "83", "84", "85", "86", "87", "88", "89",
                "91", "92", "93", "94", "95", "96", "97", "98", "99"
            };

            return ddds.Any(c => c == value);
        }

        private bool ValidaTelCelular(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            value = value.Replace("-", "");

            for (int i = 0; i < 9; i++)
            {
                var compare = "";
                for (int j = 0; j < 9; j++)
                {
                    compare += i.ToString();
                }
                if (value == compare)
                    return false;
            }

            var arrays = new string[]
            {
                "00", "01", "02", "03",  "04",  "05",  "06",  "07",  "08", "09", "10"
            };

            if (arrays.Any(c => c == value.Substring(0, 2)))
                return false;

            var arrays2 = new string[]
            {
                "6", "7", "8", "9"
            };

            if (arrays2.Any(c => c == value.Substring(2, 3)))
                return false;

            return true;
        }

        private bool ValidaTelFixo(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            value = value.Replace("-", "");

            for (int i = 0; i < 8; i++)
            {
                var compare = "";
                for (int j = 0; j < 8; j++)
                {
                    compare += i.ToString();
                }
                if (value == compare)
                    return false;
            }

            var arrays = new string[]
            {
                "00", "01", "02", "03",  "04",  "05",  "06",  "07",  "08", "09", "10"
            };

            if (arrays.Any(c => c == value.Substring(0, 2)))
                return false;

            var arrays2 = new string[]
            {
                "1", "2", "3", "4", "5"
            };

            if (arrays2.Any(c => c == value.Substring(2, 3)))
                return false;

            return true;
        }

        private bool ValidaDtNasc(DateTime dataNasc)
        {
            //Pega a data atual
            DateTime dataAtual = DateTime.Now;

            //Subtai para saber quantos anos se passaram após nascimento
            int idade = DateTime.Now.Year - dataNasc.Year;

            //data de nascimento não pode ser maior que data atual
            if (dataAtual < dataNasc)
            {
                return false;
            }

            if (DateTime.Now.Month < dataNasc.Month || (DateTime.Now.Month == dataNasc.Month && DateTime.Now.Day < dataNasc.Day))
            {
                idade--;
            }

            return idade >= 18 && idade <= 100;
        }

        private bool ValidaNome(string value)
        {
            var nome = value.Trim().Split(' ');
            if (nome.Length == 1) return false;
            return true;
        }
    }
}