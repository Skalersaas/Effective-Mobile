using Microsoft.EntityFrameworkCore;
using Effective_Mobile.Data;

namespace Effective_Mobile
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Adding log and order paths to the logger
            Helpers.Logger.Configure(builder.Configuration.GetSection("Paths"));

            // Add services to the container.
            builder.Services.AddControllers();
            var connectionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<ApplicationContext>
                (options => options.UseNpgsql(connectionString));

            //Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
