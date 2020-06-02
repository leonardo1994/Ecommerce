using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("GRUPOS")]
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Código Categoria")]
        [Column("cod_grupo")]
        [MaxLength(12)]
        public string Id { get; set; }

        [DisplayName("Descrição")]
        [Column("des_grupo")]
        public string Descricao { get; set; }

        public string Icone { get; set; }

        public string Publicar { get; set; }

        [DisplayName("Status")]
        [Column("status_grupo")]
        public string Status { get; set; }

        public virtual ICollection<SubCategoria> SubCategorias { get; set; }
        public virtual ICollection<ProdutoMontado> ProdutosMontado { get; set; }
        public virtual ICollection<Produto> Produtos { get; set; }

    }

    [Table("SGRUPOS")]
    public class SubCategoria
    {
        [MaxLength(12)]
        [Column("cod_grupo")]
        public string CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }

        [Column("cod_sgrupo")]
        [DisplayName("Código")]
        [MaxLength(12)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [DisplayName("Descrição")]
        [Column("des_sgrupo")]
        public string Descricao { get; set; }

        public string Publicar { get; set; }

        [DisplayName("Status")]
        [Column("status_sgrupo")]
        public string Status { get; set; }

        public virtual ICollection<ProdutoMontado> ProdutosMontado { get; set; }
        public virtual ICollection<Produto> Produtos { get; set; }
    }
}
