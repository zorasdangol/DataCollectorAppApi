using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Dapper;

namespace DataCollectorRestApi
{
    public static class GlobalClass
    {
        public static string DataConnectionString;
        public static string Terminal;
        public static int GraceTime;
        public static string CompanyName;
        public static string CompanyAddress;
        public static decimal VAT = 13;
        public static Exception LastException;
        public static string GetTime = "(SELECT CONVERT(VARCHAR,(SELECT GETDate()),8))";
        public static string GetDate = "(SELECT GETDATE())";
        public static Boolean LATEGRNPOSTING;
        static GlobalClass()
        {
            try
            {
                if (File.Exists(Environment.SystemDirectory + "\\DBSetting.dat"))
                {
                    DataConnectionString = File.ReadAllText(Environment.SystemDirectory + "\\DBSetting.dat");
                }

            }
            catch (Exception ex)
            {
                ProcessError(ex, "");
            }
        }

        public static Exception GetRootException(Exception ex)
        {
            if (ex.InnerException != null)
                ex = GetRootException(ex);
            return ex;
        }

        public static string GetEncryptedPWD(string pwd)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(pwd);
            data = x.ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }
        public static void bindColumns(ref DataTable dtSrc, params string[] columnName)
        {
            for (int i = 0; i < columnName.Length; i++)
            {
                dtSrc.Columns.Add(columnName.GetValue(i).ToString());
            }
        }

        public static object CopyPropertyValues(object source, object Destination)
        {
            if (source.GetType() != Destination.GetType())
                return null;
            foreach (PropertyInfo pi in source.GetType().GetProperties())
            {
                if (pi.CanWrite)
                    Destination.GetType().GetProperty(pi.Name).SetValue(Destination, pi.GetValue(source, null), null);
            }
            return Destination;
        }

        public static void ProcessError(Exception ex, string Source)
        {
            StreamWriter fs;
            string Gaps = "                    ";
            string Log;
            string HResult;
            string ExceptionType;
            string Message;
            string user;
            try
            {
                ex = GetRootException(ex);
                HResult = (Marshal.GetHRForException(ex).ToString() + Gaps).Substring(0, 20);
                ExceptionType = (ex.GetType().Name + Gaps + Gaps).Substring(0, 40);
                Message = (ex.Message + Gaps + Gaps + Gaps + Gaps + Gaps + Gaps + Gaps + Gaps + Gaps + Gaps).Substring(0, 200);
                Log = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt") + "  " + ExceptionType + "  " + HResult + "  " + Message;
                if (!File.Exists(Environment.CurrentDirectory + "\\ErrorLog.Log"))
                {
                    File.Create(Environment.CurrentDirectory + "\\ErrorLog.Log").Close();
                }
                fs = File.AppendText(Environment.CurrentDirectory + "\\ErrorLog.Log");
                fs.WriteLine(Log);
                fs.Close();
                LastException = ex;
            }
            catch
            {
                throw;
            }
        }

        public static string GetServerSequence(SqlCommand cmd, string VNAME, string DIVISION, string Prefix)
        {
            try
            {
                cmd.CommandText = "SELECT CURNO FROM RMD_SEQUENCES WHERE VNAME = '" + VNAME + "' AND DIVISION ='" + DIVISION + "'";
                object vchr = cmd.ExecuteScalar();
                if (vchr != null)
                    return Prefix + vchr.ToString();
                else
                {
                    cmd.CommandText = "INSERT INTO RMD_SEQUENCES(VNAME, CurNo, DIVISION) VALUES ('" + VNAME + "',1,'" + DIVISION + "')";
                    cmd.ExecuteNonQuery();
                    return Prefix + "1";
                    //cmd.CommandText = "SELECT CURNO FROM RMD_SEQUENCES WHERE VNAME = '" + VNAME + "'";
                    //vchr = cmd.ExecuteScalar();
                    //if (vchr != null)
                    //    return Prefix + vchr.ToString();
                }
            }
            catch (Exception ex)
            {
                GlobalClass.writeErrorToExternalFile(ex.Message,"GetServerSequence");
                return null;
            }
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static void writeErrorToExternalFile(string errorMessage, string SOURCE,string DIVISION = "", string TABLE = "")
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DataConnectionString))
                {
                    con.Execute(@"IF not exists(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DATACOLLECTORAPI_ERRORS')
                                        CREATE TABLE DATACOLLECTORAPI_ERRORS(
                                            DATE datetime,
                                            DIVISION varchar(20) NULL,
                                            [TABLE] VARCHAR(50),
                                            [SOURCE] VARCHAR(100),
											ERRORMESSAGE VARCHAR(MAX)
											)");
                    con.Execute("INSERT INTO DATACOLLECTORAPI_ERRORS (DATE,DIVISION,[TABLE],[SOURCE],ERRORMESSAGE) VALUES(@DATE,@DIVISION,@TABLE,@SOURCE,@ERRORMESSAGE)", new { DATE = DateTime.Now, DIVISION = DIVISION, TABLE = TABLE, ERRORMESSAGE = errorMessage, SOURCE = SOURCE });
                }

            }
            catch (Exception ex)
            {
                //if (File.Exists(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "masterSyncErrors.txt")))
                //{
                //    using (StreamWriter sw = new StreamWriter(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "masterSyncErrors.txt"), true))
                //    {
                //        sw.WriteLine(" Date : " + DateTime.Now + "  Error : " + ex.Message);
                //    }
                //}
            }
        }
    }
}

