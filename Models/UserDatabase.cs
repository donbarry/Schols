using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;

namespace Schols.Models
{
    public class UserDatabase
    {
        public UserModel ValidUser(UserModel user)
        {
            DBObject db = new DBObject();

            string sqlstr = "SELECT passwordhash,salt,fullname,usermajor FROM users where username= @username";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@username", user.username));
            DataTable dt = db.querySQLServer(sqlstr, parameters);
            if (dt.Rows.Count == 0)
            {
                user.accesstoken = "";
                user.username = "";
                return user;
            }
            string storedHash = dt.Rows[0]["passwordhash"].ToString();
            string storedSalt = dt.Rows[0]["salt"].ToString();
            string fullname = dt.Rows[0]["fullname"].ToString();
            string usermajor = dt.Rows[0]["usermajor"].ToString();
            string inputHash = CreatePasswordHash(user.userpassword, storedSalt);
            if (storedHash.Equals(inputHash))
            {
                user.accesstoken = generateToken(user);
                user.fullname = fullname;
                user.usermajor = usermajor;
                return user;
            }
            else
            {
                user.accesstoken = "";
                user.username = "";
                return user;
            }
        }
        public bool UserExists(UserModel user)
        {
            DBObject db = new DBObject();

            String sqlstr = "SELECT * FROM users where username= @username";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@username", user.username));
            DataTable dt = db.querySQLServer(sqlstr, parameters);
            if (dt.Rows.Count == 0)
                return false;
            else
                return true;
        }
        private UserModel GetUser(UserModel user)
        {
            DBObject db = new DBObject();
            String sqlstr = "SELECT * FROM users where username= @username";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@username", user.username));
            DataTable dt = db.querySQLServer(sqlstr, parameters);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                user.fullname = dt.Rows[0]["fullname"].ToString();
                user.usermajor = dt.Rows[0]["usermajor"].ToString();
                System.Diagnostics.Debug.WriteLine(user.fullname);
                System.Diagnostics.Debug.WriteLine(user.username);
                return user;
            }
        }

        public string RegisterUser(UserModel user)
        {
            if (UserExists(user))
            {
                return "User Exists Already";
            }
            DBObject db = new DBObject();
            String sqlstr = "INSERT INTO users (username,passwordhash,salt,fullname,usermajor) VALUES (@username, @passwordhash,@salt,@fullname,@usermajor)";
            string salt = CreateSalt(4);
            string passwordhash = CreatePasswordHash(user.userpassword, salt);
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@username", user.username));
            parameters.Add(new SqlParameter("@passwordhash", passwordhash));
            parameters.Add(new SqlParameter("@salt", salt));
            parameters.Add(new SqlParameter("@fullname", user.fullname));
            parameters.Add(new SqlParameter("@usermajor", user.usermajor));
            int count = db.queryExecuteSQLServer(sqlstr, parameters);
            if (count == 1)
            {
                return "";
            }
            else
            {
                return "Could not create user.";
            }
        }
        public string AddFavorite(string fundAcct, UserModel user)
        {
            string sqlstr = "SELECT * FROM favorites WHERE username=@username AND fund_acct=@fundacct";
            string message = "";
            DBObject db = new DBObject();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@username", user.username));
            parameters.Add(new SqlParameter("@fundacct",fundAcct));
            DataTable dt = db.querySQLServer(sqlstr, parameters);
            if (dt.Rows.Count == 0)
            {
                sqlstr = "INSERT INTO favorites (username,fund_acct) VALUES (@username,@fundacct)";
                List<SqlParameter> insertParameters = new List<SqlParameter>();
                insertParameters.Add(new SqlParameter("@username", user.username));
                insertParameters.Add(new SqlParameter("@fundacct", fundAcct));
                //same parameter list applies
                int count = db.queryExecuteSQLServer(sqlstr, insertParameters);
                message = "Added to Favorites";
            }
            else
            {
                message = "Already exists in favorites";
            }
            return message;
        }
        public string generateToken(UserModel user)
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            DBObject db = new DBObject();
            String sqlstr = "INSERT INTO tokens (username,accesstoken,granted) VALUES (@username, @accesstoken,@granted)";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@username", user.username));
            parameters.Add(new SqlParameter("@accesstoken", token));
            parameters.Add(new SqlParameter("@granted", DateTime.Now));
            int count = db.queryExecuteSQLServer(sqlstr, parameters);
            return token;
        }
        public static string generateTokenNoDB(UserModel user)
        {
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return token;
        }
        public UserModel CheckToken(string token)
        {
            DBObject db = new DBObject();
            string sqlstr = "SELECT username FROM tokens WHERE accesstoken= @accesstoken"; //and granted...
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@accesstoken", token));
            DataTable dt = db.querySQLServer(sqlstr, parameters);
            if (dt.Rows.Count == 0)
                return null;
            else
            {
                UserModel user = new UserModel();
                user.username = dt.Rows[0]["username"].ToString();
                return GetUser(user); // dt.Rows[0]["username"].ToString();
            }

        }
        private string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        private string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            byte[] saltAndPwdbytes = System.Text.Encoding.Unicode.GetBytes(saltAndPwd);
            var sha = new SHA1Managed();
            string hashedPwd = Convert.ToBase64String(sha.ComputeHash(Convert.FromBase64String(Convert.ToBase64String(saltAndPwdbytes)))); //FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");
            return hashedPwd;
        }

    }
}