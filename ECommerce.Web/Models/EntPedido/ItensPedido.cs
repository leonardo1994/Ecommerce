using ECommerce.Web.Areas.Admin.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Models.EntPedido
{
    [Table("PEDIDOI")]
    public class ItensPedido
    {
        [Key]
        public int Sequencia { get; set; }

        [StringLength(12)]
        [Column("num_pedido")]
        public string PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }

        [Column("item_pedido")]
        public string ItemPedido { get; set; }

        [StringLength(10)]
        [Column("cod_cliente")]
        public string ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        [Column("cgc_cliente")]
        public string CpfCliente { get; set; }

        [StringLength(3)]
        [Column("Cod_Local")]
        public string LocalId { get; set; }
        public virtual Local Local { get; set; }

        [StringLength(13)]
        [Column("cod_produto")]
        public string ProdutoMontadoId { get; set; }
        public virtual ProdutoMontado ProdutoMontado { get; set; }

        [Column("quantidade")]
        public double Quantidade { get; set; }

        [Column("precounitario")]
        public double ValorUnitario { get; set; }

        [Column("valorTotal1")]
        public double ValorTotal1 { get; set; }

        [Column("ValorTotal")]
        public double ValorTotal { get; set; }

        [Column("percendesconto")]
        public double PercenDesconto { get; set; }

        [Column("cod_tabela")]
        public string CodTabelaPreco { get; set; }

        [Column("TabelaId")]
        public int? TabelaPrecoId { get; set; }
        public virtual TabelaPreco TabelaPreco { get; set; }

        [Column("des_produto")]
        public string DescricaoProduto { get; set; }

        public DateTime? data_entrega { get; set; }

        public DateTime? data_emissao { get; set; }

        public string red_cliente { get; set; }

        public string origem_cliente { get; set; } = "C";

        public string usuario_gravacao { get; set; } = "Loja Virtual";

        public string horario_gravacao { get; set; } = DateTime.Now.ToString("MM/dd/yy hh:mm");

        public string usuario_alteracao { get; set; } = "Loja Virtual";

        public string horario_alteracao { get; set; } = DateTime.Now.ToString("MM/dd/yy hh:mm");

        public DateTime? data_gravacao { get; set; }

        public DateTime? data_alteracao { get; set; }

        public string status_pedido { get; set; } = "N";

        public string tiponf { get; set; } = "001";

        public string gera_duplicata { get; set; } = "S";

        public DateTime? data_original { get; set; }

        public string aprovacao_financeiro { get; set; } = "A";

        public string aprovacao_comercial { get; set; } = "A";
    }
}