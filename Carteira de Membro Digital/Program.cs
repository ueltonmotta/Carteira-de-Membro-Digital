using Microsoft.EntityFrameworkCore;
using CarteiraDeMembroDigital.Models; // Seu namespace aqui

var builder = WebApplication.CreateBuilder(args);

// Adiciona o contexto do banco de dados (Você já deve ter algo parecido com isto)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona os serviços MVC
builder.Services.AddControllersWithViews();

// ========================================================
// 1. HABILITA O SERVIÇO DE SESSÃO AQUI (Antes do builder.Build!)
// ========================================================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // A sessão do usuário expira após 2h de inatividade
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Necessário para contornar regras rígidas de cookies
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ========================================================
// 2. ATIVA A SESSÃO NA PIPELINE AQUI (Antes do Authorization e do MapController)
// ========================================================
app.UseSession();

app.UseAuthorization();

// 1º LUGAR: A rota das Áreas (Secretaria)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Painel}/{action=Index}/{id?}");

// 2º LUGAR: A rota padrão dos membros (Mobile)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Conta}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Conta}/{action=Login}/{id?}"); // Rota padrão apontando pro seu Login

app.Run();