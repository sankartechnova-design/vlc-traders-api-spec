using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VLCTraders.Api.Data;
using VLCTraders.Api.Models.Common;
using VLCTraders.Api.Models.Domain;

namespace VLCTraders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Note>>>> GetNotes()
        {
            var notes = await _context.Notes.OrderByDescending(n => n.NoteDate).ToListAsync();
            return Ok(new ApiResponse<List<Note>>
            {
                Success = true,
                Message = "Notes retrieved successfully",
                Data = notes
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Note>>> CreateNote([FromBody] Note request)
        {
            request.CreatedBy = User.Identity?.Name ?? "admin";
            _context.Notes.Add(request);
            await _context.SaveChangesAsync();

            return StatusCode(201, new ApiResponse<Note>
            {
                Success = true,
                Message = "Note created successfully",
                Data = request
            });
        }
    }
}
