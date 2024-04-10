using Ericsson.ReCapProject.Core.Contracts.Persistence.Repositories;

namespace Ericsson.ReCapProject.Core.Contracts.Persistence
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }

        bool Complete();
        Task<bool> CompleteAsync();
    }
}