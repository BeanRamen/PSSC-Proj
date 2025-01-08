using lab3.Models;
using lab3.Repos;
using lab3.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PricesContext = lab3.Repos.PricesContext;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddTransient<GenerateAWBWorkflow>();
            builder.Services.AddTransient<GenerateReceiptWorkflow>();
            builder.Services.AddTransient<ModifyCartWorkflow>();
            
            builder.Services.AddDbContext<PricesContext>
                (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddTransient<IPricesRepository, PricesRepository>();
            builder.Services.AddTransient<ICartsRepository, CartsRepository>();
            builder.Services.AddTransient<PublishItemWorkflow>();

            builder.Services.AddHttpClient();

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Example.Api", Version = "v1" });
            });


            WebApplication app = builder.Build();

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