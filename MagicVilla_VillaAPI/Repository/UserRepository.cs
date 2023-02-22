using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
            secretKey = configuration.GetValue<string>("APISettings:SecretKey");
            _roleManager = roleManager;
        }

        public async Task<bool> IsUniqueUser(string username)
        {
            //var user = _db.LocalUsers.FirstOrDefault(x => x.Username == username);
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            //var user = _db.ApplicationUsers.FirstOrDefault(u => u.Username.ToLower() == loginRequestDTO.Username.ToLower() && u.Password == loginRequestDTO.Password);
            var user = await _userManager.FindByNameAsync(loginRequestDTO.Username);
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (!isValid || user == null)
            {
                return new LoginResponseDTO()
                {
                    User = null,
                    Token = ""
                };
            }

            // If user was found - Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()) // If more than one roles => for each loop - In this will assume  that there is one role
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO = new()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                //Role = roles.FirstOrDefault(),
            };
            return loginResponseDTO;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new() 
            {
                UserName = registerationRequestDTO.UserName,
                Email = registerationRequestDTO.UserName,
                NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
                Name = registerationRequestDTO.Name,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if(result.Succeeded)
                {
                    if(!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                    }
                    await _userManager.AddToRoleAsync(user, "admin");
                    var userToReturn = await _userManager.FindByNameAsync(registerationRequestDTO.UserName);
                    //return new UserDTO()
                    //{
                    //    ID = userToReturn.Id,
                    //    UserName = userToReturn.UserName,
                    //    Name = userToReturn.Name,

                    //};
                    return _mapper.Map<UserDTO>(userToReturn);
                }
            }
            catch (Exception e)
            {

                throw;
            }
            //return new UserDTO();
            return null;
        }
    }
}
