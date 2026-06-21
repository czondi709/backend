using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KozossegAPI.Model;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace KozossegAPI.Controllers
{
    public class StatuszUpdateModel
    {
        public string UjStatusz { get; set; }

        public int? JovahagyottOra { get; set; }

        public string? IgazolasAdatok { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class JelentkezesController : ControllerBase
    {
        private readonly KozossegDbContext _context;

        public JelentkezesController(KozossegDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jelentkezesek = await _context.Jelentkezesek
                .Include(j => j.Diak)
                .Include(j => j.Munka)
                .ToListAsync();

            return Ok(jelentkezesek);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Jelentkezes jel)
        {
            try
            {
                bool marJelentkezett = await _context.Jelentkezesek.AnyAsync(j => j.munka_id == jel.munka_id && j.diak_id == jel.diak_id);

                if (marJelentkezett)
                {
                    return BadRequest(new { message = "Már jelentkeztél erre a munkára!" });
                }

                _context.Jelentkezesek.Add(jel);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Sikeres jelentkezés!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Belső szerverhiba: " + ex.Message });
            }
        }

        [HttpGet("diak/{diakId}")]
        public async Task<IActionResult> GetByDiak(int diakId)
        {
            var list = await _context.Jelentkezesek.Where(j => j.diak_id == diakId).ToListAsync();
            return Ok(list);
        }

        [HttpGet("munka/{munkaId}")]
        public async Task<IActionResult> GetByMunka(int munkaId)
        {
            var list = await _context.Jelentkezesek.Where(j => j.munka_id == munkaId).ToListAsync();
            return Ok(list);
        }

        [HttpPut("statusz/{munkaId}/{diakId}")]
        public async Task<IActionResult> UpdateStatusz(int munkaId, int diakId, [FromBody] StatuszUpdateModel dto)
        {
            var jelentkezes = await _context.Jelentkezesek
                .FirstOrDefaultAsync(j => j.munka_id == munkaId && j.diak_id == diakId);

            if (jelentkezes == null)
                return NotFound(new { message = "Jelentkezés nem található!" });

            jelentkezes.munka_statusz = dto.UjStatusz;

            if (dto.UjStatusz.ToLower() == "teljesítve")
            {
                if (dto.JovahagyottOra.HasValue)
                    jelentkezes.jovahagyott_ora = dto.JovahagyottOra.Value;

                if (!string.IsNullOrEmpty(dto.IgazolasAdatok))
                    jelentkezes.igazolas_adatok = dto.IgazolasAdatok;
            }

            _context.Entry(jelentkezes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Státusz sikeresen frissítve!" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Hiba történt az adatbázis frissítésekor." });
            }
        }


        [HttpDelete("{munkaId}/{diakId}")]
        public async Task<IActionResult> DeleteJelentkezes(int munkaId, int diakId)
        {
            try
            {
                var jelentkezes = await _context.Jelentkezesek
                    .FirstOrDefaultAsync(j => j.munka_id == munkaId && j.diak_id == diakId);

                if (jelentkezes == null)
                {
                    return NotFound(new { message = "A jelentkezés nem található!" });
                }

                _context.Jelentkezesek.Remove(jelentkezes);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Jelentkezés sikeresen visszavonva!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hiba a törlés során: " + ex.Message });
            }
        }
    }
}