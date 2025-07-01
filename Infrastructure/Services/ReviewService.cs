using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using Domain.DTOs.ReviewDTOs;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ReviewService(
        IMapper mapper,
        DoctorRepository doctorRepository,
        UserRepository userRepository,
        ReviewRepository reviewRepository) : IReviewService
{
    public async Task<Response<GetReviewDTO>> CreateAsync(ClaimsPrincipal userClaims, CreateReviewDTO createReview)
    {
        var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return new Response<GetReviewDTO>(HttpStatusCode.Unauthorized, "User ID not found in token.");

        // User
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return new Response<GetReviewDTO>(HttpStatusCode.NotFound, $"User with ID {userId} not found.");

        // Doctor
        Doctor? doctor = null;
        if (createReview.DoctorId.HasValue)
        {
            doctor = await doctorRepository.GetByIdAsync(createReview.DoctorId.Value);
            if (doctor == null || doctor.IsDeleted)
                return new Response<GetReviewDTO>(HttpStatusCode.NotFound, $"Doctor with ID {createReview.DoctorId} not found.");
        }

        // Rating
        if (createReview.Rating < 1 || createReview.Rating > 5)
            return new Response<GetReviewDTO>(HttpStatusCode.BadRequest, "Rating must be between 1 and 5.");

        // Comment
        if (createReview.Comment.Length < 1 || createReview.Comment.Length > 500)
            return new Response<GetReviewDTO>(HttpStatusCode.BadRequest, "Comment must contain between 1 and 500 characters.");

        var review = mapper.Map<Review>(createReview);
        review.UserId = userId;

        if (await reviewRepository.AddAsync(review) == 0)
            return new Response<GetReviewDTO>(HttpStatusCode.InternalServerError, "Error when saving a review.");

        var getReview = mapper.Map<GetReviewDTO>(review);
        getReview.UserName = $"{user.FirstName} {user.LastName}";
        if (doctor != null)
            getReview.DoctorName = $"{doctor.FirstName} {doctor.LastName}";

        return new Response<GetReviewDTO>(getReview);
    }

    public async Task<Response<GetReviewDTO>> UpdateAsync(int reviewId, ClaimsPrincipal userClaims, UpdateReviewDTO updateReview)
    {
        var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return new Response<GetReviewDTO>(HttpStatusCode.Unauthorized, "User ID not found in token.");

        // Review
        var existingReview = await reviewRepository.GetByIdAsync(reviewId);
        if (existingReview == null || existingReview.IsHidden)
            return new Response<GetReviewDTO>(HttpStatusCode.NotFound, $"Review with ID {reviewId} not found.");

        // User
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return new Response<GetReviewDTO>(HttpStatusCode.NotFound, $"User with ID {userId} not found.");

        // Doctor
        Doctor? doctor = null;
        if (updateReview.DoctorId.HasValue)
        {
            doctor = await doctorRepository.GetByIdAsync(updateReview.DoctorId.Value);
            if (doctor == null || doctor.IsDeleted)
                return new Response<GetReviewDTO>(HttpStatusCode.NotFound, $"Doctor with ID {updateReview.DoctorId} not found.");
        }

        // Rating
        if (updateReview.Rating < 1 || updateReview.Rating > 5)
            return new Response<GetReviewDTO>(HttpStatusCode.BadRequest, "Rating must be between 1 and 5.");

        // Comment
        if (updateReview.Comment.Length < 1 || updateReview.Comment.Length > 500)
            return new Response<GetReviewDTO>(HttpStatusCode.BadRequest, "Comment must contain between 1 and 500 characters.");

        mapper.Map(updateReview, existingReview);
        existingReview.UserId = userId;
        existingReview.UpdatedAt = DateTime.UtcNow;

        if (await reviewRepository.UpdateAsync(existingReview) == 0)
            return new Response<GetReviewDTO>(HttpStatusCode.InternalServerError, "Failed to update review.");

        var getReview = mapper.Map<GetReviewDTO>(existingReview);
        getReview.UserName = $"{user.FirstName} {user.LastName}";
        if (doctor != null)
            getReview.DoctorName = $"{doctor.FirstName} {doctor.LastName}";

        return new Response<GetReviewDTO>(getReview);
    }

    public async Task<Response<string>> DeleteAsync(int reviewId)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            return new Response<string>(HttpStatusCode.NotFound, $"Review with ID {reviewId} not found.");

        if (await reviewRepository.DeleteAsync(review) == 0)
            return new Response<string>(HttpStatusCode.InternalServerError, "Failed to delete review.");

        return new Response<string>("Review deleted successfully.");
    }

    public async Task<Response<List<GetReviewDTO>>> GetAllAsync(ReviewFilter filter)
    {
        if (filter.RatingFrom.HasValue && filter.RatingTo.HasValue && filter.RatingFrom > filter.RatingTo)
            return new Response<List<GetReviewDTO>>(HttpStatusCode.BadRequest, "RatingFrom cannot be greater than RatingTo.");
        if (filter.CreatedFrom.HasValue && filter.CreatedTo.HasValue && filter.CreatedFrom > filter.CreatedTo)
            return new Response<List<GetReviewDTO>>(HttpStatusCode.BadRequest, "CreatedFrom cannot be greater than CreatedTo.");

        var query = reviewRepository.GetAll();

        if (filter.DoctorId.HasValue)
            query = query.Where(r => r.DoctorId == filter.DoctorId);

        if (!string.IsNullOrWhiteSpace(filter.DoctorName))
        {
            var search = filter.DoctorName.ToLower();
            query = query.Where(r =>
                r.Doctor.FirstName.ToLower().Contains(search) ||
                r.Doctor.LastName.ToLower().Contains(search));
        }

        if (filter.UserId.HasValue)
            query = query.Where(r => r.UserId == filter.UserId);

        if (!string.IsNullOrWhiteSpace(filter.UserName))
        {
            var search = filter.UserName.ToLower();
            query = query.Where(r =>
                r.User.FirstName.ToLower().Contains(search) ||
                r.User.LastName.ToLower().Contains(search));
        }

        if (filter.RatingFrom.HasValue)
            query = query.Where(r => r.Rating >= filter.RatingFrom);

        if (filter.RatingTo.HasValue)
            query = query.Where(r => r.Rating <= filter.RatingTo);

        if (filter.CreatedFrom.HasValue)
            query = query.Where(r => r.CreatedAt >= filter.CreatedFrom);

        if (filter.CreatedTo.HasValue)
            query = query.Where(r => r.CreatedAt <= filter.CreatedTo);

        if (filter.IsHidden.HasValue)
            query = query.Where(r => r.IsHidden == filter.IsHidden.Value);

        var reviews = await query.ToListAsync();

        var getReviewsDto = mapper.Map<List<GetReviewDTO>>(reviews);

        return new Response<List<GetReviewDTO>>(getReviewsDto);
    }

    public async Task<Response<GetReviewDTO>> GetByIdAsync(int reviewId)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            return new Response<GetReviewDTO>(HttpStatusCode.NotFound, $"Review with ID {reviewId} not found.");

        var getReview = mapper.Map<GetReviewDTO>(review);

        return new Response<GetReviewDTO>(getReview);
    }

    public async Task<Response<List<GetReviewDTO>>> GetByUserIdAsync(int userId)
    {
        var reviews = await reviewRepository.GetByUserIdAsync(userId);
        if (reviews == null)
            return new Response<List<GetReviewDTO>>(HttpStatusCode.NotFound, $"No reviews found for user id {userId}.");

        var getReview = mapper.Map<List<GetReviewDTO>>(reviews);

        return new Response<List<GetReviewDTO>>(getReview);
    }

    public async Task<Response<List<GetReviewDTO>>> GetByDoctorIdAsync(int doctorId)
    {
        var reviews = await reviewRepository.GetByDoctorIdAsync(doctorId);
        if (reviews == null)
            return new Response<List<GetReviewDTO>>(HttpStatusCode.NotFound, $"No reviews found for doctor id {doctorId}.");

        var getReview = mapper.Map<List<GetReviewDTO>>(reviews);

        return new Response<List<GetReviewDTO>>(getReview);
    }

    public async Task<Response<string>> HideOrShowAsync(HideOrShowDTO dto)
    {
        var review = await reviewRepository.GetByIdAsync(dto.ReviewId);
        if (review == null)
            return new Response<string>(HttpStatusCode.NotFound, $"Review with ID {dto.ReviewId} not found.");

        review.IsHidden = dto.IsHidden;

        if (await reviewRepository.UpdateAsync(review) == 0)
            return new Response<string>(HttpStatusCode.InternalServerError, $"Failed to {(dto.IsHidden ? "hide" : "show")} the review.");

        return new Response<string>($"Review is now {(review.IsHidden ? "hidden" : "unhidden")}");
    }
}