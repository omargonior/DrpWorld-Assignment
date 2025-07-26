using dpWorld.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dpWorld.Shared.Common.Pagination;

namespace dpWorld.Infrastructure.Repos.Abstruct
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task AddAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
        Task<PagedResult<ApplicationUser>> GetPagedUsersAsync(UserQueryParameters parameters);

    }
}

