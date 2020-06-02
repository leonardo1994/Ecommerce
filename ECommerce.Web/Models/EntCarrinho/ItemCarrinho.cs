using ECommerce.Web.Areas.Admin.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ECommerce.Web.Models.EntCarrinho
{
    [Table("ItemCarrinho")]
    public class ItemCarrinho
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Data { get; set; }

        /// <summary>
        /// Propriedade utilizada para referenciar usuário anônimo que criou carrinho sem se cadastrar.
        /// </summary>
        public string AnonymousId { get; set; }

        /// <summary>
        /// Propriedade para guardar referencia do usuário Logado.
        /// </summary>
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [MaxLength(13)]
        public string ProdutoMontadoId { get; set; }
        public ProdutoMontado ProdutoMontado { get; set; }

        public int TabelaPrecoId { get; set; }
        public virtual TabelaPreco TabelaPreco { get; set; }

        public int? PromocaoId { get; set; }
        public virtual Promocao Promocao { get; set; }

        public Status Status { get; set; }

        [NotMapped]
        public double ValorUnitario { get
            {
                if (Promocao != null)
                    if (Promocao.TipoValor == TipoValor.Porcentual)
                        return (TabelaPreco.Valor.Value - ((TabelaPreco.Valor.Value / 100) * Promocao.Valor));
                    else
                        return TabelaPreco.Valor.Value - Promocao.Valor;
                return TabelaPreco.Valor.Value;
            }
        }

        [NotMapped]
        public double ValorTotalItem
        {
            get
            {
                if (Promocao != null)
                    if (Promocao.TipoValor == TipoValor.Porcentual)
                        return (TabelaPreco.Valor.Value - ((TabelaPreco.Valor.Value / 100) * Promocao.Valor)) * QuantidadeTotalItem;
                    else
                        return (TabelaPreco.Valor.Value - Promocao.Valor) * QuantidadeTotalItem;

                return TabelaPreco.Valor.Value * QuantidadeTotalItem;
            }
        }

        public int QuantidadeTotalItem { get; set; }
    }

    public enum Status
    {
        Alocado,
        Desalocado
    }
}