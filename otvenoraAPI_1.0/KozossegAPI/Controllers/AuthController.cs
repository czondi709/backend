using BCrypt.Net;
using KozossegAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KozossegAPI.Auth;

namespace KozossegAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly KozossegDbContext _context;
        private readonly TokenManager _tokenManager;

        public AuthController(KozossegDbContext context, TokenManager tokenManager)
        {
            _context = context;
            _tokenManager = tokenManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginData)
        {
            if (loginData == null || string.IsNullOrEmpty(loginData.Email))
            {
                return Unauthorized(new { message = "Hibás e-mail cím vagy jelszó!" });
            }

            var diak = await _context.Diakok
                .FirstOrDefaultAsync(d => d.email.ToLower() == loginData.Email.ToLower());

            if (diak != null && BCrypt.Net.BCrypt.Verify(loginData.Jelszo, diak.jelszo))
            {
                var token = _tokenManager.GenerateToken(diak.email, "Diak", diak.diak_id);
                return Ok(new
                {
                    token = token,
                    message = "Sikeres bejelentkezés!",
                    userType = "diak",
                    diakId = diak.diak_id

                });
            }

            var ceg = await _context.Cegek
                .FirstOrDefaultAsync(c => c.ceg_email.ToLower() == loginData.Email.ToLower());

            if (ceg != null && BCrypt.Net.BCrypt.Verify(loginData.Jelszo, ceg.jelszo))
            {
                var token = _tokenManager.GenerateToken(ceg.ceg_email, "Ceg", ceg.ceg_id);
                return Ok(new
                {
                    token = token,
                    userType = "ceg",
                    nev = ceg.cegnev,
                    ceg_id = ceg.ceg_id
                });
            }

            return Unauthorized(new { message = "Hibás e-mail cím vagy jelszó!" });
        }
    }

    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Jelszo { get; set; } = string.Empty;
    }
}