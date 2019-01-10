using System;
using HBDotCom.Areas.Identity.Services;
using HBDotCom.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(HBDotCom.Areas.Identity.IdentityHostingStartup))]
namespace HBDotCom.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddTransient<IEmailSender>(i =>
                new EmailSender(
                    context.Configuration["EmailSender:Host"],
                    context.Configuration.GetValue<int>("EmailSender:Port"),
                    context.Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    context.Configuration["EmailSender:UserName"],
                    context.Configuration["EmailSender:Password"]
                ));
            });
        }
    }
}