using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CarteiraDeMembroDigital.Models;
using System.Linq;
using System;

namespace CarteiraDeMembroDigital.Controllers
{
    public class ContaController : Controller
    {
        private readonly ApplicationDbContext _banco;

        public ContaController(ApplicationDbContext banco)
        {
            _banco = banco;
        }

        // ==========================================
        // 1. TELA DE LOGIN (Carregar)
        // ==========================================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ==========================================
        // 2. AÇÃO DE LOGIN (Validar Dados)
        // ==========================================
        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                ViewBag.Erro = "Por favor, preencha o e-mail e a senha.";
                return View();
            }

            // Busca o usuário no banco
            var usuario = _banco.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);

            if (usuario == null)
            {
                ViewBag.Erro = "E-mail ou senha incorretos.";
                return View();
            }

            // Salva os dados na Sessão
            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
            HttpContext.Session.SetString("UsuarioPerfil", usuario.Perfil ?? "Membro");

            // REDIRECIONAMENTO INTELIGENTE:
            // Se for admin, vai direto para o painel da Secretaria
            if (usuario.Perfil == "Secretaria" || usuario.Perfil == "Pastor Presidente")
            {
                return RedirectToAction("Index", "Painel", new { area = "Secretaria" });
            }

            // Se for membro, vai para a carteirinha digital (Home/Index)
            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // 3. TELA DE CADASTRO (Carregar)
        // ==========================================
        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

        // ==========================================
        // 4. AÇÃO DE CADASTRO (Salvar Dados)
        // ==========================================
        [HttpPost]
        public IActionResult RealizarCadastro(Usuario novoUsuario, string confirmarSenha, string codigoSeguranca)
        {
            // Validação da Senha
            if (string.IsNullOrEmpty(novoUsuario.Senha) || novoUsuario.Senha.Length != 6)
            {
                ViewBag.ErroSenha = "A senha deve conter exatamente 6 dígitos.";
                return View("Cadastro");
            }

            if (novoUsuario.Senha != confirmarSenha)
            {
                ViewBag.ErroSenha = "As senhas digitadas não conferem. Tente novamente.";
                return View("Cadastro");
            }

            // Validação do RG
            if (string.IsNullOrWhiteSpace(novoUsuario.RG) || novoUsuario.RG.Length < 5)
            {
                ViewBag.Erro = "Por favor, insira um RG válido.";
                return View("Cadastro");
            }

            // Validação do CPF
            if (!ValidarCpf(novoUsuario.CPF))
            {
                ViewBag.Erro = "O CPF informado é inválido. Verifique os números.";
                return View("Cadastro");
            }

            // VALIDAÇÃO DE SEGURANÇA (PERFIS)
            if (novoUsuario.Perfil == "Secretaria" || novoUsuario.Perfil == "Pastor Presidente")
            {
                // Este é o código mestre. Pode alterá-lo para o que desejar!
                string codigoMestreDaIgreja = "ADHEL2026";

                if (codigoSeguranca != codigoMestreDaIgreja)
                {
                    ViewBag.Erro = "Código de liberação inválido. Não tem permissão para criar conta administrativa.";
                    return View("Cadastro");
                }

                // Admin já entra aprovado
                novoUsuario.Status = "Ativo";
            }
            else
            {
                // QUALQUER OUTRA PESSOA (Mesmo que preencha Presbítero ou Pastor Auxiliar)
                novoUsuario.Perfil = "Membro";
                novoUsuario.Status = "Pendente";
            }

            // Preenche as datas automaticamente para o banco de dados não reclamar
            novoUsuario.DataEmissao = DateTime.Now;
            novoUsuario.DataValidade = DateTime.Now.AddYears(1);

            // A MÁGICA ACONTECE AQUI: Limpa o bloqueio do C#
            ModelState.Clear();

            // Salva no banco de dados direto
            _banco.Usuarios.Add(novoUsuario);
            _banco.SaveChanges();

            // Redireciona com sucesso para a tela de Login
            return RedirectToAction("Login");
        }

        // ==========================================
        // 5. LOGOUT (Sair da conta)
        // ==========================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Limpa a memória
            return RedirectToAction("Login");
        }

        // ==========================================
        // FUNÇÃO AUXILIAR: VALIDAR CPF
        // ==========================================
        private bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            // Remove pontos e traços
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11) return false;

            // Rejeita CPFs com todos os números iguais (ex: 111.111.111-11)
            if (cpf.Distinct().Count() == 1) return false;

            return true;
        }
    }
}