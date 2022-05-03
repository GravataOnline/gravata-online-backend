using GravataOnlineAuth.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GravataOnlineAuth.Scripts.User
{
    public class UserScripts
    {
        public Dictionary<string, object> Create(UserModel notaFiscal)
        {
            string sql = @"INSERT INTO USUARIO (NOME, EMAIL, SENHA, SALT, CPF, ENDERECO, FONE, TIPOUSUARIO, CONJUGE)
                            VALUES (:NOME, :EMAIL, :SENHA, :SALT, :CPF, :ENDERECO, :FONE, :TIPOUSUARIO, :CONJUGE)
                            RETURNING ID INTO :ID";

            return new Dictionary<string, object>() { { sql, notaFiscal } };
        }

        public Dictionary<string, object> GetUserByEmail(string email)
        {
            string sql = @"SELECT * FROM USUARIO WHERE EMAIL = :EMAIL";

            return new Dictionary<string, object>() { { sql, new { EMAIL = email } } };
        }

        public Dictionary<string, object> GetUserById(int id)
        {
            string sql = @"SELECT * FROM USUARIO WHERE ID = :ID";

            return new Dictionary<string, object>() { { sql, new { ID = id } } };
        }

        public Dictionary<string, object> UpdatePassword(int id, string password, string salt)
        {
            string sql = @"UPDATE USUARIO SET SENHA = :SENHA, SALT = :SALT WHERE ID = :ID";

            return new Dictionary<string, object>() { { sql, new { ID = id, SENHA = password, SALT = salt } } };
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposedValue = true;
                }
            }
        }

        ~UserScripts()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
