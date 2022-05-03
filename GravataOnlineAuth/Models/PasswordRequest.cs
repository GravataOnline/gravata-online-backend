using System;

namespace GravataOnlineAuth.Models.PasswordRequest
{
    public class PasswordRequestModel
    {
        public int IDUSUARIO { get; set; }
        public string TOKEN { get; set; }
        public string SALT { get; set; }
        public DateTime DTVENCIMENTO { get; set; }
        public DateTime DTADD { get; set; }
    }
}