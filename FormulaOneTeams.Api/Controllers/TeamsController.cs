using FormulaOneTeams.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormulaOneTeams.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private static List<Team> _teams = new();
        
        [HttpGet("get-all")]
        public IActionResult Get()
        {
            return Ok(_teams);
        }
        
        [HttpGet("get/{id:int}")]
        public IActionResult GetById(int id)
        {
            var team = _teams.FirstOrDefault(t => t.Id == id);
            if (team is null) return BadRequest();
             
            return Ok(team);
        }

        [HttpPost("add")]
        public IActionResult Add(Team team)
        {
            _teams.Add(team);
            return CreatedAtAction(nameof(Get), team.Id, team);
        }

        [HttpPatch("update-country")]
        public IActionResult UpdateCountry(int id, string country)
        {
            var team = _teams.FirstOrDefault(t => t.Id == id);
            if (team is null) return BadRequest();
            
            team.Country = country;
            return NoContent();
        }

        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            var team = _teams.FirstOrDefault(t => t.Id == id);
            if (team is null) return BadRequest();

            _teams.Remove(team);
            return NoContent();
        }
    } 
}