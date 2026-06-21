using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KozossegAPI.Model;

namespace KozossegAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MunkaController : ControllerBase
    {
        private readonly KozossegDbContext _context;
        public MunkaController(KozossegDbContext context) { _context = context; }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var munkak = await _context.Munkak
                .Include(m => m.Ceg)
                .ToListAsync();

            return Ok(munkak);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var munka = await _context.Munkak.Include(m => m.Ceg).FirstOrDefaultAsync(m => m.munka_id == id);
            return munka == null ? NotFound() : Ok(munka);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Munka munka)
        {
            if (munka.ceg_id <= 0)
            {
                return BadRequest(new { message = "Érvénytelen vagy hiányzó ceg_id! Lépj ki, és jelentkezz be újra a weblapon!" });
            }

            try
            {
                munka.Ceg = null;

                _context.Munkak.Add(munka);
                await _context.SaveChangesAsync();

                return Ok(munka);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Adatbázis hiba történt!", details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Munka updatedMunka)
        {
            if (id != updatedMunka.munka_id)
            {
                return BadRequest(new { message = "Az URL-ben lévő ID nem egyezik a küldött adat ID-jával." });
            }

            var letezoMunka = await _context.Munkak.FindAsync(id);
            if (letezoMunka == null)
            {
                return NotFound(new { message = "A szerkeszteni kívánt munka nem található." });
            }

            letezoMunka.munka_nev = updatedMunka.munka_nev;
            letezoMunka.cim = updatedMunka.cim;
            letezoMunka.idopont = updatedMunka.idopont;
            letezoMunka.letszam = updatedMunka.letszam;
            letezoMunka.oraszam = updatedMunka.oraszam;
            letezoMunka.kategoria = updatedMunka.kategoria;
            letezoMunka.leiras = updatedMunka.leiras;
            letezoMunka.statusz = updatedMunka.statusz;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(letezoMunka);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hiba a mentés során!", details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var munka = await _context.Munkak.FindAsync(id);
            if (munka == null) return NotFound();
            _context.Munkak.Remove(munka);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}