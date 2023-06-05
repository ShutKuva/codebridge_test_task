
using Adapters;
using Adapters.Abstractions;
using Bll.Abstractions;
using Core;
using DAL;
using BLL;
using Dogs.Json;
using Dogs.Middlewares.Extensions;
using Microsoft.EntityFrameworkCore;
using Core.Db;
using Dogs.Abstractions;
using Dogs.Caches;

namespace Dogs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseJsonPropertyNamingPolicy();
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DogsContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DogsConnectionString"));
            });

            builder.Services.AddScoped<IDogsAdapter, DogsAdapter>();
            builder.Services.AddScoped(typeof(ICrudService<Dog, int, Page>), typeof(CrudService<Dog>));
            builder.Services.AddSingleton<ICache<string>, MyMemoryCache>();
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            builder.Services.Configure<ServerConfiguration>(builder.Configuration.GetSection("ServerConfiguration"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRequestBottleNeck();
            app.UseErrorHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}