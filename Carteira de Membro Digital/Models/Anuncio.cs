using System;

namespace CarteiraDeMembroDigital.Models
{
    public class Anuncio
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public DateTime DataPublicacao { get; set; } = DateTime.Now;
        public bool EhImportante { get; set; } = false; // Se for true, ganha destaque vermelho
    }
}