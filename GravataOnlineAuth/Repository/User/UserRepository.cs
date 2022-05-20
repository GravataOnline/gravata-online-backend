
using GravataOnlineAuth.Common;
using GravataOnlineAuth.Common.Email;
using GravataOnlineAuth.Models.PasswordRequest;
using GravataOnlineAuth.Models.User;
using GravataOnlineAuth.Scripts.PasswordRequest;
using GravataOnlineAuth.Scripts.User;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GravataOnlineAuth.Repository.User
{
    public class UserRepository : Connections, IUserRepository
    {
        UserScripts scripts = new UserScripts();
        PasswordRequestScripts requests = new PasswordRequestScripts();
        public async Task<(HttpStatusCode, dynamic)> CreateUser(CreateUsersInputModel input)
        {
            try
            {
                var existingUser = Query<UserModel>(scripts.GetUserByEmail(input.USER.EMAIL)).FirstOrDefault();
                if (existingUser != null) return (HttpStatusCode.BadRequest, "Usuário já existe!");
                if (input.SPOUSE != null)
                {
                    var existingSpouse = Query<UserModel>(scripts.GetUserByEmail(input.SPOUSE.EMAIL)).FirstOrDefault();
                    if (existingSpouse != null) return (HttpStatusCode.BadRequest, "Conjuge já existe!");
                }
                UserModel user = input.USER;
                user.SALT = Convert.ToBase64String(Hash.GetSalt());
                user.SENHA = Hash.GetHash(Convert.ToBase64String(Encoding.ASCII.GetBytes(user.SENHA)), user.SALT);
                var userid = ExecuteReturning<int>(scripts.Create(user)).FirstOrDefault();
                var message1 = @$"Olá {user.NOME}, seja bem vindo(a) á Gravata Online!
                                <br><br>
                                Seu cadastro foi realizado com sucesso.
                                <br><br><br>
                                Atenciosamente,
                                <br><br>
                                Equipe Gravata Online";
                await MailServices.SendMailAsync(user.EMAIL, "Gravata Online - Boas vindas", message1);
                if (input.SPOUSE != null)
                {
                    UserModel spouse = input.SPOUSE;
                    spouse.SENHA = null;
                    spouse.SALT = null;
                    var spouseid = ExecuteReturning<int>(scripts.Create(spouse)).FirstOrDefault();
                    var requesttoken = CreateRequest(spouse.EMAIL);
                    var message2 = @$"Olá {user.NOME}, seja bem vindo(a) á Gravata Online!
                                <br><br>
                                Para concluir seu cadastro, favor clicar no link abaixo:
                                <br><br>
                                <a href='gravataonline.net/{requesttoken}'>Ativar conta</a>
                                <br><br><br>
                                Atenciosamente,
                                <br><br>
                                Equipe Gravata Online";
                    await MailServices.SendMailAsync(spouse.EMAIL, "Gravata Online - Confirmação de cadastro", message2);
                }
                return (HttpStatusCode.OK, new { userid });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public (HttpStatusCode, dynamic) Login(LoginInputModel input)
        {
            try
            {
                var user = Query<UserModel>(scripts.GetUserByEmail(input.EMAIL)).FirstOrDefault();
                if (user == null) return (HttpStatusCode.BadRequest, "Usuário não existe!");
                if (user.SENHA != Hash.GetHash(Convert.ToBase64String(Encoding.ASCII.GetBytes(input.PASSWORD)), user.SALT)) return (HttpStatusCode.BadRequest, "Senha incorreta!");
                UserViewModel information = user;
                information.TOKEN = this.GerarTokenJWT(user);
                return (HttpStatusCode.OK, information);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public (HttpStatusCode, dynamic) GetUserById(int id)
        {
            try
            {
                UserViewModel user = Query<UserModel>(scripts.GetUserById(id)).FirstOrDefault();
                if (user == null) return (HttpStatusCode.BadRequest, "Usuário não existe!");
                return (HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(HttpStatusCode, dynamic)> RequestPasswordReset(string email)
        {
            try
            {
                var user = Query<UserModel>(scripts.GetUserByEmail(email)).FirstOrDefault();
                if (user == null) return (HttpStatusCode.BadRequest, "Usuário não existe!");
                var token = CreateRequest(user.EMAIL);
                var message = @$"Olá {user.NOME}, segue o link para resetar sua senha:
                                <br><br>
                                <a href='gravataonline.net/{token}'>Resetar senha</a>
                                <br><br><br>
                                Atenciosamente,
                                <br><br>
                                Equipe Gravata Online";
                await MailServices.SendMailAsync(user.EMAIL, "Gravata Online - Resetar senha", message);
                return (HttpStatusCode.OK, "Email enviado com sucesso!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(HttpStatusCode, dynamic)> UpdatePassword(string token, string password)
        {
            try
            {
                var request = Query<PasswordRequestModel>(requests.Get(token)).FirstOrDefault();
                if (request == null) return (HttpStatusCode.BadRequest, "Solicitação inválida!");
                if (request.DTVENCIMENTO < DateTime.Now)
                {
                    Execute(requests.Delete(token));
                    return (HttpStatusCode.BadRequest, "Solicitação expirada!");
                }
                var user = Query<UserModel>(scripts.GetUserById(request.IDUSUARIO)).FirstOrDefault();
                if (user == null) return (HttpStatusCode.BadRequest, "Usuário não existe!");
                user.SALT = Convert.ToBase64String(Hash.GetSalt());
                user.SENHA = Hash.GetHash(Convert.ToBase64String(Encoding.ASCII.GetBytes(password)), user.SALT);
                Execute(scripts.UpdatePassword(user.ID, user.SENHA, user.SALT));
                Execute(requests.Delete(request.TOKEN));
                var message = @$"Olá {user.NOME}, sua senha foi alterada com sucesso!
                                <br><br><br>
                                Atenciosamente,
                                <br><br>
                                Equipe Gravata Online";
                await MailServices.SendMailAsync(user.EMAIL, "Gravata Online - Senha alterada", message);
                return (HttpStatusCode.OK, "Senha atualizada com sucesso!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string CreateRequest(string email)
        {
            try
            {
                var salt = Convert.ToBase64String(Hash.GetSalt());
                var user = Query<UserModel>(scripts.GetUserByEmail(email)).FirstOrDefault();
                var request = new PasswordRequestModel
                {
                    IDUSUARIO = user.ID,
                    DTVENCIMENTO = DateTime.Now.AddDays(1),
                    SALT = salt,
                    TOKEN = Hash.GetHash(Convert.ToBase64String(Encoding.ASCII.GetBytes(user.ID.ToString())), salt)
                };
                Execute(requests.Create(request));
                return request.TOKEN;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GerarTokenJWT(UserModel user)
        {
            DateTime issuedAt = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();
            var issuer = Base.ISSUER;
            var audience = Base.AUDIENCE;
            var expiry = DateTime.UtcNow.AddMinutes(120);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Base.JWTKEY));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer: issuer, audience: audience, 
            expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);
            token.Payload["idusuario"] = user.ID;
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;

        }

    }
}
