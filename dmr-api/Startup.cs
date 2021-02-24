using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DMR_API._Repositories.Interface;
using DMR_API._Repositories.Repositories;
using DMR_API._Services.Interface;
using DMR_API._Services.Services;
using DMR_API.Data;
using DMR_API.Helpers;
using DMR_API.Helpers.AutoMapper;
using DMR_API.SignalrHub;
using DMR_API._Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Quartz;
using DMR_API.SchedulerHelper;
using DMR_API.SchedulerHelper.Jobs;
using DMR_API.SignalrHub.Client;
using System.Collections.Specialized;

namespace DMR_API
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
            var appsettings = Configuration.GetSection("Appsettings").Get<Appsettings>();
            services.AddSignalR();
            services.AddLogging();
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            services.AddSingleton<IMongoDbSettings>(serviceProvider =>
        serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins(appsettings.CorsPolicy
                    ) //register for client
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<IoTContext>(options => options.UseMySQL(Configuration.GetConnectionString("IoTConnection")));
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }
            );
            //Auto Mapper
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IMapper>(sp =>
            {
                return new Mapper(AutoMapperConfig.RegisterMappings());
            });
            services.AddSingleton(AutoMapperConfig.RegisterMappings());
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                           .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };
                   //options.Events = new JwtBearerEvents
                   //{
                   //    OnMessageReceived = context =>
                   //    {
                   //        var accessToken = context.Request.Query["access_token"];
                   //        var token = context.Request.Headers.FirstOrDefault(x => x.Key == "Authorization");
                   //        // If the request is for our hub...
                   //        var path = context.HttpContext.Request.Path;
                   //        if (!string.IsNullOrEmpty(accessToken) &&
                   //            (path.StartsWithSegments("/ec-hub")))
                   //        {
                   //            // Read the token out of the query string
                   //            context.Token = accessToken;
                   //        }
                   //        return Task.CompletedTask;
                   //    }
                   //};
               });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Electronic Scale", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IJWTService, JWTService>();

            //Repository
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IGlueIngredientRepository, GlueIngredientRepository>();
            services.AddScoped<IGlueRepository, GlueRepository>();
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            services.AddScoped<IMakeGlueRepository, MakeGlueRepository>();
            services.AddScoped<IModelNameRepository, ModelNameRepository>();
            services.AddScoped<IUserDetailRepository, UserDetailRepository>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IPlanDetailRepository, PlanDetailRepository>();
            services.AddScoped<ILineRepository, LineRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IProcessRepository, ProcessRepository>();
            services.AddScoped<IArtProcessRepository, ArtProcessRepository>();
            services.AddScoped<IProcessRepository, ProcessRepository>();

            services.AddScoped<IArticleNoRepository, ArticleNoRepository>();
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IBuildingUserRepository, BuildingUserRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IModelNoRepository, ModelNoRepository>();
            services.AddScoped<IKindRepository, KindRepository>();
            services.AddScoped<IPartRepository, PartRepository>();
            services.AddScoped<IMaterialRepository, MaterialRepository>();
            services.AddScoped<IModelNoRepository, ModelNoRepository>();
            services.AddScoped<IModelNoRepository, ModelNoRepository>();
            services.AddScoped<IBPFCEstablishRepository, BPFCEstablishRepository>();
            services.AddScoped<IMixingInfoRepository, MixingInfoRepository>();
            services.AddScoped<IMixingInfoDetailRepository, MixingInfoDetailRepository>();
            services.AddScoped<IMixingRepository, MixingRepository>();
            services.AddScoped<IIngredientInfoRepository, IngredientInfoRepository>();
            services.AddScoped<IIngredientInfoReportRepository, IngredientInfoReportRepository>();
            services.AddScoped<IBPFCHistoryRepository, BPFCHistoryRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IStirRepository, StirRepository>();
            services.AddScoped<IAbnormalRepository, AbnormalRepository>();
            services.AddScoped<IRawDataRepository, RawDataRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IScaleMachineRepository, ScaleMachineRepository>();
            services.AddScoped<IDispatchRepository, DispatchRepository>();
            services.AddScoped<IDispatchListRepository, DispatchListRepository>();
            services.AddScoped<IGlueTypeRepository, GlueTypeRepository>();
            services.AddScoped<IToDoListRepository, ToDoListRepository>();
            services.AddScoped<IGlueNameRepository, GlueNameRepository>();
            services.AddScoped<IStationRepository, StationRepository>();
            services.AddScoped<ILunchTimeRepository, LunchTimeRepository>();
            services.AddScoped<IMailingRepository, MailingRepository>();
            services.AddScoped<IPeriodRepository, PeriodRepository>();
            services.AddScoped<IPeriodRepository, PeriodRepository>();
            services.AddScoped<IDispatchListDetailRepository, DispatchListDetailRepository>();


            //Services
            services.AddScoped<IMixingService, MixingService>();
            services.AddScoped<IGlueIngredientService, GlueIngredientService>();
            services.AddScoped<IGlueService, GlueService>();
            services.AddScoped<IMakeGlueService, MakeGlueService>();
            services.AddScoped<IIngredientService, IngredientService>();
            services.AddScoped<IModelNameService, ModelNameService>();
            services.AddScoped<IUserDetailService, UserDetailService>();
            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<ILineService, LineService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IArticleNoService, ArticleNoService>();
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IBuildingUserService, BuildingUserService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IBPFCEstablishService, BPFCEstablishService>();
            services.AddScoped<IModelNoService, ModelNoService>();
            services.AddScoped<IArtProcessService, ArtProcessService>();
            services.AddScoped<IProcessService, ProcessService>();
            services.AddScoped<IKindService, KindService>();
            services.AddScoped<IPartService, PartService>();
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<IMixingInfoService, MixingInfoService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IAbnormalService, AbnormalService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IScaleMachineService, ScaleMachineService>();

            services.AddScoped<IDispatchService, DispatchService>();
            services.AddScoped<IToDoListService, ToDoListService>();
            services.AddScoped<IGlueTypeService, GlueTypeService>();
            services.AddScoped<IStirService, StirService>();
            services.AddScoped<IMailingService, MailingService>();
            services.AddScoped<IBuildingLunchTimeService, BuildingLunchTimeService>();

            services.AddScoped<IStationService, StationService>(); //  duy trì trạng thái trong một request
            //extension
            services.AddScoped<IMailExtension, MailExtension>();

            services.AddHttpClient("default", client =>
            {
                client.BaseAddress = new Uri(appsettings.API_AUTH_URL);

                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            //Không bao giờ inject Scoped & Transient service vào Singleton service
            //Không bao giờ inject Transient Service vào Scope Service

            services.AddQuartz(async q =>
           {
               q.SchedulerId = "dmr-api";
               // Thuc thi luc 6:00, lap lai 1 tieng 1 lan
               await new SchedulerBase<ReloadTodoJob>().Start(1, IntervalUnit.Hour, 6, 00);

               // Thuc thi luc 6:00 đên 21 gio la ngung lap lai 30 phut 1 lan
               var startAt = TimeSpan.FromHours(6);
               var endAt = TimeSpan.FromHours(21);
               var repeatMins = 30;
               await new SchedulerBase<ReloadDispatchJob>().Start(repeatMins, startAt, endAt);
           });

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext)
        {

            dataContext.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Electronic Scale");
            });
            app.UseCors("CorsPolicy");
            app.UseCors(x => x
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           //    .SetIsOriginAllowed(origin => true) // allow any origin
                           .AllowCredentials()); // allow credentials
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ECHub>("/ec-hub");

            });
        }


    }
}
