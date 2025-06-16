// Оновлений AuthService.cs
using AutoMapper;
using FoodDelivery.BLL.DTOs;
using FoodDelivery.BLL.Interfaces;
using FoodDelivery.Common.Interfaces;
using FoodDelivery.DAL.Entities;
using FoodDelivery.DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeConverter _dateTimeConverter;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<User> userManager,
            IMapper mapper,
            IConfiguration configuration,
            IDateTimeConverter dateTimeConverter,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _dateTimeConverter = dateTimeConverter;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterUserDto registerDto)
        {
            var user = _mapper.Map<User>(registerDto);

            // Перетворюємо дату народження на UTC
            user.DateOfBirth = _dateTimeConverter.ConvertToUtc(registerDto.DateOfBirth);
            user.RefreshTokens = new List<RefreshToken>();

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                // Генеруємо токени
                var jwtToken = await GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken("127.0.0.1"); // За замовчуванням, в реальному проекті тут буде IP адреса

                // Зберігаємо refresh token в базі даних
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);

                return new AuthResultDto
                {
                    Succeeded = true,
                    AccessToken = jwtToken,
                    RefreshToken = refreshToken.Token,
                    User = _mapper.Map<UserDto>(user)
                };
            }

            return new AuthResultDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        public async Task<AuthResultDto> LoginAsync(LoginUserDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Errors = new List<string> { "Користувача з таким email не знайдено" }
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Errors = new List<string> { "Невірний пароль" }
                };
            }

            // Генеруємо токени
            var jwtToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken("127.0.0.1"); // За замовчуванням

            // Отримуємо користувача з refresh токенами
            user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            // Видаляємо старі refresh токени
            RemoveOldRefreshTokens(user);

            // Додаємо новий refresh токен
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthResultDto
            {
                Succeeded = true,
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string token, string ipAddress)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == token));

            if (user == null)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Errors = new List<string> { "Недійсний токен" }
                };
            }

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);

            if (!refreshToken.IsActive)
            {
                return new AuthResultDto
                {
                    Succeeded = false,
                    Errors = new List<string> { "Токен неактивний" }
                };
            }

            // Генеруємо новий токен
            var newRefreshToken = GenerateRefreshToken(ipAddress);

            // Відкликаємо старий токен
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            // Додаємо новий токен
            user.RefreshTokens.Add(newRefreshToken);

            // Видаляємо старі токени
            RemoveOldRefreshTokens(user);

            // Оновлюємо користувача
            await _userManager.UpdateAsync(user);

            // Генеруємо новий JWT токен
            var jwtToken = await GenerateJwtToken(user);

            return new AuthResultDto
            {
                Succeeded = true,
                AccessToken = jwtToken,
                RefreshToken = newRefreshToken.Token,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<bool> RevokeTokenAsync(string token, string ipAddress)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == token));

            if (user == null)
            {
                return false;
            }

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);

            if (!refreshToken.IsActive)
            {
                return false;
            }

            // Відкликаємо токен
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateUserAsync(string userId, UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Address = userDto.Address;
            user.PhoneNumber = userDto.PhoneNumber;

            // Перетворюємо дату народження на UTC
            user.DateOfBirth = _dateTimeConverter.ConvertToUtc(userDto.DateOfBirth);

            await _userManager.UpdateAsync(user);

            return _mapper.Map<UserDto>(user);
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:ExpirationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            // Генеруємо випадковий токен
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JWT:RefreshTokenExpirationInDays"])),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void RemoveOldRefreshTokens(User user)
        {
            // Видаляємо старі refresh токени (старші 7 днів)
            user.RefreshTokens.RemoveAll(rt =>
                !rt.IsActive &&
                rt.Created.AddDays(Convert.ToDouble(_configuration["JWT:RefreshTokenTTL"])) <= DateTime.UtcNow);
        }
    }
}