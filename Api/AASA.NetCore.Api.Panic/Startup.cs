using System;
using AASA.NetCore.Lib.Helper;
using AASA.NetCore.Lib.Helper.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using AASA.NetCore.DomainService.Panic.RabbitMQ;
using Microsoft.Extensions.Options;
using AASA.NetCore.IDomainService.Panic.Interfaces;
using AASA.NetCore.DomainService.Panic.Services;

namespace AASA.NetCore.Api.Panic
{
    public class Startup
    {
        #region Global Properties
        public IConfiguration Configuration { get; }
        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
        #endregion

        #region Constructor
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ExecuteStartup();
        }
        #endregion

        public void ExecuteStartup()
        {
            try
            {
                ApplicationSettings.AppSettingsHelper = new ApplicationSettingsHelper(Configuration, ExecuteStartup);
                ApplicationSettings.AppSettingsHelper.GetGeneralConfigs();

                ApplicationSettings.CrmContext = new WebServiceReq(ApplicationSettings.CRMAPIURL, ApplicationSettings.RescueMeUser, ApplicationSettings.RescueMePassword, ApplicationSettings.CRMDBConn);
            }
            catch (Exception ex) { throw new Exception("Application Settings could not be found", ex); }           
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSingleton<IPanicService>(sp =>
                sp.GetRequiredService<IOptions<PanicMethods>>().Value);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Panic API",
                    Description = "This is an API to help service the clients of the AA of South Africa.",
                    // TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Diseri Pearson",
                        Email = "Diseri.Pearson@aasa.co.za",
                        // Url = "https://twitter.com/spboyer"
                    },
                    //License = new OpenApiLicense
                    //{
                    //    Name = "Use under LICX",
                    //    Url = new Uri("https://example.com/license"),
                    //}
                });
                c.IncludeXmlComments(XmlCommentsFilePath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });
            });
            services.AddRabbit(Configuration);

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = $"{ApplicationSettings.KeyCloakBaseUrl}/auth/realms/{ApplicationSettings.KeyCloakRealm}";
                o.RequireHttpsMetadata = false;
                o.Audience = "AAPanic"; // Client

                //o.Events = new JwtBearerEvents()
                //{
                //    OnAuthenticationFailed = c =>
                //    {
                //        c.NoResult();

                //        c.Response.StatusCode = 500;
                //        c.Response.ContentType = "text/plain";
                //        //if (Environment.IsDevelopment())
                //        //{
                //        return c.Response.WriteAsync(c.Exception.ToString());
                //        //}
                //        //return c.Response.WriteAsync("An error occured processing your authentication.");
                //    }
                //};
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
                {
                    if (!httpRequest.Headers.ContainsKey("X-Forwarded-Host")) return;

                    var serverUrl = $"{httpRequest.Headers["X-Forwarded-Proto"]}://" +
                        $"{httpRequest.Headers["X-Forwarded-Host"]}/" +
                        $"{httpRequest.Headers["X-Forwarded-Prefix"]}";

                    swaggerDoc.Servers = new List<OpenApiServer>()
                    {
                        new OpenApiServer { Url = serverUrl.Replace(":80","") }
                    };
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Panic API");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
