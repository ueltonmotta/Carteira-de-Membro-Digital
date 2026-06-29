using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CarteiraDeMembroDigital.Models;
using System.Linq;

namespace CarteiraDeMembroDigital.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _banco;

        public HomeController(ApplicationDbContext banco)
        {
            _banco = banco;
        }

        // ==========================================
        // TELA DA CARTEIRINHA (Com Lógica de Presença)
        // ==========================================
        public IActionResult Index()
        {
            // 1. Tenta pegar o ID do usuário guardado na Sessão
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                return RedirectToAction("Login", "Conta");
            }

            int id = int.Parse(usuarioIdStr);
            var usuarioLogado = _banco.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuarioLogado == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Conta");
            }

            // --------------------------------------------------------
            // LÓGICA INTELIGENTE DE SANTA CEIA
            // --------------------------------------------------------

            // A) Verifica se HOJE tem Santa Ceia
            var santaCeiaHoje = _banco.Eventos.FirstOrDefault(e => e.DataHora.Date == DateTime.Today && e.Titulo.Contains("Santa Ceia"));

            if (santaCeiaHoje != null)
            {
                // Verifica se ele já assinou a lista hoje
                bool jaAssinou = _banco.Presencas.Any(p => p.UsuarioId == id && p.EventoId == santaCeiaHoje.Id);

                // Só mostra o botão se ele ainda não assinou
                ViewBag.MostrarBotaoGps = !jaAssinou;
                ViewBag.EventoId = santaCeiaHoje.Id;
            }

            // B) Verifica se TEVE Santa Ceia nas últimas 48 horas (para pedir justificativa)
            var limiteHoras = DateTime.Now.AddHours(-48);
            var santaCeiaRecente = _banco.Eventos.FirstOrDefault(e => e.DataHora >= limiteHoras && e.DataHora < DateTime.Today && e.Titulo.Contains("Santa Ceia"));

            if (santaCeiaRecente != null)
            {
                // Verifica se ele esteve presente ou se já justificou essa falta
                bool temRegistro = _banco.Presencas.Any(p => p.UsuarioId == id && p.EventoId == santaCeiaRecente.Id);

                if (!temRegistro)
                {
                    ViewBag.PedirJustificativa = true;
                    ViewBag.EventoFaltaId = santaCeiaRecente.Id;
                    ViewBag.DataFalta = santaCeiaRecente.DataHora.ToString("dd/MM/yyyy");
                }
            }

            // 5. Envia os dados para a tela da Carteirinha
            return View(usuarioLogado);
        }

        // ==========================================
        // TELA DO MEMBRO: ANÚNCIOS (Apenas Leitura)
        // ==========================================
        public IActionResult Anuncios()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");

            var anuncios = _banco.Anuncios.OrderByDescending(a => a.DataPublicacao).ToList();
            return View(anuncios);
        }

        public IActionResult Configuracoes() // <-- Sem acento no 'o'
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");

            return View(); // Isso vai procurar por Configuracoes.cshtml
        }

        // ==========================================
        // TELA DO MEMBRO: EVENTOS (Apenas Leitura)
        // ==========================================
        public IActionResult Eventos()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");

            var eventos = _banco.Eventos
                                .Where(e => e.DataHora >= DateTime.Today)
                                .OrderBy(e => e.DataHora)
                                .ToList();
            return View(eventos);
        }

        // ==========================================
        // TELA DO MEMBRO: AGENDA DA IGREJA
        // ==========================================
        public IActionResult Calendario()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");

            var eventos = _banco.Eventos
                                .Where(e => e.DataHora >= DateTime.Today)
                                .OrderBy(e => e.DataHora)
                                .ToList();
            return View(eventos);
        }

        // ==========================================
        // TELA DO PERFIL
        // ==========================================
        public IActionResult Perfil()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");

            int id = int.Parse(usuarioIdStr);
            var usuarioLogado = _banco.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuarioLogado == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Conta");
            }
            return View(usuarioLogado);
        }

        // ==========================================
        // CHECK-IN POR GEOLOCALIZAÇÃO
        // ==========================================
        [HttpPost]
        public IActionResult CheckInSantaCeia(double latitude, double longitude, int eventoId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");

            // Coordenadas da Igreja ADHEL
            double latIgreja = -22.765781;
            double lonIgreja = -42.921282;

            double distanciaMetros = CalcularDistanciaMetros(latitude, longitude, latIgreja, lonIgreja);

            if (distanciaMetros > 250)
            {
                TempData["ErroCheckIn"] = $"Você está muito longe da igreja ({Math.Round(distanciaMetros)} metros). O check-in falhou.";
                return RedirectToAction("Index");
            }

            var novaPresenca = new Presenca
            {
                UsuarioId = int.Parse(usuarioIdStr),
                EventoId = eventoId, // Liga ao evento correto da agenda
                NomeMembro = HttpContext.Session.GetString("UsuarioNome"),
                DataHora = DateTime.Now,
                Status = "Presente" // Atualizado para o novo formato do banco
            };

            _banco.Presencas.Add(novaPresenca);
            _banco.SaveChanges();

            TempData["SucessoCheckIn"] = "Presença na Santa Ceia confirmada com sucesso!";
            return RedirectToAction("Index");
        }

        // ==========================================
        // ENVIAR JUSTIFICATIVA DE FALTA (NOVO)
        // ==========================================
        [HttpPost]
        public IActionResult EnviarJustificativa(int eventoId, string motivo)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");

            var justificativa = new Presenca
            {
                UsuarioId = int.Parse(usuarioIdStr),
                EventoId = eventoId,
                NomeMembro = HttpContext.Session.GetString("UsuarioNome"),
                DataHora = DateTime.Now,
                Status = "Justificado",
                Motivo = motivo
            };

            _banco.Presencas.Add(justificativa);
            _banco.SaveChanges();

            TempData["SucessoCheckIn"] = "Sua justificativa foi enviada para a secretaria.";
            return RedirectToAction("Index");
        }

        // ==========================================
        // FÓRMULA MATEMÁTICA (Haversine) PARA CALCULAR DISTÂNCIA
        // ==========================================
        private double CalcularDistanciaMetros(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // Raio da Terra em metros
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        // ==========================================
        // TELA DO MEMBRO: SOBRE A IGREJA
        // ==========================================
        public IActionResult Sobre()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");
            return View();
        }

        // ==========================================
        // TELA DO MEMBRO: AJUDA E SUPORTE
        // ==========================================
        public IActionResult Suporte()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");
            return View();
        }

        // ==========================================
        // TELA DO MEMBRO: TERMOS DE USO
        // ==========================================
        public IActionResult Termos()
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Conta");
            return View();
        }
    }
}