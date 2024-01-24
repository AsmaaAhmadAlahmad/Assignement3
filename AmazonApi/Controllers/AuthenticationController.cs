using Assignement3_Domain.Handler;
using Assignement3_Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Assignement3_Domain.Controllers
{
    // تمت الاستفادة من الرابط التالي
    // https://www.c-sharpcorner.com/article/jwt-token-creation-authentication-and-authorization-in-asp-net-core-6-0-with-po/
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public ActionResult Authenticate(AuthRequest authRequest)
        {
            var user = validUserCredenctails(authRequest);

            if (user != null)
            {
                var token = GenerateToken();
                return Ok(token);
            }

             return NotFound("user not found");
        }
        private ActionResult GenerateToken()
        {
            var Cliams = new List<Claim>();
            Cliams.Add(new Claim(ClaimTypes.Name, "Asmaa"));
            Cliams.Add(new Claim(ClaimTypes.Role, "User"));


            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Athentication:secretKey"]));

            var SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                configuration["Athentication:Issuer"],
                configuration["Athentication:Audience"],
                Cliams,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(100),
                SigningCredentials);

            var SerializedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(SerializedToken);
        }
        

        private User validUserCredenctails(AuthRequest authRequest)
        {
            var currentUser = UserConstants.Users.FirstOrDefault(x=>x.Name.ToLower() ==
                authRequest.Username.ToLower() && x.Password.ToLower() == authRequest.Password.ToLower());

            if (currentUser == null) 
                return null;

            return currentUser;
           
        }
    }
}
