using Domain.Constants;
using Domain.DTOs.ReviewDTOs;
using Domain.Filters;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpPost("Reviews-create")]
    [AllowAnonymous]
    public async Task<ActionResult> CreateReviewsAsync(CreateReviewsAdminDTO createReview)
        => Ok(await reviewService.CreateReviewsAsync(createReview));

    [HttpPost]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> CreateAsync(CreateReviewDTO createReview)
        => Ok(await reviewService.CreateAsync(User, createReview));

    [HttpPut]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> UpdateAsync(int reviewId, UpdateReviewDTO updateReview)
        => Ok(await reviewService.UpdateAsync(reviewId, User, updateReview));

    [HttpDelete]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> DeleteAsync(int reviewId)
        => Ok(await reviewService.DeleteAsync(reviewId));

    [HttpGet("ById")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> GetByIdAsync(int reviewId)
        => Ok(await reviewService.GetByIdAsync(reviewId));

    [HttpGet("ByUserId")]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult> GetByUserIdAsync(int userId)
        => Ok(await reviewService.GetByUserIdAsync(userId));

    [HttpGet("ByDoctorId")]
    [AllowAnonymous]
    public async Task<ActionResult> GetByDoctorIdAsync(int doctorId)
        => Ok(await reviewService.GetByDoctorIdAsync(doctorId));

    [HttpGet("All")]
    [AllowAnonymous]
    public async Task<ActionResult> GetAllAsync([FromQuery] ReviewFilter filter)
        => Ok(await reviewService.GetAllAsync(filter));

    [HttpPut("Hide-Or-Show-Review")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> HideOrShowAsync(HideOrShowDTO dto)
        => Ok(await reviewService.HideOrShowAsync(dto));
}