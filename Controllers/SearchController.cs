using Schols.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Scholarship.Controllers
{
    //[RoutePrefix("api/scholarships")]

    public class SearchController : ApiController
    {
        //private ScholarshipsEntities db = new ScholarshipsEntities();
        //private SchEntities db = new SchEntities();

        //[Route("{id:int}")]
        /*
        [Route("title/{title}/purpose/{purpose}")]
        public IHttpActionResult GetScholarship(String title, String purpose)
        {
            //String idString = id.ToString();

            var scholarship = db.SCHLRSHPs.Where(s => (s.FRML_SCHLRSHP_NAME.Contains(title) || s.SCHLRSHP_TITLE.Contains(title) || s.SCHLRSHP_PRPS.Contains(purpose)));
            if (scholarship == null)
            {
                return NotFound();
            }
            return Ok(scholarship);
        }
        */
        [Route("api/dropdowndata")]
        public IHttpActionResult GetDropDownData()
        {
            DBObject db = new DBObject();
            DropDownData dropdownData = db.GetDropDownData();
            return Ok(dropdownData);
        }

        [Route("api/departments")]
        public IHttpActionResult GetDepartments()
        {
            DBObject db = new DBObject();
            List<Schols.Models.Department> departments = db.GetDepartments();
            return Ok(departments);
        }
        [Route("api/colleges")]
        public IHttpActionResult GetColleges()
        {
            DBObject db = new DBObject();
            List<Schols.Models.College> colleges = db.GetColleges();
            return Ok(colleges);
        }
        [Route("api/schoolyears")]
        public IHttpActionResult GetSchoolYears()
        {
            DBObject db = new DBObject();
            List<Schols.Models.SchoolYear> schoolYears = db.GetSchoolYears();
            return Ok(schoolYears);
        }

        //[HttpPost]
        //public HttpResponseMessage Post([FromBody] SearchObject searchObject)
        //{
        //            return Request.CreateResponse(HttpStatusCode.Created);

        //}
        //[Route("post")]

        //public IEnumerable<SCHLRSHP> Post([FromBody] SearchObject searchObject)
        [Route("api/scholarshippage")]
        public IHttpActionResult GetScholarshipData([FromUri]string f, [FromUri]string s)
        {
            DBObject db = new DBObject();
            ScholarshipData data = db.GetScholarshipData(f, s);
            System.Diagnostics.Debug.WriteLine("URI : " + f + ":" + s);
            return Ok(data);
        }

        public List<Schols.Models.Scholarship> Post([FromBody] SearchObject searchObject)
        {
            DBObject db = new DBObject();

            System.Diagnostics.Debug.Write(searchObject);
            //searchObject.title = "%" + searchObject.title + "%"; //no need for this since using regexp_like(FRML_SCHLRSHP_NAME, 'mauk', 'i') 
            List<Schols.Models.Scholarship> scholarships = db.GetScholarships(searchObject);
            //if (scholarship == null)
            {
                System.Diagnostics.Debug.Write(scholarships.Count);
                //System.Diagnostics.Debug.Write(scholarships[0].FRML_SCHLRSHP_NAME);
                //IEnumerable<SCHLRSHP> Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            //System.Diagnostics.Debug.Write(scholarship);
            System.Diagnostics.Debug.Write("here2");

            return scholarships;
            //Request.CreateResponse(HttpStatusCode.Created,scholarship);
            //String idString = id.ToString();
            //var scholarship = db.SCHLRSHPs.Where(s => (s.FRML_SCHLRSHP_NAME.Contains(searchObject.title) || s.SCHLRSHP_TITLE.Contains(searchObject.title) || s.SCHLRSHP_PRPS.Contains(searchObject.purpose)));
            //var query = database.Posts    // your starting point - table in the "from" statement
            //   .Join(database.Post_Metas, // the source table of the inner join
            //      post => post.ID,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
            //      meta => meta.Post_ID,   // Select the foreign key (the second part of the "on" clause)
            //      (post, meta) => new { Post = post, Meta = meta }) // selection
            //   .Where(postAndMeta => postAndMeta.Post.ID == id);
            /*
            var ss=db.SCHLRSHPs.Where(s => s.FUND_ACCT.Trim().Contains("307"));
            var lst = db.SCHLRSHPs.Where(s => s.FUND_ACCT == "307846").Include(sc => sc.KPIScores).ToList();
            var scholarship = db.SCHLRSHPs
                                .Join(db.FUNDs,
                                s => s.FUND_ACCT, f => f.FUND_ACCT,
                                (s,f) => new { S=s, F=f})
              */
            /*
            .Join(db
            .Where(s => (s.FRML_SCHLRSHP_NAME.Contains(searchObject.title) || s.SCHLRSHP_TITLE.Contains(searchObject.title)
                            || s.SCHLRSHP_PRPS.Contains(searchObject.purpose)));
             * */
            //var scholarship = db.SCHLRSHPs.Where(s => s.FUND_ACCT.Trim().Contains("307"));

        }

    }
}
