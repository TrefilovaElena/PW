using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PW.DataModel.Entities;
using PW.Helpers;
using PW.Services;
using PW.ViewModels;


namespace PW.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public string LoggedInUser => User.Identity.Name;

        public UserController(
            IUserService userService,
           IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("/authenticate")]
        public IActionResult Authenticate([FromBody]UserViewModel model)
        {
            var user = _userService.Authenticate(model.Email, model.Password);
            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new ObjectResult(new
            {
                user.Id,
                Username = user.UserName,
                user.Email,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody]UserViewModel model)
        {
            var user = _mapper.Map<User>(model);

            var userTuple = _userService.Create(user, model.Password);

            return new ObjectResult(new
            {
                Message = userTuple.Item2,
                Success = userTuple.Item1!=null?true:false
            });

        }

        [HttpGet("/api/getusers")]
        public IActionResult GetUserByName(AutocompleteCodeDto model)
        {
            try
            {
                int currentUserId = int.Parse(LoggedInUser);
                List<AutocomleteSelectModel> names = _userService.GetUserBySearchString(currentUserId, model.Term, model.Count);
                return Ok(names);
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
        }
     

    }
}