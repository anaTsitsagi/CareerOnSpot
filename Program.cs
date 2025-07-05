using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using AccountingSoftware;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace AccountingSoftware
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Root components
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // HttpClient
            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // 1) Register localization and point at Resources folder
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            var host = builder.Build();

            // 2) Determine culture (from localStorage or fallback to browser)
            var js = host.Services.GetRequiredService<IJSRuntime>();
            string storedCulture = await js.InvokeAsync<string>("localStorage.getItem", "blazorCulture");

            CultureInfo culture;
            if (!string.IsNullOrWhiteSpace(storedCulture))
            {
                culture = new CultureInfo(storedCulture);
            }
            else
            {
                // fallback to browser language, or "en" if not supported
                var browserLang = await js.InvokeAsync<string>("getBrowserLanguage");
                culture = new CultureInfo(browserLang ?? "en");
            }

            // 3) Apply culture globally
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // 4) Run
            await host.RunAsync();
        }
    }
}
