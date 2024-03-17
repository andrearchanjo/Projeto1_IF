﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Projeto1_IF.Models;

[Table("tbHistoricoSocialAlimentar")]
[Index("IdPaciente", Name = "IX_tbHistoricoSocialAlimentar_IdPaciente")]
public partial class TbHistoricoSocialAlimentar
{
    [Key]
    public int IdHistSocialAlimentar { get; set; }

    public int IdPaciente { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Profissao { get; set; }

    public int? CargaHoraria { get; set; }

    public int? NroPessoasRes { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? RendaFamiliar { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Escolaridade { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string EstadoCivil { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string NomeCompraAlimento { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string NomeCozinhaAlimento { get; set; }

    public int? CompraFeita { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string NomeRealizaRefeicao { get; set; }

    public bool? FlgTabagismo { get; set; }

    public int? QtdTabagismoDia { get; set; }

    public bool? FlgEtilismo { get; set; }

    public int? QtdEtilismoDia { get; set; }

    public bool? FlgCafe { get; set; }

    public int? QtdCafeDia { get; set; }

    public bool? FlgPaiMaeHas { get; set; }

    public bool? FlgAvosHas { get; set; }

    public bool? FlgIrmaosHas { get; set; }

    public bool? FlgPaiMaeDiabetes { get; set; }

    public bool? FlgAvosDiabetes { get; set; }

    public bool? FlgIrmaosDiabetes { get; set; }

    public bool? FlgPaiMaeCancer { get; set; }

    public bool? FlgAvosCancer { get; set; }

    public bool? FlgIrmaosCancer { get; set; }

    public bool? FlgPaiMaeObesidade { get; set; }

    public bool? FlgAvosObesidade { get; set; }

    public bool? FlgIrmaosObesidade { get; set; }

    [ForeignKey("IdPaciente")]
    [InverseProperty("TbHistoricoSocialAlimentar")]
    public virtual TbPaciente IdPacienteNavigation { get; set; }
}