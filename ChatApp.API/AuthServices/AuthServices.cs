using ChatApp.API.Database;
using ChatApp.API.Dtos;
using ChatApp.API.Extend;
using ChatApp.API.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.API.AuthServices
{
    public class AuthServices:IAuthServices
    {
        #region fields
        private readonly UserManager<AppUser> userManager;
        private readonly IOptions<Jwt> jwt;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AppDbContext context;

        #endregion

        #region Ctor
        public AuthServices(UserManager<AppUser> userManager, IOptions<Jwt> jwt, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.jwt = jwt;
            this.roleManager = roleManager;
            this.context = context;
        }
        #endregion

        #region Login
        public async Task<AuthModel> Login(LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null || !await userManager.CheckPasswordAsync(user, loginDTO.Password))
                return new AuthModel { Message = "Inavlid Email Or Password" };

            //if (user.EmailConfirmed == false)
            //{
            //    return new AuthModel { Message = "Please Check Your Inbox To Confirm Email" };
            //}

            await context.SaveChangesAsync();

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Message = "Success",
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthencated = true
            };
        }
        #endregion

        #region Register

        public async Task<AuthModel> Register(RegisterDto registerDTO)
        {
            try
            {
                if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
                    return new AuthModel { Message = "Email Is Already Token" };
                var user = new AppUser
                {
                    Email = registerDTO.Email,
                    UserName = registerDTO.Email,
                    FullName = registerDTO.FullName,
                };
                var result = await userManager.CreateAsync(user, registerDTO.Password);

                if (!result.Succeeded)
                {
                    var error = string.Empty;
                    foreach (var item in result.Errors)
                    {
                        error += $"{item.Description},";
                    }
                    return new AuthModel { Message = error };
                }

                var RoleExsit = await roleManager.RoleExistsAsync("Admin");
                if (!RoleExsit)
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await userManager.AddToRoleAsync(user, "Admin");
                    await userManager.UpdateAsync(user);
                }
                else
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                    await userManager.AddToRoleAsync(user, "User");
                }

                var jwtSecurityToken = await CreateJwtToken(user);


                return new AuthModel
                {
                    Message = "Success",
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    IsAuthencated = true
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region Create Token
        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullName", user.FullName),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwt.Value.Issuer,
                audience: jwt.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(jwt.Value.DurationInDays),
                signingCredentials: signingCredentials);



            return jwtSecurityToken;


        }
        #endregion
    }
}
