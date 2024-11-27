using AppAPI.Models.Domain;
using AppAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CRUD for UserActionLog

        // GET: api/UserLogs
        [HttpGet("GetActionLogs")]
        public async Task<ActionResult<IEnumerable<UserActionLog>>> GetUserActionLogs()
        {
            return await _context.UserActionLogs
                .Include(log => log.Action)
                .Include(log => log.UserAudit)
                .ToListAsync();
        }

        // GET: api/UserLogs/ActionLogs/{id}
        [HttpGet("ActionLogsById")]
        public async Task<ActionResult<UserActionLog>> GetUserActionLog(Guid id)
        {
            var log = await _context.UserActionLogs
                .Include(log => log.Action)
                .Include(log => log.UserAudit)
                .FirstOrDefaultAsync(log => log.ActionLogId == id);

            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        // POST: api/UserLogs/ActionLogs
        [HttpPost("AddActionLogs")]
        public async Task<ActionResult<UserActionLog>> CreateUserActionLog(UserActionLog log)
        {
            _context.UserActionLogs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserActionLog), new { id = log.ActionLogId }, log);
        }

        // PUT: api/UserLogs/ActionLogs/{id}
        [HttpPut("UpdateActionLogs")]
        public async Task<IActionResult> UpdateUserActionLog(Guid id, UserActionLog log)
        {
            if (id != log.ActionLogId)
            {
                return BadRequest();
            }

            _context.Entry(log).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserActionLogExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/UserLogs/ActionLogs/{id}
        [HttpDelete("RemoveActionLogs")]
        public async Task<IActionResult> DeleteUserActionLog(Guid id)
        {
            var log = await _context.UserActionLogs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            _context.UserActionLogs.Remove(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // CRUD for UserAction

        [HttpGet("GetActions")]
        public async Task<ActionResult<IEnumerable<Models.Domain.Action>>> GetActions()
        {
            return await _context.UserActions.ToListAsync();
        }

        // Additional endpoints for UserAudit, if necessary...

        private bool UserActionLogExists(Guid id)
        {
            return _context.UserActionLogs.Any(log => log.ActionLogId == id);
        }
    }
}
