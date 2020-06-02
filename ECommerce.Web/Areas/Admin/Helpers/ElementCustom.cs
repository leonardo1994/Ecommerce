using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ECommerce.Web.Areas.Admin.Helpers
{
    public static class ElementCustom
    {
        /// <summary>
        /// Cria um objeto html do tipo data.
        /// Com formatações para Data e Hora.
        /// </summary>
        /// <typeparam name="TModel">Modelo</typeparam>
        /// <typeparam name="TValue">Valor</typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression">Propriedade do contexto</param>
        /// <param name="additional">Parametros adicionais para adicionar outros atributos ao elemento html como ClassCss, JavaScript, etc.</param>
        /// <returns>Retorna um elemento html do tipo data</returns>
        public static MvcHtmlString DatePickerFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, object additional)
        {
            var divFormGroup = new TagBuilder("div");
            divFormGroup.AddCssClass("form-group");

            var labelControl = htmlHelper.LabelFor(expression, new { @class = "control-label col-md-2" });
            divFormGroup.InnerHtml = labelControl.ToHtmlString();

            var divDatePickerFor = new TagBuilder("div");
            divDatePickerFor.AddCssClass("col-md-10");

            // Cria uma tag <input></input>
            var datePicker = new TagBuilder("input");

            // pego o modelo atual, da propriedade passada na expressão.
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            // Preencho o atributo id do elemento <input id='name' />
            datePicker.MergeAttribute("id", metadata.PropertyName);
            // Preencho o atributo name do elemento <input name='name' />
            datePicker.MergeAttribute("name", metadata.PropertyName);
            // Passo a class css de formatação padrão. <input class="form-control" />
            datePicker.AddCssClass("form-control");

            // Realizo está condição para verificar o tipo de formatação a ser apresentado.
            // Se será do do tipo data normal ou hora.
            if (metadata.DataTypeName == DataType.Date.ToString())
            {
                datePicker.Attributes.Add("value",
                    ((DateTime?)metadata.Model)?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd"));
                datePicker.MergeAttribute("type", metadata.DataTypeName.ToLower());
            }
            else if (metadata.DataTypeName == DataType.Time.ToString())
            {
                datePicker.Attributes.Add("value",
                    ((DateTime?)metadata.Model)?.ToString("HH:mm") ?? DateTime.Now.ToString("HH:mm"));
                datePicker.MergeAttribute("type", metadata.DataTypeName.ToLower());
            }

            divDatePickerFor.InnerHtml = datePicker.ToString();

            var validation = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            divDatePickerFor.InnerHtml += validation.ToHtmlString();

            divFormGroup.InnerHtml += divDatePickerFor;

            // Retorno o elemento montado
            return MvcHtmlString.Create(divFormGroup.ToString());
        }

        /// <summary>
        /// Cria os elementos Text, Number, CheckBox com Label automática.
        /// </summary>
        /// <typeparam name="TModel">Modelo</typeparam>
        /// <typeparam name="TValue">Valor</typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression">Propriedade do contexto</param>
        /// <param name="cssClass">Personalizar os elementos passando outras class css</param>
        /// <returns></returns>
        public static MvcHtmlString EditFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, string cssClass = "", bool disabled = false)
        {
            var divFormGroup = new TagBuilder("div");
            divFormGroup.AddCssClass("form-group");

            var labelControl = htmlHelper.LabelFor(expression, new { @class = "control-label col-md-2" });
            divFormGroup.InnerHtml = labelControl.ToHtmlString();

            var divEditFor = new TagBuilder("div");
            divEditFor.AddCssClass("col-md-10");

            var type = expression.Body.Type;

            // Verifico se é do tipo boleano, pois existe uma tratativa diferente na estrutura html.
            var isCheckBok = type == typeof(bool);

            if (!isCheckBok)
            {
                // Se não encontrar o tipo tento novamente por outro meio para garantir.
                try
                {
                    isCheckBok = expression.Body.Type.UnderlyingSystemType.GenericTypeArguments[0] == typeof(bool);
                }
                catch (Exception)
                {
                    isCheckBok = false;
                }

            }

            MvcHtmlString editFor;

            if (isCheckBok)
            {
                var divCheckBox = new TagBuilder("div");
                divCheckBox.AddCssClass("checkbox");
                if (disabled)
                {
                    divCheckBox.InnerHtml += htmlHelper.EditorFor(expression, new { @disabled = "" });
                }
                else
                {
                    divCheckBox.InnerHtml += htmlHelper.EditorFor(expression);
                }
                    
                editFor = new MvcHtmlString(divCheckBox.ToString());
            }
            else
            {
                if (disabled)
                {
                    editFor = htmlHelper.EditorFor(expression, new { htmlAttributes = new { @class = "form-control", @disabled = "" } });
                }else
                {
                    editFor = htmlHelper.EditorFor(expression, new { htmlAttributes = new { @class = "form-control" } });
                }
                
            }

            divEditFor.InnerHtml = editFor.ToHtmlString();

            var validation = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            divEditFor.InnerHtml += validation.ToHtmlString();

            divFormGroup.InnerHtml += divEditFor;

            return MvcHtmlString.Create(divFormGroup.ToString());
        }

        /// <summary>
        /// Gera um dropdown com todos os elemento de uma tipo enum, já com label automática
        /// </summary>
        /// <typeparam name="TModel">Modelo</typeparam>
        /// <typeparam name="TValue">Valor</typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression">Propriedade do contexto</param>
        /// <param name="cssClass">Personalizar os elementos passando outras class css</param>
        /// <returns></returns>
        public static MvcHtmlString EnumDropDownList<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, string cssClass = "")
        {
            var divFormGroup = new TagBuilder("div");
            divFormGroup.AddCssClass("form-group");

            var labelControl = htmlHelper.LabelFor(expression, new { @class = "control-label col-md-2" });
            divFormGroup.InnerHtml = labelControl.ToHtmlString();

            var divenumDropDwonListFor = new TagBuilder("div");
            divenumDropDwonListFor.AddCssClass("col-md-10");

            var enumDropDwonListFor = htmlHelper.EnumDropDownListFor(expression, new { @class = "form-control" });
            divenumDropDwonListFor.InnerHtml = enumDropDwonListFor.ToHtmlString();

            var validation = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            divenumDropDwonListFor.InnerHtml += validation.ToHtmlString();

            divFormGroup.InnerHtml += divenumDropDwonListFor;

            return MvcHtmlString.Create(divFormGroup.ToString());
        }

        /// <summary>
        /// Representa um elemento para inserir um arquivo
        /// </summary>
        /// <typeparam name="TModel">Modelo</typeparam>
        /// <typeparam name="TValue">Valor</typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression">Passar a propriedade</param>
        /// <param name="additional">Adicionar atributos ao elemento como Class css, javascript, etc.</param>
        /// <param name="displayImage">Se true, será apresentado a imagem selecionada. Valido apenas para arquivos do tipo image, não ativar para outros formatos de arquivo</param>
        /// <param name="changeFile">Se true, dará opção de alterar o arquivo já existente</param>
        /// <returns></returns>
        public static MvcHtmlString InputFileFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, object additional, bool displayImage, bool changeFile)
        {
            var divGeneral = new TagBuilder("div");
            var divImageGroup = new TagBuilder("div");
            divImageGroup.AddCssClass("form-group");

            var divFileGroup = new TagBuilder("div");
            divFileGroup.AddCssClass("form-group");

            var divImageCol10 = new TagBuilder("div");
            divImageCol10.AddCssClass("col-md-10");

            var divFileCol10 = new TagBuilder("div");
            divFileCol10.AddCssClass("col-md-10");

            // Pego contexto atual
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            // Verifico se a imagem será apresentada quando selecionada, então crio um elemento <img />
            if (displayImage)
            {
                var img = new TagBuilder("img");
                img.MergeAttribute("id", "imgSelected");
                img.MergeAttribute("alt", "");
                if (changeFile)
                    img.MergeAttribute("src", metadata.Model.ToString());
                divImageGroup.InnerHtml += htmlHelper.Label("Imagem selecionada", new { @class = "col-md-2" });
                divImageCol10.InnerHtml += img.ToString();
                divImageGroup.InnerHtml += divImageCol10.ToString();
            }

            // Crio um elemento do tipo <inputt />
            var inputFile = new TagBuilder("input");
            // Preencho o atributo id do elemento <input id='name' />
            inputFile.MergeAttribute("id", "fileImage");
            // Preencho o atributo name do elemento <input name='name' />
            inputFile.MergeAttribute("name", "fileImage");
            // Passo a class css de formatação padrão. <input class="form-control" />
            inputFile.AddCssClass("form-control");
            // Preencho o atributo que define o tipo do elemento <input type="file" />
            inputFile.MergeAttribute("type", "file");
            // Se o elemento <img /> for aparecer, passo a função javascript.
            if (displayImage)
                inputFile.MergeAttribute("onchange", "show(this, 'imgSelected')");

            divFileGroup.InnerHtml = htmlHelper.LabelFor(expression, new { @class = "control-label col-md-2" }).ToString();
            divFileCol10.InnerHtml = inputFile.ToString();
            divFileCol10.InnerHtml += htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            // Se for alteravel o arquivo
            if (changeFile)
                // Crio um elemento para apresentar o arquivo anterior
                divFileCol10.InnerHtml += htmlHelper.EditorFor(expression, new { htmlAttributes = new { @class = "form-control locked", @type = "text", disabled = "disabled" } });

            // Crio um elemento oculto para guardar o valor anterior, caso não seja selecionado nenhum arquivo novo.
            divFileCol10.InnerHtml += htmlHelper.Hidden(metadata.PropertyName, metadata.Model ?? "sem imagem");

            divFileGroup.InnerHtml += divFileCol10.ToString();

            divGeneral.InnerHtml = divImageGroup.ToString();
            divGeneral.InnerHtml += divFileGroup.ToString();

            // Retorna o elemento criado
            return new MvcHtmlString(divGeneral.ToString());
        }

        /// <summary>
        /// Gera um elemento botão nos formatos padrões.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="typeHtmlButton">Tipo de botão a ser exibido</param>
        /// <param name="additionalArguments">Valor que o elemento vai receber</param>
        /// <returns></returns>
        public static MvcHtmlString Button(this HtmlHelper htmlHelper, TypeHtmlButton typeHtmlButton, object additionalArguments)
        {
            if (typeHtmlButton == TypeHtmlButton.Submit || typeHtmlButton == TypeHtmlButton.Button)
            {
                var divFormGroup = new TagBuilder("div");
                divFormGroup.AddCssClass("form-group");

                var atributos = Utilities.GetValuesAnonymous(additionalArguments);

                var divCols = new TagBuilder("div");

                var inputButton = new TagBuilder("input");
                inputButton.AddCssClass("btn btn-default");
                if (typeHtmlButton == TypeHtmlButton.Submit)
                    inputButton.MergeAttribute("type", "submit");
                if (typeHtmlButton == TypeHtmlButton.Button)
                    inputButton.MergeAttribute("type", "button");

                foreach (var atributo in atributos)
                {
                    if (atributo.Key == "@class")
                        inputButton.AddCssClass(atributo.Value.ToString());
                    inputButton.MergeAttribute(atributo.Key, atributo.Value.ToString());
                }

                divCols.InnerHtml += inputButton;
                divFormGroup.InnerHtml = divCols.ToString();

                return new MvcHtmlString(divFormGroup.ToString());
            }
            else
            {
                var divButtonGroup = new TagBuilder("div");
                divButtonGroup.AddCssClass("btn-group");

                var button = new TagBuilder("button");
                button.MergeAttribute("type", "button");
                button.MergeAttribute("data-toggle", "dropdown");
                button.MergeAttribute("aria-haspopup", "true");
                button.MergeAttribute("aria-expanded", "false");
                button.AddCssClass("btn btn-default dropdown-toggle");

                var spanButton = new TagBuilder("span");
                spanButton.AddCssClass("caret");

                button.InnerHtml = "Opções " + spanButton;

                divButtonGroup.InnerHtml += button;

                var dropDownMenu = new TagBuilder("ul");
                dropDownMenu.AddCssClass("dropdown-menu");

                var edit = new TagBuilder("li")
                {
                    // ReSharper disable once Mvc.ActionNotResolved
                    InnerHtml = htmlHelper.ActionLink("Editar", "Edit", additionalArguments).ToString()
                };

                var details = new TagBuilder("li")
                {
                    // ReSharper disable once Mvc.ActionNotResolved
                    InnerHtml = htmlHelper.ActionLink("Visualizar", "Details", additionalArguments).ToString()
                };

                var delete = new TagBuilder("li")
                {
                    // ReSharper disable once Mvc.ActionNotResolved
                    InnerHtml = htmlHelper.ActionLink("Excluir", "Delete", additionalArguments).ToString()
                };

                dropDownMenu.InnerHtml += edit;
                dropDownMenu.InnerHtml += details;
                dropDownMenu.InnerHtml += delete;

                divButtonGroup.InnerHtml += dropDownMenu;

                return new MvcHtmlString(divButtonGroup.ToString());
            }
        }
    }

    /// <summary>
    /// Enumeração para específicar o tipo de botão e sua formatação será apresentado.
    /// </summary>
    public enum TypeHtmlButton
    {
        [Description("Cria um botão de opções em uma dropdownlist Editar, Visualizar, Excluir")]
        Table,
        [Description("Cria um botão para executar formulários")]
        Submit,
        [Description("Cria um botão normal")]
        Button
    }
}
