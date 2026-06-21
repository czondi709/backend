using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using KozossegAPI.Model;
using Microsoft.Extensions.Configuration;

namespace KozossegAPI.Auth
{
    public class TokenManager
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly Dictionary<string, List<string>> _rolesPermissions = new();

        public List<string> Permissions
        {
            get
            {
                var permissions = new List<string>();
                foreach (var values in _rolesPermissions.Values)
                {
                    permissions.AddRange(values);
                }
                return permissions.Distinct().ToList();
            }
        }

        public TokenManager(IConfiguration configuration)
        {
            _secretKey = configuration["Auth:Jwt:Key"] ?? throw new ArgumentNullException("JWT Key hiányzik!");
            _issuer = configuration["Auth:Jwt:Issuer"] ?? "KozossegAPI";
            _audience = configuration["Auth:Jwt:Audience"] ?? "KozossegUsers";

            var rolesSection = configuration.GetSection("Auth:Roles");
            foreach (var role in rolesSection.GetChildren())
            {
                var permissions = role.GetChildren().Select(p => p.Value!).ToList();
                _rolesPermissions.Add(role.Key, permissions);
            }
        }

        public string GenerateToken(string email, string role, int id)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, email),
                new ("id", id.ToString()),
                new (ClaimTypes.Role, role)
            };

            if (_rolesPermissions.ContainsKey(role))
            {
                foreach (var permission in _rolesPermissions[role])
                {
                    claims.Add(new Claim("permission", permission));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}