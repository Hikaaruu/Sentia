using Microsoft.EntityFrameworkCore;
using Sentia.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SentiaDatabase")));

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSignalR()
    .AddAzureSignalR(builder.Configuration.GetConnectionString("AzureSignalR"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//.MapIdentityApi()

app.UseHttpsRedirection();

app.Run();

