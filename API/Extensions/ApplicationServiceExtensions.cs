using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
       
         public static IServiceCollection AddApplicationServices(this 
         IServiceCollection services, IConfiguration config)
         {
             
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });                        
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IUserRepository, UserRepository>();            
            //services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly); //finding where we mapped our automapper class
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 
            return services;
         }
    }
}