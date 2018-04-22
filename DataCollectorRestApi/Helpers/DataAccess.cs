using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollectorRestApi
{
    public class DataAccess : IDisposable
    {
        /// <summary>
        /// Returns dataTable as per selectCommand passed
        /// </summary>
        /// <param name="strSQL">Select Command string to retrieve data </param>
        /// <param name="tblName">Table name</param>
        /// <returns>Populated Data table  </returns>
        public DataTable getData(string strSQL, SqlConnection conn, string tblName = "Template")
        {
            SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
            DataTable dt = new DataTable(tblName);
            da.Fill(dt);
            return dt;
        }

        public DataTable getData(string strSQL, SqlConnection conn, SqlTransaction trn, string tblName = "Template")
        {
            SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
            da.SelectCommand.Transaction = trn;
            DataTable dt = new DataTable(tblName);
            da.Fill(dt);
            return dt;
        }

        public DataTable getData(SqlCommand cmd, string tblName = "Template")
        {
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable(tblName);
            da.Fill(dt);
            return dt;
        }

        public DataTable getData(string strSQL, string tblName, SqlConnection conn, params string[] parameters)
        {
            SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
            for (int i = 0; i < parameters.Count(); i = i + 2)
                da.SelectCommand.Parameters.AddWithValue(parameters[i], parameters[i + 1]);
            DataTable dt = new DataTable(tblName);
            da.Fill(dt);
            return dt;
        }

        public object getScalarData(string strSQL, SqlConnection conn)
        {
            SqlCommand cmdScalar = new SqlCommand(strSQL, conn);
            return cmdScalar.ExecuteScalar();
        }
        public void executeNonQuery(string strSql, SqlTransaction trn, SqlConnection conn)
        {
            {
                SqlCommand command = new SqlCommand(strSql, conn, trn);
                command.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {

        }
    }
}
