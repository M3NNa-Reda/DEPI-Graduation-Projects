using EasyShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace EasyShop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseExceptionHandler("/error");
            //catches exceptions thrown in the following middlewares so it must come early
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            // Determines which endpoint will handle the request
            app.UseAuthentication();
            app.UseAuthorization();
            // Checks if the user is authorized to access the selected endpoint
            // so it must come after routing 
            app.MapControllers();

            app.Run();
        }
    }
}