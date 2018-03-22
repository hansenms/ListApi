using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.IdentityModel.Tokens;
using ListApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ListApi
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
            services.AddDbContext<ListContext>(opt => opt.UseInMemoryDatabase("List"));
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new Info { Title = "List API", Version = "v1" });

            });

            services.AddAuthentication(sharedOptions =>
            {

                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options => {
                options.Audience = Configuration["ADClientID"];
                options.Authority = Configuration["ADInstance"];
                options.TokenValidationParameters = new TokenValidationParameters() {
                    //WebApp API is currently a multi-tenant application
                    //Code below would make it check that issuer is from certain tenants 
                    ValidateIssuer = false,
                    /* 
                        ValidateIssuer = true,
                        ValidIssuers =  new List<string>() { "https://sts.windows.net/TENANT-ID/" },
                    */
                    ValidateAudience = true
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "List API V1");
            });

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
