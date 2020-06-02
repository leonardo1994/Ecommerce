using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using ECommerce.Web.Areas.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ECommerce.Web.Models.EntCarrinho;
using ECommerce.Web.Models.EntPedido;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [NotMapped]
        public Cliente Cliente { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        //: base(@"Data Source=177.105.119.175;Initial Catalog=FAMARA;User ID=sa;Password=Sql@2>0<1*4")
        //: base(@"Data Source=177.105.119.175;Initial Catalog=LOJA_VIRTUAL;User ID=sa;Password=Sql@2>0<1*4")
        : base(@"Data Source=FHSRVFS01;Initial Catalog=LOJA_VIRTUAL;User ID=sa;Password=Sql@2>0<1*4")
        //: base(@"Data Source=DELL-LEO\SQLEXPRESS;Initial Catalog=LOJAVIRTUAL;Integrated Security=True")
        //: base(@"Data Source=FHSRVFS01;Initial Catalog=FAMARA;User ID=sa;Password=Sql@2>0<1*4")
        {
        }

        public virtual DbSet<Promocao> Promocoes { get; set; }

        public virtual DbSet<ProdutoMontado> ProdutosMontados { get; set; }

        public virtual DbSet<TabelaPreco> TabelaPrecos { get; set; }

        public virtual DbSet<PromocaoItem> PromocaoItems { get; set; }

        public virtual DbSet<Categoria> Categorias { get; set; }

        public virtual DbSet<SubCategoria> SubCategorias { get; set; }

        public virtual DbSet<Produto> Produtos { get; set; }

        public virtual DbSet<Cor> Cores { get; set; }

        public virtual DbSet<ProdutoCor> ProdutoCores { get; set; }

        public virtual DbSet<Tamanho> Tamanhos { get; set; }

        public virtual DbSet<ProdutoTamanho> ProdutoTamanhos { get; set; }

        public virtual DbSet<ProdutoImagem> ProdutoImagens { get; set; }

        public virtual DbSet<Local> Locais { get; set; }

        public virtual DbSet<Estoque> Estoques { get; set; }

        public virtual DbSet<ItemCarrinho> ItemCarrinhos { get; set; }

        public virtual DbSet<Cliente> Clientes { get; set; }

        public virtual DbSet<Pedido> Pedidos { get; set; }

        public virtual DbSet<ItensPedido> ItensPedido { get; set; }

        public virtual DbSet<AvisoDisponibilidade> AvisoDisponibilidade { get; set; }

        public virtual DbSet<FichaTecnica> FichaTecnica { get; set; }

        public virtual DbSet<AvaliacaoProduto> AvaliacaoProduto { get; set; }

        public virtual DbSet<Paramentros> Parametros { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TabelaPreco>()
                .HasRequired(c => c.ProdutoMontado)
                .WithMany(c => c.TabelaPrecos)
                .HasForeignKey(c => c.ProdutoMontadoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Produto>()
                .HasMany(p => p.ProdutoTamanhos)
                .WithRequired(p => p.Produto)
                .HasForeignKey(p => p.ProdutoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Produto>()
                .HasMany(p => p.ProdutosMontado)
                .WithRequired(p => p.Produto)
                .HasForeignKey(p => p.ProdutoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProdutoTamanho>()
                .HasRequired(p => p.Produto)
                .WithMany(p => p.ProdutoTamanhos)
                .HasForeignKey(p => p.ProdutoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProdutoCor>()
                .HasRequired(p => p.ProdutoTamanho)
                .WithMany(p => p.ProdutoCores)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProdutoImagem>()
                .HasRequired(p => p.ProdutoCor)
                .WithMany(p => p.ProdutoImagens)
                .HasForeignKey(p => p.ProdutoCorId);

            modelBuilder.Entity<ProdutoMontado>()
                .HasOptional(c => c.ProdutoTamanho)
                .WithMany(c => c.ProdutosMontado)
                .HasForeignKey(c => c.ProdutoTamanhoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProdutoMontado>()
                .HasOptional(c => c.ProdutoCor)
                .WithMany(c => c.ProdutosMontado)
                .HasForeignKey(c => c.ProdutoCorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProdutoMontado>()
                .HasOptional(c => c.Categoria)
                .WithMany(c => c.ProdutosMontado)
                .HasForeignKey(c => c.CategoriaId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProdutoMontado>()
                .HasOptional(c => c.SubCategoria)
                .WithMany(c => c.ProdutosMontado)
                .HasForeignKey(c => c.SubCategoriaId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AvisoDisponibilidade>()
                .HasRequired(x => x.ProdutoMontado)
                .WithMany(c => c.AvisoDisponibilidade)
                .HasForeignKey(c => c.ProdutoMontadoId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<FichaTecnica>()
                .HasMany(c => c.ProdutoMontado)
                .WithOptional(c => c.FichaTecnica)
                .HasForeignKey(c => c.FichaTecnicaId)
                .WillCascadeOnDelete(false);


            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}