using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Site.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHangfire(x =>
{
    x.UseMemoryStorage();
});

builder.Services.AddHangfireServer();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.LoginPath = "/Autenticacao";
        options.AccessDeniedPath = "/Autenticacao";
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseCookiePolicy();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=index}/{id?}");

// redirecionamento para o login quando tentar
// acessar o painel do hangfire sem logar
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/jobs" && !context.User.Identity.IsAuthenticated)
    {
        await context.ChallengeAsync();
        return;
    }
    await next();
});

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    //Authorization = new[] { new AutorizacaoFilter(app.Services.GetService<IHttpContextAccessor>()) }
    Authorization = new[] { new AutorizacaoFilter() }
});

BackgroundJob.Enqueue(() => Console.WriteLine("Teste Hangfire!"));

app.Run();
