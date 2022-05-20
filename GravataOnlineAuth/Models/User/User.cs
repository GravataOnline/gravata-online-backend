using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace GravataOnlineAuth.Models.User
{
    [Table("USUARIO")]
    public class UserModel
    {
        [Key]
        public int ID { get; set; }
        public string NOME { get; set; }
        public string EMAIL { get; set; }
        public string SENHA { get; set; }
        [JsonIgnore]
        public string SALT { get; set; }
        public string CPF { get; set; }
        public string ENDERECO { get; set; }
        public string FONE { get; set; }
        public UserType TIPOUSUARIO { get; set; }
        public Spouse CONJUGE { get; set; }
        public static implicit operator UserModel(UserInputModel user)
        {
            return new UserModel
            {
                NOME = user.NAME,
                EMAIL = user.EMAIL,
                SENHA = user.PASSWORD,
                FONE = user.PHONE,
                TIPOUSUARIO = user.USERTYPE,
                CONJUGE = user.SPOUSE
            };
        }

    }

    public class UserInputModel
    {
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string PHONE { get; set; }
        public UserType USERTYPE { get; set; }
        public Spouse SPOUSE { get; set; }
    }

    public class UserViewModel {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public UserType TIPOUSUARIO { get; set; }
        public Spouse CONJUGE { get; set; }
        public string TOKEN { get; set; } 
        public static implicit operator UserViewModel(UserModel user)
        {
            return new UserViewModel
            {
                ID = user.ID,
                NAME = user.NOME,
                EMAIL = user.EMAIL,
                TIPOUSUARIO = user.TIPOUSUARIO,
                CONJUGE = user.CONJUGE
            };
        }
    }

    public class CreateUsersInputModel
    {
        public UserInputModel USER { get; set; }
        public UserInputModel SPOUSE { get; set; }
    }

    public class LoginInputModel {
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
    }


    public enum UserType
    {
        ADMIN = 1,
        USER = 2
    }

    public enum Spouse
    {
        HUSBAND = 1,
        WIFE = 2
    }

}

