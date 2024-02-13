using Customers.Api;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var customersConnectionString = builder.Configuration.GetConnectionString("CustomersConnectionString");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentMigratorCore();
builder.Services.ConfigureRunner(rb => rb
        .AddSQLite()
        .WithGlobalConnectionString(customersConnectionString)
        .ScanIn(typeof(_20240213_2026_Table_Customer_Create).Assembly).For.Migrations());
builder.Services
    .AddDbContext<CustomersDbContext>(options =>
        options.UseSqlite(customersConnectionString));
builder.Services.AddTransient<ICustomersService, CustomersService>();

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

using (var scope = app.Services.CreateScope())
{
    CustomersDbMigrator.Run(scope.ServiceProvider);
}

app.Run();
