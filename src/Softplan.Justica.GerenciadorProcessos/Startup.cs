using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Softplan.Justica.GerenciadorProcessos.Application.Services;
using Softplan.Justica.GerenciadorProcessos.Application.Services.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Converters;
using Softplan.Justica.GerenciadorProcessos.Domain.Repository;
using Softplan.Justica.GerenciadorProcessos.Domain.Service;
using Softplan.Justica.GerenciadorProcessos.Domain.Service.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators;
using Softplan.Justica.GerenciadorProcessos.Domain.Validators.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Extensions;
using Softplan.Justica.GerenciadorProcessos.Handlers;
using Softplan.Justica.GerenciadorProcessos.Handlers.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Helpers;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Email.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification;
using Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Notification.Interfaces;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Context;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Repository;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.UnitOfWork;

namespace Softplan.Justica.GerenciadorProcessos
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
                });

            services.AddLogging(log =>
            {
                log.AddConsole();
            });

            services.Configure<EmailServiceSettings>(Configuration.GetSection("EmailServiceSettings"));

            services.AddDbContext<GerenciadorProcessosDbContext>(_ => _.UseMySql(this.Configuration.GetConnectionString("gepro")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IGerenciadorProcessosDbContext>((sp) => sp.GetService<GerenciadorProcessosDbContext>());
            services.AddScoped<INotificationContext, NotificationContext>();

            services.AddTransient<IApiResultHandler, ApiResultHandler>();
            services.AddTransient<IProcessoService, ProcessoService>();
            services.AddTransient<IResponsavelService, ResponsavelService>();
            services.AddTransient<IProcessoDomainService, ProcessoDomainService>();

            services.AddTransient<IProcessoRepository, ProcessoRepository>();
            services.AddTransient<IResponsavelRepository, ResponsavelRepository>();
            services.AddTransient<ISituacaoProcessoRepository, SituacaoProcessoRepository>();

            services.AddTransient<IEmailMessageBuilder, EmailMessageBuilder>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddTransient<IAtualizarProcessoCommandValidator, AtualizarProcessoCommandValidator>();
            services.AddTransient<IAtualizarResponsavelCommandValidator, AtualizarResponsavelCommandValidator>();
            services.AddTransient<ICriarProcessoCommandValidator, CriarProcessoCommandValidator>();
            services.AddTransient<ICriarResponsavelCommandValidator, CriarResponsavelCommandValidator>();
            services.AddTransient<IObterProcessoQueryValidator, ObterProcessoQueryValidator>();
            services.AddTransient<IObterProcessosQueryValidator, ObterProcessosQueryValidator>();
            services.AddTransient<IObterResponsaveisQueryValidator, ObterResponsaveisQueryValidator>();
            services.AddTransient<IObterResponsavelQueryValidator, ObterResponsavelQueryValidator>();
            services.AddTransient<IRemoverProcessoCommandValidator, RemoverProcessoCommandValidator>();
            services.AddTransient<IRemoverResponsavelCommandValidator, RemoverResponsavelCommandValidator>();

            services.AddMediatR(Assembly.Load("Softplan.Justica.GerenciadorProcessos.Domain"));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gerenciador Processo Api", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gerenciador Processo Api v1");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.ConfigureExceptionHandler();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<IGerenciadorProcessosDbContext>();

            DbInitializerHelper.EnsureDatabase(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope());
        }
    }
}