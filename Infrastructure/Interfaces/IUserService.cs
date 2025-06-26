using System.Security.Claims;
using Domain.DTOs.UserDTOs;
using Domain.Filters;
using Domain.Responses;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<Response<GetUserDTO>> CreateAsync(CreateUserDTO createUser);
    Task<Response<GetUserDTO>> UpdateAsync(int id, UpdateUserDTO updateUser);
    Task<Response<string>> DeleteAsync(int userId);
    Task<Response<string>> DeleteSelfAsync(ClaimsPrincipal userClaims);
    Task<Response<GetUserDTO>> GetByIdAsync(int userId);
    Task<Response<GetUserDTO>> GetCurrentUserAsync(ClaimsPrincipal userClaims);
    Task<Response<List<GetUserDTO>>> GetAllAsync(UserFilter filter);
    Task<Response<string>> RestoreAsync(RestoreUserDTO restoreUser);
    Task<Response<GetUserDTO>> ChangeUserRoleAsync(ChangeUserRoleDTO changeUserRole);
    Task<Response<string>> UploadOrUpdateProfileImageAsync(ClaimsPrincipal userClaims, IFormFile file);
    Task<Response<string>> DeleteProfileImageAsync(ClaimsPrincipal userClaims);
}