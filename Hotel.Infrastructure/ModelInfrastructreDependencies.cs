using Hotel.Application.Common.InterFaces;
using Hotel.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.Infrastructure
{
    public static class ModelInfrastructreDependencies
    {
        public static IServiceCollection AddInfrastructreDependencies(this IServiceCollection service)
        {
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            return service;
        }
    }
}
