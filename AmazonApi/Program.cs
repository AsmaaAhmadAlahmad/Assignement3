using Assignement3_Domain.Services;
using Assignement3_Domain.Helper;
using Assignement3_Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace Assignement3_Domain
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext
            builder.Services.AddDbContext<Assingment3DbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Assingment3DbContext")));



            // serilog »«” Œœ«„ „ﬂ »… Logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log/", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Information("Hello, world!");




            // Services
            builder.Services.AddControllers()
                            .AddNewtonsoftJson() // Â–Â  „‰ √Ã· «· ⁄œÌ· «·Ã“∆Ì
                            .AddXmlDataContractSerializerFormatters(); // xml Â–Â „‰ √Ã· œ⁄„ «·«” Ã«»… „‰ «·‰Ê⁄

            builder.Services.AddEndpointsApiExplorer();

            // «÷«›… «·”Ê«€— „⁄ «÷«›… ŒÌ«—«   „ﬂÌ‰ «÷«›… «· ÊﬂÌ‰ „‰ Œ·«·Â
            builder.Services.AddSwaggerGen(options => {
                options.AddSecurityDefinition("Assingment3ApiAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Description = "Please enter the authentication token "
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Assingment3ApiAuth"
                            }
                        },new List<string>()
                    }
                }) ;


                }) ;


            //  ”ÃÌ· «·—Ì»Ê“Ì Ê—Ì «·⁄«„…
            builder.Services.AddScoped(typeof(IGenericRepository<,,>), typeof(GenericRepository<,,>));

            // Authentication
            builder.Services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["Athentication:Issuer"],
                    ValidAudience = builder.Configuration["Athentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Athentication:secretKey"]))
                };
            });

            // Authorization Policy
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ShouldBeUser", policy =>
                {
                    policy.RequireClaim(ClaimTypes.Role, "User");
                    policy.RequireClaim(ClaimTypes.Name, "Asmaa");
                    policy.RequireAuthenticatedUser();
                });
            });

            // AutoMapper
            builder.Services.AddTransient(typeof(HelperMapper<,,>));

            

            var app = builder.Build();  

         
            app.UseHttpsRedirection();

           app.UseAuthentication();  

            app.UseAuthorization();

            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.Run();
        }
    }
}
