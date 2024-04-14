using Hotel.Application.Common.InterFaces;
using Hotel.Application.Common.Intilizer;
using Hotel.Infrastructure.Data;
using Hotel.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.Infrastructure
{
    public static class ModelInfrastructreDependencies
    {
        public static IServiceCollection AddInfrastructreDependencies(this IServiceCollection service)
        {
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            service.AddScoped<IDbInitializer, DbInitializer>();
            return service;
        }
    }
}
