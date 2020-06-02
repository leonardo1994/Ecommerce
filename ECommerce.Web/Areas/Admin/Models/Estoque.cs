using ECommerce.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ECommerce.Web.Areas.Admin.Models
{
    [Table("SALDO_MOVIMENTO")]
    public class Estoque
    {
        [Key]
        [Column("sequencia")]
        public int Id { get; set; }

        [Column("mov_anterior")]
        public double? MovAnterior { get; set; }

        [Column("emp_anterior")]
        public double? EmpAnterior { get; set; }

        [Column("cod_produto")]
        public string ProdutoMontadoId { get; set; }
        public virtual ProdutoMontado ProdutoMontado { get; set; }

        [Column("cod_local")]
        public string LocalId { get; set; }
        public virtual Local Local { get; set; }

        #region Janeiro
        [Column("mov_jan")]
        public double? MovJaneiro { get; set; }

        [Column("ent_jan")]
        public double? EntJaneiro { get; set; }

        [Column("sai_jan")]
        public double? SaidaJaneiro { get; set; }

        [Column("emp_jan")]
        public double? EmpJaneiro { get; set; }

        [Column("empe_jan")]
        public double? EmpeJaneiro { get; set; }

        [Column("emps_jan")]
        public double? EmpsJaneiro { get; set; }

        [Column("final_jan")]
        public double? FinalJaneiro { get; set; }
        #endregion
        #region Fevereiro
        [Column("mov_fev")]
        public double? MovFevereiro { get; set; }

        [Column("ent_fev")]
        public double? EntFevereiro { get; set; }

        [Column("sai_fev")]
        public double? SaidaFevereiro { get; set; }

        [Column("emp_fev")]
        public double? EmpFevereiro { get; set; }

        [Column("empe_fev")]
        public double? EmpeFevereiro { get; set; }

        [Column("emps_fev")]
        public double? EmpsFevereiro { get; set; }

        [Column("final_fev")]
        public double? FinalFevereiro { get; set; }
        #endregion
        #region Marco
        [Column("mov_mar")]
        public double? MovMarco { get; set; }

        [Column("ent_mar")]
        public double? EntMarco { get; set; }

        [Column("sai_mar")]
        public double? SaidaMarco { get; set; }

        [Column("emp_mar")]
        public double? EmpMarco { get; set; }

        [Column("empe_mar")]
        public double? EmpeMarco { get; set; }

        [Column("emps_mar")]
        public double? EmpsMarco { get; set; }

        [Column("final_mar")]
        public double? FinalMarco { get; set; }
        #endregion
        #region Abril
        [Column("mov_abr")]
        public double? MovAbril { get; set; }

        [Column("ent_abr")]
        public double? EntAbril { get; set; }

        [Column("sai_abr")]
        public double? SaidaAbril { get; set; }

        [Column("emp_abr")]
        public double? EmpAbril { get; set; }

        [Column("empe_abr")]
        public double? EmpeAbril { get; set; }

        [Column("emps_abr")]
        public double? EmpsAbril { get; set; }

        [Column("final_abr")]
        public double? FinalAbril { get; set; }
        #endregion
        #region Maio
        [Column("mov_mai")]
        public double? MovMaio { get; set; }

        [Column("ent_mai")]
        public double? EntMaio { get; set; }

        [Column("sai_mai")]
        public double? SaidaMaio { get; set; }

        [Column("emp_mai")]
        public double? EmpMaio { get; set; }

        [Column("empe_mai")]
        public double? EmpeMaio { get; set; }

        [Column("emps_mai")]
        public double? EmpsMaio { get; set; }

        [Column("final_mai")]
        public double? FinalMaio { get; set; }
        #endregion
        #region Junho
        [Column("mov_jun")]
        public double? MovJunho { get; set; }

        [Column("ent_jun")]
        public double? EntJunho { get; set; }

        [Column("sai_jun")]
        public double? SaidaJunho { get; set; }

        [Column("emp_jun")]
        public double? EmpJunho { get; set; }

        [Column("empe_jun")]
        public double? EmpeJunho { get; set; }

        [Column("emps_jun")]
        public double? EmpsJunho { get; set; }

        [Column("final_jun")]
        public double? FinalJunho { get; set; }
        #endregion
        #region Julho
        [Column("mov_jul")]
        public double? MovJulho { get; set; }

        [Column("ent_jul")]
        public double? EntJulho { get; set; }

        [Column("sai_jul")]
        public double? SaidaJulho { get; set; }

        [Column("emp_jul")]
        public double? EmpJulho { get; set; }

        [Column("empe_jul")]
        public double? EmpeJulho { get; set; }

        [Column("emps_jul")]
        public double? EmpsJulho { get; set; }

        [Column("final_jul")]
        public double? FinalJulho { get; set; }
        #endregion
        #region Agosto
        [Column("mov_ago")]
        public double? MovAgosto { get; set; }

        [Column("ent_ago")]
        public double? EntAgosto { get; set; }

        [Column("sai_ago")]
        public double? SaidaAgosto { get; set; }

        [Column("emp_ago")]
        public double? EmpAgosto { get; set; }

        [Column("empe_ago")]
        public double? EmpeAgosto { get; set; }

        [Column("emps_ago")]
        public double? EmpsAgosto { get; set; }

        [Column("final_ago")]
        public double? FinalAgosto { get; set; }
        #endregion
        #region Setembro
        [Column("mov_set")]
        public double? MovSetembro { get; set; }

        [Column("ent_set")]
        public double? EntSetembro { get; set; }

        [Column("sai_set")]
        public double? SaidaSetembro { get; set; }

        [Column("emp_set")]
        public double? EmpSetembro { get; set; }

        [Column("empe_set")]
        public double? EmpeSetembro { get; set; }

        [Column("emps_set")]
        public double? EmpsSetembro { get; set; }

        [Column("final_set")]
        public double? FinalSetembro { get; set; }
        #endregion
        #region Outubro
        [Column("mov_out")]
        public double? MovOutubro { get; set; }

        [Column("ent_out")]
        public double? EntOutubro { get; set; }

        [Column("sai_out")]
        public double? SaidaOutubro { get; set; }

        [Column("emp_out")]
        public double? EmpOutubro { get; set; }

        [Column("empe_out")]
        public double? EmpeOutubro { get; set; }

        [Column("emps_out")]
        public double? EmpsOutubro { get; set; }

        [Column("final_out")]
        public double? FinalOutubro { get; set; }
        #endregion
        #region Novembro
        [Column("mov_nov")]
        public double? MovNovembro { get; set; }

        [Column("ent_nov")]
        public double? EntNovembro { get; set; }

        [Column("sai_nov")]
        public double? SaidaNovembro { get; set; }

        [Column("emp_nov")]
        public double? EmpNovembro { get; set; }

        [Column("empe_nov")]
        public double? EmpeNovembro { get; set; }

        [Column("emps_nov")]
        public double? EmpsNovembro { get; set; }

        [Column("final_nov")]
        public double? FinalNovembro { get; set; }
        #endregion
        #region Dezembro
        [Column("mov_dez")]
        public double? MovDezembro { get; set; }

        [Column("ent_dez")]
        public double? EntDezembro { get; set; }

        [Column("sai_dez")]
        public double? SaidaDezembro { get; set; }

        [Column("emp_dez")]
        public double? EmpDezembro { get; set; }

        [Column("empe_dez")]
        public double? EmpeDezembro { get; set; }

        [Column("emps_dez")]
        public double? EmpsDezembro { get; set; }

        [Column("final_dez")]
        public double? FinalDezembro { get; set; }
        #endregion
        #region Saldos

        [MaxLength(1)]
        [Column("c_trm", TypeName = "nvarchar")]
        public string Ctrm { get; set; }

        [MaxLength(20)]
        [Column("numero_lote", TypeName = "nvarchar")]
        public string NumeroLote { get; set; }

        [Column("saldoi")]
        public double? SaldoI { get; set; }

        [Column("saldo_alocado_inspecao")]
        public double? SaldoAlocadoInspecao { get; set; }

        [Column("saldo_alocado")]
        public double? SaldoAlocado { get; set; }

        [Column("saldo_processo")]
        public double? SaldoProcesso { get; set; }

        [Column("data_saldoinicial", TypeName = "date")]
        public DateTime? DataSaldoInicial { get; set; }

        [Column("saldo_devolucao")]
        public double? SaldoDevolucao { get; set; }

        [Column("saldo_alocado_processo")]
        public double? SaldoAlocadoProcesso { get; set; }
        #endregion

        /// <summary>
        /// Avalia o saldo do item de acordo com a disponibilidade atual.
        /// </summary>
        /// <param name="produtoMontadoId"></param>
        /// <returns>Caso não encontre o item retorna null</returns>
        public double AvaliaDisponibilidade(string produtoMontadoId)
        {
            var db = new ApplicationDbContext();
            var localLoja = db.Parametros.FirstOrDefault().local_loja;

            var result = (from est in db.Estoques.AsNoTracking()
             where est.ProdutoMontadoId == produtoMontadoId && est.LocalId == localLoja
                    select est
             ).Sum(est => (est.MovJaneiro + est.MovFevereiro + est.MovMarco + est.MovAbril + est.MovMaio + est.MovJunho + est.MovJulho + est.MovAgosto + est.MovSetembro + est.MovOutubro + est.MovNovembro + est.MovDezembro + est.MovAnterior) - est.SaldoAlocado) ?? 0;
            return result;
        } 
    }
}
