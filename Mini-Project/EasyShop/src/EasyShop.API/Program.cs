using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Infrastructure.Persistence.Context;
using EasyShop.Infrastructure.Repositories;
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
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICouponRepository, CouponRepository>();
            builder.Services.AddScoped<ICouponUsageRepository, CouponUsageRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

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