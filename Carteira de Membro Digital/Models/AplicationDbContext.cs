using Microsoft.EntityFrameworkCore;

namespace CarteiraDeMembroDigital.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Esta propriedade diz que queremos uma tabela chamada "Usuarios" baseada no modelo acima
        public DbSet<Usuario> Usuarios { get; set; }
    }
}