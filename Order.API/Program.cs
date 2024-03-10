using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Servisleri konteyn�ra ekleyin.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderRequestCompletedEventConsumer>();
    x.AddConsumer<OrderRequestFailedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint(RabbitMQSettingsConst.OrderRequestCompletedEventtQueueName, x =>
        {
            x.ConfigureConsumer<OrderRequestCompletedEventConsumer>(context);
        });

        cfg.ReceiveEndpoint(RabbitMQSettingsConst.OrderRequestFailedEventtQueueName, x =>
        {
            x.ConfigureConsumer<OrderRequestFailedEventConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

// services.AddMassTransitHostedService();

builder.Services.AddControllers();
// Swagger/OpenAPI'nin nas�l yap�land�r�laca��n� daha fazla ��renmek i�in https://aka.ms/aspnetcore/swashbuckle adresine gidin
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// HTTP istek i�leme boru hatt�n� yap�land�r�n.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
