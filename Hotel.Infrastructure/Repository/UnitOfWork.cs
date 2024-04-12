using Hotel.Application.Common.Implementation;
using Hotel.Application.Common.InterFaces;
using Hotel.Infrastructure.Data;

namespace Hotel.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IVillaRepository VillaRepository { get; private set; }

        public IVillaNumberRepository VillaNumberRepository { get; private set; }

        public IAmenityRepository AmenityRepository { get; private set; }

        public IBookingRepository BookingRepository { get; private set; }

        public IAppUserRepository AppUserRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            VillaRepository = new VillaRepository(_context);
            VillaNumberRepository = new VillaNumberRepository(_context);
            AmenityRepository = new AmenityRepository(_context);
            BookingRepository = new BookingRepository(_context);
            AppUserRepository = new AppUserRepository(_context);
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
