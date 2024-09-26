using Empresa.Inv.Application.Shared.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Empresa.Inv.Web.Host.Services
{
    public class LoginServices
    {
        public UserDTO AuthenticateUser(LoginModel login)
        {
            // Aquí deberías autenticar al usuario con tu lógica de negocio
            // Este es solo un ejemplo de usuario
            return new UserDTO
            {
                Id = 2,
                UserName = "exampleUser",
                Roles = "admin"
            };


        }

        //public string GetToken(User user, string clientType, JwtSettings jwtSettings)
        //{
        //    var claims = new[]
        //   {
        //    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        //    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        //    new Claim("client_type", clientType), // Claim específico para el tipo de cliente
        //    new Claim(ClaimTypes.Role, user.Roles)
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(claims),
        //        Expires = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiresInMinutes),
        //        SigningCredentials = creds,
        //        Issuer = jwtSettings.Issuer,
        //        Audience = jwtSettings.Audience
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    var tokenString = tokenHandler.WriteToken(token);

        //    return tokenString;

        //}

    }
}
