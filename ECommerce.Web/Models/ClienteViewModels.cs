using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using static ECommerce.Web.HtmlHelpers.Validators;

namespace ECommerce.Web.Models
{
    public class ClienteViewModels
    {
        [HiddenInput]
        public string Id { get; set; }

        [Required(ErrorMessage ="Nome completo obrigatório")]
        [DisplayName("Nome Completo:")]
        public string Nome { get; set; }

        [CpfValidator]
        [Required(ErrorMessage = "CPF obrigatório")]
        [DisplayName("CPF:")]
        public string Cpf { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Data de nascimento obrigatório")]
        [DisplayName("Data de Nascimento:")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Sexo é obrigatório")]
        [DisplayName("Sexo:")]
        public Sexo Sexo { get; set; }

        [Required(ErrorMessage = "DDD do telefone fixo é obrigatório")]
        [DisplayName("DDD:")]
        [StringLength(maximumLength: 2, MinimumLength =2, ErrorMessage = "Mínimo é 2 digitos")]
        public string DddFixo { get; set; }

        [Required(ErrorMessage = "Telefone fixo é obrigatório")]
        [StringLength(maximumLength: 9, MinimumLength = 8, ErrorMessage = "Mínimo é 8 digitos")]
        [DisplayName("Telefone Fixo:")]
        public string TelefoneFixo { get; set; }

        [Required(ErrorMessage = "DDD do celular é obrigatório")]
        [DisplayName("DDD:")]
        [StringLength(maximumLength: 2, MinimumLength = 2, ErrorMessage = "Mínimo é 2 digitos")]
        public string DddCelular { get; set; }

        [Required(ErrorMessage = "Telefone celular é obrigatório")]
        [StringLength(maximumLength: 10, MinimumLength = 9, ErrorMessage = "Mínimo é 9 digitos")]
        [DisplayName("Telefone Celular:")]
        public string TelefoneCelular { get; set; }

        [Required(ErrorMessage = "Cep é obrigatório")]
        [DisplayName("CEP:")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "Rua é obrigatório")]
        [DisplayName("Rua:")]
        public string Rua { get; set; }

        [Required(ErrorMessage = "Número é obrigatório")]
        [DisplayName("Número:")]
        public string Numero { get; set; }

        public string Complemento { get; set; }

        [Required(ErrorMessage = "Bairro é obrigatório")]
        [DisplayName("Bairro:")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Cidade é obrigatório")]
        [DisplayName("Cidade:")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "Estado é obrigatório")]
        [DisplayName("Estado:")]
        public Estado Estado { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [DisplayName("E-mail:")]
        public string Email { get; set; }

        public string Ibge { get; set; }

        public string red_cliente { get; set; }

        [HiddenInput]
        public string ApplicationUserId { get; set; }

        public Cliente GetCliente()
        {
            return new Cliente
            {
                ApplicationUserId = ApplicationUserId,
                Bairro = Bairro?.ToUpper(),
                Cep = Cep,
                Cidade = Cidade?.ToUpper(),
                Complemento = Complemento?.ToUpper(),
                Cpf = Cpf,
                DataNascimento = DataNascimento,
                DddCelular = DddCelular,
                DddFixo = DddFixo,
                Email = Email,
                Estado = Estado.ToString().ToUpper(),
                Id = Id,
                Nome = Nome?.ToUpper(),
                Numero = Numero,
                Rua = Rua?.ToUpper(),
                Sexo = Sexo.ToString().ToUpper(),
                TelefoneCelular = TelefoneCelular,
                TelefoneFixo = TelefoneFixo,
                BairroC = Bairro?.ToUpper(),
                BairroE = Bairro?.ToUpper(),
                CepC = Cep,
                CepE = Cep,
                CidadeC = Cidade?.ToUpper(),
                CidadeE = Cidade?.ToUpper(),
                Contato = Nome?.Split(' ')[0]?.ToUpper(),
                EmailXml = Email,
                EnderecoC = Rua?.ToUpper(),
                EnderecoE = Rua?.ToUpper(),
                NumeroC = Numero,
                NumeroE = Numero,
                Referencia = Complemento?.ToUpper(),
                UfC = Estado.ToString().ToUpper(),
                UfE = Estado.ToString().ToUpper(),
                NumeroCliente = Numero,
                codigo_num_ibge = Ibge,
                codigo_num_ibgeC = Ibge,
                codigo_num_ibgeE = Ibge,
                red_cliente = Nome?.Split(' ')[0]?.ToUpper()
            };
        }
    }
}