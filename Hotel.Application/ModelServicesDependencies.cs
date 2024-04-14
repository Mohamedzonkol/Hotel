using Hotel.Application.Services.Implementation;
using Hotel.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.Application
{
    public static class ModelServicesDependencies
    {
        public static IServiceCollection AddServicesDependencies(this IServiceCollection service)
        {
            service.AddScoped<IDashboardServices, DashboardServices>();
            return service;
        }
    }
}
