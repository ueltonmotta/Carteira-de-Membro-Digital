using System;

namespace CarteiraDeMembroDigital.Models
{
    public class Presenca
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeMembro { get; set; }
        public DateTime DataHora { get; set; }
        public string Culto { get; set; } // Ex: "Santa Ceia"
        public int EventoId { get; set; } // Liga a presença ao evento exato da agenda
        public string Status { get; set; } // Pode ser "Presente" ou "Justificado"
        public string? Motivo { get; set; } // O texto da justificativa
    }
}