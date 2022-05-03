using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GravataOnlineAuth.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {

        protected MainController()
        {
        }

        protected ActionResult BaseResponse((HttpStatusCode, dynamic) result)
        {
            return Request.Method == "GET" ?
            StatusCode((int)result.Item1, result.Item2) :
            StatusCode((int)result.Item1, new
            {
                success = result.Item1 == HttpStatusCode.OK,
                message = result.Item2
            });
        }
    }
}