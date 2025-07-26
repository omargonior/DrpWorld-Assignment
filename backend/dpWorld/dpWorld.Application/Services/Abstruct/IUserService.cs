using dpWorld.Application.DTOs.UserDTO;
using dpWorld.Shared.Common.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dpWorld.Application.Services.Abstruct
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(string id);
        Task<bool> CreateAsync(CreateUserDto dto, string role);
        Task<bool> UpdateAsync(string id, UpdateUserDto dto);
        Task<bool> DeleteAsync(string id);
        Task<PagedResult<UserDto>> GetPagedUsersAsync(UserQueryParameters parameters);
        Task<(bool Succeeded, string? Errors)> CreateWithErrorsAsync(CreateUserDto dto, string role);
        Task UpdateSignatureAsync(string userId, string base64Signature);
    }
}
