using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Schols.Controllers
{
    public class ValuesController : ApiController
    {

        // GET api/<controller>
        [Authorize]
        public IEnumerable<string> Get()
        {
            //IEnumerable<string> headerValues = request.Headers.GetValues("MyCustomID");
            //var id = headerValues.FirstOrDefault();
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [Route("api/login3")]
        public string Post([FromBody]string username, [FromBody]string password)
        {
            return "nothing";
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}