using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Services;
using BoxTrackLabel.API.Utils;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace BoxTrackLabel.API
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            try
            { 
                //Configurando la clase para manejar las config. en el archivo appSettings.json
                var appSettingsSection = Configuration.GetSection("AppSettings");
                services.Configure<AppSettings>(appSettingsSection);
                var appSettings = appSettingsSection.Get<AppSettings>();
                //Configurando el contexto EF para los Datos Maestros
                services.AddDbContext<MaestrosDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"), SqlServerOptions=>SqlServerOptions.CommandTimeout(appSettings.CommandTimeout)));
                //Configurando el contexto EF para Box Track Label
                services.AddEntityFrameworkSqlServer()
                .AddDbContext<BoxTrackDbContext>(options=>
                options.UseSqlServer(Configuration.GetConnectionString("boxTrackConnection"), SqlServerOptions=>SqlServerOptions.CommandTimeout(appSettings.CommandTimeout)));
                //Obteniendo la clave secreta para cifrar los tokens desde appSettings.json
                var key = Encoding.UTF8.GetBytes(appSettings.Secret);
                //Configurando la autenticación y los parámetros de validación de los JWT
                services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = "laaurora.do",
                        ValidAudience = "laaurora.do",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
                //Configurando la ruta de nuestra single page application (angular)
                services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "ClientApp/dist";
                });
                //Permitiendo conexiones desde otros origenes
                services.AddCors(action => {
                    action.AddPolicy("MyCorsPolicy", builder =>
                    {
                        builder //.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetIsOriginAllowed(x => true)
                        .Build();
                    });
                });
                //Configurando servicios y repositorios del proyecto para poder ser inyectados
                services.AddHttpContextAccessor();
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<ProcessRepository>();
                services.AddScoped<ProductosRepository>();
                services.AddScoped<SuplidoresRepository>();
                services.AddScoped<Productos_ProvRepository>();
                services.AddScoped<ModuleRepository>();
                services.AddScoped<ProductionRepository>();
                services.AddScoped<LabelRepository>();
                services.AddScoped<ProductsRepository>();
                services.AddScoped<ShiftRepository>();
                services.AddScoped<CustomerRepository>(); 
                services.AddScoped<ScheduleRepository>();
                services.AddScoped<StorageRepository>();
                services.AddScoped<AccessRepository>();
                services.AddScoped<ConfigurationRepository>();
                services.AddScoped<IRolPermissionRepository, RolPermissionRepository>();
                services.AddScoped<OrderRepository>();
                services.AddScoped<CodesRepository>();
                services.AddScoped<OrderSettingsRepository>();
                services.AddScoped<EmailAccountRepository>();
                services.AddScoped<OmsRepository>();
                services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
                services.AddScoped<EmailSender>();
                services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
                //services.AddHostedService<BackgroundTaskService>();
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(ConfigureJson);

                //Configurando documentación
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "BoxTrackLabel API",
                        Version = "v1",
                        Description = "Interfaz de la aplicación BoxTrackLabel",
                        Contact = new OpenApiContact()
                        {
                            Name = "Guseppe Rodríguez Peralta",
                            Email = "Guseppe.Rodriguez@laaurora.do"
                        }
                    });
                    c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
                    {
                        Description = "Header de autorización mediante el esquema JWT. \r\n\r\n Ingrese su JWT en el campo value. Ejemplo: \"Authorization: Bearer {token}\"",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        { 
                            new OpenApiSecurityScheme 
                            {
                                Name = "Authorization",
                                Type = SecuritySchemeType.ApiKey,
                                In = ParameterLocation.Header,
                                Reference = new OpenApiReference
                                { 
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Authorization"
                                },
                            },
                            new string[] {}
                        }
                    });

                    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex,ex.Message);
            }
        }

        private void ConfigureJson(MvcJsonOptions obj)
        {
            obj.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try 
            { 
                //Mantiene la estructura de la base de datos actualizada y se asegura de que esté creada.
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<BoxTrackDbContext>();
                    context.Database.Migrate();
                    SeedBoxtrackDataBase(context);
                }
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                app.UseCors("MyCorsPolicy");
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    if(env.IsDevelopment())
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BoxTrackLabel API");
                    }
                    else
                    {
                        c.SwaggerEndpoint("/BoxTrackLabel/swagger/v1/swagger.json", "BoxTrackLabel API");
                    }
                    c.RoutePrefix = "documentacion";
                });
                app.UseAuthentication();
                app.UseMvc();

                app.UseSpaStaticFiles();
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";
                    if (env.IsDevelopment())
                    {
                        //spa.UseAngularCliServer(npmScript: "start"); //inicia el servidor de angular con la depuración de la aplicación
                        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); //usar la app de angular sirviendo en esta ruta (para desarrollo)
                    }
                });
                //Log.Error("Starting up...");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

        }

        /// <summary>
        /// Verifica si ciertas tablas de configuraciones están vacias para precargarlas con datos.
        /// </summary>
        /// <param name="context">Contexto de datos EF en el cual se insertará la data de ser necesario</param>
        private void SeedBoxtrackDataBase(BoxTrackDbContext context)
        {
            if(!context.Processes.Any())
            {
                context.Processes.AddRange(new List<Process>()
                {
                    new Process
                    {
                        Codigo = "EmpMan", Descripcion="Empaque Manual", CodigoPermiso = 304,
                        UsuarioRegistro = "ITUSER"
                    },
                    new Process
                    {
                        Codigo = "EmpMec", Descripcion="Empaque Mecanizado", CodigoPermiso = 305,
                        UsuarioRegistro = "ITUSER"
                    }
                    //,
                    //new Process
                    //{
                    //    Codigo = "EmpPou", Descripcion="Empaque Pouch", , CodigoPermiso = 306,
                    //    UsuarioRegistro = "ITUSER"
                    //}
                });
                context.SaveChanges();
            }
            if(!context.Modules.Any())
            {
                var moduleList = new List<Module>()
                {
                    new Module
                    {
                        Codigo="ManMod1",IdProceso=1,Descripcion="Módulo 1",
                        UsuarioRegistro="ITUSER", NumeroModulo="01", TextoModulo="01"
                    },
                    new Module
                    {
                        Codigo="ManMod2",IdProceso=1,Descripcion="Módulo 2",
                        UsuarioRegistro="ITUSER", NumeroModulo="02", TextoModulo="02"
                    },
                    new Module
                    {
                        Codigo="ManMod3",IdProceso=1,Descripcion="Módulo 3",
                        UsuarioRegistro="ITUSER", NumeroModulo="03", TextoModulo="03"
                    },
                    new Module
                    {
                        Codigo="ManMod4",IdProceso=1,Descripcion="Módulo 4",
                        UsuarioRegistro="ITUSER", NumeroModulo="04", TextoModulo="04"
                    },
                    new Module
                    {
                        Codigo="ManMod5",IdProceso=1,Descripcion="Módulo 5",
                        UsuarioRegistro="ITUSER", NumeroModulo="05", TextoModulo="05"
                    },
                    new Module
                    {
                        Codigo="ManMod6",IdProceso=1,Descripcion="Módulo 6",
                        UsuarioRegistro="ITUSER", NumeroModulo="06", TextoModulo="06"
                    },
                    new Module
                    {
                        Codigo="MecMod1",IdProceso=2,Descripcion="M1-1",
                        UsuarioRegistro="ITUSER", NumeroModulo="01",TextoModulo="1-1"
                    },
                    new Module
                    {
                        Codigo="MecMod2",IdProceso=2,Descripcion="M1-2",
                        UsuarioRegistro="ITUSER", NumeroModulo="02",TextoModulo="1-2"
                    },
                    new Module
                    {
                        Codigo="MecMod3",IdProceso=2,Descripcion="M1-3",
                        UsuarioRegistro="ITUSER", NumeroModulo="03",TextoModulo="1-3"
                    },
                    new Module
                    {
                        Codigo="MecMod4",IdProceso=2,Descripcion="M1-4",
                        UsuarioRegistro="ITUSER", NumeroModulo="04",TextoModulo="1-4"
                    },
                    new Module
                    {
                        Codigo="MecMod5",IdProceso=2,Descripcion="M2-1",
                        UsuarioRegistro="ITUSER", NumeroModulo="05",TextoModulo="2-1"
                    },
                    new Module
                    {
                        Codigo="MecMod6",IdProceso=2,Descripcion="M2-2",
                        UsuarioRegistro="ITUSER", NumeroModulo="06",TextoModulo="2-2"
                    },
                    new Module
                    {
                        Codigo="MecMod7",IdProceso=2,Descripcion="M2-3",
                        UsuarioRegistro="ITUSER", NumeroModulo="07",TextoModulo="2-3"
                    },
                    new Module
                    {
                        Codigo="MecMod8",IdProceso=2,Descripcion="M2-4",
                        UsuarioRegistro="ITUSER", NumeroModulo="08",TextoModulo="2-4"
                    },
                    new Module
                    {
                        Codigo="MecMod9",IdProceso=2,Descripcion="M2-5",
                        UsuarioRegistro="ITUSER", NumeroModulo="09",TextoModulo="2-5"
                    },
                    new Module
                    {
                        Codigo="MecMod10",IdProceso=2,Descripcion="M2-6",
                        UsuarioRegistro="ITUSER", NumeroModulo="10",TextoModulo="2-6"
                    }
                    
                };
                context.BulkInsert(moduleList);
            }
            if(!context.Shifts.Any())
            {
                context.Shifts.AddRange(new List<Shift>()
                {
                    new Shift
                    {
                        Codigo="T1", Descripcion="Turno 1",
                        HoraInicio = new TimeSpan(6,0,0), HoraFin = new TimeSpan(15,0,0),
                        LetraRepresentacion = "X", UsuarioRegistro = "ITUSER"
                    },
                    new Shift
                    {
                        Codigo="T2", Descripcion="Turno 2",
                        HoraInicio = new TimeSpan(15,0,0), HoraFin = new TimeSpan(23,0,0),
                        LetraRepresentacion = "Y", UsuarioRegistro = "ITUSER"
                    },
                    new Shift
                    {
                        Codigo="T3", Descripcion="Turno 3",
                        HoraInicio = new TimeSpan(23,0,0), HoraFin = new TimeSpan(6,0,0),
                        LetraRepresentacion = "Z", UsuarioRegistro = "ITUSER"
                    }
                });
                context.SaveChanges();
            }
            if (!context.Storages.Any())
            {
                context.Storages.AddRange(new List<Storage>()
                {
                    new Storage
                    {
                        Codigo="X", 
                        Descripcion="Regular (X)",
                        UsuarioRegistro = "ITUSER"
                    },
                    new Storage
                    {
                        Codigo="S",
                        Descripcion="Frio (S)",
                        UsuarioRegistro = "ITUSER"
                    }
                });
                context.SaveChanges();
            }
            if (!context.ConfigurationValues.Any())
            {
                context.ConfigurationValues.Add(
                    new ConfigurationValue { Codigo = "confirmacionModulo" ,TextoConfiguracion = "Confirmación de módulo en impresión de etiquetas", ValorConfiguracion = "True", UsuarioRegistro= "ITUSER" }
                );
                context.SaveChanges();
            }
            if (!context.LabelConfigs.Any())
            {
                context.AddRange(
                    new List<LabelConfig>() 
                    { 
                        new LabelConfig
                        {
                            IdPais = "DO",
                            Direccion = "La Aurora,  S.A. Tamboril, Santiago, Rep. Dom.",
                            TextoPais = "Hecho en República Dominicana / Made in Dominican Republic",
                            TipoEtiqueta = "BOX",
                            LlevaLogo = false,
                            LlevaTextoInferior = true
                        },
                        new LabelConfig
                        {
                            IdPais = "DO",
                            TipoEtiqueta = "BOX",
                            LlevaLogo = false,
                            LlevaTextoInferior = false
                        },
                        new LabelConfig
                        {
                            IdPais = "DO",
                            TipoEtiqueta = "INDIVIDUAL",
                            LlevaLogo = true,
                            LlevaTextoInferior = false
                        },
                        new LabelConfig
                        {
                            IdPais = "DO",
                            TipoEtiqueta = "INDIVIDUAL",
                            LlevaLogo = false,
                            LlevaTextoInferior = false
                        }
                        //,
                        //new LabelText
                        //{
                        //    IdPais = "US",
                        //    Direccion = "La Aurora,  S.A. Tamboril, Santiago, Rep. Dom.",
                        //    Advertencia = "Sale only allowed in the united states",
                        //    TextoPais = "Made in Dominican Republic"
                        //},
                        //new LabelText
                        //{
                        //    IdPais = "US",
                        //    ClienteEspecifico = "230194",
                        //    Direccion = "La Aurora,  S.A. Tamboril, Santiago, Rep. Dom.",
                        //    Advertencia = "Sale only allowed in the united states",
                        //    TextoPais = "Made in Dominican Republic"
                        //},
                    }
                );
                context.SaveChanges();
            }
            if (!context.OrderSettings.Any())
            {
                context.AddRange(
                    new List<OrderSetting>() 
                    { 
                        new OrderSetting
                        {
                            ConnectionId = "c5825750-e925-4f29-a3cc-4a46b797236d",
                            OmsId = "34e4a261-9842-46d3-96cc-2c22fab4be05",
                            OmsUrl = "https://intuot.crpt.ru:12011",
                            Token = "1a866e5a-38d3-447c-a761-50be5fd86316",
                            ContactPerson = "La Aurora",
                            CreateMethodType = "SELF_MADE",
                            FactoryAddress = "Santiago",
                            FactoryCountry = "Dominican Republic",
                            FactoryId = 1234,
                            FactoryName = "La Aurora, S. A.",
                            ProductionLineId = 1,
                            ReleaseMethodType = "IMPORT"
                        }
                    }
                );
                context.SaveChanges();
            }
            if (!context.EmailAccounts.Any())
            {
                context.AddRange(
                    new List<EmailAccount>() 
                    { 
                        new EmailAccount
                        {
                            Email = "Roman.Aksenov@neska.ru",
                            IsCopy = false
                        },
                        new EmailAccount
                        {
                            Email = "Guseppe.Rodriguez@laaurora.do",
                            IsCopy = true
                        }
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
