using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbAccess.Repositories;
using DbAccess.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using DbAccess;

using Microsoft.VisualBasic;
using StudentManagement.API.RedisCache;


namespace StudentManagement.API
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

            services.AddControllers();
            services.AddSingleton<IRedisCacheService, RedisCacheService>();
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<StudentManagementContext>(opt => opt.UseMySql(connectionString,
                ServerVersion.AutoDetect(connectionString)));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["RedisCacheServerUrl"];
            });
            services.AddScoped<IStudentRepository, StudentRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "StudentManagement.API", Version = "v1"});
            });
            // Code omitted for brevity


        }

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors(builder => builder.AllowAnyOrigin());
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentManagement.API v1"));


            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
