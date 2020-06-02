using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("ProdutoGrade")]
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

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

        public int QtdVisitada { get; set; }

        public int QtdVendida { get; set; }

        public virtual ICollection<ProdutoTamanho> ProdutoTamanhos { get; set; }
        public virtual ICollection<ProdutoMontado> ProdutosMontado { get; set; }
    }

    [Table("ProdutoCor")]
    public class ProdutoCor
    {
        [Key]
        public int Id { get; set; }

        public int ProdutoTamanhoId { get; set; }
        public virtual ProdutoTamanho ProdutoTamanho { get; set; }

        public int CorId { get; set; }
        public virtual Cor Cor { get; set; }

        public virtual ICollection<ProdutoImagem> ProdutoImagens { get; set; }
        public virtual ICollection<ProdutoMontado> ProdutosMontado { get; set; }
    }

    [Table("ProdutoTamanho")]
    public class ProdutoTamanho
    {
        [Key]
        public int Id { get; set; }

        [Column("ID_GRADE")]
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }

        public int TamanhoId { get; set; }
        public virtual Tamanho Tamanho { get; set; }

        public virtual ICollection<ProdutoCor> ProdutoCores { get; set; }
        public virtual ICollection<ProdutoMontado> ProdutosMontado { get; set; }
    }

    [Table("ProdutoImagem")]
    public class ProdutoImagem
    {
        [Key]
        public int Id { get; set; }

        public string Imagem { get; set; }

        public string Imagem2 { get; set; }

        public TipoArquivo TipoArquivo { get; set; }

        public int ProdutoCorId { get; set; }
        public virtual ProdutoCor ProdutoCor { get; set; }
    }

    public enum TipoArquivo
    {
        Imagem,
        Video
    }
}