using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Services.Implementations;
using WebAPI.Mapping;

var builder = WebApplication.CreateBuilder(args);

MappingConfig.RegisterMappings();

// Füge CORS-Dienst hinzu
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        // URL der Blazor WebAssembly-Anwendung (Client), der auf die API zugreifen darf
        policy.WithOrigins("http://localhost:5205", "http://localhost:8080", "http://localhost:8081")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHealthChecks();

// Füge weitere Dienste hinzu
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* REPOSITORIES */
builder.Services.AddScoped<IVineyardRepository, VineyardRepository>();
builder.Services.AddScoped<IWorkInfoRepository, WorkinformationRepository>();
builder.Services.AddScoped<IFinanceRepository, FinanceRepository>();
builder.Services.AddScoped<IBookingObjectRepository, BookingObjectRepository>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<IVineyardHasStatusRepository, VineyardHasStatusRepository>();
builder.Services.AddScoped<IMachineRepository, MachineRepository>();
builder.Services.AddScoped<IRabbitMQRepository, RabbitMQRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IMachineHasStatuRepository, MachineHasStatusRepository>();
builder.Services.AddScoped<IBatchRepository, BatchRepository>();
builder.Services.AddScoped<ITankRepository, TankRepository>();
builder.Services.AddScoped<IVineyardHasBatchRepository, VineyardHasBatchRepositoryRepository>();

builder.Services.AddDbContextFactory<ApplicationDbContext>(
    options => options.UseMySql(
        builder.Configuration.GetConnectionString("DatabaseConnection"),
        new MySqlServerVersion(new Version(8, 0, 32))
    )
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Verwende die CORS-Richtlinie
app.UseCors("AllowBlazorClient");

app.UseAuthorization();

app.MapControllers();

app.Run();