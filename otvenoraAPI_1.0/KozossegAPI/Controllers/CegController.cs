using KozossegAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KozossegAPI.Controllers
{
    public class CegJelszoModositasDto
    {
        public string RegiJelszo { get; set; }
        public string UjJelszo { get; set; }


    }

    [Route("api/[controller]")]
    [ApiController]
    public class CegController : ControllerBase
    {
        private readonly KozossegDbContext _context;
        public CegController(KozossegDbContext context) { _context = context; }

        [HttpPut("jelszo/{id}")]
        public async Task<IActionResult> ModositCegJelszo(int id, [FromBody] CegJelszoModositasDto dto)
        {
            var ceg = await _context.Cegek.FindAsync(id);

            if (ceg == null)
            {
                return NotFound(new { message = "A cég nem található az adatbázisban!" });
            }

            bool isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.RegiJelszo, ceg.jelszo);


            if (!isOldPasswordCorrect)
            {
                return BadRequest(new { message = "A megadott jelenlegi jelszó helytelen!" });
            }

            ceg.jelszo = BCrypt.Net.BCrypt.HashPassword(dto.UjJelszo);

            _context.Entry(ceg).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "A jelszó sikeresen frissítve!" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Szerverhiba történt a mentés során!" });
            }
        }


        [Authorize(Policy = "diak.read")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get() => Ok(await _context.Cegek.ToListAsync());

        [HttpGet("{id}")]
        
        public async Task<IActionResult> GetWithDetails(int id)
        {
            var ceg = await _context.Cegek
                .Include(c => c.Dokumentumok)
                .FirstOrDefaultAsync(c => c.ceg_id == id);

            return ceg == null ? NotFound() : Ok(ceg);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCeg(Ceg ceg)
        {
            if (_context.Cegek.Any(c => c.ceg_email == ceg.ceg_email || c.adoszam == ceg.adoszam))
            {
                return BadRequest("Ez a cég már regisztrálva van!");
            }
            if (_context.Cegek.Any(c => c.telefonszam == ceg.telefonszam))
            {
                return BadRequest("Ez a telefonszám már regisztrálva van!");
            }

            ceg.jelszo = BCrypt.Net.BCrypt.HashPassword(ceg.jelszo);

            _context.Cegek.Add(ceg);
            await _context.SaveChangesAsync();

            return Ok("Cég sikeresen regisztrálva, a jelszó titkosítva!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Ceg updatedCeg)
        {
            if (id != updatedCeg.ceg_id)
            {
                return BadRequest(new { message = "Az URL-ben lévő ID nem egyezik a küldött adat ID-jával." });
            }

            var letezoCeg = await _context.Cegek.FindAsync(id);
            if (letezoCeg == null)
            {
                return NotFound(new { message = "A szerkeszteni kívánt cég nem található." });
            }

            letezoCeg.cegnev = updatedCeg.cegnev;
            letezoCeg.ceg_email = updatedCeg.ceg_email;
            letezoCeg.cim = updatedCeg.cim;
            letezoCeg.adoszam = updatedCeg.adoszam;
            letezoCeg.telefonszam = updatedCeg.telefonszam;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(letezoCeg);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hiba a mentés során!", details = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize] 
        public async Task<IActionResult> DeleteCeg(int id)
        {
            try
            {
                var ceg = await _context.Cegek.FindAsync(id);

                if (ceg == null)
                {
                    return NotFound(new { message = "A cég nem található!" });
                }

                _context.Cegek.Remove(ceg);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Céges fiók sikeresen törölve!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hiba a törlés során: " + ex.Message });
            }
        }
    }
}