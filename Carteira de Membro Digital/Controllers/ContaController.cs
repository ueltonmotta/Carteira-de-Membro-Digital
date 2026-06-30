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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                ViewBag.Erro = "Por favor, preencha o e-mail e a senha.";
                return View();
            }

            var usuario = _banco.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);

            if (usuario == null)
            {
                ViewBag.Erro = "E-mail ou senha incorretos.";
                return View();
            }

            HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
            HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
            HttpContext.Session.SetString("UsuarioPerfil", usuario.Perfil ?? "Membro");

            if (usuario.Perfil == "Secretaria" || usuario.Perfil == "Pastor Presidente")
            {
                return RedirectToAction("Index", "Painel", new { area = "Secretaria" });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

        // ==========================================
        // ETAPA 1: VALIDAR DADOS E EXIBIR TERMOS
        // ==========================================
        [HttpPost]
        public IActionResult ValidarCadastro(Usuario novoUsuario, string confirmarSenha, string codigoSeguranca)
        {
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

            if (string.IsNullOrWhiteSpace(novoUsuario.RG) || novoUsuario.RG.Length < 5)
            {
                ViewBag.Erro = "Por favor, insira um RG válido.";
                return View("Cadastro");
            }

            if (!ValidarCpf(novoUsuario.CPF))
            {
                ViewBag.Erro = "O CPF informado é inválido. Verifique os números.";
                return View("Cadastro");
            }

            var usuarioExistente = _banco.Usuarios.FirstOrDefault(u => u.Email == novoUsuario.Email);
            if (usuarioExistente != null)
            {
                ViewBag.Erro = "Este e-mail já está cadastrado no sistema.";
                return View("Cadastro");
            }

            if (novoUsuario.Perfil == "Secretaria" || novoUsuario.Perfil == "Pastor Presidente")
            {
                string codigoMestreDaIgreja = "ADHEL2026";
                if (codigoSeguranca != codigoMestreDaIgreja)
                {
                    ViewBag.Erro = "Código de liberação inválido. Não tem permissão para criar conta administrativa.";
                    return View("Cadastro");
                }
                novoUsuario.Status = "Ativo";
            }
            else
            {
                novoUsuario.Perfil = "Membro";
                novoUsuario.Status = "Pendente";
            }

            // Manda os dados temporariamente para a tela de Termos
            return View("TermosConduta", novoUsuario);
        }

        // ==========================================
        // ETAPA 2: SALVAR APÓS ACEITAR TERMOS
        // ==========================================
        [HttpPost]
        public IActionResult RealizarCadastroFinal(Usuario novoUsuario)
        {
            novoUsuario.DataEmissao = DateTime.Now;
            novoUsuario.DataValidade = DateTime.Now.AddYears(1);

            ModelState.Clear();

            _banco.Usuarios.Add(novoUsuario);
            _banco.SaveChanges();

            TempData["MensagemSucesso"] = "Cadastro efetuado! Aguarde a aprovação do seu perfil pela secretaria.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;
            cpf = new string(cpf.Where(char.IsDigit).ToArray());
            if (cpf.Length != 11) return false;
            if (cpf.Distinct().Count() == 1) return false;
            return true;
        }
    }
}