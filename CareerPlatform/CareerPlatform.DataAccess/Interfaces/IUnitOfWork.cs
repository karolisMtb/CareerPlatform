namespace CareerPlatform.DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsyn();
    }
}
