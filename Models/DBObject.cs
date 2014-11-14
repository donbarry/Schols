﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;


namespace Schols.Models
{
    public class DBObject
    {
        OracleConnection cn;
        string ConnectionString;
        string SQLConnString="Data Source=.; Integrated Security=true; Initial Catalog=manualowin";
        public string OracleConnString(string host, string port, string servicename, string user, string pass)
        {
            return String.Format(
              "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})" +
              "(PORT={1}))(CONNECT_DATA=(SID={2})(SERVER=DEDICATED)));User Id={3};Password={4};",
              host,
              port,
              servicename,
              user,
              pass);
        }
        
        public DataTable query(string sqlstr, List<OracleParameter> parameters)
        {

            ConnectionString = OracleConnString("atoraagilon01.at.illinoisstate.edu", "1521", "ONEPRD", "BUAJOKU", "Hu481919");
            //ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ScholarshipsEntities"].ConnectionString;
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(ConnectionString)) // connect to oracle
            {
                conn.Open(); // open the oracle connection
                using (OracleCommand comm = new OracleCommand(sqlstr, conn)) // create the oracle sql command
                {
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            comm.Parameters.Add(parameter);
                            System.Diagnostics.Debug.WriteLine("Param and val");
                            System.Diagnostics.Debug.Write(parameter.ParameterName);
                            System.Diagnostics.Debug.Write(parameter.Value);
                        }

                    }
                    System.Diagnostics.Debug.WriteLine(comm.CommandText);
                    using (OracleDataAdapter myadapter = new OracleDataAdapter())
                    {
                        myadapter.SelectCommand = comm;
                        myadapter.Fill(dt);
                    }
                    comm.Dispose();
                }
                conn.Close(); // close the oracle connection
            }
            return dt;
        }
        public int queryExecute(string sqlstr, List<OracleParameter> parameters)
        {

            ConnectionString = OracleConnString("atoraagilon01.at.illinoisstate.edu", "1521", "ONEPRD", "BUAJOKU", "Hu481919");
            //ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ScholarshipsEntities"].ConnectionString;
            int result = 0;

            using (OracleConnection conn = new OracleConnection(ConnectionString)) // connect to oracle
            {
                conn.Open(); // open the oracle connection
                using (OracleCommand comm = new OracleCommand(sqlstr, conn)) // create the oracle sql command
                {
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            comm.Parameters.Add(parameter);
                            System.Diagnostics.Debug.WriteLine("Param and val");
                            System.Diagnostics.Debug.Write(parameter.ParameterName);
                            System.Diagnostics.Debug.Write(parameter.Value);
                        }
                    }
                    result = comm.ExecuteNonQuery();
                    comm.Dispose();
                }
                conn.Close(); // close the oracle connection
            }
            return result;
        }
        public DropDownData GetDropDownData()
        {
            DropDownData dropdownData = new DropDownData();
            dropdownData.colleges = GetColleges();
            dropdownData.departments = GetDepartments();
            dropdownData.schoolyears = GetSchoolYears();
            return dropdownData;
        }

        public List<Department> GetDepartments()
        {
            string sqlstr = "SELECT * FROM UHELP.FUND_DEPT_ATTRB";

            DataTable dt = query(sqlstr, null);
            List<Department> departmentList = new List<Department>();
            Department aDepartment;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                aDepartment = new Department();
                Type t = aDepartment.GetType();
                foreach (PropertyInfo info in t.GetProperties())
                {
                    if (!DBNull.Value.Equals(dt.Rows[i][info.Name])) info.SetValue(aDepartment, dt.Rows[i][info.Name]);
                }
                departmentList.Add(aDepartment);
            }

            return departmentList;
        }
        public List<College> GetColleges()
        {
            string sqlstr = "SELECT * FROM UHELP.FUND_COLL_ATTRB";
            DataTable dt = query(sqlstr, null);
            List<College> collegeList = new List<College>();
            College aCollege;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                aCollege = new College();
                Type t = aCollege.GetType();
                foreach (PropertyInfo info in t.GetProperties())
                {
                    if (!DBNull.Value.Equals(dt.Rows[i][info.Name])) info.SetValue(aCollege, dt.Rows[i][info.Name]);
                }
                collegeList.Add(aCollege);

            }
            return collegeList;
        }
        public List<SchoolYear> GetSchoolYears()
        {
            string sqlstr = "select * from uhelp.user_cd where USER_GRP LIKE '%SCHYR%'";
            DataTable dt = query(sqlstr, null);
            List<SchoolYear> schoolYears = new List<SchoolYear>();
            SchoolYear schoolYear;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                schoolYear = new SchoolYear();
                Type t = schoolYear.GetType();
                foreach (PropertyInfo info in t.GetProperties())
                {
                    if (!DBNull.Value.Equals(dt.Rows[i][info.Name])) info.SetValue(schoolYear, dt.Rows[i][info.Name]);
                }
                schoolYears.Add(schoolYear);

            }
            return schoolYears;
        }
        public ScholarshipData GetScholarshipData(string fundAcct, string scholarNum)
        {
            string sqlstr = "SELECT * FROM summit.schlrshp s inner join SUMMIT.FUND f ON S.FUND_ACCT=F.FUND_ACCT LEFT OUTER JOIN UHELP.FUND_COLL_ATTRB coll on f.FUND_COLL_ATTRB=coll.FUND_COLL_ATTRB LEFT OUTER JOIN UHELP.FUND_DEPT_ATTRB dept on f.FUND_COLL_ATTRB=dept.FUND_DEPT_ATTRB ";
            sqlstr += "LEFT OUTER JOIN Summit.USER_CODE su on (s.audit_tran_id=su.parent_audit_tran_id and (su.USER_GRP='SCHMJ' or su.USER_GRP='SCHYR'))";
            sqlstr += "LEFT OUTER JOIN UHELP.USER_CD uu on su.USER_CD=UU.USER_CD ";
            sqlstr += " WHERE regexp_like(s.FUND_ACCT, :fundAcct, 'i')  AND regexp_like(s.SCHLRSHP_NUM, :scholarNum, 'i') AND s.SCHLR_USER_VARBL2 = 'Y' and f.FUND_OPEN_ATTRB='O'";
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(new OracleParameter("fundAcct", fundAcct));
            parameters.Add(new OracleParameter("scholarNum", scholarNum));
            ScholarshipData data = new ScholarshipData();
            DataTable dt = query(sqlstr, parameters);
            List<Scholarship> ScholarshipList = new List<Scholarship>();
            Scholarship aScholarship;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                aScholarship = new Scholarship();
                Type t = aScholarship.GetType();
                foreach (FieldInfo info in t.GetFields())
                {
                    if (!DBNull.Value.Equals(dt.Rows[i][info.Name]))
                    {
                        if (info.FieldType == typeof(string))
                        {
                            info.SetValue(aScholarship, dt.Rows[i][info.Name].ToString().Trim());
                        }
                        else
                        {
                            info.SetValue(aScholarship, dt.Rows[i][info.Name]);
                        }
                    }
                    else
                    {
                        if (info.FieldType == typeof(string)) info.SetValue(aScholarship, "");
                    }
                }
                System.Diagnostics.Debug.WriteLine("Row : " + i.ToString() + ":" + aScholarship.FRML_SCHLRSHP_NAME);
                ScholarshipList.Add(aScholarship);
                data.title = aScholarship.FRML_SCHLRSHP_NAME;
                data.purpose = aScholarship.SCHLRSHP_PRPS;
                data.gradGPA = aScholarship.SCHLR_USER_VARBL13;
                data.undergradGPA = aScholarship.SCHLR_USER_VARBL14;
                data.highschoolGPA = aScholarship.SCHLR_USER_VARBL15;
                data.financialneed = aScholarship.SCHLR_USER_VARBL4.Length == 0 ? "" : "<b>Financial Need</b> : This scholarship requires a student to have a Financial Need to be eligible to receive the scholarship. In order to establish \"Financial Need\", a student must file the Free Application for Federal Student Aid (FAFSA). You can file the FAFSA at <a target='_blank' href=\"http://www.fafsa.ed.gov/\">www.fafsa.ed.gov</a>.";
                data.essay = (aScholarship.SCHLR_USER_VARBL11.Length == 0) ? "There is no essay required for this scholarship" : "An Essay is required towards applying for this scholarship";
                data.international = (aScholarship.SCHLR_USER_VARBL3.ToLower().Equals("n")) ? "This scholarship is not open to International Students" : "";
                data.referenceletter = (aScholarship.SCHLR_USER_VARBL31.Length == 0) ? "" : ("<b>Reference Letters : </b>" + aScholarship.SCHLR_USER_VARBL31 + " reference letter(s) needed.");
                if (aScholarship.SCHLR_USER_VARBL32.Length > 0)
                {
                    String deadline = aScholarship.SCHLR_USER_VARBL32;
                    int x = Convert.ToInt16(deadline);
                    String y = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x);
                    data.deadline = "<p><b>Application Deadline: </b>" + y + "</p>";
                }
                else
                {
                    data.deadline = "<p><b>Application Deadline: </b> No deadline has been specified. </p>";
                }
                data.communityservice = aScholarship.SCHLR_USER_VARBL8.Length == 0 ? "" : "<p><b>Community Service: </b>  There is a community service requirement in applying for this scholarship.</p>";
                if (!DBNull.Value.Equals(dt.Rows[i]["FUND_DEPT_DESCR"])) data.department = dt.Rows[i]["FUND_DEPT_DESCR"].ToString();
                else data.department = "";
                if (!DBNull.Value.Equals(dt.Rows[i]["FUND_COLL_DESCR"])) data.college = dt.Rows[i]["FUND_COLL_DESCR"].ToString();
                else data.college = "";
                //if (!DBNull.Value.Equals(dt.Rows[i]["FUND_DEPT_DESC"])) data.communityservice = dt.Rows[i]["FUND_DEPT_DESC"];
                //if (!DBNull.Value.Equals(dt.Rows[i]["FUND_DEPT_DESC"])) data.deadline = dt.Rows[i]["FUND_DEPT_DESC"];

                ///county...
            }
            System.Diagnostics.Debug.WriteLine("sql : " + sqlstr);
            System.Diagnostics.Debug.WriteLine("params : " + fundAcct + ":" + scholarNum);
            System.Diagnostics.Debug.WriteLine("Data : " + data.title);
            return data;

        }
        public List<Scholarship> GetScholarships(SearchObject searchObject)
        {
            //string sqlstr = "SELECT * FROM summit.schlrshp s inner join SUMMIT.FUND f ON S.FUND_ACCT=F.FUND_ACCT WHERE (FRML_SCHLRSHP_NAME like @title )";
            string sqlstr = "SELECT DISTINCT FRML_SCHLRSHP_NAME,s.FUND_ACCT,s.SCHLRSHP_NUM FROM summit.schlrshp s inner join SUMMIT.FUND f ON S.FUND_ACCT=F.FUND_ACCT LEFT OUTER JOIN UHELP.FUND_COLL_ATTRB coll on f.FUND_COLL_ATTRB=coll.FUND_COLL_ATTRB LEFT OUTER JOIN UHELP.FUND_DEPT_ATTRB dept on f.FUND_COLL_ATTRB=dept.FUND_DEPT_ATTRB ";
            sqlstr += "LEFT OUTER JOIN Summit.USER_CODE su on (s.audit_tran_id=su.parent_audit_tran_id and (su.USER_GRP='SCHMJ' or su.USER_GRP='SCHYR'))";
            sqlstr += "LEFT OUTER JOIN UHELP.USER_CD uu on su.USER_CD=UU.USER_CD ";
            sqlstr += " WHERE s.SCHLR_USER_VARBL2 = 'Y' and f.FUND_OPEN_ATTRB='O' AND rownum<3000 "; //explain this number later. choosing 3 cols also allows me have DISTINCT
            //sqlstr += " WHERE FRML_SCHLRSHP_NAME like :title ";// AND s.SCHLR_USER_VARBL2 = 'Y' and f.FUND_OPEN_ATTRB='O'";
            /*  **reduce query if some fields not used
            sqlstr += " and f.FUND_COLL_ATTRB like :college ";
            sqlstr += " and f.FUND_DEPT_ATTRB like :department ";
            sqlstr += " and uu.USER_CD_DESCR like :major and su.USER_GROUP='SCHMJ' ";
            sqlstr += " and uu.USER_CD like :year and su.USER_GROUP='SCHYR'";
            if (college.Equals("-1")) college = "%%";
            if (department.Equals("-1")) department = "%%";
             * 
             * */
            List<OracleParameter> parameters = new List<OracleParameter>();
            if (searchObject.title != null && !searchObject.title.Trim().Equals("")) //decided to allow for empty title after testing performance with toad. satisfactory for 1000 rows if selecting 3 cols
            {
                sqlstr += " AND regexp_like(FRML_SCHLRSHP_NAME, :title, 'i') ";
                parameters.Add(new OracleParameter("title", searchObject.title));
            }

            if (searchObject.college != null && !searchObject.college.Equals("-1"))
            {
                sqlstr += " and f.FUND_COLL_ATTRB like :college or f.FUND_COLL_ATTRB IS NULL OR f.FUND_COLL_ATTRB=''"; //no need regex since we have dropdown for these
                parameters.Add(new OracleParameter("college", searchObject.college));
            }
            if (searchObject.department != null && !searchObject.department.Equals("-1"))
            {
                sqlstr += " and f.FUND_DEPT_ATTRB like :department ";
                parameters.Add(new OracleParameter("department", searchObject.department));
            }
            if (searchObject.schoolYear != null && !searchObject.schoolYear.Equals("-1"))
            {
                sqlstr += " and uu.USER_CD like :year and su.USER_GRP='SCHYR'";
                parameters.Add(new OracleParameter("schoolYear", searchObject.schoolYear));
            }
            if (searchObject.major != null && !searchObject.major.Trim().Equals(""))
            {
                sqlstr += " and regexp_like(uu.USER_CD_DESCR, :major, 'i') and su.USER_GRP='SCHMJ' ";
                parameters.Add(new OracleParameter("major", searchObject.major));
            }
            if (searchObject.undergradGPA != null && !searchObject.undergradGPA.Trim().Equals(""))
            {
                sqlstr += " and CAST(SCHLR_USER_VARBL13 AS number) <= :undergradGPA ";
                parameters.Add(new OracleParameter("undergradGPA", searchObject.undergradGPA));
            }
            if (searchObject.gradGPA != null && !searchObject.gradGPA.Trim().Equals(""))
            {
                sqlstr += " and CAST(SCHLR_USER_VARBL14 AS number) <= :gradGPA ";
                parameters.Add(new OracleParameter("gradGPA", searchObject.gradGPA));
            }
            if (searchObject.highschoolGPA != null && !searchObject.highschoolGPA.Trim().Equals(""))
            {
                sqlstr += " and CAST(SCHLR_USER_VARBL15 AS number) <= :highschoolGPA ";
                parameters.Add(new OracleParameter("highschoolGPA", searchObject.highschoolGPA));
            }

            //OracleParameterCollection parameters = new OracleParameterCollection();
            //OracleParameter[] parameters = new OracleParameter[3];

            DataTable dt = query(sqlstr, parameters);
            List<Scholarship> ScholarshipList = new List<Scholarship>();
            Scholarship aScholarship;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                aScholarship = new Scholarship();
                Type t = aScholarship.GetType();
                //System.Diagnostics.Debug.Write("schFields");
                //System.Diagnostics.Debug.WriteLine(t.ToString());
                //System.Diagnostics.Debug.WriteLine(t.GetFields().Count());
                //System.Diagnostics.Debug.WriteLine(t.GetProperties().Count());

                //var schFields = aScholarship.GetType().GetProperties(); // I called this inspectionReportfields because the entity properties correspond to form/report fields I'm generating from this data.

                /*
                 * remove below 10/16/2014 to select just 3 columns needed for search.
            foreach (FieldInfo info in t.GetFields())
            {
                //var propName=field.Name; 
                //var p = GetType().GetField("aScholarship").GetValue(this);
                //p.GetType().GetProperty(propName).SetValue(p, dt.Rows[i][propName], null);
                if (!DBNull.Value.Equals(dt.Rows[i][info.Name]))
                {
                    if (info.FieldType==typeof(string))
                    {
                        info.SetValue(aScholarship, dt.Rows[i][info.Name].ToString().Trim());
                    }else{
                        info.SetValue(aScholarship, dt.Rows[i][info.Name]);
                    }
                }else{
                    if (info.FieldType == typeof(string)) info.SetValue(aScholarship, "");
                }
                //aScholarship.get
                //GetValue(aScholarship,null);
                    
                //System.Diagnostics.Debug.Write(info.Name);
                //System.Diagnostics.Debug.Write(field);
            }
                 * */
                aScholarship.SCHLRSHP_NUM = dt.Rows[i]["SCHLRSHP_NUM"].ToString().Trim();
                aScholarship.FUND_ACCT = dt.Rows[i]["FUND_ACCT"].ToString().Trim();
                aScholarship.FRML_SCHLRSHP_NAME = dt.Rows[i]["FRML_SCHLRSHP_NAME"].ToString().Trim();
                System.Diagnostics.Debug.WriteLine("Row : " + i.ToString() + ":" + aScholarship.FRML_SCHLRSHP_NAME);
                //System.Diagnostics.Debug.WriteLine("Row : " + i.ToString());
                ScholarshipList.Add(aScholarship);

            }
            return ScholarshipList;
        }

        /* sql server */
        public DataTable querySQLServer(string sqlstr, List<SqlParameter> parameters)
        {

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(SQLConnString)) // connect to oracle
            {
                conn.Open(); // open the oracle connection
                using (SqlCommand comm = new SqlCommand(sqlstr, conn)) // create the oracle sql command
                {
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            comm.Parameters.Add(parameter);
                            System.Diagnostics.Debug.WriteLine("Param and val");
                            System.Diagnostics.Debug.Write(parameter.ParameterName);
                            System.Diagnostics.Debug.Write(parameter.Value);
                        }

                    }
                    System.Diagnostics.Debug.WriteLine(comm.CommandText);
                    using (SqlDataAdapter myadapter = new SqlDataAdapter())
                    {
                        myadapter.SelectCommand = comm;
                        myadapter.Fill(dt);
                    }
                    comm.Dispose();
                }
                conn.Close(); // close the oracle connection
            }
            return dt;
        }
        public int queryExecuteSQLServer(string sqlstr, List<SqlParameter> parameters)
        {

            int result = 0;
            using (SqlConnection conn = new SqlConnection(SQLConnString)) // connect to oracle
            {
                conn.Open(); // open the oracle connection
                using (SqlCommand comm = new SqlCommand(sqlstr, conn)) // create the oracle sql command
                {
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            comm.Parameters.Add(parameter);
                            System.Diagnostics.Debug.WriteLine("Param and val");
                            System.Diagnostics.Debug.Write(parameter.ParameterName);
                            System.Diagnostics.Debug.Write(parameter.Value);
                        }
                    }
                    result = comm.ExecuteNonQuery();
                    comm.Dispose();
                }
                conn.Close(); // close the oracle connection
            }
            return result;
        }

    }

}