using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WindesheimCalendar.Functions;
using WindesheimCalendar.Functions.Clients;

[assembly: FunctionsStartup(typeof(Startup))]
namespace WindesheimCalendar.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient<IWindesheimApiClient, WindesheimApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://api.windesheim.nl/api/");
            });
        }
    }
}
