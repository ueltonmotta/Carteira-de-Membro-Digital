using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CarteiraDeMembroDigital.Controllers // Confirme se o namespace é o mesmo do seu projeto
{
    public class ValidacaoController : Controller
    {
        // A tag [Authorize] é a fechadura. Só entra quem tiver a "chave" (Role) de PastorPresidente.
        [Authorize(Roles = "PastorPresidente")]
        public IActionResult AvaliarMembro(string id)
        {
            // Simulação: Em um sistema real, você buscaria os dados do membro no Banco de Dados aqui.
            ViewBag.MembroId = id ?? "GC-345.678.910";
            ViewBag.MembroNome = "Ana Silva";

            return View(); // Abre a tela para o Pastor ver
        }

        // Esta função é chamada quando o Pastor clica no botão "Aprovar"
        [HttpPost]
        [Authorize(Roles = "PastorPresidente")]
        public IActionResult ConfirmarAprovacao(string id)
        {
            // Aqui entraria o código para salvar no Banco de Dados que o membro foi aprovado
            // Exemplo: _bancoDados.Membros.Find(id).Status = "Aprovado";
            // _bancoDados.SaveChanges();

            return Content("Membro validado com sucesso! A assinatura pastoral já está ativa no cartão.");
        }
    }
}