using ECommerce.Web.Areas.Admin.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Web.Models.EntCarrinho
{
    public class AvisoDisponibilidade
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MaxLength(13)]
        public string ProdutoMontadoId { get; set; }
        public virtual ProdutoMontado ProdutoMontado { get; set; }
    }
}