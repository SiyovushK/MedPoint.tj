using System.Security.Claims;
using Domain.DTOs.ReviewDTOs;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IReviewService
{
    Task<Response<GetReviewDTO>> CreateAsync(ClaimsPrincipal userClaims, CreateReviewDTO createReview);
    Task<Response<GetReviewDTO>> UpdateAsync(int reviewId, ClaimsPrincipal userClaims, UpdateReviewDTO updateReview);
    Task<Response<string>> DeleteAsync(int reviewId);

    Task<Response<GetReviewDTO>> GetByIdAsync(int reviewId);
    Task<Response<List<GetReviewDTO>>> GetAllAsync(ReviewFilter filter);

    Task<Response<string>> HideOrShowAsync(HideOrShowDTO dto);
}