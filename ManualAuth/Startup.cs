using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManualAuth.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using MimeKit;
using MailKit.Net.Smtp;
using ManualAuth.Models;
using ManualAuth.Controllers;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace ManualAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Baza danych. Dodanie kontekstu 

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection")));

            // Dodanie identity, żeby uzywać logowania
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc()/*.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)*/;


            //TUTAJ WŁĄCZA SIĘ USŁUGA POWIADOMIEŃ
            services.AddHostedService<TimeReminderHostedService>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Dodanie Cultures, żeby prawidłowo formatować daty

            IList<CultureInfo> sc = new List<CultureInfo>();
            sc.Add(new CultureInfo("en-US"));
            sc.Add(new CultureInfo("zh-TW"));

            var lo = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = sc,
                SupportedUICultures = sc
            };
            var cp = lo.RequestCultureProviders.OfType<CookieRequestCultureProvider>().First();
            cp.CookieName = "UserCulture"; 
            app.UseRequestLocalization(lo);

            //niezbędne, by używać logowania
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    // Klasa TimeReminderHostedService obsluguje Timer, który liczy pewne interwaly czasu. Przy każdym interwale wywołuje metodę SendNotification, która wyszukuje rekord z bazy,
    // dla którego należy wysłać powiadomienie


    public class TimeReminderHostedService : IHostedService
    {
        private readonly IServiceScopeFactory scopeFactory;


        public TimeReminderHostedService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        private Timer timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(SendNotification, null, 0, 60000);
            
            return Task.CompletedTask;
        }

        void SendNotification(object state)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // ZŁA METODA: Przy dużej liczbie rekordów będzie długo mielić
                // Propozycja poprawy: zamienić na LINQ
                foreach (var item in dbContext.Medicine)
                {
                    var today = DateTime.Today;
                    

                    if (item.HourOfTaking==DateTime.Now.Hour&& item.MinuteOfTaking == DateTime.Now.Minute && item.Day==today.DayOfWeek)
                    {

                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("Medicine Reminder", "test.app.kd@gmail.com"));

                        // Na potrzeby prezentacji dwa warianty dodawania odbiorcy do maila: 
                        // 1. Zakodowanie "Na twardo" swojego adresu email, które następnie zostało zamienione na poprawną implementację, czyli:
                        // 2. Pobranie przypisanego do rekordu adresu e-mail pacjenta.

                        //message.To.Add(new MailboxAddress("To Add", "krzysztofwozniak1234@gmail.com"));
                        message.To.Add(new MailboxAddress("To Add", item.Id_patient));
                        
                        message.Subject = "Hey! It's time to take your medicine!";
                        message.Body = new TextPart("plain")
                        {
                            Text = "Hello, " + item.Id_patient + "! Don't forget to take " + item.Name
                        };

                        // Stworzenie klienta SMTP który wyśle wiadomość
                        using (var client = new SmtpClient())
                        {
                            client.Connect("smtp.gmail.com", 587, false);
                            client.Authenticate("test.app.kd@gmail.com", "Application_1");
                            client.Send(message);
                            client.Disconnect(true);
                        }
                    }
                }       
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

}
