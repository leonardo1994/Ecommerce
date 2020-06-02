using ECommerce.Web.Models.EntCarrinho;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("Tamanho")]
    public class Tamanho
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(5), Index(IsUnique = true)]
        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public virtual ICollection<ProdutoTamanho> ProdutoTamanhos { get; set; }
    }
}