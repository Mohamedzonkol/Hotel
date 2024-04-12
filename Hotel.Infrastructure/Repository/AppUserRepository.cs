
using Hotel.Application.Common.InterFaces;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;

namespace Hotel.Infrastructure.Repository
{
    public class AppUserRepository(ApplicationDbContext context)
        : Repository<ApplicationUser>(context), IAppUserRepository
    {

    }

}
