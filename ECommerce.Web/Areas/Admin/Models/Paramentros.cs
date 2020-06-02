using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("param")]
    public class Paramentros
    {
        [Key]
        public int Id { get; set; }

        public string local_loja { get; set; }
    }
}