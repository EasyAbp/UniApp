using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyAbp.UniappManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.UniappManagement.Uniapps
{
    public class UniappRepository : EfCoreRepository<IUniappManagementDbContext, Uniapp, Guid>, IUniappRepository
    {
        public UniappRepository(IDbContextProvider<IUniappManagementDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public virtual async Task<Uniapp> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync()).Where(app => app.Name == name.Trim() && app.IsAvailable)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}