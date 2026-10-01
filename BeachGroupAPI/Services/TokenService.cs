using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BeachGroupAPI.Model;
using Microsoft.IdentityModel.Tokens;

namespace BeachGroupAPI.Services
{
    //Geração de autenticação JWT (JSON Web Token) 
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GerarToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            // Informações (Claims) gravadas dentro do Token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.OidUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.NomUsuario),
                new Claim(ClaimTypes.Email, usuario.NomEmail)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"] ?? "8")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}