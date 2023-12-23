using CareerPlatform.DataAccess.DatabaseContext;
using CareerPlatform.DataAccess.Interfaces;

namespace CareerPlatform.DataAccess.UnitOfWork
{
    public class UnitOfWork(ApplicationDbContext _platformDbContext) : IUnitOfWork
    {
        //private readonly ApplicationDbContext _platformDbContext;
        //public UnitOfWork(ApplicationDbContext platformDbContext)
        //{
        //    _platformDbContext = platformDbContext;
        //}
        public async Task SaveChangesAsync()
        {
            await _platformDbContext.SaveChangesAsync();
        }
    }
}
