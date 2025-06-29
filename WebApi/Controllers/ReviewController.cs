using Domain.Constants;
using Domain.DTOs.ReviewDTOs;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult<Response<GetReviewDTO>>> CreateAsync(CreateReviewDTO createReview)
    {
        var result = await reviewService.CreateAsync(User, createReview);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult<Response<GetReviewDTO>>> UpdateAsync(int reviewId, UpdateReviewDTO updateReview)
    {
        var result = await reviewService.UpdateAsync(reviewId, User, updateReview);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpDelete]
    [Authorize(Roles = $"{Roles.User}, {Roles.Admin}")]
    public async Task<ActionResult<Response<string>>> DeleteAsync(int reviewId)
    {
        var result = await reviewService.DeleteAsync(reviewId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("ById")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<GetReviewDTO>>> GetByIdAsync(int reviewId)
    {
        var result = await reviewService.GetByIdAsync(reviewId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("All")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<List<GetReviewDTO>>>> GetAllAsync([FromQuery] ReviewFilter filter)
    {
        var result = await reviewService.GetAllAsync(filter);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPut("Hide-Or-Show-Review")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<Response<string>>> HideOrShowAsync(HideOrShowDTO dto)
    {
        var result = await reviewService.HideOrShowAsync(dto);
        return StatusCode((int)result.StatusCode, result);
    }
}