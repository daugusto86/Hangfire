using Hangfire.Site.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hangfire.Site.Controllers
{
    public class AutenticacaoController : Controller
    {
        public IActionResult Index()
        {
            return View(new Login { Usuario = "teste", Senha = "teste" });
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            // simulação de login
            if (login.Usuario == "teste" && login.Senha == "teste")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login.Usuario)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(claimsPrincipal);

                return Redirect("/jobs");
            }

            TempData["mensagem"] = "Usuário e/ou senha incorretos.";
            return RedirectToAction("Index", "Autenticacao");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
