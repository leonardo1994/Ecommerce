using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("ficha_tecnica")]
    public class FichaTecnica
    {
        [Key]
        [StringLength(12)]
        [Column(name: "cod_ficha")]
        public string Codigo { get; set; }

        [Column(name: "des_ficha")]
        public string Descricao { get; set; }

        [Column(name: "imagem")]
        public string Imagem { get; set; }

        [Column(name: "descricao")]
        public string Informacoes { get; set; }

        [Column(name: "beneficio")]
        public string Beneficio { get; set; }

        [Column(name: "lavagem")]
        public string Lavagem { get; set; }

        [Column(name: "advertencia")]
        public string Advertencia { get; set; }

        [Column(name: "caracteristica")]
        public string Caracteristica { get; set; }

        [Column(name: "finalidade")]
        public string Finalidade { get; set; }

        [Column(name: "indicacao")]
        public string Indicacao { get; set; }

        [Column(name: "composicao")]
        public string Composicacao { get; set; }

        [Column(name: "certificado")]
        public string Certificado { get; set; }

        [Column(name: "texto_comercial")]
        public string Comercial { get; set; }

        [Column(name: "embalagem_grafica")]
        public string EmbalagemGrafica { get; set; }

        public virtual ICollection<ProdutoMontado> ProdutoMontado { get; set; }
    }
}