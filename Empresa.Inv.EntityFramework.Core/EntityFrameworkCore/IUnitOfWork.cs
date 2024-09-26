namespace Empresa.Inv.EntityFramework.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();

        Task RollbacktransactionAsync();

        Task<int> SaveAsync();
    }
}
