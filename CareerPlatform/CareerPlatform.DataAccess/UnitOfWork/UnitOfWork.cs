using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Interfaces;

namespace CareerPlatform.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PlatformDbContext _platformDbContext;
        public UnitOfWork(PlatformDbContext platformDbContext)
        {
            _platformDbContext = platformDbContext;
        }
        public async Task SaveChangesAsyn()
        {
            await _platformDbContext.SaveChangesAsync();
        }
    }
}
