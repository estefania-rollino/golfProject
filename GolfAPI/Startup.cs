﻿using System;
using System.Text;
using System.Threading.Tasks;
using GolfAPI.Configuration;
using GolfAPI.Core.Contracts;
using GolfAPI.Core.Contracts.DataAccess;
using GolfAPI.Core.Contracts.Managers;
using GolfAPI.Core.Contracts.Managers.Builders;
using GolfAPI.Core.Contracts.Managers.Parsers;
using GolfAPI.Core.Managers;
using GolfAPI.Core.Managers.Builders;
using GolfAPI.Core.Managers.parsers;
using GolfAPI.DataLayer.ADL;
using GolfAPI.DataLayer.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace GolfAPI
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
            //enable request from our angular app
            //services.AddCors(
            //    o => o.AddPolicy("AllowAngularApp", builder =>
            //    {
            //        builder.WithOrigins("http://localhost:4200")
            //               .AllowAnyMethod()
            //               .AllowAnyHeader();
            //    }));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var appSettings = Configuration.GetSection("appSettings");
            services.Configure<AppSettings>(appSettings);
            var settings = appSettings.Get<AppSettings>();
            var encodedKey = Encoding.ASCII.GetBytes(settings.SecretKey);

            // create repository service
            services.AddScoped<BaseRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IComponentRepository, ComponentRepository>();

            services.AddScoped<IOrderParser, OrderParser>();
            services.AddScoped<IOrderBuilder, OrderBuilder>();

            //create managers
            services.AddScoped<IOrderManager, OrderManager>();
            services.AddScoped<IComponentOrderManager, ComponentOrderManager>();
            services.AddScoped<IUserManager, UserManager>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               x.Events = new JwtBearerEvents
               {
                   OnTokenValidated = context =>
                   {
                       var userService = context.HttpContext.RequestServices.GetRequiredService<IUserManager>();
                       var userId = int.Parse(context.Principal.Identity.Name);
                       var user = userService.GetUserById(userId);
                       if (user == null)
                       {
                           // return unauthorized if user no longer exists
                           context.Fail("Unauthorized");
                       }
                       return Task.CompletedTask;
                   }
               };
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(encodedKey),
                   ValidateIssuer = false,
                   ValidateAudience = false
               };
           });


                //database config
                var connection = Configuration.GetConnectionString("GolfTestConnection");
            services.AddDbContext<GolfDatabaseContext>
                (options => {
                    options.UseSqlServer(connection);
             
                });

            //swagger config
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "golf API", Version = "V1" });
            });

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(x => { x.AllowAnyHeader(); x.AllowAnyMethod(); x.AllowAnyOrigin(); });

            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "golfAPI");
            });

        }
    }
}
