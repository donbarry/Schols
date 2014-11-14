using Schols.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Schols.Controllers
{
    public class AccountController : ApiController
    {
        [Route("api/login2")]
        [HttpPost]
        [HttpGet]
        public IHttpActionResult Login([FromBody]string username)
        {
            //return Unauthorized();
            return Json<string>("nfsdkdfs");
        }
        [Route("api/fakelogin")]
        public IHttpActionResult FakeLogin(UserModel user)
        {
            //UserDatabase udb = new UserDatabase();
            //user = udb.ValidUser(user);
            System.Diagnostics.Debug.WriteLine(user.ToString());
            if (user.username.Equals("uche")) user.accesstoken = UserDatabase.generateTokenNoDB(user);
            else user.accesstoken = "";
            if (user.accesstoken.Equals(""))
                return Unauthorized();
            else
                return Ok(user);
        }

        [Route("api/login")]
        public IHttpActionResult Login(UserModel user)
        {
            UserDatabase udb = new UserDatabase();
            user = udb.ValidUser(user);
            if (user.accesstoken.Equals(""))
                return Unauthorized();
            else
                return Ok(user);
        }
        [Route("api/loginwithtoken")]
        [HttpGet]
        public IHttpActionResult TokenLogin()
        {
            HttpContext httpContext = HttpContext.Current;
            NameValueCollection headerList = httpContext.Request.Headers;
            string authorizationField = headerList.Get("Authorization");
            if (authorizationField != null)
            {
                authorizationField = authorizationField.Replace("Bearer ", "");
                UserDatabase udb = new UserDatabase();
                UserModel user = udb.CheckToken(authorizationField);
                System.Diagnostics.Debug.WriteLine(authorizationField);
                System.Diagnostics.Debug.WriteLine(user.username);
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("api/register")]
        public IHttpActionResult Register(UserModel user)
        {
            UserDatabase udb = new UserDatabase();
            string message = udb.RegisterUser(user);
            if (message.Equals(""))
                return Ok(user);
            else
                return NotFound();
        }
        [Route("api/login2")]
        public IHttpActionResult Login(string username, string password)
        {
            return Unauthorized();
        }
        [Route("api/profile")]
        [HttpGet]
        public IHttpActionResult Profile()
        {
            HttpContext httpContext = HttpContext.Current;
            NameValueCollection headerList = httpContext.Request.Headers;
            string authorizationField = headerList.Get("Authorization");
            authorizationField = authorizationField.Replace("Bearer ", "");
            UserDatabase udb = new UserDatabase();
            UserModel user=udb.CheckToken(authorizationField);
            //return "{Message" + ":" + "You-accessed-this-message-with-authorization" + "}"; return Ok(headers.ToString());
            return Ok(user);
        }
        [Route("api/addfavorite")]
        [HttpPost]
        public IHttpActionResult AddFavorite(Favorite fav)
        {
            HttpContext httpContext = HttpContext.Current;
            NameValueCollection headerList = httpContext.Request.Headers;
            string authorizationField = headerList.Get("Authorization");
            authorizationField = authorizationField.Replace("Bearer ", "");
            UserDatabase udb = new UserDatabase();
            UserModel user = udb.CheckToken(authorizationField);
            string message=udb.AddFavorite(fav.fundacct, user);
            //return "{Message" + ":" + "You-accessed-this-message-with-authorization" + "}"; return Ok(headers.ToString());
            return Ok(message);
        }

    }
}
