﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Projeto1_IF.Models;

public partial class ProfissionalResumido
{
    public int IdProfissional { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Nome { get; set; }

    [Display(Name="Plano")]
    public string NomePlano { get; set; }

    [Display(Name = "Cidade")]
    public string NomeCidade { get; set; }

    [Required]
    [StringLength(15)]
    [Unicode(false)]
    [Display(Name = "CPF")]
    public string Cpf { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    [Display(Name = "CRM / CRN")]
    public string CrmCrn { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Especialidade { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    [Display(Name = "Rua")]
    public string Logradouro { get; set; }

    [Required]
    [StringLength(10)]
    [Unicode(false)]
    [Display(Name = "Número")]
    public string Numero { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    [Display(Name = "Bairro")]
    public string Bairro { get; set; }

    [Required]
    [StringLength(10)]
    [Unicode(false)]
    [Display(Name = "CEP")]
    public string Cep { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string Ddd1 { get; set; }

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
    [Display(Name = "Salário")]
    public decimal? Salario { get; set; }
}