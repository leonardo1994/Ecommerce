using ECommerce.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerce.Web.HtmlHelpers
{
    public class Validators
    {
        public class CpfValidator : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                try
                {
                    if (value == null || string.IsNullOrEmpty(value.ToString())) return ValidationResult.Success;

                    var cpf = value as string;
                    var valor = cpf?.Replace(".", ""); valor = valor?.Replace("-", "");
                    if (valor?.Length != 11)
                        return new ValidationResult("Cpf inválido");
                    var igual = true;
                    for (var i = 1; i < 11 && igual; i++)
                        if (valor[i] != valor[0]) igual = false;
                    if (igual || valor == "12345678909")
                        return new ValidationResult("Cpf inválido");
                    var numeros = new int[11];
                    for (var i = 0; i < 11; i++) numeros[i] = int.Parse(valor[i].ToString());
                    var soma = 0;
                    for (var i = 0; i < 9; i++) soma += (10 - i) * numeros[i];
                    var resultado = soma % 11;
                    if (resultado == 1 || resultado == 0)
                    {
                        if (numeros[9] != 0) return new ValidationResult("Cpf inválido");
                    }
                    else if (numeros[9] != 11 - resultado) return new ValidationResult("Cpf inválido");
                    soma = 0;
                    for (var i = 0; i < 10; i++) soma += (11 - i) * numeros[i];
                    resultado = soma % 11;
                    if (resultado == 1 || resultado == 0)
                    {
                        if (numeros[10] != 0)
                            return new ValidationResult("Cpf inválido");
                    }
                    else if (numeros[10] != 11 - resultado) return new ValidationResult("Cpf inválido");

                    var userId = HttpContext.Current.User.Identity.GetUserId();

                    if (new ApplicationDbContext().Clientes.Any(c => c.Cpf == valor && c.ApplicationUserId != userId))
                    {
                        return new ValidationResult("Cpf já cadastrado");
                    }
                    else
                    {
                        return ValidationResult.Success;
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult("Não foi possível validar o cpf, tente novamente, certifique que é válido.");
                }
            }
        }
    }
}