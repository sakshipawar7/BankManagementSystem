using BankManagementSystem.DAL;
using BankManagementSystem.Repo;
using BankManagementSystem.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BankManagementSystem.Helpers;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddAutoMapper(typeof(CustomerMapper));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//builder.Services.AddScoped<ISavingsService, SavingsAccountService>();
//builder.Services.AddScoped<ICurrentService, CurrentAccountService>();
builder.Services.AddScoped<SavingsAccountService>();
builder.Services.AddScoped<CurrentAccountService>();
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();

//builder.Services.AddScoped<AccountServiceDelegate>(provider => (type) =>
//{
//    switch (type)
//    {
//        case AccountType.Savings: return provider.GetRequiredService<SavingsAccountService>();
//        case AccountType.Current: return provider.GetRequiredService<CurrentAccountService>();
//        default: throw new NotImplementedException();
//    }
//});

//Register the delegate
builder.Services.AddScoped<AccountServiceDelegate>(provider => type =>
{
    switch (type)
    {
        case AccountType.Savings:
            return provider.GetService<SavingsAccountService>();
        case AccountType.Current:
            return provider.GetService<CurrentAccountService>();
        default:
            throw new NotImplementedException($"No service registered for account type {type}");
    }
});

//builder.Services.AddScoped<Savings>();
//builder.Services.AddScoped<Current>();
//builder.Services.AddScoped<IAccountsRepo, AccountsRepo>();
//builder.Services.AddScoped<AccountServiceDelegate2>(provider => type =>
//{
//    switch (type)
//    {
//        case AccountType.Savings:
//            return provider.GetService<Savings>();
//        case AccountType.Current:
//            return provider.GetService<Current>();
//        default:
//            throw new NotImplementedException($"No service registered for account type {type}");
//    }
//});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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
