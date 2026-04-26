using Microsoft.Extensions.DependencyInjection;
using Sentia.Application.Common.Interfaces;
using Sentia.Infrastructure.RealTime.Services;
using Microsoft.Extensions.Configuration;

namespace Sentia.Infrastructure.RealTime;

public static class RealTimeServiceRegistration
{
    public static IServiceCollection AddRealTime(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PresenceTracker>();
        services.AddScoped<ISignalRService, SignalRService>();
        services.AddSignalR()
            .AddAzureSignalR(configuration.GetConnectionString("AzureSignalR"));

        return services;
    }
}
