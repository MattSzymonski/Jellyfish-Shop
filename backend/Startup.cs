using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using AutoMapper;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Console.WriteLine("Backend started! ({0})", env.EnvironmentName);

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Read application settings
            var applicationSettings = Configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();
            services.AddSingleton<ApplicationSettings>(applicationSettings); // Make them available in all services

            // Services
            services.AddSingleton<SecurityService>();
            services.AddSingleton<JellyfishService>();
            services.AddAutoMapper(typeof(Startup));

            // Controllers
            services.AddControllers();
                //.AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<EnumSchemaFilter>(); // Display enum variables of model as strings instead of integers
                c.EnableAnnotations(); // Enable endpoints annotations
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend", Version = "v1" });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
            });

            //services.AddSwaggerGenNewtonsoftSupport();

            // JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = applicationSettings.JwtSettings.Issuer,
                    ValidAudience =  applicationSettings.JwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(applicationSettings.JwtSettings.Secret)),
                };
            });

            // CORS
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Read application settings
            var applicationSettings = Configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            
            app.UseAuthorization();

            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
                app.UseSwagger(option =>
                {   
                    // Json file location (mydomain.com/api/swagger/v1/swagger.json)
                    option.RouteTemplate = "api/swagger/{documentName}/swagger.json";
                });
                app.UseSwaggerUI(c => {
                    // Where to look for json file
                    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Backend");
                    // Prefix in the URL of the UI (mydomain.com/api/swagger/index.html)
                    c.RoutePrefix = "api/swagger";
                });

                // Shows UseCors with CorsPolicyBuilder.
                app.UseCors(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    // Needed to display enum variables of model as strings instead of integers
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                model.Type = "string";
                model.Format = null;
                model.Enum.Clear();
                foreach (string enumName in Enum.GetNames(context.Type))
                {
                    System.Reflection.MemberInfo memberInfo = context.Type.GetMember(enumName).FirstOrDefault(m => m.DeclaringType == context.Type);
                    EnumMemberAttribute enumMemberAttribute = memberInfo == null ? null : memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false).OfType<EnumMemberAttribute>().FirstOrDefault();
                    string label = enumMemberAttribute == null || string.IsNullOrWhiteSpace(enumMemberAttribute.Value) ? enumName : enumMemberAttribute.Value;
                    model.Enum.Add(new OpenApiString(label));
                }
            }
        }
    }
	
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Jellyfish, JellyfishDTO>();
            CreateMap<AddJellyfishDTO, Jellyfish>();
        }
    }
}

