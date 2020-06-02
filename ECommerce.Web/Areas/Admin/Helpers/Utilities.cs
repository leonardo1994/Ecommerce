using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace ECommerce.Web.Areas.Admin.Helpers
{
    /// <summary>
    /// Para uso de utilidades diversas do sistema, procurando otimizar tarefas repetivas.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Salva uma imagem no diretório padrão do server.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="image">Arquivo de imagem a ser salvo.</param>
        /// <param name="imageOld">Arquivo de imagem a ser sobreposto.</param>
        /// <returns>Retorna o caminho full da imagem salva no servidor, caso não seja salvo retorna vazio.</returns>
        public static string SaveImage(this Controller controller, HttpPostedFileBase image, string imageOld)
        {
            if (image == null)
            {
                controller.DisplayMessage("Por favor selecione uma imagem válida", TypeMessage.Danger);
                return "";
            }
            
            // Caso informado o arquivo old, verifico se o mesmo existe e faço a exclusão.
            if (!string.IsNullOrEmpty(imageOld))
            {
                var fullPath = Path.Combine(controller.Server.MapPath("~/Img"), imageOld);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            // Lista de extensões permitidas
            var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", ".jpeg", ".jpeg" };

            // Pega o arquivo
            var fileName = Path.GetFileName(image.FileName); //getting only file name(ex-ganesh.jpg)  

            // Pega a extensão do arquivo
            var ext = Path.GetExtension(image.FileName); //getting the extension(ex-.jpg)  

            if (allowedExtensions.Contains(ext)) //check what type of extension  
            {
                // Pega o nome da imagem sem a extensão
                var name = Path.GetFileNameWithoutExtension(fileName);

                // Gero um novo nome com identificador único.
                var myfile = name + "_" + Guid.NewGuid() + ext;

                // Combino o caminho da pasta do servidor com o nome da imagem
                var path = Path.Combine(controller.Server.MapPath("~/Img"), myfile);

                // Salvo a imagem no caminho físico
                image.SaveAs(path);

                // retorno caminho criado para o contexto.
                return @"\Img\" + myfile;
            }
            controller.DisplayMessage("Por favor selecione uma imagem válida", TypeMessage.Danger);
            return "";
        }

        /// <summary>
        /// Método padrão para disparar menssagem
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="message">Mensagem a ser exibida</param>
        /// <param name="typeMessage">Tipo de mensagem</param>
        public static void DisplayMessage(this Controller controller, string message, TypeMessage typeMessage)
        {
            controller.ViewBag.TypeMessage = typeMessage;
            controller.ViewBag.Message = message;
        }

        /// <summary>
        /// Usar enumeração para identificar tipo de menssagem a ser exibida.
        /// </summary>
        public enum TypeMessage
        {
            Danger,
            Info,
            Link,
            Warning,
            Success
        }

        /// <summary>
        /// Método retorna uma expressão anonima em um dicionário. 
        /// Key = Nome da propriedade.
        /// Value = Valor da propriedade.
        /// GetValuesAnonymous(new { id = 1, @class = 'btn btn-default' })
        /// </summary>
        /// <param name="anonymous">Passar um objeto do tipo anonymos</param>
        /// <returns>
        /// [id][1]
        /// [@class]['btn btn-default']
        /// </returns>
        public static Dictionary<string, object> GetValuesAnonymous(object anonymous)
        {
            var properties = anonymous.GetType().GetProperties();
            return properties.ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(anonymous));
        }

        /// <summary>
        /// Retorna um objeto anonimo formatado para atributos de elementos html.
        /// </summary>
        /// <param name="anonymous">Objeto do tipo anonymous</param>
        /// <returns>"id='elementId' name='elementName' onClick='eventClick(this)'"</returns>
        public static string GetAnonymousFormatAttributeHtml(object anonymous)
        {
            var dicionario = GetValuesAnonymous(anonymous);
            var format = dicionario.Aggregate("", (current, o) => current + (o.Key + " = \"" + o.Value + "\" "));
            return format;
        }
    }
}
