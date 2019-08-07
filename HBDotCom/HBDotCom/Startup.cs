using HBDotCom.Areas.Identity.Models;
using HBDotCom.Data;
using HBDotCom.Models;
using HBDotCom.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Threading;
using Tweetinvi;
using Tweetinvi.AspNet;
using Tweetinvi.Core.Public.Models.Authentication;

namespace HBDotCom
{
    public class Startup
    {
        private readonly string _connectionString;

        public static WebhookConfiguration WebhookConfiguration { get; set; }

        public IConfiguration Configuration { get; }
        public IConfiguration BuilderConfig { get; }

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();

            BuilderConfig = builder.Build();
            Configuration = configuration;

            if (env.IsDevelopment())
            {
                _connectionString = Configuration.GetConnectionString("DefaultConnection");
            } else
            {
                _connectionString = $@"Server={BuilderConfig["MYSQL_SERVER_NAME"]};Database={BuilderConfig["MYSQL_DATABASE"]};Uid={BuilderConfig["MYSQL_USER"]};Pwd={BuilderConfig["MYSQL_PASSWORD"]}";
            }
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Configure Identity
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
            });

            WaitForDBInit(_connectionString);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(_connectionString));

            

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddDefaultIdentity<ApplicationUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddAuthentication()
            .AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                twitterOptions.RetrieveUserDetails = true;
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            })
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });


            EmailServerConfiguration config = new EmailServerConfiguration
            {
                SmtpPassword = "Password",
                SmtpServer = "smtp.someserver.com",
                SmtpUsername = "awesomeemail@nickolasfisher.com"
            };

            EmailAddress FromEmailAddress = new EmailAddress
            {
                Address = "myemailaddress@somesite.com",
                Name = "Nick Fisher"
            };


            services.AddSingleton<EmailServerConfiguration>(config);
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton<EmailAddress>(FromEmailAddress);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
            }

            context.Database.Migrate();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.Use((httpContext, next) =>
            {
                httpContext.Request.Scheme = "https";
                return next();
            });

            app.UseAuthentication();

            //app.UseHttpsRedirection();

            Plugins.Add<WebhooksPlugin>();

            var consumerToken = Configuration["Authentication:Twitter:ConsumerKey"];
            var consumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
            var accessToken = Configuration["Authentication:Twitter:AccessToken"];
            var accessTokenSecret = Configuration["Authentication:Twitter:AccessTokenSecret"];
            var appCreds = Auth.SetApplicationOnlyCredentials(consumerToken, consumerSecret, true);

            WebhookConfiguration = new WebhookConfiguration(new ConsumerOnlyCredentials(consumerToken, consumerSecret)
            {
                ApplicationOnlyBearerToken = appCreds.ApplicationOnlyBearerToken
            });

            app.UseTweetinviWebhooks(WebhookConfiguration);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Try to connect to the db with exponential backoff on fail.
        private static void WaitForDBInit(string connectionString)
        {
            var connection = new MySqlConnection(connectionString);
            int retries = 1;
            while (retries < 7)
            {
                try
                {
                    Console.WriteLine("Connecting to db. Trial: {0} with {1}", retries, connectionString);
                    connection.Open();
                    connection.Close();
                    break;
                }
                catch (MySqlException)
                {
                    Thread.Sleep((int)Math.Pow(2, retries) * 1000);
                    retries++;
                }
            }
        }
    }
}
