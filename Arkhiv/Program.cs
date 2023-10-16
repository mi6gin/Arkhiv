using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SammanWebSite.DataBase;


namespace SammanWebSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var dbContext = new AccountDbContext())
            {
                dbContext.Database.EnsureCreated();
            }
            using (var dbContext = new PdffileDbContext())
            {
                dbContext.Database.EnsureCreated();
            }
            using (var dbContext = new PdfnameDbContext())
            {
                dbContext.Database.EnsureCreated();
            }
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Program>();
                })
                .Build();

            host.Run();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "YourSessionCookieName";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Время жизни аутентификации
                    options.LoginPath = "/Home/Adminpanel"; // Путь к странице входа
                    options.AccessDeniedPath = "/Home/LoginError"; // Путь к странице запрета доступа
                    options.SlidingExpiration = true;
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddScoped<PdfnameDbContext>();
            services.AddScoped<PdffileDbContext>();
            services.AddScoped<AccountDbContext>();
            services.AddHttpContextAccessor();


        }



        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            bool redirectFlag = true;

            app.Use(async (context, next) =>
            {
                if (redirectFlag)
                {
                    redirectFlag = false;
                    // Удаление куки аутентификации
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    // Перенаправление на главную страницу (или любую другую страницу, если требуется)
                    context.Response.Redirect("/Home/Index");
                    return;
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "Zall",
                    pattern: "Zall/{action=Index}/{id?}",
                    defaults: new { controller = "Zall" });

            });
        }
    }
}
