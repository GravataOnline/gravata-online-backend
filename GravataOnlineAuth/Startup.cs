using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using GravataOnlineAuth.Common;
using GravataOnlineAuth.Ioc;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GravataOnlineAuth.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;
using GravataOnlineAuth.Common.Email;

namespace GravataOnlineAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Base.DBTYPE = Configuration["Settings:TIPOBANCO"];
            Base.JWTKEY = Configuration["Jwt:Key"];
            Base.ISSUER = Configuration["Jwt:Issuer"];
            Base.AUDIENCE = Configuration["Jwt:Audience"];
            switch (Base.DBTYPE)
            {
                case "SQL":
                    break;
                case "ORACLE":
                    Base.AUTHCONNECTIONSTRING = Configuration["ConnectionStrings:AuthOracle"];
                    Base.CONNECTIONSTRING = Configuration["ConnectionStrings:Oracle"];
                    break;
            }
            Mail.SENDER = Configuration["MailSettings:Sender"];
            Mail.USER = Configuration["MailSettings:User"];
            Mail.SENDERNAME = Configuration["MailSettings:SenderName"];
            Mail.MAILPORT = Configuration["MailSettings:MailPort"];
            Mail.MAILSERVER = Configuration["MailSettings:MailServer"];
            Mail.PASSWORD = Configuration["MailSettings:Password"];
            Mail.USESSL = Configuration["MailSettings:UseSSL"];
            Mail.USEAUTHENTICATION = Configuration["MailSettings:UseAuthentication"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsAllowAll",
                builder => builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            });

            services.AddAuthentication
                 (JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true,
                          ValidateIssuerSigningKey = true,

                          ValidIssuer = Configuration["Jwt:Issuer"],
                          ValidAudience = Configuration["Jwt:Audience"],
                          IssuerSigningKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                      };
                  });
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddControllers();
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Gravata Online - Autorização e Autenticação",
                    Version = "v1",
                    Description = "Repositório de API para autorização e autenticação de usuários",
                    Contact = new OpenApiContact
                    {
                        Name = "",
                        Email = ""
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Gravata Online - Autorização e Autenticação",
                    }

                });
                // To Enable authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Authorization",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
            });
            });
            RepositoryInjector.RegisterRepositories(services);
            Connections.SetMainConnection();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/html";
                    IExceptionHandlerPathFeature exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    new MyLog().GerarLog(exceptionHandlerPathFeature.Path, " : ", exceptionHandlerPathFeature.Error);
                    await context.Response.WriteAsync($"Internal Error \nPath : {exceptionHandlerPathFeature.Path} \nError: {exceptionHandlerPathFeature.Error.Message} ");

                });
            });

            var supportedCultures = new[] {
                new CultureInfo("en-US"),
                new CultureInfo("en-AU"),
                new CultureInfo("en-GB"),
                new CultureInfo("en"),
                new CultureInfo("es-ES"),
                new CultureInfo("es-MX"),
                new CultureInfo("es"),
                new CultureInfo("fr-FR"),
                new CultureInfo("fr"),
            };



            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsAllowAll");
            app.UseSwagger();
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/v1/swagger.json", "Gravata Online - Autorização e Autenticação");
                option.RoutePrefix = string.Empty;
            });

            app.UseMiddleware<HXMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
