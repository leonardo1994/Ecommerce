using ECommerce.Web.Models.EntCarrinho;
using ECommerce.Web.Models.EntPedido;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("CADPROD")]
    public class ProdutoMontado
    {
        #region Campo Padrões
        [Key]
        [MaxLength(13)]
        [Column("cod_produto")]
        [DisplayName("Código do Produto")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [DisplayName("Descrição")]
        [Column("des_produto")]
        public string Descricao { get; set; }

        [Required]
        [DisplayName("Descrição 1")]
        [Column("des1_produto")]
        public string Descricao1 { get; set; }

        [DisplayName("Código do Grupo")]
        [Column("cod_grupo")]
        [MaxLength(12)]
        public string CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }

        [DisplayName("Código Sub Grupo")]
        [Column("cod_sgrupo")]
        [MaxLength(12)]
        public string SubCategoriaId { get; set; }
        public virtual SubCategoria SubCategoria { get; set; }

        [DisplayName("Composição")]
        [Column("obs_produto")]
        public string Obs { get; set; }

        [DisplayName("Especificação")]
        [Column("especificacao")]
        public string Especificacao { get; set; }

        [DisplayName("Status")]
        [Column("status_produto")]
        public string Status { get; set; }

        [StringLength(12)]
        [Column(name: "cod_ficha")]
        public string FichaTecnicaId { get; set; }
        [ForeignKey("FichaTecnicaId")]
        public virtual FichaTecnica FichaTecnica { get; set; }

        [Column(name: "cod_tipo")]
        public string CodTipo { get; set; }

        #endregion

        #region Campos Ecommerce
        public int? ProdutoTamanhoId { get; set; }
        public virtual ProdutoTamanho ProdutoTamanho { get; set; }

        public int? ProdutoCorId { get; set; }
        public virtual ProdutoCor ProdutoCor { get; set; }

        public int Publica { get; set; }

        [Column("teto_estoque")]
        public double TetoEstoque { get; set; }

        public int QtdVisitada { get; set; }

        public int QtdVendida { get; set; }

        [Column("id_grade")]
        public int? ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }

        /// <summary>
        /// Promoções que o produto participou.
        /// </summary>
        public virtual ICollection<PromocaoItem> PromocaoItems { get; set; }

        /// <summary>
        /// Tabela de preços do item
        /// </summary>
        public virtual ICollection<TabelaPreco> TabelaPrecos { get; set; }

        /// <summary>
        /// Movimentações do item no estoque
        /// </summary>
        public virtual ICollection<Estoque> Estoques { get; set; }

        public virtual ICollection<ItemCarrinho> ItensCarrinho { get; set; }

        public virtual ICollection<ItensPedido> ItensPedido { get; set; }

        public virtual ICollection<AvisoDisponibilidade> AvisoDisponibilidade { get; set; }

        public virtual ICollection<AvaliacaoProduto> AvaliacaoProduto { get; set; }

        [Column(name: "comprimento")]
        public double Comprimento { get; internal set; }

        [Column(name: "altura")]
        public double Altura { get; internal set; }

        [Column(name: "largura")]
        public double Largura { get; internal set; }

        [Column(name: "pesobruto")]
        public double PesoBruto { get; set; }
        #endregion
    }
}
