using ECommerce.Web.Models.EntCarrinho;
using ECommerce.Web.Models.EntPedido;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("PRODUTO_PRECO")]
    public class TabelaPreco
    {
        [Key]
        [Column("sequencia")]
        public int Id { get; set; }

        [Column("codigo_tabela")]
        public string Codigo { get; set; }
        
        [Column("descricao_tabela")]
        public string Descricao { get; set; }

        [Column("preco_max")]
        [DisplayName("Valor")]
        public double? Valor { get; set; }

        [Column(name: "opt_loja")]
        public int? OptLoja { get; set; }

        [DisplayName("Produto")]
        [Column("codigo_poduto")]
        public string ProdutoMontadoId { get; set; }
        public virtual ProdutoMontado ProdutoMontado { get; set; }

        /// <summary>
        /// Promoções que a tabela de preço participou
        /// </summary>
        public virtual ICollection<PromocaoItem> PromocaoItems { get; set; }

        public virtual ICollection<ItensPedido> ItensPedido { get; set; }

        public virtual ICollection<ItemCarrinho> ItensCarrinho { get; set; }
    }
}
