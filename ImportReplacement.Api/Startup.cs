using ImportReplacement.Api.Repositories;
using ImportReplacement.Api.Repositories.Configurations;
using ImportReplacement.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ImportReplacement.Api
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
            services.AddControllers();
            services.AddScoped<IReplacementRepository, ReplacementRepository>();
            services.AddScoped<ITypesRepository, TypesRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<ISiteRepository, SiteRepository>();
            services.AddScoped<IConsumerRepository, ConsumerRepository>();
            services.AddScoped<IReasonRepository, ReasonRepository>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IElementRepository, ElementRepository>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mtr.SmartNet.Api", Version = "v1" });

            });
            var sqlConnectionConfiguration = new SqlConnectionConfiguration(Configuration.GetConnectionString("SmartNetDb"));
            services.AddSingleton(sqlConnectionConfiguration);
            var npgsqlConnectionConfiguration = new NpgsqlConnectionConfiguration(Configuration.GetConnectionString("ReplaceDb"));
            services.AddSingleton(npgsqlConnectionConfiguration);
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mtr.SmartNet.Api v1"));
            }
            // app.UseCors(policy =>
            //     policy.WithOrigins( "*")
            //         .AllowAnyMethod()
            //         .WithHeaders(HeaderNames.ContentType));
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
