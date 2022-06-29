using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TweetAPI.Models;
using TweetAPI.Services;
using TweetAPI.Repository;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TweetAPI.RabbitQueue;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace TweetAPI
{
    public class Startup
    {
        private IConfiguration _config;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _config = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TweetAppDatabaseSettings>(Configuration.GetSection("TweetAppDatabase"));

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IAuthRepository, AuthRepository>();
            services.AddSingleton<ITweetService, TweetService>();
            services.AddSingleton<ITweetRepository, TweetRepository>();

            services.AddControllers()
               .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "TweetAPI Service", Version = "1.0" }); });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = _config["Jwt:Issuer"],
                       ValidAudience = _config["Jwt:Issuer"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]))
                   };
               });
            services.AddMvc(setupAction =>
            {
                //JSON accept and return type  format
                var jsonInputFormatter =
                    setupAction.InputFormatters.OfType<NewtonsoftJsonInputFormatter>().FirstOrDefault();
                jsonInputFormatter?.SupportedMediaTypes.Add("application/json");
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddMemoryCache();
            services.AddSingleton(sp => RabbitHutch.CreateBus("localhost"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1.0/swagger.json", "TweetApp");

            });

            loggerFactory.AddLog4Net();
            app.UseHttpsRedirection();

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
