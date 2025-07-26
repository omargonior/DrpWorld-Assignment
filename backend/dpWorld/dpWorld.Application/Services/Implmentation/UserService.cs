using AutoMapper;
using dpWorld.Application.DTOs.UserDTO;
using dpWorld.Application.Services.Abstruct;
using dpWorld.Data.Models;
using dpWorld.Infrastructure.Repos.Abstruct;
using dpWorld.Shared.Common.Pagination;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dpWorld.Application.Services.Implmentation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto?> GetByIdAsync(string id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<bool> CreateAsync(CreateUserDto dto, string role)
        {
            var user = _mapper.Map<ApplicationUser>(dto);
            user.UserName = dto.UserName; // Use the username from the DTO
            user.Email = dto.Email;

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return false;

            await _userManager.AddToRoleAsync(user, role);
            return true;
        }

        public async Task<bool> UpdateAsync(string id, UpdateUserDto dto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return false;

            _mapper.Map(dto, user);
            await _userRepo.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepo.DeleteAsync(user);
            return true;
        }

        public async Task<PagedResult<UserDto>> GetPagedUsersAsync(UserQueryParameters parameters)
        {
            var pagedUsers = await _userRepo.GetPagedUsersAsync(parameters);

            return new PagedResult<UserDto>
            {
                Items = _mapper.Map<List<UserDto>>(pagedUsers.Items),
                TotalCount = pagedUsers.TotalCount,
                PageNumber = pagedUsers.PageNumber,
                PageSize = pagedUsers.PageSize
            };
        }

        public async Task<(bool Succeeded, string? Errors)> CreateWithErrorsAsync(CreateUserDto dto, string role)
        {
            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName, // <-- Set from DTO
                PhoneNumber = dto.PhoneNumber,
                NationalId = dto.NationalId,
                Age = dto.Age,
                SignatureUrl = dto.SignatureUrl
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            await _userManager.AddToRoleAsync(user, role);
            return (true, null);
        }

        public async Task UpdateSignatureAsync(string userId, string base64Signature)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return;
            user.SignatureUrl = base64Signature;
            await _userRepo.UpdateAsync(user);
        }
    }
}