using GravataOnlineAuth.Attributes;
using GravataOnlineAuth.Models.User;

using GravataOnlineAuth.Repository.User;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GravataOnlineAuth.Controllers.User
{
    [EnableCors("CorsAllowAll")]
    [Route("api/[controller]")]
    [ApiController]
    [Permission("notasfiscais")]
    public class UserController : MainController
    {
        private readonly IUserRepository _invoices;

        public UserController(IUserRepository UserRepository)
        {
            _invoices = UserRepository;
        }


        /// <summary>
        /// Create
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Ok</response>
        /// <response code="511">Usuário não autenticado</response>
        /// <response code="500">Erro no método</response>
        [HttpPost("create")]
        public async Task<IActionResult> UserAdd(CreateUsersInputModel user)
        {
            return BaseResponse(await _invoices.CreateUser(user));
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Ok</response>
        /// <response code="511">Usuário não autenticado</response>
        /// <response code="500">Erro no método</response>
        [HttpPost("login")]
        public IActionResult Login(LoginInputModel user)
        {
            var info = _invoices.Login(user);
            return BaseResponse(info);
        }

        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Ok</response>
        /// <response code="511">Usuário não autenticado</response>
        /// <response code="500">Erro no método</response>
        [HttpPut("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            return BaseResponse(await _invoices.RequestPasswordReset(email));
        }

          /// <summary>
        /// Update Password
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Ok</response>
        /// <response code="511">Usuário não autenticado</response>
        /// <response code="500">Erro no método</response>
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword(string token, string password)
        {
            return BaseResponse(await _invoices.UpdatePassword(token, password));
        }



    }
}

