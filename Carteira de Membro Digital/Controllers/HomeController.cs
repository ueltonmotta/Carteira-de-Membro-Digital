using Microsoft.AspNetCore.Mvc;
using CarteiraDeMembroDigital.Models;
using System.Linq;
using System;

namespace CarteiraDeMembroDigital.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _banco;

        public HomeController(ApplicationDbContext banco)
        {
            _banco = banco;
        }

        public IActionResult Anuncios()
        {
            // Busca todos os anúncios, trazendo os mais recentes primeiro
            var anuncios = _banco.Anuncios.OrderByDescending(a => a.DataPublicacao).ToList();
            return View(anuncios);
        }

        public IActionResult Calendario()
        {
            // Puxa os eventos futuros agendados da igreja
            var eventos = _banco.Eventos.Where(e => e.DataHora >= DateTime.Today).OrderBy(e => e.DataHora).ToList();
            return View(eventos);
        }

        public IActionResult Index()


        {
            // Vai buscar o membro à base de dados
            var usuario = _banco.Usuarios.OrderByDescending(u => u.Id).FirstOrDefault();

            if (usuario == null)
            {
                return RedirectToAction("Login", "Conta");
            }

            // =========================================================
            // AUTO-CURA: Se a data for nula ou for o "Ano 0001" (bugada)
            // =========================================================
            // =========================================================
            // AUTO-CURA: Conserta o erro do "Ano 0001"
            // =========================================================
            if (usuario.DataValidade.Year < 2000)
            {
                usuario.DataValidade = DateTime.Now.AddMonths(12);
                _banco.SaveChanges();
            }

            // LÓGICA DE EXPIRAÇÃO AUTOMÁTICA
            if (DateTime.Now > usuario.DataValidade)
            {
                ViewBag.CartaoExpirado = true;
                ViewBag.StatusExibicao = "EXPIRADO";
            }
            else
            {
                ViewBag.CartaoExpirado = false;
                ViewBag.StatusExibicao = usuario.Status ?? "ATIVO";
            }

            // Envia os dados do membro para a tela do Cartão!
            return View(usuario);
        }
        // ==========================================
        // PÁGINAS DO MENU LATERAL
        // ==========================================
        public IActionResult Configuracoes()
        {
            return View();
        }

        public IActionResult Sobre()
        {
            return View();
        }

        public IActionResult Suporte()
        {
            return View();
        }

        public IActionResult Termos()
        {
            return View();
        }
    }
}