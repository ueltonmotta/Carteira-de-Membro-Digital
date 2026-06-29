using System;
using System.ComponentModel.DataAnnotations;

namespace CarteiraDeMembroDigital.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public string? FotoPerfil { get; set; }
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        [Required]
        public string Perfil { get; set; } = "Membro"; // Valores padrão: "Membro" ou "Pastor Presidente"

        public string? Status { get; set; } = "Pendente";

        // === NOVOS CAMPOS PESSOAIS ===
        public string? Telefone { get; set; }
        public string? RG { get; set; }
        public string? CPF { get; set; }
        public string? EstadoCivil { get; set; }
        public string? Nacionalidade { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        public string? Conjuge { get; set; }

        // === FILIAÇÃO ===
        public string? NomePai { get; set; }
        public string? NomeMae { get; set; }

        // === DADOS ECLESIASTICOS ===
        public string? Cargo { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataBatismo { get; set; }

        public DateTime DataEmissao { get; set; } = DateTime.Now;
        public DateTime DataValidade { get; set; } = DateTime.Now.AddMonths(12); // Já calcula +12 meses no cadastro

        // Adicione esta propriedade no fim do seu modelo de Usuário
    }
}