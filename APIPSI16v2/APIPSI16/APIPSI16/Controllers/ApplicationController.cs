using APIPSI16.Data;
using APIPSI16.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIPSI16.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require JWT for all actions by default
    public class ApplicationController : ControllerBase
    {
        private readonly xcleratesystemslinks_SampleDBContext _context;

        public ApplicationController(xcleratesystemslinks_SampleDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Application
        // Admins see all applications; users see only their own
        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            var currentUserId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            IQueryable<Application> query = _context.Applications
                .Include(a => a.Opportunity)
                .Include(a => a.User);

            // Admins (role 0) see all; regular users see only their own
            if (userRole != "0")
            {
                if (!currentUserId.HasValue) return Unauthorized();
                query = query.Where(a => a.UserId == currentUserId.Value);
            }

            var applications = await query.ToListAsync();
            return Ok(applications);
        }

        // GET: api/Application/5
        // Users can see their own application; admins can see any
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplication(int id)
        {
            var application = await _context.Applications
                .Include(a => a.Opportunity)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (application == null) return NotFound();

            var currentUserId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            // Enforce ownership: users can only see their own applications (admins can see all)
            if (userRole != "0" && application.UserId != currentUserId)
                return Forbid();

            return Ok(application);
        }

        // POST: api/Application
        // Users can create applications for themselves; admins can create for anyone
        [HttpPost]
        [Authorize(Roles = "0,1")] // Admin or User
        public async Task<IActionResult> CreateApplication([FromBody] Application application)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentUserId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            // Regular users can only create applications for themselves
            if (userRole == "1")
            {
                if (!currentUserId.HasValue) return Unauthorized();
                application.UserId = currentUserId.Value; // Force to current user
            }

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }

        // PUT: api/Application/5
        // Only admins can update applications
        [HttpPut("{id}")]
        [Authorize(Roles = "0")] // Admin only
        public async Task<IActionResult> UpdateApplication(int id, [FromBody] Application application)
        {
            if (id != application.Id) return BadRequest();

            _context.Entry(application).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Application/5
        // Only admins can delete applications
        [HttpDelete("{id}")]
        [Authorize(Roles = "0")] // Admin only
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null) return NotFound();

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.Id == id);
        }

        // Helper: get current user ID from JWT claims
        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }

        // Helper: get current user role from JWT claims
        private string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}