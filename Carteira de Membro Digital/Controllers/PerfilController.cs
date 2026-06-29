using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using CarteiraDeMembroDigital.Models;
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

        // ==========================================
        // 1. TELA DO PERFIL DO MEMBRO
        // ==========================================
        public IActionResult Membro()
        {
            // Como ainda não ativamos o sistema de "Sessão" (Login real), 
            // vamos pegar o último usuário que você cadastrou no banco para testar a tela:
            var usuario = _banco.Usuarios.OrderByDescending(u => u.Id).FirstOrDefault();

            if (usuario == null)
            {
                return RedirectToAction("Login", "Conta"); // Se não tiver ninguém, volta pro login
            }

            // Envia os dados verdadeiros do banco para a tela do celular
            ViewBag.Nome = usuario.Nome;
            ViewBag.Cargo = usuario.Cargo ?? "Membro";
            ViewBag.Email = usuario.Email;
            ViewBag.Telefone = usuario.Telefone ?? "Não informado";
            ViewBag.Status = usuario.Status ?? "Pendente";

            return View(usuario);
        }

        // ==========================================
        // 2. TELA DO ADMINISTRADOR (PASTOR)
        // ==========================================
        public IActionResult Administrador()
        {
            ViewBag.Nome = "Pr. João Marcos";
            ViewBag.Cargo = "Pastor Presidente";

            // Conta automaticamente quantos membros existem no banco de dados
            ViewBag.MembrosAtivos = _banco.Usuarios.Count();
            ViewBag.ValidacoesPendentes = 12;

            return View();
        }

        // ==========================================
        // 3. TELA DE EDIÇÃO DE PERFIL
        // ==========================================
        public IActionResult Editar()
        {
            // Pega o mesmo último usuário para editar
            var usuario = _banco.Usuarios.OrderByDescending(u => u.Id).FirstOrDefault();
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // ==========================================
        // 4. SALVAR A EDIÇÃO COM FOTO
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> SalvarEdicao(int Id, string Nome, string Telefone, string RG, string CPF, string EstadoCivil, string Conjuge, IFormFile? novaFoto)
        {
            var usuario = _banco.Usuarios.Find(Id);
            if (usuario == null) return NotFound();

            // Salva os dados de texto
            usuario.Nome = Nome;
            usuario.Telefone = Telefone;
            usuario.RG = RG;
            usuario.CPF = CPF;
            usuario.EstadoCivil = EstadoCivil;
            usuario.Conjuge = Conjuge;

            // Salva a foto (se o membro enviou uma nova)
            if (novaFoto != null && novaFoto.Length > 0)
            {
                var pastaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(pastaUploads)) Directory.CreateDirectory(pastaUploads);

                var nomeUnico = Guid.NewGuid().ToString() + Path.GetExtension(novaFoto.FileName);
                var caminhoCompleto = Path.Combine(pastaUploads, nomeUnico);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await novaFoto.CopyToAsync(stream);
                }

                usuario.FotoPerfil = "/uploads/" + nomeUnico;
            }

            _banco.SaveChanges();

            return RedirectToAction("Membro");
        }
    }
}