using Domain.DTOs.UserDTOs;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<Response<GetUserDTO>> CreateAsync(CreateUserDTO createUser); 
    Task<Response<GetUserDTO>> UpdateAsync(int id, UpdateUserDTO updateUser);
    Task<Response<string>> DeleteAsync(int userId);
    Task<Response<GetUserDTO>> GetByIdAsync(int userId);
    Task<PagedResponse<List<GetUserDTO>>> GetAllAsync(UserFilter filter);
    Task<Response<GetUserDTO>> RestoreAsync(RestoreUserDTO restoreUser);
}