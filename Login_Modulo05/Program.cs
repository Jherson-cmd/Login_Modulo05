using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Login_Administracion_de_Modulos.Seguridad;

var builder = WebApplication.CreateBuilder(args);

// Soporte para Controladores MVC + Vistas Razor + Razor Pages
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // Permitir búsqueda de vistas en ~/Vista/ además de ~/Views/ y ~/Areas/
        options.ViewLocationFormats.Add("/Vista/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Vista/Shared/{0}.cshtml");
        options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
        options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
        options.AreaViewLocationFormats.Add("/Vista/Shared/{0}.cshtml");
    });

builder.Services.AddRazorPages();

// Configuración de Sesión en Memoria para Autenticación de Roles y Modo Demo
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.Name = ".IESTPArgentina.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Inicialización segura de datos semilla en PostgreSQL (si las tablas operativas están vacías)
try
{
    string connStr = ConfiguracionSegura.ObtenerCadenaConexion(app.Configuration);
    InicializadorBaseDatos.AsegurarDatosSemilla(connStr);
}
catch
{
    // Si no hay red al iniciar, el sistema sigue funcionando con Modo Demo Local y reconexión bajo demanda
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

// Rutas para Áreas (Areas/Modulo05)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Enfermeria}/{action=Panel}/{id?}");

// Ruta por defecto hacia el Login Institucional
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
