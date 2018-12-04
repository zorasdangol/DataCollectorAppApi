using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Text;

namespace DataCollectorRestApi.Controllers
{
    [Produces("application/json")]

    public class DataCollectController : Controller
    {
        DataAccess db = new DataAccess();
        DataTable dt, ddt;
        SqlTransaction trn;


        [Route("api/getDateTime")]
        [HttpGet]
        public string getDateTime()
        {
            string SERVERDATE;
            string SERVERTIME;
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                try
                {
                    var now = (DateTime)db.getScalarData("SELECT GETDATE() DATE", cnMain);
                    SERVERDATE = now.ToString("yyyy-MM-dd");
                    SERVERTIME = now.ToString("hh:mm tt");

                    return SERVERDATE + "<;>" + SERVERTIME;
                }
                catch (Exception)
                {
                    return "no";
                }
            }
        }

        [Route("api/validateUser")]
        [HttpGet("{password}")]
        public string validateUser(string password)
        {
            string username = "";
            string encPassword;
            string key = "AmitLalJoshi";
            encPassword = Encrypt(password, key);
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();
                try
                {
                    dt = db.getData("SELECT UNAME FROM USERPROFILES WHERE PASSWORD='" + encPassword + "'", cnMain);
                    if (dt.Rows.Count > 0)
                    {
                        username = dt.Rows[0]["UNAME"].ToString();
                        return username;
                    }
                    else
                    {
                        return "Empty";
                    }

                }
                catch (Exception)
                {
                    return "no";
                }
            }
        }

        public static string Encrypt(string txtValue, string Key)
        {
            int i;
            string TextChar;
            string KeyChar;
            long Encrypted;
            string retMsg = "";
            int ind = 1;

            for (i = 1; i <= Convert.ToInt32(txtValue.Length); i++)
            {
                TextChar = txtValue.Substring(i - 1, 1);
                ind = i % Key.Length;
                //if(ind==0)
                //{
                //KeyChar = Key.Substring(0);
                //}
                //else
                //{
                KeyChar = Key.Substring((ind));
                //}

                //Encrypted = Convert.ToInt32(Encoding.ASCII.(TextChar)) ^ Convert.ToInt32(Encoding.ASCII.GetBytes(KeyChar));
                byte str1 = Encoding.Default.GetBytes(TextChar)[0];
                byte str2 = Encoding.Default.GetBytes(KeyChar)[0];
                //Encrypted = str1 ^ str2;
                var encData = str1 ^ str2;
                retMsg = retMsg + Convert.ToChar(encData).ToString();
            }
            return retMsg;
        }


        //static string Encrypt(string Text, string Key)
        //{
        //    int i;
        //    string TEXTCHAR;
        //    string KEYCHAR;
        //    string encoded = string.Empty;
        //    for (i = 0; i < Text.Length; i++)
        //    {
        //        TEXTCHAR = Text.Substring(i, 1);
        //        var keysI = ((i + 1) % Key.Length);
        //        KEYCHAR = Key.Substring(keysI);
        //        var encrypted = Microsoft.VisualBasic.Strings.Asc(TEXTCHAR) ^ Microsoft.VisualBasic.Strings.Asc(KEYCHAR);
        //        encoded += Microsoft.VisualBasic.Strings.Chr(encrypted);
        //    }
        //    return encoded;
        //}


        public IEnumerable<Dictionary<string, object>> DataTableToDictionary(System.Data.DataTable dataTable)
        {

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Dictionary<string, object> ret = new Dictionary<string, object>();
                for (int c = 0; c < dataTable.Columns.Count; c++)
                {
                    ret.Add(dataTable.Columns[c].Caption, dataTable.Rows[i][c]);
                }
                yield return ret;
            }
        }


        [Route("api/getUserProfile")]
        [HttpGet]
        public ObjectResult getUserProfile()
        {
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();
                try
                {
                    dt = db.getData("SELECT UNAME, PASSWORD, ROLE FROM USERPROFILES", cnMain);
                    var retDt = DataTableToDictionary(dt);
                    return new OkObjectResult(retDt);
                }
                catch (Exception Es)
                {
                    return new NotFoundObjectResult(Es);
                }
            }
        }

        [Route("api/getMenuitem")]
        [HttpGet("{settingIndex}")]
        public ObjectResult getMenuitem(String settingIndex)
        {
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();

                try
                {
                    if (settingIndex == "1")
                    {
                        dt = db.getData("SELECT ISNULL(MCODE,'') MCODE,ISNULL(MENUCODE,'') MENUCODE,ISNULL(DESCA,'') DESCA,ISNULL(PARENT,'') PARENT,ISNULL(VAT,0) VAT,ISNULL(TYPE,'') TYPE, ISNULL(SUPCODE,'') SUPCODE, ISNULL(BASEUNIT,'PC') BASEUNIT,ISNULL(RATE_A,0) RATE_A, ISNULL(PRATE_A,0) PRATE_A,ISNULL(MGROUP,'') MGROUP, ISNULL(CONVERT(VARCHAR,EDATE,101),'') EDATE,'' CRDATE FROM MENUITEM WHERE TYPE = 'A'", cnMain);
                        //dt = db.getData("SELECT MENUCODE AS MCODE, MENUCODE, DESCRIPTION AS DESCA, PARENT,'' TYPE,'' SUPCODE,'' BASEUNIT,'' RATE_A,'' PRATE_A,'' MGROUP, '' EDATE,'' CRDATE  FROM MENUITEM_PART", cnMain);
                    }
                    else
                    {
                        dt = db.getData("SELECT ISNULL(MCODE,'') MCODE,ISNULL(MENUCODE,'') MENUCODE,ISNULL(DESCA,'') DESCA,ISNULL(PARENT,'') PARENT,ISNULL(VAT,0) VAT,ISNULL(TYPE,'') TYPE, ISNULL(SUPCODE,'') SUPCODE, ISNULL(BASEUNIT,'PC') BASEUNIT,ISNULL(RATE_A,0) RATE_A, ISNULL(PRATE_A,0) PRATE_A,ISNULL(MGROUP,'') MGROUP, ISNULL(CONVERT(VARCHAR,EDATE,101),'') EDATE,'' CRDATE FROM MENUITEM WHERE TYPE = 'A'", cnMain);
                    }

                    var retdt = DataTableToDictionary(dt);
                    return new OkObjectResult(retdt);
                }
                catch (Exception E)
                {
                    return new NotFoundObjectResult( E);
                }
            }
        }

        [Route("api/getBarcode")]
        [HttpGet("{settingIndex}")]
        public ObjectResult getBarcode(String settingIndex)
        {
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();

                try
                {//SELECT B.BCODE,B.MCODE,A.ACNAME,B.UNIT,B.SUPCODE FROM BARCODE B LEFT JOIN RMD_ACLIST A ON B.SUPCODE = A.ACID 

                    if (settingIndex == "1")
                    {
                        dt = db.getData("SELECT ISNULL(BCODE,'') BCODE,ISNULL(MCODE,'') MCODE,'' SUPCODE,'' EDATE,'' EXPIRY,'' REMARKS FROM BarCode", cnMain);
                    }
                    else
                    {
                        dt = db.getData("SELECT ISNULL(BCODE,'') BCODE,ISNULL(MCODE,'') MCODE, ISNULL(SUPCODE,'') SUPCODE, ISNULL(CONVERT(VARCHAR,EDATE,101),'') EDATE, ISNULL(CONVERT(VARCHAR,EXPIRY,101),'') EXPIRY,REMARKS FROM BarCode", cnMain);
                    }

                    var retdt = DataTableToDictionary(dt);
                    return new OkObjectResult(retdt);
                }
                catch (Exception E)
                {
                    return new NotFoundObjectResult( E);
                }
            }
        }

        [Route("api/getBarcodeChunk")]
        [HttpGet("{settingIndex},{skip},{limit}")]
        public ObjectResult getBarcodeChunk(string settingIndex, string skip, string limit)
        {
            int skips = 0; int limits = 0;
            int.TryParse(skip, out skips);
            int.TryParse(limit, out limits);

            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();

                try
                {//SELECT B.BCODE,B.MCODE,A.ACNAME,B.UNIT,B.SUPCODE FROM BARCODE B LEFT JOIN RMD_ACLIST A ON B.SUPCODE = A.ACID 
                    string sql;
                    if (settingIndex == "1")
                    {
                        sql = @";WITH Results_CTE AS
                                    (
                                        SELECT
                                            BCODE, MCODE, '' SUPCODE, '' EDATE, '' EXPIRY, '' REMARKS,
                                            ROW_NUMBER() OVER(ORDER BY MCODE, BCODE) AS RowNum
                                        FROM Barcode

                                    )
                                    SELECT*
                                    FROM Results_CTE
                                    WHERE RowNum >= " + skips.ToString() + @"
                                    AND RowNum <  " + (skips + limits).ToString();
                        dt = db.getData(sql, cnMain);
                    }
                    else
                    {
                        sql = @";WITH Results_CTE AS
                                    (
                                        Select
                                        BCODE, MCODE, ISNULL(SUPCODE,'') SUPCODE, ISNULL(CONVERT(VARCHAR,EDATE,101),'') EDATE, ISNULL(CONVERT(VARCHAR,EXPIRY,101),'') EXPIRY,REMARKS,
                                            ROW_NUMBER() OVER(ORDER BY MCODE, BCODE) AS RowNum
                                        FROM Barcode

                                    )
                                    SELECT*
                                    FROM Results_CTE
                                    WHERE RowNum >= " + skips.ToString() + @"
                                    AND RowNum <  " + (skips + limits).ToString();
                        dt = db.getData(sql, cnMain);
                    }

                    var tbl = DataTableToDictionary(dt);
                    return new OkObjectResult(tbl);

                    //return JsonConvert.SerializeObject(tbl, Formatting.Indented);
                }
                catch (Exception Ex)
                {
                    return new NotFoundObjectResult(Ex);
                }
            }
        }


        //[Route("api/getWareHouse")]
        //[HttpGet("{settingIndex}")]
        //public ObjectResult getWareHouse(String settingIndex)
        //{
        //    using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
        //    //using (SqlConnection cnMain = new SqlConnection(constr))
        //    {
        //        cnMain.Open();
        //        dt = new DataTable();

        //        try
        //        {
        //            if (settingIndex == "1")
        //            {
        //                dt = db.getData("SELECT NAME, ADDRESS, PHONE, REMARKS, ISDEFAULT,IsAdjustment,AdjustmentAcc,ISVIRTUAL,VIRTUAL_PARENT,'' DIVISION FROM RMD_WAREHOUSE", cnMain);
        //            }
        //            else
        //            {
        //                dt = db.getData("SELECT NAME, ADDRESS, PHONE, REMARKS, ISDEFAULT,IsAdjustment,AdjustmentAcc,ISVIRTUAL,VIRTUAL_PARENT, DIVISION FROM RMD_WAREHOUSE", cnMain);
        //            }
        //            var retdt = DataTableToDictionary(dt);
        //            List<Dictionary<string,Object>> retDict  = new List<Dictionary<string,object>>() ;
        //            foreach(var dic in retdt)
        //            {
        //                retDict.Add(dic);
        //            }
        //            return new OkObjectResult(retDict);
        //        }
        //        catch (Exception E)
        //        {
        //            return new NotFoundObjectResult(E);
        //        }
        //    }
        //}
        
        [Route("api/getwarehouse")]
        [HttpGet]
        public ObjectResult getwarehouse()
        {
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();
                try
                {

                    //dt = db.getData("SELECT NAME, ADDRESS, PHONE, REMARKS, ISDEFAULT,IsAdjustment,AdjustmentAcc,ISVIRTUAL,VIRTUAL_PARENT,'' DIVISION FROM RMD_WAREHOUSE", cnMain);
                               
                    dt = db.getData("SELECT NAME, ISNULL(ADDRESS,'') ADDRESS, ISNULL(DIVISION,'') DIVISION FROM RMD_WAREHOUSE", cnMain);

                    var retdt = DataTableToDictionary(dt);
                    return new OkObjectResult(retdt);
                }
                catch (Exception E)
                {
                    return new NotFoundObjectResult(E);
                }
            }
        }

        [Route("api/getAcList")]
        [HttpGet]
        public ObjectResult getAcList()
        {
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();
                try
                {
                    dt = db.getData("SELECT ACID,ISNULL(ACNAME,'') ACNAME,ISNULL(PARENT,'') PARENT,ISNULL(ACCODE,'') ACCODE, ISNULL(CONVERT(VARCHAR,EDATE,101),'') EDATE, ISNULL(PTYPE,'') PTYPE FROM RMD_ACLIST", cnMain);
                    var retdt = DataTableToDictionary(dt);
                    return new OkObjectResult(retdt);
                }
                catch (Exception E)
                {
                    return  new NotFoundObjectResult( E);
                }
            }
        }

        [Route("api/getOrderProd")]
        [HttpGet("{settingIndex}")]
        public ObjectResult getOrderProd(String settingIndex)
        {
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();

                try
                {
                    if (settingIndex == "1")
                    {
                        dt = db.getData("SELECT distinct OP.VCHRNO, ISNULL(TM.TRNAC, '') SUPPLIERCODE FROM RMD_ORDERPROD OP INNER JOIN RMD_TRNMAIN TM ON OP.VCHRNO = TM.VCHRNO WHERE VoucherStatus IS NULL order by vchrno", cnMain);
                                                
                        //dt = db.getData("SELECT OP.VCHRNO, OP.MCODE,'' BARCODE, OP.QUANTITY, OP.RATE, ISNULL(TM.TRNAC,'') SUPPLIERCODE FROM RMD_ORDERPROD OP INNER JOIN RMD_TRNMAIN TM ON OP.VCHRNO = TM.VCHRNO", cnMain);
                    }
                    else
                    {
                        dt = db.getData("SELECT distinct OP.VCHRNO, ISNULL(TM.TRNAC, '') SUPPLIERCODE FROM RMD_ORDERPROD OP INNER JOIN RMD_TRNMAIN TM ON OP.VCHRNO = TM.VCHRNO WHERE VoucherStatus IS NULL order by vchrno", cnMain);

                        //dt = db.getData("SELECT OP.VCHRNO, OP.MCODE,ISNULL(OP.BC,'') BARCODE, OP.QUANTITY, OP.RATE, ISNULL(TM.TRNAC,'') SUPPLIERCODE FROM RMD_ORDERPROD OP INNER JOIN RMD_TRNMAIN TM ON OP.VCHRNO = TM.VCHRNO WHERE VoucherStatus IS NULL", cnMain);
                    }
                    var retdt = DataTableToDictionary(dt);
                    return new OkObjectResult(retdt);
                }
                catch (Exception E)
                {
                    return new NotFoundObjectResult(E);
                }
            }
        }


        [Route("api/getDivision")]
        [HttpGet]
        public ObjectResult getDivision()
        {
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();
                try
                {
                    dt = db.getData("SELECT INITIAL, ISNULL(NAME,'') NAME FROM DIVISION", cnMain);
                    var retdt = DataTableToDictionary(dt);
                    return new OkObjectResult(retdt);
                }
                catch (Exception E)
                {
                    return new NotFoundObjectResult(E);
                }
            }
        }

        [Route("api/getLocation")]
        [HttpGet]
        public ObjectResult getLocation()
        {
            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();
                try
                {
                    dt = db.getData("SELECT LOC_ID, ISNULL(NAME,'') NAME,ISNULL(WAREHOUSE,'') WAREHOUSE,ISNULL(PARENT,'') PARENT,ISNULL(LEVELS, '') LEVELS FROM RMD_LOCATION", cnMain);
                    var retdt = DataTableToDictionary(dt);
                    return new OkObjectResult(retdt);
                }
                catch (Exception E)
                {
                    return new NotFoundObjectResult(E);
                }
            }
        }
                
        [Route("api/saveDataCollect")]
        [HttpPost]
        //[HttpGet("{dataCollect}")]
        public string saveDataCollect([FromBody]List<LoadDataCollect> dataCollect)
        {
            List<LoadDataCollect> ldcJson;
            SqlCommand cmd = new SqlCommand();
            string TNO = "";
            string batch = "";
            string menucode = "";
            string sheetNo = "0";
            string mcode = "";
            string qty = "";
            string wareHouse = "";
            string trnDate = "";
            string bcode = "";
            string trnUser = "";
            string trnTime = "";
            string division = "";
            string location = "";
            //string timeStamp = "";

            ldcJson = dataCollect;
            //ldcJson = JsonConvert.DeserializeObject<LoadDataCollect[]>(dataCollect);

            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            {
                cnMain.Open();
                dt = new DataTable();
                ddt = new DataTable();
                trn = cnMain.BeginTransaction();
                try
                {
                    dt = db.getData("SELECT CurNo FROM RMD_SEQUENCES WHERE VNAME='MANUALSTOCK'", cnMain, trn);
                    TNO = dt.Rows[0][0].ToString();
                    
                    foreach (LoadDataCollect ldc in ldcJson)
                    {
                        batch = ldc.sid;
                        menucode = ldc.menucode;
                        mcode = ldc.mcode;
                        qty = ldc.qty;
                        wareHouse = ldc.warehouse;
                        trnTime = ldc.trnTime;
                        trnDate = ldc.trnDate;
                        bcode = ldc.bcode;
                        trnUser = ldc.trnUser;
                        division = ldc.division;
                        location = ldc.location;
                        
                        db.executeNonQuery("INSERT INTO MANUALSTOCKS (TNO, BATCH, SHEETNO, MCODE, MENUCODE, QTY, WAREHOUSE, TRNTIME, TRNDATE, BCODE, TRNUSER, DIVISION, LOCATION) SELECT '" + TNO
                            + "','" + batch + "', '" + sheetNo + "', '" + mcode + "', '" + menucode
                            + "', '" + qty + "', '" + wareHouse + "', '" + trnTime + "', '" + trnDate
                            + "', '" + bcode + "','" + trnUser + "','" + division + "', '" + location + "'", trn, cnMain);


                        /*   db.executeNonQuery("INSERT INTO MANUALSTOCKS (TNO, BATCH, SHEETNO, MCODE, MENUCODE, QTY, WAREHOUSE, TRNTIME, TRNDATE, BCODE, TRNUSER, DIVISION) SELECT '" + TNO + "','" + batch + "', '" + sheetNo + "', '" + mcode + "', '" + menucode + "', '" + qty + "', '" + wareHouse + "', '" + DateTime.Now.ToString("h:mm:ss tt")
                               + "', convert(datetime,convert(varchar,GETDATE(),106)), '" + bcode + "','" + trnUser + "'", trn, cnMain);*/


                    }

                    db.executeNonQuery("UPDATE RMD_SEQUENCES SET CURNO=CURNO + 1 WHERE VNAME ='MANUALSTOCK'", trn, cnMain);
                    trn.Commit();
                    return TNO;
                }
                catch (Exception ex)
                {
                    GlobalClass.writeErrorToExternalFile(ex.Message, "SaveDatacollect");
                 
                    trn.Rollback();
                    return "no";
                }
            }
        }

        [Route("api/saveGrnData")]
        [HttpPost]
        //[HttpGet("{grnDataCollect}")]
        public string saveGrnData([FromBody]LoadGrnCollect[] grnDataCollect)
        {
            LoadGrnCollect[] lgcJson;
            string SERVERDATE;
            string SERVERTIME;
            string VCHRNO;
            string vchrNo = "", division = "", chalanNo = "", trnDate = "", trnAc = "", ParAc = "", trnMode = "",
            refOrdBill = "", remarks = "", wareHouse = "", isTaxInvoice = "";
            List<string> mcode = new List<string>();
            List<string> barcode = new List<string>();
            List<string> quantity = new List<string>();
            List<string> rate = new List<string>();
            List<string> expDate = new List<string>();
            List<string> unit = new List<string>();
            string userName = "";

            decimal AMOUNT = 0, totAmount = 0;
            decimal SRATE = 0, totSRate = 0;
            decimal totVat = 0, totTaxable = 0, totNonTaxable = 0;
            List<decimal> VAT = new List<decimal>();
            List<decimal> TAXABLE = new List<decimal>();
            List<decimal> NONTAXABLE = new List<decimal>();
            decimal DISCOUNT = 0;

            lgcJson = grnDataCollect;
            //lgcJson = JsonConvert.DeserializeObject<LoadGrnCollect[]>(grnDataCollect);

            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            using (SqlCommand cmd = cnMain.CreateCommand())
            using (SqlCommand cmdGetItemInfo = cnMain.CreateCommand())
            {
                cnMain.Open();
                trn = cnMain.BeginTransaction();
                try
                {
                    cmd.Transaction = trn;
                    cmdGetItemInfo.Transaction = trn;
                    cmd.CommandText = "SELECT GETDATE()";

                    var now = (DateTime)cmd.ExecuteScalar();
                    SERVERDATE = now.ToString("MM/dd/yyyy");
                    SERVERTIME = now.ToString("hh:mm tt");


                    foreach (LoadGrnCollect lgc in lgcJson)
                    {
                        if (lgc.mcode == null)
                        {
                            vchrNo = lgc.vchrNo;
                            division = lgc.division;
                            chalanNo = lgc.chalanNo;
                            trnDate = lgc.trnDate;
                            trnAc = lgc.trnAc;
                            ParAc = lgc.ParAc;
                            trnMode = lgc.trnMode;
                            refOrdBill = lgc.trnMode;
                            remarks = lgc.remarks;
                            wareHouse = lgc.wareHouse;
                            isTaxInvoice = lgc.isTaxInvoice;
                            userName = lgc.userName;
                        }
                        else
                        {
                            mcode.Add(lgc.mcode);
                            barcode.Add(lgc.barcode);
                            quantity.Add(lgc.quantity);
                            rate.Add(lgc.rate);
                            expDate.Add(lgc.expDate);
                            unit.Add(lgc.unit);
                        }
                    }

                    /*cmd.CommandText = "SELECT CURNO FROM RMD_SEQUENCES WHERE VNAME = 'Purchase' AND DIVISION ='" + division + "'";
                    object vchr = cmd.ExecuteScalar();
                    if (vchr != null)
                    {
                        VCHRNO = "PI" + vchr.ToString();
                    }
                    else
                    {
                        cmd.CommandText = "SELECT CURNO FROM RMD_SEQUENCES WHERE VNAME = 'Purchase' AND DIVISION ='" + division + "'";
                        VCHRNO = "PI" + cmd.ExecuteScalar().ToString();
                    }*/

                    VCHRNO = GlobalClass.GetServerSequence(cmd, "Purchase", division, "PI");


                    for (int i = 0; i < mcode.Count; i++)
                    {
                        cmdGetItemInfo.CommandText = "SELECT CONVERT(VARCHAR,RATE_A) + ':' + CONVERT(VARCHAR,VAT) FROM MENUITEM WHERE MCODE = '" + mcode[i] + "'";
                        string[] parameters = cmdGetItemInfo.ExecuteScalar().ToString().Split(new char[] { ':' });
                        AMOUNT = Convert.ToDecimal(quantity[i]) * Convert.ToDecimal(rate[i]);
                        totAmount += AMOUNT;
                        SRATE = decimal.Parse(parameters[0]);
                        totSRate += SRATE;
                        if (isTaxInvoice == "1" && parameters[1] == "1")
                        {
                            VAT.Add(AMOUNT * (decimal)GlobalClass.VAT / 100);
                            totVat += AMOUNT * (decimal)GlobalClass.VAT / 100;
                            TAXABLE.Add(AMOUNT - DISCOUNT);
                            totTaxable += AMOUNT - DISCOUNT;
                        }
                        else
                        {
                            NONTAXABLE.Add(AMOUNT - DISCOUNT);
                            totNonTaxable += AMOUNT - DISCOUNT;
                        }
                    }

                    cmd.CommandText = "SP_TRNMAIN_ENTRY_GRN";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VCHRNO", VCHRNO);
                    cmd.Parameters.AddWithValue("@BILLNO", chalanNo);
                    cmd.Parameters.AddWithValue("@REFNO", vchrNo);
                    cmd.Parameters.AddWithValue("@DATE", now.Date);
                    cmd.Parameters.AddWithValue("@TIME", SERVERTIME);
                    cmd.Parameters.AddWithValue("@TRNAC", trnAc);
                    cmd.Parameters.AddWithValue("@PARAC", ParAc);
                    cmd.Parameters.AddWithValue("@TRNMODE", trnMode);
                    cmd.Parameters.AddWithValue("@GROSS", totAmount);
                    cmd.Parameters.AddWithValue("@DISCOUNT", DISCOUNT);
                    cmd.Parameters.AddWithValue("@VAT", totVat);
                    cmd.Parameters.AddWithValue("@NET", (totAmount + totVat));
                    cmd.Parameters.AddWithValue("@REMARKS", remarks);
                    cmd.Parameters.AddWithValue("@USER", userName);
                    cmd.Parameters.AddWithValue("@DIV", division);
                    cmd.Parameters.AddWithValue("@TAXABLE", totTaxable);
                    cmd.Parameters.AddWithValue("@NONTAXABLE", totNonTaxable);
                    cmd.Parameters.AddWithValue("@PONUMBER", refOrdBill);
                    cmd.ExecuteNonQuery();
                    
                    cmd.CommandText = "SP_TRNPROD_ENTRY_GRN";
                    for (int i = 0; i < mcode.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@VCHRNO", VCHRNO);
                        cmd.Parameters.AddWithValue("@REFNO", DBNull.Value);
                        cmd.Parameters.AddWithValue("@MCODE", mcode[i]);
                        cmd.Parameters.AddWithValue("@UNIT", unit[i]);
                        cmd.Parameters.AddWithValue("@QTY", quantity[i]);
                        cmd.Parameters.AddWithValue("@WAREHOUSE", wareHouse);
                        cmd.Parameters.AddWithValue("@RATE", rate[i]);
                        cmd.Parameters.AddWithValue("@SPRICE", SRATE);
                        cmd.Parameters.AddWithValue("@AMOUNT", AMOUNT);
                        cmd.Parameters.AddWithValue("@DISCOUNT", DISCOUNT);
                        cmd.Parameters.AddWithValue("@VAT", VAT.Count == 0 ? 0 : VAT[i]);
                        cmd.Parameters.AddWithValue("@DIVISION", division);
                        cmd.Parameters.AddWithValue("@SUPPLIER", ParAc);
                        cmd.Parameters.AddWithValue("@TAXABLE", TAXABLE.Count == 0 ? 0 : TAXABLE[i]);
                        cmd.Parameters.AddWithValue("@NONTAXABLE", NONTAXABLE.Count == 0 ? 0 : NONTAXABLE[i]);
                        cmd.Parameters.AddWithValue("@BC", barcode[i]);
                        cmd.Parameters.AddWithValue("@SNO", i + 1);
                        cmd.Parameters.AddWithValue("@EXPDATE", expDate[i]);//(Convert.ChangeType(expDate, List<DateTime>) == new DateTime()) ? (object)DateTime.Parse(expDate[i]).ToString("MM/dd/yyyy") : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISTEMP", GlobalClass.LATEGRNPOSTING);
                        cmd.ExecuteNonQuery();
                    }


                    cmd.Parameters.Clear();
                    cmd.CommandText = "SP_TRNTRAN_ENTRY_GRN";
                    cmd.Parameters.AddWithValue("@VCHR", VCHRNO);
                    cmd.Parameters.AddWithValue("@REFNO", DBNull.Value);
                    cmd.Parameters.AddWithValue("@GAMNT", totAmount);
                    cmd.Parameters.AddWithValue("@DAMNT", DISCOUNT);
                    cmd.Parameters.AddWithValue("@VAMNT", totVat);
                    cmd.Parameters.AddWithValue("@TRNAC", trnAc);
                    cmd.Parameters.AddWithValue("@NAR", remarks);
                    cmd.Parameters.AddWithValue("@DIVISION", division);
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE RMD_SEQUENCES SET CURNO = CURNO + 1 WHERE  VNAME = 'Purchase' AND DIVISION ='" + division + "'";
                    cmd.ExecuteNonQuery();
                    trn.Commit();
                    return VCHRNO;
                }
                catch (Exception e)
                {
                    trn.Rollback();
                    return "no";
                }
            }
        }

        [Route("api/savePrnData")]
        [HttpPost]
        //[HttpGet("{prnDataCollect}")]
        public string savePrnData([FromBody]LoadGrnCollect[] prnDataCollect)
        {
            LoadGrnCollect[] lgcJson;
            string SERVERDATE;
            string SERVERTIME;
            string VCHRNO;
            string vchrNo = "", division = "", chalanNo = "", trnDate = "", trnAc = "", ParAc = "", trnMode = "",
            refOrdBill = "", remarks = "", wareHouse = "", isTaxInvoice = "";
            List<string> mcode = new List<string>();
            List<string> barcode = new List<string>();
            List<string> quantity = new List<string>();
            List<string> rate = new List<string>();
            List<string> expDate = new List<string>();
            List<string> unit = new List<string>();
            string userName = "";

            decimal AMOUNT = 0, totAmount = 0;
            decimal SRATE = 0, totSRate = 0;
            decimal totVat = 0, totTaxable = 0, totNonTaxable = 0;
            List<decimal> VAT = new List<decimal>();
            List<decimal> TAXABLE = new List<decimal>();
            List<decimal> NONTAXABLE = new List<decimal>();
            decimal DISCOUNT = 0;

            lgcJson = prnDataCollect;
            //lgcJson = JsonConvert.DeserializeObject<LoadGrnCollect[]>(prnDataCollect);

            using (SqlConnection cnMain = new SqlConnection(GlobalClass.DataConnectionString))
            //using (SqlConnection cnMain = new SqlConnection(constr))
            using (SqlCommand cmd = cnMain.CreateCommand())
            using (SqlCommand cmdGetItemInfo = cnMain.CreateCommand())
            {
                cnMain.Open();
                trn = cnMain.BeginTransaction();
                try
                {
                    cmd.Transaction = trn;
                    cmdGetItemInfo.Transaction = trn;
                    cmd.CommandText = "SELECT GETDATE()";

                    var now = (DateTime)cmd.ExecuteScalar();
                    SERVERDATE = now.ToString("MM/dd/yyyy");
                    SERVERTIME = now.ToString("hh:mm tt");


                    foreach (LoadGrnCollect lgc in lgcJson)
                    {
                        if (lgc.mcode == null)
                        {
                            vchrNo = lgc.vchrNo;
                            division = lgc.division;
                            chalanNo = lgc.chalanNo;
                            trnDate = lgc.trnDate;
                            trnAc = lgc.trnAc;
                            ParAc = lgc.ParAc;
                            trnMode = lgc.trnMode;
                            refOrdBill = lgc.trnMode;
                            remarks = lgc.remarks;
                            wareHouse = lgc.wareHouse;
                            isTaxInvoice = lgc.isTaxInvoice;
                            userName = lgc.userName;
                        }
                        else
                        {
                            mcode.Add(lgc.mcode);
                            barcode.Add(lgc.barcode);
                            quantity.Add(lgc.quantity);
                            rate.Add(lgc.rate);
                            expDate.Add(lgc.expDate);
                            unit.Add(lgc.unit);
                        }
                    }

                    /*cmd.CommandText = "SELECT CURNO FROM RMD_SEQUENCES WHERE VNAME = 'PurchaseReturn' AND DIV ='" + division + "'";
                    object vchr = cmd.ExecuteScalar();
                    if (vchr != null)
                    {
                        VCHRNO = "PR" + vchr.ToString();
                    }
                    else
                    {
                        cmd.CommandText = "SELECT CURNO FROM RMD_SEQUENCES WHERE VNAME = 'PurchaseReturn' AND DIV ='" + division + "'";
                        VCHRNO = "PR" + cmd.ExecuteScalar().ToString();
                    }*/

                    VCHRNO = GlobalClass.GetServerSequence(cmd, "PurchaseReturn", division, "PR");

                    for (int i = 0; i < mcode.Count; i++)
                    {
                        cmdGetItemInfo.CommandText = "SELECT CONVERT(VARCHAR,RATE_A) + ':' + CONVERT(VARCHAR,VAT) FROM MENUITEM WHERE MCODE = '" + mcode[i] + "'";
                        string[] parameters = cmdGetItemInfo.ExecuteScalar().ToString().Split(new char[] { ':' });
                        AMOUNT = Convert.ToDecimal(quantity[i]) * Convert.ToDecimal(rate[i]);
                        totAmount += AMOUNT;
                        SRATE = decimal.Parse(parameters[0]);
                        totSRate += SRATE;
                        if (isTaxInvoice == "1" && parameters[1] == "1")
                        {
                            VAT.Add(AMOUNT * (decimal)GlobalClass.VAT / 100);
                            totVat += AMOUNT * (decimal)GlobalClass.VAT / 100;
                            TAXABLE.Add(AMOUNT - DISCOUNT);
                            totTaxable += AMOUNT - DISCOUNT;
                        }
                        else
                        {
                            NONTAXABLE.Add(AMOUNT - DISCOUNT);
                            totNonTaxable += AMOUNT - DISCOUNT;
                        }
                    }

                    cmd.CommandText = "SP_TRNMAIN_ENTRY_GRN";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VCHRNO", VCHRNO);
                    cmd.Parameters.AddWithValue("@BILLNO", VCHRNO);
                    cmd.Parameters.AddWithValue("@REFNO", vchrNo);
                    cmd.Parameters.AddWithValue("@DATE", now.Date);
                    cmd.Parameters.AddWithValue("@TIME", SERVERTIME);
                    cmd.Parameters.AddWithValue("@TRNAC", trnAc);
                    cmd.Parameters.AddWithValue("@PARAC", ParAc);
                    cmd.Parameters.AddWithValue("@TRNMODE", trnMode);
                    cmd.Parameters.AddWithValue("@GROSS", totAmount);
                    cmd.Parameters.AddWithValue("@DISCOUNT", DISCOUNT);
                    cmd.Parameters.AddWithValue("@VAT", totVat);
                    cmd.Parameters.AddWithValue("@NET", (totAmount + totVat));
                    cmd.Parameters.AddWithValue("@REMARKS", remarks);
                    cmd.Parameters.AddWithValue("@USER", userName);
                    cmd.Parameters.AddWithValue("@DIV", division);
                    cmd.Parameters.AddWithValue("@TAXABLE", totTaxable);
                    cmd.Parameters.AddWithValue("@NONTAXABLE", totNonTaxable);
                    cmd.Parameters.AddWithValue("@PONUMBER", refOrdBill);
                    cmd.ExecuteNonQuery();
                    
                    cmd.CommandText = "SP_TRNPROD_ENTRY_PR";
                    for (int i = 0; i < mcode.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@VCHRNO", VCHRNO);
                        cmd.Parameters.AddWithValue("@REFNO", DBNull.Value);
                        cmd.Parameters.AddWithValue("@MCODE", mcode[i]);
                        cmd.Parameters.AddWithValue("@UNIT", unit[i]);
                        cmd.Parameters.AddWithValue("@QTY", quantity[i]);
                        cmd.Parameters.AddWithValue("@WAREHOUSE", wareHouse);
                        cmd.Parameters.AddWithValue("@RATE", rate[i]);
                        cmd.Parameters.AddWithValue("@SPRICE", SRATE);
                        cmd.Parameters.AddWithValue("@AMOUNT", AMOUNT);
                        cmd.Parameters.AddWithValue("@DISCOUNT", DISCOUNT);
                        cmd.Parameters.AddWithValue("@VAT", VAT.Count == 0 ? 0 : VAT[i]);
                        cmd.Parameters.AddWithValue("@DIVISION", division);
                        cmd.Parameters.AddWithValue("@SUPPLIER", ParAc);
                        cmd.Parameters.AddWithValue("@TAXABLE", TAXABLE.Count == 0 ? 0 : TAXABLE[i]);
                        cmd.Parameters.AddWithValue("@NONTAXABLE", NONTAXABLE.Count == 0 ? 0 : NONTAXABLE[i]);
                        cmd.Parameters.AddWithValue("@BC", barcode[i]);
                        cmd.Parameters.AddWithValue("@SNO", i + 1);
                        cmd.Parameters.AddWithValue("@EXPDATE", expDate[i]);//(Convert.ChangeType(expDate, List<DateTime>) == new DateTime()) ? (object)DateTime.Parse(expDate[i]).ToString("MM/dd/yyyy") : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISTEMP", GlobalClass.LATEGRNPOSTING);
                        cmd.ExecuteNonQuery();
                    }


                    cmd.Parameters.Clear();
                    cmd.CommandText = "SP_TRNTRAN_ENTRY_PR";
                    cmd.Parameters.AddWithValue("@VCHR", VCHRNO);
                    cmd.Parameters.AddWithValue("@REFNO", DBNull.Value);
                    cmd.Parameters.AddWithValue("@GAMNT", totAmount);
                    cmd.Parameters.AddWithValue("@DAMNT", DISCOUNT);
                    cmd.Parameters.AddWithValue("@VAMNT", totVat);
                    cmd.Parameters.AddWithValue("@TRNAC", trnAc);
                    cmd.Parameters.AddWithValue("@NAR", remarks);
                    cmd.Parameters.AddWithValue("@DIVISION", division);
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE RMD_SEQUENCES SET CURNO = CURNO + 1 WHERE  VNAME = 'PurchaseReturn' AND DIVISION ='" + division + "'";
                    cmd.ExecuteNonQuery();
                    trn.Commit();
                    return VCHRNO;
                }
                catch (Exception e)
                {
                    trn.Rollback();
                    return "no";
                }
            }
        }


        [Route("api/saveBoutData")]
        //[HttpGet("{bOutDataCollect}")]
        [HttpPost]
        public string saveBoutData([FromBody]LoadBranchTransfer[] bOutDataCollect)
        {
            LoadBranchTransfer[] lbtJson;
            string SERVERDATE;
            string SERVERTIME;
            string VCHRNO;
            string vchrNo = "", division = "", chalanNo = "", trnDate = "", trnAc = "", ParAc = "", trnMode = "",
            refOrdBill = "", remarks = "", wareHouse = "", isTaxInvoice = "", billTo = "", billToAdd = "";
            List<string> mcode = new List<string>();
            List<string> barcode = new List<string>();
            List<string> quantity = new List<string>();
            List<string> rate = new List<string>();
            List<string> expDate = new List<string>();
            List<string> unit = new List<string>();
            string userName = "Admin";

            decimal AMOUNT = 0, totAmount = 0;
            decimal SRATE = 0, totSRate = 0;
            decimal totVat = 0, totTaxable = 0, totNonTaxable = 0;
            List<decimal> VAT = new List<decimal>();
            List<decimal> TAXABLE = new List<decimal>();
            List<decimal> NONTAXABLE = new List<decimal>();
            decimal DISCOUNT = 0;

            lbtJson = bOutDataCollect;
            //lbtJson = JsonConvert.DeserializeObject<LoadBranchTransfer[]>(bOutDataCollect);

            using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            using (SqlCommand cmdGetItemInfo = conn.CreateCommand())
            {
                conn.Open();
                trn = conn.BeginTransaction();
                try
                {

                    cmd.Transaction = trn;
                    cmdGetItemInfo.Transaction = trn;
                    cmd.CommandText = "SELECT GETDATE()";

                    var now = (DateTime)cmd.ExecuteScalar();
                    SERVERDATE = now.ToString("MM/dd/yyyy");
                    SERVERTIME = now.ToString("hh:mm tt");

                    foreach (LoadBranchTransfer lbt in lbtJson)
                    {
                        if (lbt.mcode == null)
                        {
                            vchrNo = lbt.vchrNo;
                            division = lbt.division;
                            chalanNo = lbt.chalanNo;
                            trnDate = lbt.trnDate;
                            trnAc = lbt.trnAc;
                            ParAc = lbt.ParAc;
                            trnMode = lbt.trnMode;
                            refOrdBill = lbt.refOrdBill;
                            remarks = lbt.remarks;
                            wareHouse = lbt.wareHouse;
                            isTaxInvoice = lbt.isTaxInvoice;
                            userName = lbt.userName;
                            billTo = lbt.billTo;
                            billToAdd = lbt.billToAdd;
                        }
                        else
                        {
                            mcode.Add(lbt.mcode);
                            barcode.Add(lbt.barcode);
                            quantity.Add(lbt.quantity);
                            rate.Add(lbt.rate);
                            expDate.Add(lbt.expDate);
                            unit.Add(lbt.unit);
                        }
                    }

                    VCHRNO = GlobalClass.GetServerSequence(cmd, "BranchTransfer", division, "TO");

                    for (int i = 0; i < mcode.Count; i++)
                    {
                        cmdGetItemInfo.CommandText = "SELECT CONVERT(VARCHAR,RATE_A) + ':' + CONVERT(VARCHAR,VAT) FROM MENUITEM WHERE MCODE = '" + mcode[i] + "'";
                        string[] parameters = cmdGetItemInfo.ExecuteScalar().ToString().Split(new char[] { ':' });
                        AMOUNT = Convert.ToDecimal(quantity[i]) * Convert.ToDecimal(rate[i]);
                        totAmount += AMOUNT;
                        SRATE = decimal.Parse(parameters[0]);
                        totSRate += SRATE;
                        if (isTaxInvoice == "1" && parameters[1] == "1")
                        {
                            VAT.Add(AMOUNT * (decimal)GlobalClass.VAT / 100);
                            totVat += AMOUNT * (decimal)GlobalClass.VAT / 100;
                            TAXABLE.Add(AMOUNT - DISCOUNT);
                            totTaxable += AMOUNT - DISCOUNT;
                        }
                        else
                        {
                            NONTAXABLE.Add(AMOUNT - DISCOUNT);
                            totNonTaxable += AMOUNT - DISCOUNT;
                        }
                    }

                    cmd.CommandText = "SP_TRNMAIN_ENTRY_BRANCH_TRANSFER";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VCHRNO", VCHRNO);
                    cmd.Parameters.AddWithValue("@DATE", now.Date);
                    cmd.Parameters.AddWithValue("@TIME", SERVERTIME);
                    cmd.Parameters.AddWithValue("@GROSS", totAmount);
                    cmd.Parameters.AddWithValue("@NET", totAmount + totVat);
                    cmd.Parameters.AddWithValue("@REMARKS", remarks);
                    cmd.Parameters.AddWithValue("@USER", userName);
                    cmd.Parameters.AddWithValue("@DIV", division);
                    cmd.Parameters.AddWithValue("@BILLTO", billTo);
                    cmd.Parameters.AddWithValue("@BILLTOADD", billToAdd);
                    cmd.ExecuteNonQuery();


                    cmd.CommandText = "SP_TRNPROD_ENTRY_BRANCH_TRANSFER";
                    for (int i = 0; i < mcode.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@VCHRNO", VCHRNO);
                        cmd.Parameters.AddWithValue("@MCODE", mcode[i]);
                        cmd.Parameters.AddWithValue("@UNIT", unit[i]);
                        cmd.Parameters.AddWithValue("@QTY", quantity[i]);
                        cmd.Parameters.AddWithValue("@WAREHOUSE", wareHouse);
                        cmd.Parameters.AddWithValue("@RATE", rate[i]);
                        cmd.Parameters.AddWithValue("@AMOUNT", AMOUNT);
                        cmd.Parameters.AddWithValue("@DIVISION", division);
                        cmd.Parameters.AddWithValue("@BC", barcode[i]);
                        cmd.Parameters.AddWithValue("@SNO", i + 1);
                        cmd.ExecuteNonQuery();
                    }

                    cmd.Parameters.Clear();

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE RMD_SEQUENCES SET CURNO = CURNO + 1 WHERE  VNAME = 'BranchTransfer' AND DIVISION ='" + division + "'";
                    cmd.ExecuteNonQuery();
                    trn.Commit();

                    /*using (SqlCeConnection connce = new SqlCeConnection(GlobalClass.LocalDBConnectionString))
                    using (SqlCeCommand cmdCe = connce.CreateCommand())
                    {
                        connce.Open();
                        cmdCe.CommandText = "UPDATE RMD_TRNMAIN SET UPLOADED = 1, COMDOCNO = '" + VCHRNO + "' WHERE VCHRNO ='" + TMMAIN.VCHRNO + "'";
                        cmdCe.ExecuteNonQuery();
                    }*/
                    return VCHRNO;
                }
                catch (SqlException SqlEx)
                {
                    //MessageBox.Show("Data saved to Local Database but upload to server failed", "Manual Stock Take", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    //  MessageBox.Show(SqlEx.Number + ":" + SqlEx.Message, "Branch Transfer", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    if (trn.Connection != null)
                        trn.Rollback();
                    return "no";
                }
            }
        }
     
        [Route("api/saveBinData")]        
        //[HttpGet("{bInDataCollect}")]
        [HttpPost]
        public string saveBinData([FromBody]LoadBranchTransfer[] bInDataCollect)
        {
            LoadBranchTransfer[] lbtJson;
            string SERVERDATE;
            string SERVERTIME;
            string VCHRNO;
            string vchrNo = "", division = "", chalanNo = "", trnDate = "", trnAc = "", ParAc = "", trnMode = "",
            refOrdBill = "", remarks = "", wareHouse = "", isTaxInvoice = "", billTo = "", billToAdd = "";
            List<string> mcode = new List<string>();
            List<string> barcode = new List<string>();
            List<string> quantity = new List<string>();
            List<string> rate = new List<string>();
            List<string> expDate = new List<string>();
            List<string> unit = new List<string>();
            string userName = "";

            decimal AMOUNT = 0, totAmount = 0;
            decimal SRATE = 0, totSRate = 0;
            decimal totVat = 0, totTaxable = 0, totNonTaxable = 0;
            List<decimal> VAT = new List<decimal>();
            List<decimal> TAXABLE = new List<decimal>();
            List<decimal> NONTAXABLE = new List<decimal>();
            decimal DISCOUNT = 0;

            lbtJson = bInDataCollect;

            //lbtJson = JsonConvert.DeserializeObject<LoadBranchTransfer[]>(bInDataCollect);


            using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            using (SqlCommand cmdGetItemInfo = conn.CreateCommand())
            {
                conn.Open();
                trn = conn.BeginTransaction();
                try
                {

                    cmd.Transaction = trn;
                    cmdGetItemInfo.Transaction = trn;
                    cmd.CommandText = "SELECT GETDATE()";

                    var now = (DateTime)cmd.ExecuteScalar();
                    SERVERDATE = now.ToString("MM/dd/yyyy");
                    SERVERTIME = now.ToString("hh:mm tt");

                    foreach (LoadBranchTransfer lbt in lbtJson)
                    {
                        if (lbt.mcode == null)
                        {
                            vchrNo = lbt.vchrNo;
                            division = lbt.division;
                            chalanNo = lbt.chalanNo;
                            trnDate = lbt.trnDate;
                            trnAc = lbt.trnAc;
                            ParAc = lbt.ParAc;
                            trnMode = lbt.trnMode;
                            refOrdBill = lbt.trnMode;
                            remarks = lbt.remarks;
                            wareHouse = lbt.wareHouse;
                            isTaxInvoice = lbt.isTaxInvoice;
                            userName = lbt.userName;
                            billTo = lbt.billTo;
                            billToAdd = lbt.billToAdd;
                        }
                        else
                        {
                            mcode.Add(lbt.mcode);
                            barcode.Add(lbt.barcode);
                            quantity.Add(lbt.quantity);
                            rate.Add(lbt.rate);
                            expDate.Add(lbt.expDate);
                            unit.Add(lbt.unit);
                        }
                    }

                    for (int i = 0; i < mcode.Count; i++)
                    {
                        cmdGetItemInfo.CommandText = "SELECT CONVERT(VARCHAR,RATE_A) + ':' + CONVERT(VARCHAR,VAT) FROM MENUITEM WHERE MCODE = '" + mcode[i] + "'";
                        string[] parameters = cmdGetItemInfo.ExecuteScalar().ToString().Split(new char[] { ':' });
                        AMOUNT = Convert.ToDecimal(quantity[i]) * Convert.ToDecimal(rate[i]);
                        totAmount += AMOUNT;
                        SRATE = decimal.Parse(parameters[0]);
                        totSRate += SRATE;
                        if (isTaxInvoice == "1" && parameters[1] == "1")
                        {
                            VAT.Add(AMOUNT * (decimal)GlobalClass.VAT / 100);
                            totVat += AMOUNT * (decimal)GlobalClass.VAT / 100;
                            TAXABLE.Add(AMOUNT - DISCOUNT);
                            totTaxable += AMOUNT - DISCOUNT;
                        }
                        else
                        {
                            NONTAXABLE.Add(AMOUNT - DISCOUNT);
                            totNonTaxable += AMOUNT - DISCOUNT;
                        }
                    }

                    cmd.CommandText = "SP_TRNMAIN_ENTRY_BRANCH_TRANSFER";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VCHRNO", chalanNo);
                    cmd.Parameters.AddWithValue("@TMODE", true);
                    cmd.Parameters.AddWithValue("@DATE", now.Date);
                    cmd.Parameters.AddWithValue("@TIME", SERVERTIME);
                    cmd.Parameters.AddWithValue("@GROSS", totAmount);
                    cmd.Parameters.AddWithValue("@NET", totAmount + totVat);
                    cmd.Parameters.AddWithValue("@REMARKS", remarks);
                    cmd.Parameters.AddWithValue("@USER", userName);
                    cmd.Parameters.AddWithValue("@DIV", division);
                    cmd.Parameters.AddWithValue("@BILLTO", billTo);
                    cmd.Parameters.AddWithValue("@BILLTOADD", billToAdd);
                    cmd.ExecuteNonQuery();


                    cmd.CommandText = "SP_TRNPROD_ENTRY_BRANCH_TRANSFER";
                    for (int i = 0; i < mcode.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@VCHRNO", chalanNo);
                        cmd.Parameters.AddWithValue("@TMODE", true);
                        cmd.Parameters.AddWithValue("@MCODE", mcode[i]);
                        cmd.Parameters.AddWithValue("@UNIT", unit[i]);
                        cmd.Parameters.AddWithValue("@QTY", quantity[i]);
                        cmd.Parameters.AddWithValue("@WAREHOUSE", wareHouse);
                        cmd.Parameters.AddWithValue("@RATE", rate[i]);
                        cmd.Parameters.AddWithValue("@AMOUNT", AMOUNT);
                        cmd.Parameters.AddWithValue("@DIVISION", division);
                        cmd.Parameters.AddWithValue("@BC", barcode[i]);
                        cmd.Parameters.AddWithValue("@SNO", i + 1);
                        cmd.ExecuteNonQuery();
                    }

                    cmd.Parameters.Clear();

                    //cmd.CommandType = CommandType.Text;
                    //cmd.CommandText = "UPDATE RMD_SEQUENCES SET CURNO = CURNO + 1 WHERE  VNAME = 'BTRansferIn' AND DIVISION ='" + TMMAIN.DIVISION + "'";
                    //cmd.ExecuteNonQuery();
                    trn.Commit();
                    return "success";
                    /*  using (SqlCeConnection connce = new SqlCeConnection(GlobalClass.DataConnectionString))
                      using (SqlCeCommand cmdCe = connce.CreateCommand())
                      {
                          connce.Open();
                          cmdCe.CommandText = "UPDATE RMD_TRNMAIN SET UPLOADED = 1 WHERE VCHRNO ='" + TMMAIN.VCHRNO + "'";
                          cmdCe.ExecuteNonQuery();
                      }
                      MessageBox.Show("Data Successfully Uploaded.", "Branch Transfer", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                      this.Close();*/
                }
                catch (SqlException SqlEx)
                {
                    //MessageBox.Show("Data saved to Local Database but upload to server failed", "Manual Stock Take", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    //MessageBox.Show(SqlEx.Number + ":" + SqlEx.Message, "Branch Transfer", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    if (trn.Connection != null)
                        trn.Rollback();
                    return "no";
                }
            }
        }
    }
}
