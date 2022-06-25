using AutoMapper;
using Gamgingroup.Data;
using Gamgingroup.DTOs;
using Gamgingroup.Intefaces;
using Gamgingroup.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Gamgingroup.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext dataContext, ITokenService tokenService, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username;
            user.UserName = registerDto.Username;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            user.DateOfBirth = registerDto.DateOfBirth;
            user.KnownAs = registerDto.KnownAs;
            user.Gender = registerDto.Gender;
            user.Introduction = registerDto.Introduction;
            user.MyProperty = registerDto.MyProperty;
            user.Interests = registerDto.Interests;
            user.City = registerDto.City;
            user.Country = registerDto.Country;

            var photo = new Photo
            {
                Url = registerDto.Url,
                IsMain = registerDto.IsMain,
                PublicId = registerDto.PublicId,
            };
            _dataContext.AppUsers.Add(user);
            _dataContext.Photos.Add(photo);
            await _dataContext.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender

            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _dataContext.AppUsers
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(a => a.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i =0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user.Gender
            };

        }

        private async Task<bool> UserExists(string username)
        {
            return await _dataContext.AppUsers.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
