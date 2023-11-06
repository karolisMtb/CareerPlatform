using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Interfaces;

namespace CareerPlatform.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _platformDbContext;
        public UnitOfWork(ApplicationDbContext platformDbContext)
        {
            _platformDbContext = platformDbContext;
        }
        public async Task SaveChangesAsync()
        {
            await _platformDbContext.SaveChangesAsync();
        }
    }
}
