using dpWorld.Data.Context;
using dpWorld.Data.Models;
using dpWorld.Infrastructure.Repos.Abstruct;
using dpWorld.Shared.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dpWorld.Infrastructure.Repos.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ApplicationUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // ... existing code ...
        public async Task<PagedResult<ApplicationUser>> GetPagedUsersAsync(UserQueryParameters parameters)
        {
            var query = _context.Users.AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                query = query.Where(u =>
                    u.FirstName.Contains(parameters.SearchTerm) ||
                    u.LastName.Contains(parameters.SearchTerm) ||
                    u.Email.Contains(parameters.SearchTerm));
            }
            // If SearchTerm is empty or missing, return all users (paged)

            if (parameters.MinAge.HasValue)
                query = query.Where(u => u.Age >= parameters.MinAge.Value);

            if (parameters.MaxAge.HasValue)
                query = query.Where(u => u.Age <= parameters.MaxAge.Value);

            // Sorting
            query = parameters.SortBy.ToLower() switch
            {
                "firstname" => parameters.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                "email" => parameters.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "age" => parameters.SortDescending ? query.OrderByDescending(u => u.Age) : query.OrderBy(u => u.Age),
                _ => parameters.SortDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<ApplicationUser>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }
        // ... existing code ...


    }
}
