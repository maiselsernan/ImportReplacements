using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorTable;
using ImportReplacement.Web.Services;
using ImportReplacement.Web.Services.Interfaces;
using ImportReplacement.Web.Shared;
using Tewr.Blazor.FileReader;
using Microsoft.Extensions.Logging;

namespace ImportReplacement.Web
{
    public class Program
    {

        private static Uri _baseAddress;
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            var apiBaseAddress = builder.Configuration["ApiBaseAddress"];
            _baseAddress  = new Uri(apiBaseAddress);
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
            builder.Services.AddBlazoredToast();
            builder.Services.AddBlazorTable();
            builder.Services.AddHttpClient<IChannelService, ChannelService>(client =>
            {
                client.BaseAddress = _baseAddress;
            });
            builder.Services.AddHttpClient<IReplacementService, ReplacementService>(client =>
            {
                 client.BaseAddress = _baseAddress;
            });
            builder.Services.AddHttpClient<ITypesService, TypesService>(client =>
            {
                client.BaseAddress = _baseAddress;
            });
            builder.Services.AddHttpClient<IFileService, FileService>(client =>
            {
                client.BaseAddress = _baseAddress;
            });
            builder.Services.AddHttpClient<ISiteService, SiteService>(client =>
            {
                client.BaseAddress = _baseAddress;
            });
            builder.Services.AddHttpClient<IConsumerService, ConsumerService>(client =>
            {
                client.BaseAddress = _baseAddress;
            });
            builder.Services.AddHttpClient<IReasonService, ReasonService>(client =>
            {
                client.BaseAddress = _baseAddress;
            });
            builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
            {
                client.BaseAddress = _baseAddress;
            });
              builder.Services.AddHttpClient<IElementService, ElementService>(client =>
            {
                client.BaseAddress = _baseAddress;
            });
            
            builder.RootComponents.Add<App>("app");
            builder.Services.AddSingleton<RoutingHelper>();
            builder.Services.AddFileReaderService(options =>
            {
                options.UseWasmSharedBuffer = true;
            });
            builder.Services.AddBlazorDownloadFile();
            builder.Logging.SetMinimumLevel(LogLevel.Warning);
            await builder.Build().RunAsync();
        }
    }
}
