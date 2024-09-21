using ChatApp.Models;
using ChatApp.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace ChatApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services
            builder.Services.AddDbContext<ChatappContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("chat"))
                    .EnableSensitiveDataLogging());

            builder.Services.AddSingleton<DataSecurityProvider>();
            builder.Services.AddDataProtection(); // Ensure IDataProtectionProvider is registered


            // Add SignalR
            builder.Services.AddSignalR();  // Adding SignalR service

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(c => c.LoginPath = "/Account/Login");

            builder.Services.AddSession(o =>
            {
                o.IdleTimeout = TimeSpan.FromMinutes(2);
                o.Cookie.HttpOnly = true;
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Add Authentication and Session Middleware
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            // Configure SignalR endpoint
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Static}/{action=Index}/{id?}");

                 // Map the SignalR hub route
            endpoints.MapHub<ChatHub>("/chathub");
            });

            app.Run();
        }
    }
}
