using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KozossegAPI.Model;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace KozossegAPI.Controllers
{

    public class JelszoModositasModel
    {
        public string RegiJelszo { get; set; }
        public string UjJelszo { get; set; }
    }

    public class OraUpdateDto
    {
        public int HozzaadottOrak { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class DiakController : ControllerBase
    {
        private readonly KozossegDbContext _context;
        public DiakController(KozossegDbContext context) { _context = context; }

        [HttpPut("orak/{id}")]
        public async Task<IActionResult> AddOrak(int id, [FromBody] OraUpdateDto dto)
        {
            var diak = await _context.Diakok.FindAsync(id);
            if (diak == null)
            {
                return NotFound(new { message = "Diák nem található!" });
            }

            diak.ledolgozott_ora += dto.HozzaadottOrak;

            _context.Entry(diak).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Órák sikeresen jóváírva!", ujOraszam = diak.ledolgozott_ora });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Szerverhiba az órák mentésekor!" });
            }
        }


        [HttpPut("jelszo/{id}")]
        public async Task<IActionResult> UpdateJelszo(int id, [FromBody] JelszoModositasModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.RegiJelszo) || string.IsNullOrEmpty(model.UjJelszo))
            {
                return BadRequest(new { message = "Hiányzó adatok!" });
            }

            var diak = await _context.Diakok.FindAsync(id);
            if (diak == null) return NotFound(new { message = "Diák nem található!" });

            bool helyesJelszo = BCrypt.Net.BCrypt.Verify(model.RegiJelszo.Trim(), diak.jelszo);

            if (!helyesJelszo)
            {
                return BadRequest(new { message = "A megadott jelenlegi jelszó helytelen!" });
            }

            diak.jelszo = BCrypt.Net.BCrypt.HashPassword(model.UjJelszo.Trim());

            await _context.SaveChangesAsync();
            return Ok(new { message = "Jelszó sikeresen megváltoztatva!" });
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _context.Diakok.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var diak = await _context.Diakok.FindAsync(id);
            return diak == null ? NotFound() : Ok(diak);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Diak diak)
        {
            var hibak = new List<string>();

            if (_context.Diakok.Any(d => d.email == diak.email))
            {
                hibak.Add("Ez az e-mail cím már foglalt!");
            }

            if (!string.IsNullOrEmpty(diak.telefonszam))
            {
                if (_context.Diakok.Any(d => d.telefonszam == diak.telefonszam))
                {
                    hibak.Add("Ez a telefonszám már regisztrálva van!");
                }
            }

            if (hibak.Any())
            {
                return BadRequest(new { messages = hibak });
            }

            try
            {
                diak.jelszo = BCrypt.Net.BCrypt.HashPassword(diak.jelszo);

                _context.Diakok.Add(diak);
                await _context.SaveChangesAsync();

                diak.jelszo = null;

                return Ok(diak);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { messages = new List<string> { "Szerver hiba történt a mentés során!" } });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiak(int id, Diak frissitettDiak)
        {
            if (id != frissitettDiak.diak_id)
            {
                return BadRequest(new { message = "Hibás azonosító!" });
            }

            var jelenlegiDiak = await _context.Diakok.FindAsync(id);

            if (jelenlegiDiak == null)
            {
                return NotFound(new { message = "Diák nem található!" });
            }

            jelenlegiDiak.nev = frissitettDiak.nev;
            jelenlegiDiak.email = frissitettDiak.email;
            jelenlegiDiak.telefonszam = frissitettDiak.telefonszam;
            jelenlegiDiak.iskola = frissitettDiak.iskola;
            jelenlegiDiak.om_azonosito = frissitettDiak.om_azonosito;
            jelenlegiDiak.iskola_om_kod = frissitettDiak.iskola_om_kod;
            jelenlegiDiak.osztaly = frissitettDiak.osztaly;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Profiladatok sikeresen frissítve!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hiba az adatbázis mentésekor: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiak(int id)
        {
            try
            {
                var diak = await _context.Diakok.FindAsync(id);

                if (diak == null)
                {
                    return NotFound(new { message = "A diák nem található!" });
                }

                _context.Diakok.Remove(diak);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Diák fiók sikeresen törölve!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hiba a törlés során: " + ex.Message });
            }
        }
    }
}