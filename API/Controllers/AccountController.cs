using API.Entities;
using API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using AutoMapper;

namespace API.Controllers
{
    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        public readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, 
        ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;           
            _context = context;
        }

        [HttpPost("register")]
         public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
         {
            if(await UserExists(registerDTO.Username)) 
             return BadRequest("Username already taken");
            
            var user = _mapper.Map<AppUser>(registerDTO);
            
             using var hmac = new HMACSHA512();

           
               user.UserName = registerDTO.Username;
               user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
               user.PasswordSalt = hmac.Key;
            
             _context.Users.Add(user);
             await _context.SaveChangesAsync();

             return new UserDTO{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
             };

         }

        [HttpPost("login")]
         public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
         {
                                  
              var user = await _context.Users
               .Include(x => x.Photos)
              .SingleOrDefaultAsync (x => x.UserName == loginDTO.Username);
                         
              if(user ==null) return Unauthorized("Invalid username");

             using var hmac = new HMACSHA512(user.PasswordSalt);
              
              var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        
                for(int i =0; i<ComputeHash.Length; i++)
                {

                    if(ComputeHash[i] != user.PasswordHash[i]){
                        return Unauthorized("Invalid password");
                    }
                }           

              return new UserDTO{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
             };
         }

           private async Task<bool> UserExists(string username)
           {
                   return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
           }
    
    }
}