﻿using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace API.Controllers
{
    
    public class UsersController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
       private readonly IMapper _mapper;
       private readonly IPhotoService _photoService;
        public UsersController(  IMapper mapper,
        IUnitOfWork unitOfWork, IPhotoService photoService) //remember to return
        {           
            _unitOfWork = unitOfWork;
            _mapper = mapper;   
            _photoService = photoService;       
        }

         
        //[Authorize(Roles ="Admin")] //temporary
         [HttpGet]      
         public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParam userParam)
        {   
            var gender = await _unitOfWork.UserRepository.GetUserGenderAsync(User.GetUsername());
             
            userParam.CurrentUsername =User.GetUsername();

            if(string.IsNullOrEmpty(userParam.Gender))
              userParam.Gender = gender == "male" ? "female" : "male";
            
            
             var users = await _unitOfWork.UserRepository.GetMembersAsync(userParam);  
              
             Response.AddPaginationHeader(users.CurrentPage, users.PageSize,
             users.TotalCount, users.TotalPages);
              

             return Ok(users);
          
             
        }
        
         //[HttpGet("{id }", Name = "GetUsers")]      
         public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsersById(int id)
        {   
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(User.GetUserId());
             
             UserParam userParam = new UserParam();

            userParam.CurrentUsername = user.Id.ToString();
            
             var users = await _unitOfWork.UserRepository.GetMembersAsync(userParam);  
              
             Response.AddPaginationHeader(users.CurrentPage, users.PageSize,
             users.TotalCount, users.TotalPages);
              

             return Ok(users);
          
             
        }
       
       // [Authorize(Roles ="Member")] //temporary
        [HttpGet("{username}", Name = "GetUser")]
        public async Task <ActionResult<MemberDto>> GetUser(string username)
        {
            return await _unitOfWork.UserRepository.GetMemberByNameAsync(username);
        }

        
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
             
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

             if (user == null) return NotFound();
           
           
            _mapper.Map(memberUpdateDto, user);
                
           // _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Complete())
            
            return NoContent();
                
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return BadRequest("Cannot update user");

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            user.Photos.Add(photo);

            if (await _unitOfWork.Complete())
                return CreatedAtAction(nameof(GetUser),
                    new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            //Geting hold of our user
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
             
             //getting the photo
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            //checking to prevent user setting the main photo to main
            if(photo.IsMain) return BadRequest("This is alread your main photo");
            //else we set to main
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
           
           if(await _unitOfWork.Complete()) return NoContent();

           return BadRequest("Failed to set main photo");
        }

        [HttpDelete(("delete-photo/{photoId}"))]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
              var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

              var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
              if(photo is null) return NotFound();
              if(photo.IsMain) return BadRequest("You can not delete your main photo");
              if(photo.PublicId != null) 
              {
                 var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                 if(result.Error != null) return BadRequest(result.Error.Message);
              }
              
              user.Photos.Remove(photo);
              if(await _unitOfWork.Complete()) return Ok();
              return BadRequest("Failed to delete the photo");
        }
    }
}
