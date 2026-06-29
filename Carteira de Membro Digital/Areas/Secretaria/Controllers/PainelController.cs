using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CarteiraDeMembroDigital.Models;
using System.Linq;
using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CarteiraDeMembroDigital.Areas.Secretaria.Controllers
{
    [Area("Secretaria")]
    public class PainelController : Controller
    {
        private readonly ApplicationDbContext _banco;
        private readonly IConfiguration _configuration;

        // CORREÇÃO: O construtor agora recebe os dois parâmetros corretamente
        public PainelController(ApplicationDbContext banco, IConfiguration configuration)
        {
            _banco = banco;
            _configuration = configuration;
        }

        private bool ValidarAcessoAdministrativo()
        {
            var perfil = HttpContext.Session.GetString("UsuarioPerfil");
            return perfil == "Secretaria" || perfil == "Pastor Presidente";
        }

        // ==========================================
        // DASHBOARD INTERATIVA (Resumo do Sistema)
        // ==========================================
        public IActionResult Dashboard()
        {
            // Conta os dados diretamente do banco de dados
            ViewBag.TotalMembros = _banco.Usuarios.Count();
            ViewBag.MembrosAtivos = _banco.Usuarios.Count(u => u.Status == "Ativo");
            ViewBag.CadastrosPendentes = _banco.Usuarios.Count(u => u.Status == "Pendente");

            // Conta quantos eventos futuros existem na agenda
            ViewBag.ProximosEventos = _banco.Eventos.Count(e => e.DataHora >= System.DateTime.Today);
            // NOVA LINHA: Conta quantos membros precisam de atenção pastoral
            ViewBag.MembrosEmAlerta = ObterMembrosComTresFaltasSeguidas().Count;

            return View();
        }

        // ==========================================
        // 1. TELA PRINCIPAL: LISTA DE MEMBROS
        // ==========================================
        public IActionResult Index(string busca, string status)
        {
            // if (!ValidarAcessoAdministrativo()) return RedirectToAction("Login", "Conta", new { area = "" });

            var query = _banco.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                query = query.Where(u => u.Nome.Contains(busca));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(u => u.Status == status);
            }

            var membros = query.OrderBy(u => u.Nome).ToList();
            ViewBag.BuscaAtual = busca;
            ViewBag.StatusAtual = status;

            ViewBag.MembrosAlertaIds = ObterMembrosComTresFaltasSeguidas();

            return View(membros);
        }

        [HttpPost]
        public IActionResult AprovarMembro(int id)
        {
            // if (!ValidarAcessoAdministrativo()) return Forbid();

            var usuario = _banco.Usuarios.Find(id);
            if (usuario != null)
            {
                usuario.Status = "Ativo";
                usuario.DataValidade = DateTime.Today.AddYears(1);
                _banco.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // ==========================================
        // 4. AÇÃO: EDITAR DADOS DO MEMBRO
        // ==========================================
        [HttpGet]
        public IActionResult EditarMembro(int id)
        {
            var usuario = _banco.Usuarios.Find(id);
            if (usuario == null)
            {
                return RedirectToAction("Index"); // Se não achar o membro, volta pra lista
            }

            return View(usuario);
        }

        [HttpPost]
        public IActionResult EditarMembro(Usuario usuarioAtualizado)
        {
            var usuarioBanco = _banco.Usuarios.Find(usuarioAtualizado.Id);

            if (usuarioBanco != null)
            {
                // Atualiza os dados permitidos
                usuarioBanco.Nome = usuarioAtualizado.Nome;
                usuarioBanco.Email = usuarioAtualizado.Email;
                usuarioBanco.CPF = usuarioAtualizado.CPF;
                usuarioBanco.RG = usuarioAtualizado.RG;
                usuarioBanco.Cargo = usuarioAtualizado.Cargo;
                usuarioBanco.Status = usuarioAtualizado.Status;

                // Nota: A senha não é alterada aqui por segurança. 
                // A data de validade atualiza sozinha ao aprovar, mas pode ser editada se adicionar o campo.

                _banco.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ExcluirMembro(int id)
        {
            // if (!ValidarAcessoAdministrativo()) return Forbid();

            var usuario = _banco.Usuarios.Find(id);
            if (usuario != null)
            {
                _banco.Usuarios.Remove(usuario);
                _banco.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // ==========================================
        // MÓDULO: ANÚNCIOS
        // ==========================================
        public IActionResult Anuncios(int? editarId)
        {
            var anuncios = _banco.Anuncios.OrderByDescending(a => a.DataPublicacao).ToList();

            if (editarId.HasValue)
            {
                ViewBag.AnuncioParaEditar = _banco.Anuncios.Find(editarId.Value);
            }
            return View(anuncios);
        }

        [HttpPost]
        public IActionResult CriarAnuncio(Anuncio novoAnuncio)
        {
            novoAnuncio.DataPublicacao = DateTime.Now;
            _banco.Anuncios.Add(novoAnuncio);
            _banco.SaveChanges();

            if (novoAnuncio.EhImportante)
            {
                DispararNotificacaoMembros(novoAnuncio.Titulo, novoAnuncio.Conteudo);
            }

            return RedirectToAction("Anuncios");
        }

        [HttpPost]
        public IActionResult EditarAnuncio(Anuncio anuncioEditado)
        {
            var anuncioBanco = _banco.Anuncios.Find(anuncioEditado.Id);
            if (anuncioBanco != null)
            {
                anuncioBanco.Titulo = anuncioEditado.Titulo;
                anuncioBanco.Conteudo = anuncioEditado.Conteudo;
                anuncioBanco.EhImportante = anuncioEditado.EhImportante;
                _banco.SaveChanges();
            }
            return RedirectToAction("Anuncios");
        }

        [HttpPost]
        public IActionResult ExcluirAnuncio(int id)
        {
            var anuncio = _banco.Anuncios.Find(id);
            if (anuncio != null)
            {
                _banco.Anuncios.Remove(anuncio);
                _banco.SaveChanges();
            }
            return RedirectToAction("Anuncios");
        }

        // ==========================================
        // MÓDULO: EVENTOS
        // ==========================================
        public IActionResult Eventos(int? editarId)
        {
            var eventos = _banco.Eventos.OrderBy(e => e.DataHora).ToList();

            if (editarId.HasValue)
            {
                ViewBag.EventoParaEditar = _banco.Eventos.Find(editarId.Value);
            }
            return View(eventos);
        }

        [HttpPost]
        public IActionResult CriarEvento(Evento novoEvento)
        {
            _banco.Eventos.Add(novoEvento);
            _banco.SaveChanges();
            return RedirectToAction("Eventos");
        }

        [HttpPost]
        public IActionResult EditarEvento(Evento eventoEditado)
        {
            var eventoBanco = _banco.Eventos.Find(eventoEditado.Id);
            if (eventoBanco != null)
            {
                eventoBanco.Titulo = eventoEditado.Titulo;
                eventoBanco.Descricao = eventoEditado.Descricao;
                eventoBanco.DataHora = eventoEditado.DataHora;
                eventoBanco.Local = eventoEditado.Local;
                _banco.SaveChanges();
            }
            return RedirectToAction("Eventos");
        }

        [HttpPost]
        public IActionResult ExcluirEvento(int id)
        {
            var evento = _banco.Eventos.Find(id);
            if (evento != null)
            {
                _banco.Eventos.Remove(evento);
                _banco.SaveChanges();
            }
            return RedirectToAction("Eventos");
        }

        // ==========================================
        // RELATÓRIO DE PRESENÇA (SANTA CEIA)
        // ==========================================
        public IActionResult RelatorioSantaCeia()
        {
            // Pega o último evento de Santa Ceia que já aconteceu ou está acontecendo
            var ultimaCeia = _banco.Eventos
                .Where(e => e.Titulo.Contains("Santa Ceia") && e.DataHora <= DateTime.Now.AddDays(1))
                .OrderByDescending(e => e.DataHora)
                .FirstOrDefault();

            if (ultimaCeia == null)
            {
                ViewBag.Mensagem = "Nenhuma Santa Ceia registrada no sistema ainda.";
                return View(new List<Usuario>()); // Retorna vazio
            }

            ViewBag.DataCeia = ultimaCeia.DataHora.ToString("dd/MM/yyyy");

            // Busca todos os membros ativos da igreja
            var membrosAtivos = _banco.Usuarios.Where(u => u.Status == "Ativo").ToList();

            // Busca a lista de assinaturas/justificativas daquela Ceia
            var registros = _banco.Presencas.Where(p => p.EventoId == ultimaCeia.Id).ToList();

            // Passa os registros para a View usar (usando ViewBag para cruzar dados)
            ViewBag.Registros = registros;

            return View(membrosAtivos);
        }

        // ==========================================
        // SERVIÇOS: DISPARO DE E-MAIL
        // ==========================================
        private void DispararNotificacaoMembros(string titulo, string mensagemTexto)
        {
            try
            {
                var emailsAtivos = _banco.Usuarios
                                         .Where(u => u.Status == "Ativo" && !string.IsNullOrEmpty(u.Email))
                                         .Select(u => u.Email)
                                         .ToList();

                if (!emailsAtivos.Any()) return;

                var smtpHost = _configuration["SmtpSettings:Host"];
                var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
                var smtpUsername = _configuration["SmtpSettings:Username"];
                var smtpPassword = _configuration["SmtpSettings:Password"];

                using (var mensagem = new MailMessage())
                {
                    mensagem.From = new MailAddress(smtpUsername, "Secretaria ADHEL");
                    mensagem.Subject = "🔔 Novo Aviso Importante: " + titulo;
                    mensagem.IsBodyHtml = true;

                    foreach (var email in emailsAtivos)
                    {
                        mensagem.Bcc.Add(new MailAddress(email));
                    }

                    mensagem.Body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 10px; overflow: hidden;'>
                            <div style='background-color: #0d1326; color: #bba371; padding: 20px; text-align: center;'>
                                <h2 style='margin: 0;'>ADHEL Digital</h2>
                            </div>
                            <div style='padding: 30px; background-color: #ffffff;'>
                                <span style='background-color: #fed7d7; color: #9b2c2c; padding: 5px 10px; border-radius: 5px; font-size: 12px; font-weight: bold;'>AVISO DA SECRETARIA</span>
                                <h3 style='color: #2d3748; margin-top: 15px;'>{titulo}</h3>
                                <p style='color: #4a5568; line-height: 1.6; font-size: 15px;'>{mensagemTexto}</p>
                            </div>
                        </div>";

                    using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mensagem);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar e-mails: " + ex.Message);
            }


        }

        private List<int> ObterMembrosComTresFaltasSeguidas()
        {
            // 1. Pega os IDs das últimas 3 Santas Ceias que já aconteceram
            var ultimas3CeiasIds = _banco.Eventos
                .Where(e => e.Titulo.Contains("Santa Ceia") && e.DataHora <= DateTime.Now)
                .OrderByDescending(e => e.DataHora)
                .Take(3)
                .Select(e => e.Id)
                .ToList();

            var membrosComAlertaIds = new List<int>();

            // Se ainda não existirem pelo menos 3 Santas Ceias no sistema, não há dados para alertar
            if (ultimas3CeiasIds.Count < 3) return membrosComAlertaIds;

            // 2. Pega todos os membros ativos
            var membrosAtivos = _banco.Usuarios.Where(u => u.Status == "Ativo").ToList();

            foreach (var membro in membrosAtivos)
            {
                // Conta em quantas das últimas 3 Ceias este membro registou Presença ou Justificativa
                int participacoes = _banco.Presencas
                    .Count(p => p.UsuarioId == membro.Id && ultimas3CeiasIds.Contains(p.EventoId));

                // Se o resultado for 0, significa que nas últimas 3 ceias ele não esteve presente E não justificou nenhuma
                if (participacoes == 0)
                {
                    membrosComAlertaIds.Add(membro.Id);
                }
            }

            return membrosComAlertaIds;
        }
    }
}