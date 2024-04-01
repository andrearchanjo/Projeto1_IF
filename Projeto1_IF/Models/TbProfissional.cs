﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Projeto1_IF.Models;

[Table("tbProfissional")]
[Index("IdCidade", Name = "IX_tbProfissional_IdCidade")]
[Index("IdContrato", Name = "IX_tbProfissional_IdContrato")]
[Index("IdTipoAcesso", Name = "IX_tbProfissional_IdTipoAcesso")]
public partial class TbProfissional
{
    [Key]
    public int IdProfissional { get; set; }

    [Display(Name = "Área de Atuação")]
    public int? IdTipoProfissional { get; set; }

    public int IdContrato { get; set; }

    [Display(Name = "Tipo de Acesso")]
    public int? IdTipoAcesso { get; set; }

    [Display(Name = "Cidade")]
    public int IdCidade { get; set; }

    [Required]
    [StringLength(128)]
    public string IdUser { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Nome { get; set; }

    [Required]
    [Column("CPF")]
    [StringLength(15)]
    [Unicode(false)]
    public string Cpf { get; set; }

    [Column("CRM_CRN")]
    [StringLength(20)]
    [Unicode(false)]
    public string CrmCrn { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Especialidade { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Logradouro { get; set; }

    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string Numero { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Bairro { get; set; }

    [Required]
    [Column("CEP")]
    [StringLength(10)]
    [Unicode(false)]
    public string Cep { get; set; }

    [Column("DDD1")]
    [StringLength(2)]
    [Unicode(false)]
    public string Ddd1 { get; set; }

    [Column("DDD2")]
    [StringLength(2)]
    [Unicode(false)]
    public string Ddd2 { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string Telefone1 { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string Telefone2 { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Salario { get; set; }

    [ForeignKey("IdCidade")]
    [InverseProperty("TbProfissional")]
    public virtual TbCidade IdCidadeNavigation { get; set; }

    [ForeignKey("IdContrato")]
    [InverseProperty("TbProfissional")]
    public virtual TbContrato IdContratoNavigation { get; set; }

    [ForeignKey("IdTipoAcesso")]
    [InverseProperty("TbProfissional")]
    public virtual TbTipoAcesso IdTipoAcessoNavigation { get; set; }

    [InverseProperty("IdProfissionalNavigation")]
    public virtual ICollection<TbHoraPacienteProfissional> TbHoraPacienteProfissional { get; set; } = new List<TbHoraPacienteProfissional>();

    [InverseProperty("IdProfissionalNavigation")]
    public virtual ICollection<TbMedicoPaciente> TbMedicoPaciente { get; set; } = new List<TbMedicoPaciente>();

    [InverseProperty("IdProfissionalNavigation")]
    public virtual ICollection<TbPergunta> TbPergunta { get; set; } = new List<TbPergunta>();

    [InverseProperty("IdProfissionalNavigation")]
    public virtual ICollection<TbReceitaAlimentarPadrao> TbReceitaAlimentarPadrao { get; set; } = new List<TbReceitaAlimentarPadrao>();

    [InverseProperty("IdProfissionalNavigation")]
    public virtual ICollection<TbReceitaMedicaPadrao> TbReceitaMedicaPadrao { get; set; } = new List<TbReceitaMedicaPadrao>();
}