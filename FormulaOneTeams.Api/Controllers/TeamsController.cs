using FormulaOneTeams.Api.Data;
using FormulaOneTeams.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormulaOneTeams.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme )]
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeamsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Teams.ToListAsync());
        }
        
        [HttpGet("get/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var team = await GetTeamByIdAsync(id);
            if (team is null) return BadRequest();
             
            return Ok(team);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), team.Id, team);
        }

        [HttpPatch("update-country")]
        public async Task<IActionResult> UpdateCountry(int id, string country)
        {
            var team = await GetTeamByIdAsync(id);
            if (team is null) return BadRequest();
            
            team.Country = country;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var team = await GetTeamByIdAsync(id);
            if (team is null) return BadRequest(); 

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<Team?> GetTeamByIdAsync(int id) => 
            await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
    } 
}