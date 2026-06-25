using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarteiraDeMembroDigital.Controllers
{
    public class PerfilController : Controller
    {
        // 1. Tela do Perfil do Membro Comum
        public IActionResult Membro()
        {
            // Simulando dados vindos do banco
            ViewBag.Nome = "Ana Carolina da Silva";
            ViewBag.Cargo = "Membro Ativo";
            ViewBag.Email = "ana.silva@email.com";
            ViewBag.Telefone = "(11) 98765-4321";
            ViewBag.Status = "Aprovado";

            return View();
        }

        // 2. Tela do Perfil do Administrador (Pastor)
        // [Authorize(Roles = "PastorPresidente")] // Descomente quando ativar o login
        public IActionResult Administrador()
        {
            ViewBag.Nome = "Pr. João Marcos";
            ViewBag.Cargo = "Pastor Presidente";
            ViewBag.MembrosAtivos = 145;
            ViewBag.ValidacoesPendentes = 12;

            return View();
        }

        // 3. Tela de Edição de Perfil
        public IActionResult Editar()
        {
            // Carrega os dados atuais para o formulário
            ViewBag.Nome = "Ana Carolina da Silva";
            ViewBag.Email = "ana.silva@email.com";
            ViewBag.Telefone = "(11) 98765-4321";

            return View();
        }

        // 4. Ação para salvar os dados editados
        [HttpPost]
        public IActionResult SalvarEdicao(string nome, string email, string telefone)
        {
            // Aqui você salvaria no Banco de Dados.
            // Exemplo: _banco.AtualizarUsuario(nome, email, telefone);

            // Redireciona de volta para o perfil do membro após salvar
            return RedirectToAction("Membro");
        }
    }
}