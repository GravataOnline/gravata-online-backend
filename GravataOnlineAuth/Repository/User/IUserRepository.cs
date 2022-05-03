using GravataOnlineAuth.Models.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GravataOnlineAuth.Repository.User
{
    public interface IUserRepository
    {
        Task<(HttpStatusCode, dynamic)> CreateUser(CreateUsersInputModel input);
        (HttpStatusCode, dynamic) Login(LoginInputModel input);
        Task<(HttpStatusCode, dynamic)> RequestPasswordReset(string email);
        Task<(HttpStatusCode, dynamic)> UpdatePassword(string token, string password);
    }
}
