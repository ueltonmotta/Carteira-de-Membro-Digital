using Microsoft.AspNetCore.Mvc;
using CarteiraDeMembroDigital.Models; // Importante para reconhecer o Banco e o Usuario

namespace CarteiraDeMembroDigital.Controllers
{
    public class ContaController : Controller
    {
        // Variável que conecta com o Banco de Dados
        private readonly ApplicationDbContext _banco;

        public ContaController(ApplicationDbContext banco)
        {
            _banco = banco;
        }

        // ==========================================
        // 1. TELA DE LOGIN
        // ==========================================
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FazerLogin(string email, string senha)
        {
            // Simulação de redirecionamento. 
            // Mais para frente, vamos consultar o banco aqui para ver se a senha está certa.
            if (email == "pastor@adhel.com")
            {
                return RedirectToAction("Administrador", "Perfil");
            }

            return RedirectToAction("Membro", "Perfil");
        }

        // ==========================================
        // 2. TELA DE CADASTRO
        // ==========================================
        public IActionResult Cadastro()
        {
            return View();
        }

        // DEIXAMOS APENAS ESTE MÉTODO (O QUE SALVA NO BANCO)
        [HttpPost]
        public IActionResult RealizarCadastro(Usuario novoUsuario, string confirmarSenha)
        {
            // 1. Validação: Verifica se a senha tem exatamente 6 caracteres
            if (string.IsNullOrEmpty(novoUsuario.Senha) || novoUsuario.Senha.Length != 6)
            {
                ViewBag.ErroSenha = "A senha deve conter exatamente 6 dígitos.";
                return View("Cadastro");
            }

            // 2. Validação: Verifica se a senha digitada é igual à confirmação
            if (novoUsuario.Senha != confirmarSenha)
            {
                ViewBag.ErroSenha = "As senhas digitadas não conferem. Tente novamente.";
                return View("Cadastro");
            }

            // Se passou nas validações de segurança e os dados do formulário forem válidos
            if (ModelState.IsValid)
            {
                _banco.Usuarios.Add(novoUsuario);
                _banco.SaveChanges();

                return RedirectToAction("Login");
            }

            return View("Cadastro");
        }

        // ==========================================
        // 3. TELA DE RECUPERAR SENHA
        // ==========================================
        public IActionResult RecuperarSenha()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnviarRecuperacao(string email)
        {
            ViewBag.Mensagem = "Se este e-mail estiver cadastrado, você receberá um link para redefinir sua senha.";
            return View("RecuperarSenha");
        }
    }
}