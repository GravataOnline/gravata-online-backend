﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GravataOnlineAuth.Common
{
    public class Base
    {
        public static string DBTYPE { get; set; }
        public static string CONNECTIONSTRING { get; set; }
        public static string AUTHCONNECTIONSTRING { get; set; }
        public static string JWTKEY { get; set; }
        public static string ISSUER { get; set; }
        public static string AUDIENCE { get; set; }
    }

   
}
