using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using CarteiraDeMembroDigital.Models; // Para acessar o banco
using System.Linq;

namespace CarteiraDeMembroDigital.Controllers
{
    public class PerfilController : Controller
    {
        private readonly ApplicationDbContext _banco;

        public PerfilController(ApplicationDbContext banco)
        {
            _banco = banco;
        }

        // ... suas outras rotas de Membro() e Administrador() continuam aqui ...

        // TELA DE EDIÇÃO
        public IActionResult Editar(int id)
        {
            // Busca o usuário real no banco (Você pode pegar o ID da sessão depois)
            var usuario = _banco.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario == null) return NotFound();

            return View(usuario); // Passa o objeto usuário inteiro para a tela
        }

        // SALVAR A EDIÇÃO COM FOTO
        [HttpPost]
        public async Task<IActionResult> SalvarEdicao(int Id, string Nome, string Telefone, IFormFile? novaFoto)
        {
            var usuario = _banco.Usuarios.Find(Id);
            if (usuario == null) return NotFound();

            // Atualiza os dados de texto
            usuario.Nome = Nome;
            usuario.Telefone = Telefone;

            // Se o membro enviou uma foto nova...
            if (novaFoto != null && novaFoto.Length > 0)
            {
                // 1. Define onde salvar (wwwroot/uploads)
                var pastaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // 2. Cria um nome único e aleatório para a foto (ex: 5f4d8...jpg)
                var nomeUnico = Guid.NewGuid().ToString() + Path.GetExtension(novaFoto.FileName);
                var caminhoCompleto = Path.Combine(pastaUploads, nomeUnico);

                // 3. Copia a foto do celular/PC para a pasta uploads
                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await novaFoto.CopyToAsync(stream);
                }

                // 4. Salva o caminho da foto no Banco de Dados
                usuario.FotoPerfil = "/uploads/" + nomeUnico;
            }

            _banco.SaveChanges();

            // Redireciona de volta para o perfil
            return RedirectToAction("Membro");
        }
    }
}