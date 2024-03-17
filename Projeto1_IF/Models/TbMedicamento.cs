﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Projeto1_IF.Models;

[Table("tbMedicamento")]
[Index("IdCategoriaMedicamento", Name = "IX_tbMedicamento_IdCategoriaMedicamento")]
public partial class TbMedicamento
{
    [Key]
    public int IdMedicamento { get; set; }

    public int IdCategoriaMedicamento { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Nome { get; set; }

    [Unicode(false)]
    public string Bula { get; set; }

    public byte[] BulaArquivo { get; set; }

    [ForeignKey("IdCategoriaMedicamento")]
    [InverseProperty("TbMedicamento")]
    public virtual TbCategoriaMedicamento IdCategoriaMedicamentoNavigation { get; set; }
}