// Aplica o tema assim que a página carrega
document.addEventListener("DOMContentLoaded", function () {
    // Define "dark" como padrão inicial para combinar com a identidade da ADHEL
    const currentTheme = localStorage.getItem("adhel_theme") || "dark";
    document.documentElement.setAttribute("data-theme", currentTheme);
    atualizarIconeToggle(currentTheme);
});

// Função ativada pelo botão nas configurações
function toggleModoEscuro() {
    let currentTheme = document.documentElement.getAttribute("data-theme");
    let newTheme = currentTheme === "dark" ? "light" : "dark";

    // Altera o tema e guarda a preferência
    document.documentElement.setAttribute("data-theme", newTheme);
    localStorage.setItem("adhel_theme", newTheme);

    atualizarIconeToggle(newTheme);
}

// Muda o visual do botão liga/desliga
function atualizarIconeToggle(theme) {
    const icone = document.getElementById("icone-modo-escuro");
    if (icone) {
        if (theme === "dark") {
            icone.className = "fa-solid fa-toggle-on";
            icone.style.color = "#38a169"; // Verde (Ligado)
        } else {
            icone.className = "fa-solid fa-toggle-off";
            icone.style.color = "#a0aec0"; // Cinza (Desligado)
        }
    }

    /* ========================================== */
    /* SISTEMA GLOBAL DE MODO ESCURO / CLARO      */
    /* ========================================== */

    /* --- 1. MODO CLARO (Inverte o App do Membro que é escuro por padrão) --- */
    html[data - theme= "light"] body { background - color: #f8fafc!important; color: #2d3748!important; }
    html[data - theme= "light"] .top - bar, html[data - theme= "light"] .sidebar - header { background - color: #ffffff!important; border - bottom: 2px solid #e2e8f0!important; }
    html[data - theme= "light"] .logo - text, html[data - theme= "light"] .sidebar - header h3 { color: #0d1326!important; }
    html[data - theme= "light"] .hamburger - btn, html[data - theme= "light"] .back - btn { color: #2d3748!important; }
    html[data - theme= "light"] .content, html[data - theme= "light"] .mobile - wrapper { background - color: #f8fafc!important; }
    html[data - theme= "light"] .settings - group, html[data - theme= "light"] .contact - card, html[data - theme= "light"] .term - box, html[data - theme= "light"] .info - text, html[data - theme= "light"] .event - box { background - color: #ffffff!important; color: #2d3748!important; border: 1px solid #e2e8f0!important; }
    html[data - theme= "light"] .settings - item { color: #2d3748!important; border - bottom: 1px solid #edf2f7!important; }
    html[data - theme= "light"]p, html[data - theme= "light"]span.label { color: #4a5568!important; }

    /* --- 2. MODO ESCURO (Inverte o Painel da Secretaria que é claro por padrão) --- */
    html[data - theme= "dark"] .admin - wrapper, html[data - theme= "dark"] .main - content { background - color: #1a202c!important; color: #e2e8f0!important; }
    html[data - theme= "dark"] .table - responsive, html[data - theme= "dark"] .filter - bar, html[data - theme= "dark"] .form - card, html[data - theme= "dark"] .dash - card { background - color: #2d3748!important; border: 1px solid #4a5568!important; }
    html[data - theme= "dark"] table th { background - color: #1a202c!important; color: #a0aec0!important; border - bottom: 2px solid #4a5568!important; }
    html[data - theme= "dark"] table td { color: #e2e8f0!important; border - bottom: 1px solid #4a5568!important; }
    html[data - theme= "dark"]tr:hover td { background - color: #4a5568!important; }
    html[data - theme= "dark"]h2, html[data - theme= "dark"] h3 { color: #ffffff!important; }
    html[data - theme= "dark"] .filter - bar input, html[data - theme= "dark"] .filter - bar select, html[data - theme= "dark"] .form - control { background - color: #1a202c!important; color: #fff!important; border: 1px solid #4a5568!important; }
    html[data - theme= "dark"] .dash - info p { color: #fff!important; }
}