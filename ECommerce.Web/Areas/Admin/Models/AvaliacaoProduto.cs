using ECommerce.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("AvaliacaoProduto")]
    public class AvaliacaoProduto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Título é obrigatório")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Nota é obrigatório")]
        public int Nota { get; set; }

        [Required(ErrorMessage = "Recomendaria é obrigatório")]
        public Recomendaria Recomendaria { get; set; }

        [Required(ErrorMessage = "Opnião é obrigatório")]
        public string Opniao { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Data { get; set; }

        public Recomendaria Publica { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        public Tipo Tipo { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [StringLength(13)]
        public string ProdutoMontadoId { get; set; }
        public virtual ProdutoMontado ProdutoMontado { get; set; }
    }

    public enum Tipo
    {
        [Display(Description = "Avaliação")]
        Avaliacao,
        [Display(Description = "Sugestão")]
        Sugestao,
        [Display(Description = "Reclamação")]
        Reclamacao,
        [Display(Description = "Outros")]
        Outros

    }
}

public enum Recomendaria
{
    Sim,
    Nao
}