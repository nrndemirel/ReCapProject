using Ericsson.ReCapProject.Core.Attributes;
using Ericsson.ReCapProject.Core.Contracts.Persistence;
using Ericsson.ReCapProject.Core.Contracts.Persistence.Repositories;
using Ericsson.ReCapProject.Persistence.DbContexts;
using Ericsson.ReCapProject.Persistence.Repositories;

namespace Ericsson.ReCapProject.Persistence
{
    [InjectableScoped]
    public class UnitOfWork(ReCapProjectDbContext dbContext) : IUnitOfWork
    {
        private readonly ReCapProjectDbContext _ReCapProjectDbContext = dbContext;

        public IProductRepository Products { get; private set; } = new ProductRepository(dbContext);

        public bool Complete()
        {
            return _ReCapProjectDbContext.SaveChanges() > 0;
        }
        public async Task<bool> CompleteAsync()
        {
            return await _ReCapProjectDbContext.SaveChangesAsync() > 0;
        }

    }
}
