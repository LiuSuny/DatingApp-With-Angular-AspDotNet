﻿using System.Text;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.SignaIR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API
{
    public class Startup
    {
      
        private readonly IConfiguration _config;
        
        public Startup(IConfiguration config)
        {
            _config = config;
           
            
        }

      /// <summary>
      /// This method is use to add services to our project like DI, DTO or overall Injection
      /// </summary>
      /// <param name="services">Config service</param>
        public void ConfigureServices(IServiceCollection services)
        {
           
           services.AddApplicationServices(_config); //created an extension method for it 

            services.AddControllers();
                    //.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter()));
            
            services.AddCors();
            
            services.AddIdentityServices(_config);

           //injecting the signaIR service 
            services.AddSignalR();
            
        }

                /// <summary>
                ///  This use contain our mddleware and use to get our https run environment
                /// </summary>
                /// <param name="app">request pipeline</param>
                /// <param name="env">runtime environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMiddleware<ExceptionMiddleware>(); //this handle our error messages instead of the below

            // Configure the HTTP request pipeline.
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }

            app.UseHttpsRedirection();


            app.UseRouting();
           
            // app.UseCors(x => 
            // x.AllowAnyHeader()
            // .AllowAnyMethod()
            // .AllowCredentials() //we need to supply b/c of use of singaIR accesstoken
            // .WithOrigins("http://localhost:4200"));
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
           .WithOrigins("http://localhost:4200", "https://localhost:4200"));

            app.UseAuthentication();
           
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllers();
                //telling our routing or our api about signaIR endpoint and the (hubs/presence--the route to access from)
                endpoint.MapHub<PresenceHub>("hubs/presence");
                //Message hub endpoint
                endpoint.MapHub<MessageHub>("hubs/message");
                //endpoint.MapFallbackToController("index", "FallBack"); //uncomment once u have wwwroot folder
            });

             

        }
    }
}
