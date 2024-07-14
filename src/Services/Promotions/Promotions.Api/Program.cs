using Promotions.Api.Data;
using Promotions.Api.Data.Models;
using Promotions.Api.Services;
using Promotions.Api.Services.PromotionTypes;
using System.Reflection;

namespace Promotions.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddSingleton<IPromotionsRepository, OptionsPromotionsRepository>()
            .AddSingleton<ICartAdjustmentService, CartAdjustmentService>()
            .AddSingleton<IMoneyOffPromotion, MoneyOffPromotion>();

        builder.Services
            .Configure<List<Promotion>>(builder.Configuration.GetSection("Promotions"));

        builder.Services.AddControllers();
        builder.Services.AddProblemDetails();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
