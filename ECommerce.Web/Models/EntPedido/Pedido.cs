using ECommerce.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Models.EntPedido
{
    [Table("PEDIDOC")]
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("num_pedido")]
        [StringLength(12)]
        public string Id { get; set; }

        [StringLength(10)]
        [Column("cod_cliente")]
        public string ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        [Column("cgc_cliente")]
        public string CpfCliente { get; set; }

        [Column("data_emissao", TypeName = "date")]
        public DateTime DataEmissao { get; set; }

        [Column("cod_vendedor")]
        public string VendedorId { get; set; } = "000";

        [Column("valorpedido")]
        public double ValorPedido { get; set; }

        [Column("Num_Pedido_Cliente")]
        public string NumPedidoCliente { get; set; }

        [Column("cond_pagto")]
        public string PagamentoId { get; set; } = "034";

        [Column("cod_formapag")]
        public string FormaPagamentoId { get; set; }

        [Column("percencomissao")]
        public double PercenComissao { get; set; }

        [Column("des_condPagto")]
        public string DescricaoCondPagto { get; set; }

        [Column("cod_transportadora")]
        public string TransportadoraId { get; set; } = "09582";

        [Column("valor_frete")]
        public double ValorFrete { get; set; }

        [Column("enderecoe")]
        public string Endereco { get; set; }

        [Column("bairroe")]
        public string Bairro { get; set; }

        [Column("cidadee")]
        public string Cidade { get; set; }

        [Column("ufe")]
        public string Uf { get; set; }

        [Column("enderecoe_compl")]
        public string Complemento { get; set; }

        [Column("numeroe")]
        public string Numero { get; set; }

        [Column("cepee")]
        public string Cep { get; set; }

        [Column("Ibge")]
        public string Ibge { get; set; }

        public string red_cliente { get; set; }

        public string opt_frete { get; set; } = "0";

        public string usuario_gravacao { get; set; } = "Loja Virtual";

        public string horario_gravacao { get; set; } = DateTime.Now.ToString("MM/dd/yy hh:mm");

        public DateTime? data_gravacao { get; set; }

        public DateTime? data_alteracao { get; set; }

        public string horario_alteracao { get; set; } = DateTime.Now.ToString("MM/dd/yy hh:mm");

        #region Dados Cartão de Crédito
        [NotMapped]
        [DisplayName("Nome do Titular")]
        public string NomeTitular { get; set; }

        [DisplayName("Parcelamento")]
        public string Parcelamento { get; set; }

        [NotMapped]
        [DisplayName("Número do Cartão")]
        public string NumeroCartao { get; set; }

        [NotMapped]
        [DisplayName("Mês")]
        public int? ValidadeMes { get; set; }

        [NotMapped]
        [DisplayName("Ano")]
        public int? ValidadeAno { get; set; }

        [NotMapped]
        [DisplayName("Código de segurança")]
        public int? CodigoSeguranca { get; set; }
        #endregion

        [Column("des_tipoped")]
        public string TipoVenda { get; set; } = "Pedido de Vendas";

        [Column("origem_cliente")]
        public string OrigemCliente { get; set; } = "C";

        [Column("cod_tipoped")]
        public string CodTipoPedido { get; set; } = "P";

        [Column("cod_empresa")]
        public string codEmpresa { get; set; } = "01";

        [Column("red_transportadora")]
        public string Transportadora { get; set; } = "CORREIOS";

        [Column("valorpedido1")]
        public double ValorPedido1 { get; set; }

        public string RefereciaPagSeguro { get; set; }

        public string red_vendedor { get; set; } = "PADRÃO";

        public string des_formapag { get; set; }

        public string status_pedido { get; set; } = "N";

        public string opt_entrega { get; set; } = "S";

        public string usuario_aprovacao { get; set; } = "PagSeguro";

        public string usuario_comercial { get; set; } = "PagSeguro";

        public string aprovacao_financeiro { get; set; }

        public string aprovacao_comercial { get; set; } = "A";

        public string horario_aprovacao { get; set; } = DateTime.Now.ToString("MM/dd/yy hh:dd");

        public string horario_comercial { get; set; } = DateTime.Now.ToString("MM/dd/yy hh:dd");

        public string opt_categoria { get; set; } = "F";

        public string lbl_categoria { get; set; } = "NÃO CONTRIBUINTE";

        public string opt_revenda { get; set; } = "C";

        public string opt_faturamento { get; set; } = "S";

        [Column(TypeName = "date")]
        public DateTime? prazo_entrega { get; set; }

        public virtual ICollection<ItensPedido> ItensPedido { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescricao { get; set; }
        public string CodigoTransacao { get; set; }
        public string UrlPagamento { get; set; }
        public string PagSeguroPgtoCodigo { get; set; }
        public string PagSeguroPgtoDescricao { get; set; }
        public string LinkPagamento { get; set; }
        public DateTime? DataEmissaoPagSeguro { get; set; }

        public int? PromocaoId { get; set; }
        public virtual Promocao Promocao { get; set; }
    }
}