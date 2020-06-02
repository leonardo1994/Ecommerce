using ECommerce.Web.Models.EntPedido;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("LOCAIS")]
    public class Local
    {
        [Key]
        [MaxLength(3)]
        [Column("cod_local", TypeName = "nvarchar")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [MaxLength(40)]
        [Column("des_local", TypeName = "nvarchar")]
        public string Descricao { get; set; }

        [MaxLength(1)]
        [Column("tipo_local", TypeName = "nvarchar")]
        public string TipoLocal { get; set; }

        [MaxLength(1)]
        [Column("status_local", TypeName = "nvarchar")]
        public string StatusLocal { get; set; }

        public virtual ICollection<Estoque> Estoques { get; set; }

        public virtual ICollection<ItensPedido> ItensPedido { get; set; }
    }
}
