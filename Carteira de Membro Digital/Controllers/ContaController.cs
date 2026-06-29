using Microsoft.AspNetCore.Mvc;
using CarteiraDeMembroDigital.Models;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration; // Necessário para ler o appsettings.json
using Microsoft.AspNetCore.Http; // Necessário para a Sessão

namespace CarteiraDeMembroDigital.Controllers
{
    public class ContaController : Controller
    {
        private readonly ApplicationDbContext _banco;
        private readonly IConfiguration _configuration;

        // Construtor agora recebe o Banco e a Configuração (para o e-mail)
        public ContaController(ApplicationDbContext banco, IConfiguration configuration)
        {
            _banco = banco;
            _configuration = configuration;
        }

        // ==========================================
        // 1. TELA DE LOGIN
        // ==========================================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            var usuario = _banco.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);

            if (usuario != null)
            {
                // Salva os dados na sessão
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
                HttpContext.Session.SetString("UsuarioPerfil", usuario.Perfil ?? "Membro");

                // Redireciona dependendo do perfil
                if (usuario.Perfil == "Pastor Presidente")
                {
                    return RedirectToAction("PainelPastor", "Home");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Credenciais inválidas.";
            return View();
        }

        // ==========================================
        // 2. TELA DE CADASTRO
        // ==========================================
        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RealizarCadastro(Usuario novoUsuario, string confirmarSenha)
        {
            // 1. Validação da Senha
            if (string.IsNullOrEmpty(novoUsuario.Senha) || novoUsuario.Senha.Length != 6)
            {
                ViewBag.ErroSenha = "A senha deve conter exatamente 6 dígitos.";
                return View("Cadastro");
            }

            // 2. Validação da Confirmação da Senha
            if (novoUsuario.Senha != confirmarSenha)
            {
                ViewBag.ErroSenha = "As senhas digitadas não conferem. Tente novamente.";
                return View("Cadastro");
            }

            // 3. Validação do RG (CORRIGIDO: Agora tem o IF)
            if (string.IsNullOrWhiteSpace(novoUsuario.RG) || novoUsuario.RG.Length < 5)
            {
                ViewBag.Erro = "Por favor, insira um RG válido com pelo menos 5 caracteres.";
                return View("Cadastro");
            }

            // 4. Validação do CPF (CORRIGIDO)
            if (!ValidarCpf(novoUsuario.CPF))
            {
                ViewBag.Erro = "O CPF informado é inválido. Verifique os números.";
                return View("Cadastro");
            }

            // Se passou em tudo, salva no banco!
            if (ModelState.IsValid)
            {
                // Define o perfil padrão para quem se cadastra sozinho
                novoUsuario.Perfil = "Membro";

                _banco.Usuarios.Add(novoUsuario);
                _banco.SaveChanges();

                return RedirectToAction("Login");
            }

            return View("Cadastro");
        }

        // FUNÇÃO MATEMÁTICA DO CPF (CORRIGIDA)
        private bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");

            if (cpf.Length != 11 || new string(cpf[0], 11) == cpf) return false;

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }

        // ==========================================
        // 3. TELA DE RECUPERAR SENHA
        // ==========================================
        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            return View();
        }

        // CORRIGIDO: Nome igual ao formulário e com a lógica de envio real
        [HttpPost]
        public IActionResult RecuperarSenha(string email)
        {
            var usuario = _banco.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario == null)
            {
                ViewBag.Erro = "E-mail não encontrado no sistema.";
                return View();
            }

            // Cria uma senha temporária
            string novaSenhaTemporaria = "ADHEL" + new Random().Next(1000, 9999);
            usuario.Senha = novaSenhaTemporaria;
            _banco.SaveChanges();

            try
            {
                var smtpHost = _configuration["SmtpSettings:Host"];
                var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
                var smtpUsername = _configuration["SmtpSettings:Username"];
                var smtpPassword = _configuration["SmtpSettings:Password"];

                using (var mensagem = new MailMessage())
                {
                    mensagem.From = new MailAddress(smtpUsername, "Secretaria ADHEL");
                    mensagem.To.Add(new MailAddress(email));
                    mensagem.Subject = "Recuperação de Senha - ADHEL";
                    mensagem.IsBodyHtml = true;
                    mensagem.Body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 500px; padding: 20px; border: 1px solid #e2e8f0; border-radius: 10px;'>
                            <h2 style='color: #0d1326; border-bottom: 2px solid #bba371; padding-bottom: 10px;'>ADHEL Digital</h2>
                            <p>Olá, <strong>{usuario.Nome}</strong>,</p>
                            <p>Você solicitou a recuperação de acesso à sua Carteira.</p>
                            <div style='background-color: #f7fafc; padding: 15px; border-radius: 8px; text-align: center; margin: 20px 0;'>
                                <span style='font-size: 12px; color: #718096; display:block;'>Sua Nova Senha Temporária</span>
                                <strong style='font-size: 22px; color: #2c5282;'>{novaSenhaTemporaria}</strong>
                            </div>
                        </div>";

                    using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mensagem);
                    }
                }

                ViewBag.Sucesso = "E-mail enviado! Verifique sua caixa de entrada.";
            }
            catch (System.Exception)
            {
                ViewBag.Erro = "Erro ao enviar e-mail. Verifique as configurações de SMTP no appsettings.json.";
            }

            return View();
        }
    }
}