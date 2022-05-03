using System.Collections.Generic;
using GravataOnlineAuth.Models.PasswordRequest;

namespace GravataOnlineAuth.Scripts.PasswordRequest
{
    public class PasswordRequestScripts
    {

        public Dictionary<string, object> Create(PasswordRequestModel passwordRequest)
        {
            string sql = @"INSERT INTO ESQUECERSENHA (IDUSUARIO, TOKEN, DTADD, DTVENCIMENTO, SALT)
                            VALUES (:IDUSUARIO, :TOKEN, SYSTIMESTAMP, SYSTIMESTAMP + 1, :SALT)";

            return new Dictionary<string, object>() { { sql, passwordRequest } };
        }

        public Dictionary<string, object> Get(string token)
        {
            string sql = @"SELECT * FROM ESQUECERSENHA WHERE TOKEN = :TOKEN";

            return new Dictionary<string, object>() { { sql, new { TOKEN = token } } };
        }
        public Dictionary<string, object> Delete(string token)
        {
            string sql = @"DELETE FROM ESQUECERSENHA WHERE TOKEN = :TOKEN";

            return new Dictionary<string, object>() { { sql, new { TOKEN = token } } };
        }
        
    }
}