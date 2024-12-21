using APIDemo.BAL.Entity;
using APIDemo.BAL.Interface;
using APIDemo.DAL.Common.Helpers;
using APIDemo.DAL.Common.Interface;
using APIDemo.DAL.Enum;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDapperService, DapperService>();
// Get the database type from configuration
var databaseType = builder.Configuration.GetValue<string>("DatabaseType");

if (databaseType == DatabaseType.PostgreSQL.ToString())
{ builder.Services.AddScoped<IMaster<User>, APIDemo.DAL.Repositories.PgSQL.Master.UserRepository>(); }
else if (databaseType == DatabaseType.SQL.ToString())
{ builder.Services.AddScoped<IMaster<User>, APIDemo.DAL.Repositories.SQL.Master.UserRepository>(); }
else
{
    throw new InvalidOperationException("Database type not configured properly.");
}


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
