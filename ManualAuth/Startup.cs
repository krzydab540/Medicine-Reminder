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

            // Adding database context

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection")));

            // Adding identity to implement login
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc()/*.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)*/;


            // Enable reminder service
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

            // Add Cultures for proper date and time formatting

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

            // necessary to use authentication
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

    // TimeReminderHostedService has a Timer, which calculates a minute-long intervals. After each one, it  finds medicine records from
    // database matching current date and time and if found runs SendNotification(), which sends an email via SMTP
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

                // WRONG METHOD! Should be replaced by LINQ espression, although it works (on small dataset)

                foreach (var item in dbContext.Medicine)
                {
                    var today = DateTime.Today;
                    

                    if (item.HourOfTaking==DateTime.Now.Hour&& item.MinuteOfTaking == DateTime.Now.Minute && item.Day==today.DayOfWeek)
                    {
                        // Creating message via template and adressing it to the patient, whose record was matching database.
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("Medicine Reminder", "test.app.kd@gmail.com"));

                        message.To.Add(new MailboxAddress("To Add", item.Id_patient));
                        
                        message.Subject = "Hey! It's time to take your medicine!";
                        message.Body = new TextPart("plain")
                        {
                            Text = "Hello, " + item.Id_patient + "! Don't forget to take " + item.Name
                        };

                        // Create SMTP Client
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
