using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Amigo.Application.Services
{
    public class JWTTokenService(IConfiguration _configuration,
                      UserManager<ApplicationUser> _userManager) 
        : IJWTTokenService
    {
        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        public async Task<string> GenerateToken(ApplicationUser User)
        {
            // header 
            var secretKey = _configuration["JWTOptions:SecretKey"];

            var EncodedSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var Creds = new SigningCredentials(EncodedSecurityKey, SecurityAlgorithms.HmacSha256);


            //paylodad
            //
            var UserClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,User.Email),
                new Claim(ClaimTypes.Name , User.UserName),
                new Claim(ClaimTypes.NameIdentifier,User.Id)
            };
            var Roles = await _userManager.GetRolesAsync(User);

            foreach (var role in Roles)
            {
                UserClaims.Add(new Claim(ClaimTypes.Role, role));
            }



            //signeture

            var token = new JwtSecurityToken
            (
                issuer: _configuration["JWTOptions:Issuer"],
                audience: _configuration["JWTOptions:Audience"],
                expires: DateTime.Now.AddDays(1),
                claims: UserClaims,
                signingCredentials: Creds

            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
