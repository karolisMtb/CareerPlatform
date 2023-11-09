using CareerPlatform.BusinessLogic.Interfaces;
using CareerPlatform.DataAccess.DTOs;
using CareerPlatform.Shared.Exceptions;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CareerPlatform.BusinessLogic.Services.SecurityServices
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        public AuthenticateService(IConfiguration configuration, 
            UserManager<IdentityUser> userManager,
            IUserService userService,
            IEmailSender emailSender)
        {
            _configuration = configuration;
            _userManager = userManager;
            _userService = userService;
            _emailSender = emailSender;
        }

        private async Task<List<Claim>> GetUserClaimsAsync(IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = await GetAuthenticationClaimsAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return claims;
        }

        private async Task<List<Claim>> GetAuthenticationClaimsAsync(IdentityUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            return authClaims;
        }

        private async Task<JwtSecurityToken> GetTokenAsync(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(15),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private async Task<IdentityUser> NewUserAsync(RegisterModel model)
        {
            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            return user;
        }

        public async Task<LoginValidationDto> ValidateUserLoginAsync(IdentityUser user)
        {
            List<Claim> claims = await GetUserClaimsAsync(user);
            JwtSecurityToken token = await GetTokenAsync(claims);

            var authToken = new JwtSecurityTokenHandler().WriteToken(token);
            var expiration = token.ValidTo;
            LoginValidationDto loginValidationDto = new(authToken, expiration);

            return loginValidationDto;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
        {
            IdentityUser userByName = await _userManager.FindByNameAsync(model.Username);
            IdentityUser userByEmail = await _userManager.FindByEmailAsync(model.Email);

            if (userByName is not null  || userByEmail is not null)
            {
                throw new ExistingUserFoundException("User already exists with this credential. Try another one, please");
            }

            IdentityUser user = await NewUserAsync(model);
            var result = _userManager.CreateAsync(user, model.Password);
            var nonIdentityUserResult = await _userService.CreateNonIdentityUserAsync(user);

            if(!result.IsCompleted && !nonIdentityUserResult.Succeeded)
            {
                return IdentityResult.Failed(
                    new IdentityError()
                    {
                        Description = "Invalid username or password"
                    },
                    new IdentityError()
                    {
                        Description = "Server side error"
                    }
                );
            }
            else
            {
                await _emailSender.ConfirmUserRegistrationAsync(user);

                //send user registration confirmation email
                //email linka paspaudzia
                //request nueina i accountConfirmation endpoint
                //_userManager setina emailConfirmation to 1
            }
            
            return IdentityResult.Success;
        }

        public async Task<bool> ValidateUserPasswordAsync(LoginModel model)
        {
            bool validPassword = await _userManager.CheckPasswordAsync(await _userManager.FindByNameAsync(model.Username), model.Password);
            
            return validPassword;
        }

        public async Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string password)
        {

            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, password);
            if(!result.Succeeded)
            {
                throw new Exception("Error occured while resetting your password.");
            }

            return result;
        }

        public async Task SendPasswordResetLinkAsync(IdentityUser user)
        {
           await _emailSender.ResetPasswordAsync(user);           
        }
    }
}
