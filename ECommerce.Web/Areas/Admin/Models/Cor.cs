using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("Cor")]
    public class Cor
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20), Index(IsUnique = true)]
        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public string Hexadecimal { get; set; }

        public string HexadecimalCombinacao { get; set; }

        public string Imagem { get; set; }

        public virtual ICollection<ProdutoCor> ProdutoCores { get; set; }
    }
}
