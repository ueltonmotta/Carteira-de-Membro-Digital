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
            // CORREÇÃO: Pega o ID de quem está logado de verdade
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Conta"); // Se não estiver logado, bloqueia
            }

            int id = int.Parse(usuarioIdStr);
            var usuario = _banco.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Conta");
            }

            // Envia os dados verdadeiros do usuário logado para a tela do celular
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
            // CORREÇÃO: Garante segurança para que só quem está logado aceda
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            var perfilLogado = HttpContext.Session.GetString("UsuarioPerfil");

            if (string.IsNullOrEmpty(usuarioIdStr) || (perfilLogado != "Secretaria" && perfilLogado != "Pastor Presidente"))
            {
                return RedirectToAction("Login", "Conta");
            }

            int id = int.Parse(usuarioIdStr);
            var usuario = _banco.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Conta");
            }

            ViewBag.Nome = usuario.Nome;
            ViewBag.Cargo = usuario.Perfil ?? "Pastor Presidente";

            // Conta automaticamente quantos membros existem no banco de dados
            ViewBag.MembrosAtivos = _banco.Usuarios.Count(u => u.Status == "Ativo");
            ViewBag.ValidacoesPendentes = _banco.Usuarios.Count(u => u.Status == "Pendente");

            return View();
        }

        // ==========================================
        // 3. TELA DE EDIÇÃO DE PERFIL
        // ==========================================
        public IActionResult Editar()
        {
            // CORREÇÃO: Garante que o membro edita apenas a sua própria conta
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Conta");
            }

            int id = int.Parse(usuarioIdStr);
            var usuario = _banco.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // ==========================================
        // 4. SALVAR A EDIÇÃO COM FOTO
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> SalvarEdicao(int Id, string Nome, string Telefone, string RG, string CPF, string EstadoCivil, string Conjuge, IFormFile? novaFoto)
        {
            // Segurança extra: impede que alterem o ID na requisição para editar outra pessoa
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr) || Id.ToString() != usuarioIdStr)
            {
                return RedirectToAction("Login", "Conta");
            }

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

            // Atualiza o nome na sessão caso tenha sido alterado
            HttpContext.Session.SetString("UsuarioNome", usuario.Nome);

            return RedirectToAction("Membro");
        }
    }
}