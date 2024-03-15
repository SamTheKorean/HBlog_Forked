﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KevBlog.Domain.Entities;
using KevBlog.Domain.Repositories;
using KevBlog.Infrastructure.Extensions;
using KevBlog.Contract.DTOs;
using KevBlog.Application.Services;


namespace KevBlog.Api.Controllers
{
    [Authorize]
    [Route("api")]
    public class PostsController : BaseApiController
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostService _postService;

        public PostsController(IPostService postService, IPostRepository postRepository)
        {
            _postService = postService;
            _postRepository = postRepository;
        }

        [AllowAnonymous]
        [HttpGet("posts")]
        public async Task<ActionResult<IEnumerable<PostDisplayDto>>> GetPosts()
        {
            return Ok(new ApiResponse<IEnumerable<PostDisplayDto>>(await _postService.GetPosts()));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("categories/{categoryId}/posts")]
        public async Task<ActionResult<IEnumerable<PostDisplayDto>>> GetPostsByCategory(int categoryId) => Ok(await _postService.GetPostsByCategory(categoryId));

        [AllowAnonymous]
        [HttpGet("users/{username}/posts")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsByUsername(string username) => Ok(await _postRepository.GetPostsByUserName(username));

        [AllowAnonymous]
        [HttpGet("tags/{tagName}/posts")]
        public async Task<ActionResult<IEnumerable<PostDisplayDto>>> GetPostsByTagName(string tagName) => Ok(await _postService.GetPostsByTagName(tagName));

        [AllowAnonymous]
        [HttpGet("posts/{id}")]
        public async Task<ActionResult<PostDisplayDetailsDto>> GetPostById(int id)
        {
            var postDetails = await _postService.GetByIdAsync(id);
            return (await _postService.GetByIdAsync(id)).IsSuccess ? 
                    (ActionResult<PostDisplayDetailsDto>)Ok(postDetails.Value) : 
                    (ActionResult<PostDisplayDetailsDto>)NotFound(postDetails.Message);
        }

        [HttpPut("posts")]
        public async Task<IActionResult> Put(PostUpdateDto postUpdateDto)
        {
            if(postUpdateDto is null)
                throw new ArgumentNullException(nameof(postUpdateDto));

            if (postUpdateDto.Id == 0)
                return BadRequest("Id field cannot be empty or 0");

            var result = await _postService.UpdatePost(postUpdateDto);
            if (!result.IsSuccess && result.Message == "Post does not exist.")
                RedirectToRoute("Posts");

            return NoContent();
        }

        [HttpPut("posts/{postId}/AddTag")] 
        public async Task<IActionResult> AddTag(int postId, [FromBody]int tagId)
        {
            if (postId == 0 || tagId == 0)
                return BadRequest("Post Id or Tag Id cannot be null");
            
            var result = await _postService.AddTagForPost(postId, tagId);
            if(!result.IsSuccess)
                return BadRequest("Failed to add tags.");
            
            return Ok();
        }

        [HttpPut("posts/{id}/status")]
        public async Task<IActionResult> SetPostStatus(int id, string status)
        {
            Post post = await _postRepository.GetById(id);
            if (post is null)
                return NotFound();

            post.Status = status;
            await _postRepository.UpdateAsync(post);
            return Ok();
        }

        [HttpPost("posts")]
        public async Task<IActionResult> Create(PostCreateDto postCreateDto)
        {
            if (postCreateDto is null)
                throw new ArgumentNullException(nameof(postCreateDto));

            var result = await _postService.CreatePost(User.GetUsername(), postCreateDto);
            if(!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.IsSuccess);
        }

        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _postService.DeletePost(id);
            if(!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok();
        }

    }
}
