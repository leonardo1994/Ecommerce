using ECommerce.Web.Models.EntPedido;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ECommerce.Web.HtmlHelpers.Validators;

namespace ECommerce.Web.Models
{
    [Table("CADCLI")]
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("cod_cliente")]
        [StringLength(10)]
        public string Id { get; set; }

        [Column("raz_cliente")]
        public string Nome { get; set; }

        [CpfValidator]
        [Column("cnpj_cpf_cliente")]
        public string Cpf { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DataNascimento { get; set; }

        public string Sexo { get; set; }

        [Column("ddd_cliente")]
        public string DddFixo { get; set; }

        [Column("tel_cliente")]
        public string TelefoneFixo { get; set; }

        [Column("ddd_tel2_cliente")]
        public string DddCelular { get; set; }

        [Column("tel2_cliente")]
        public string TelefoneCelular { get; set; }

        [Column("cep_cliente")]
        public string Cep { get; set; }

        [Column("des_endereco")]
        public string Rua { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        [Column("des_bairro")]
        public string Bairro { get; set; }

        [Column("des_cidade")]
        public string Cidade { get; set; }

        [Column("uf_cliente")]
        public string Estado { get; set; }

        [Column("email_cliente")]
        public string Email { get; set; }

        [Column("des_enderecoc")]
        public string EnderecoC { get; set; }

        [Column("des_cidadec")]
        public string CidadeC { get; set; }

        [Column("des_bairroc")]
        public string BairroC { get; set; }

        [Column("ufc_cliente")]
        public string UfC { get; set; }

        [Column("cepc_cliente")]
        public string CepC { get; set; }

        [Column("des_enderecoe")]
        public string EnderecoE { get; set; }

        [Column("des_cidadee")]
        public string CidadeE { get; set; }

        [Column("des_bairroe")]
        public string BairroE { get; set; }

        [Column("ufe_cliente")]
        public string UfE { get; set; }

        [Column("cepe_cliente")]
        public string CepE { get; set; }

        [Column("des_contato")]
        public string Contato { get; set; }

        [Column("dt_inicio", TypeName = "date")]
        public DateTime? DataCadastro { get; set; }

        [Column("cod_vendedor")]
        public string Vendedor { get; set; } = "000";

        [Column("cod_regiao")]
        public string Regiao { get; set; } = "SP";

        [Column("tipo_cliente")]
        public string TipoCliente { get; set; } = "F";

        [Column("lim_credito")]
        public double Credito { get; set; } = 99999.99;

        [Column("cod_atividade")]
        public string Atividade { get; set; } = "008";

        [Column("grupo_cliente")]
        public string Grupo { get; set; } = "006";

        [Column("status_cliente")]
        public string Status { get; set; } = "A";

        [Column("numero_cliente")]
        public string NumeroCliente { get; set; }

        [Column("numeroc_cliente")]
        public string NumeroC { get; set; }

        [Column("numeroe_cliente")]
        public string NumeroE { get; set; }

        [Column("usuario_gravacao")]
        public string UsuarioGravacao { get; set; } = "Loja Virtual";

        [Column("horario_gravacao")]
        public string HorarioGravacao { get; set; } = DateTime.Now.ToString("MM/dd/yy hh:mm");

        [Column("usuario_alteracao")]
        public string UsuarioAlteracao { get; set; } = "Loja Virtual";

        [Column("horario_alteracao")]
        public string HorarioAlteracao { get; set; } = DateTime.Now.ToString("MM/dd/yy hh:mm");

        [Column("opt_exportacao")]
        public string OptExportacao { get; set; } = "N";

        [Column("paisf")]
        public string PaisF { get; set; } = "1058";

        [Column("paisC")]
        public string PaisC { get; set; } = "1058";

        [Column("paisE")]
        public string PaisE { get; set; } = "1058";

        [Column("npaisF")]
        public string NpaisF { get; set; } = "BASIL";

        [Column("npaisC")]
        public string NpaisC { get; set; } = "BASIL";

        [Column("npaisE")]
        public string NpaisE { get; set; } = "BASIL";

        [Column("emailXml")]
        public string EmailXml { get; set; }

        [Column("opt_contri")]
        public string OptContri { get; set; } = "N";

        [Column("liberado_vendas")]
        public string LiberadoVendas { get; set; } = "S";

        [Column("usuario_liberado_vendas")]
        public string UsuarioLiberadoVendas { get; set; } = "Loja Virtual";

        [Column("horario_liberado_vendas")]
        public string HorarioLiberadoVendas { get; set; } = DateTime.Now.ToString("MM/dd/yy hh:mm");

        [Column("opt_terceiro")]
        public string OptTerceiro { get; set; } = "N";

        [Column("enviado_site")]
        public string EnviadoSite { get; set; } = "N";

        [Column("codigo_origem")]
        public string CodigoOrigem { get; set; } = "13";

        [Column("descr_origem")]
        public string DescricaoOrigem { get; set; } = "Loja Virtual";

        [Column("opt_evento")]
        public string Opt_Evento { get; set; } = "N";

        [Column("opt_maodeobra")]
        public string OptMaoDeObra { get; set; } = "N";

        public string Referencia { get; set; }

        [Column("codigo_mun_ibge")]
        public string codigo_num_ibge { get; set; }

        [Column("codigo_mun_ibgee")]
        public string codigo_num_ibgeE { get; set; }

        [Column("codigo_mun_ibgec")]
        public string codigo_num_ibgeC { get; set; }

        public string red_cliente { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
        public virtual ICollection<ItensPedido> ItensPedido { get; set; }

        public ClienteViewModels GetClienteViewModels()
        {
            Estado estado = 0;
            Sexo sexo = 0;
            Enum.TryParse(Estado, true, out estado);
            Enum.TryParse(Sexo, out sexo);

            return new ClienteViewModels
            {
                ApplicationUserId = ApplicationUserId,
                Bairro = Bairro?.ToUpper(),
                Cep = Cep,
                Cidade = Cidade?.ToUpper(),
                Complemento = Complemento?.ToUpper(),
                Cpf = Cpf,
                DataNascimento = DataNascimento.Value,
                DddCelular = DddCelular,
                DddFixo = DddFixo,
                Email = Email,
                Estado = estado,
                Id = Id,
                Nome = Nome?.ToUpper(),
                Numero = Numero,
                Rua = Rua?.ToUpper(),
                Sexo = sexo,
                TelefoneCelular = TelefoneCelular,
                TelefoneFixo = TelefoneFixo,
                Ibge = codigo_num_ibge,
                red_cliente = red_cliente?.ToUpper()
            };
        }
    }

    public enum Sexo
    {
        Masculino,
        Feminino
    }

    public enum Estado
    {
        AC,
        AL,
        AP,
        AM,
        BA,
        CE,
        DF,
        ES,
        GO,
        MA,
        MT,
        MS,
        MG,
        PA,
        PB,
        PR,
        PE,
        PI,
        RJ,
        RS,
        RO,
        RR,
        SC,
        SP,
        SE,
        TO
    }
}