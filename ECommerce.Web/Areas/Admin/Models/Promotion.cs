using ECommerce.Web.Models.EntPedido;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Areas.Admin.Models
{
    /// <summary>
    /// Promoções de produtos
    /// </summary>
    [Table("Promocao")]
    public class Promocao
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Código")]
        public string Codigo { get; set; }

        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DataType(DataType.ImageUrl)]
        [Required(AllowEmptyStrings = true)]
        [DisplayName("Selecione uma imagem")]
        public string Imagem { get; set; }

        [Required]
        [DisplayName("Tipo de exibição")]
        public TipoDeExibicao TipoExibicao { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Data de início")]
        public DateTime DataInicial { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayName("Hora de início")]
        public DateTime HoraInicial { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Data de Fim")]
        public DateTime DataFinal { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayName("Hora de Fim")]
        public DateTime HoraFinal { get; set; }

        [DisplayName("Ativo")]
        public bool Ativo { get; set; }

        [DisplayName("Promoção válida para todos os Itens (a listagem abaixo é desconsiderada)")]
        public bool TodosItens { get; set; }

        [DisplayName("Informe se o descont será em Real (R$) ou Porcentagem (%)")]
        public TipoValor TipoValor { get; set; }

        [DisplayName("Inoforme o desconto")]
        public double Valor { get; set; }

        [DisplayName("Promoção válida somente para novos usuários")]
        public bool NovosUsuarios { get; set; }

        [DisplayName("Deseja publicar no site ?")]
        public bool Publicar { get; set; }

        /// <summary>
        /// Grupo de itens da promoção.
        /// Ex.: Promoção de 20%.
        /// Adicionar itens que se enquadram a este 20%;
        /// </summary>
        public virtual ICollection<PromocaoItem> PromocaoItems { get; set; } = new List<PromocaoItem>();

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }

    [Table("PromocaoItem")]
    public class PromocaoItem
    {
        [Key]
        public int Id { get; set; }
        
        public int PromocaoId { get; set; }
        public virtual Promocao Promocao { get; set; }

        [MaxLength(13)]
        public string ProdutoMontadoId { get; set; }
        public virtual ProdutoMontado ProdutoMontado { get; set; }

        public int TabelaDePrecoId { get; set; }
        public virtual TabelaPreco TabelaDePreco { get; set; }
    }

    public enum TipoDeExibicao
    {
        [Display(Name = "Anúncio no topo")]
        Topo,
        [Display(Name = "Banner inicial - Pop Up")]
        PopUp,
        [Display(Name = "Cupom de desconto")]
        CupomDesconto
        //[Display(Name = "Anúncio a esquerda")]
        //AdLeft,
        //[Display(Name = "Anúncio a direita")]
        //AdRight,
        //[Display(Name = "Anúncio no centro")]
        //AdCenter,
        //[Display(Name = "Anúncio no rodapé")]
        //AdButton,
    }

    public enum TipoValor
    {
        [Display(Name = "Porcentual %")]
        Porcentual,
        [Display(Name = "Moeda R$")]
        Valor
    }
}