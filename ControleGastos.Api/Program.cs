using ControleGastos.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Lê a connection string do appsettings.json.
// Se não existir/estiver vazia, usa um fallback para garantir que a API suba com SQLite local.

var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? "Data Source=controle-gastos.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(cs)
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", p =>
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevCors");

app.MapControllers();

app.Run();
