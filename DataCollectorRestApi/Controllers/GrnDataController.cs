using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataCollectorRestApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace DataCollectorRestApi.Controllers
{
    [Produces("application/json")]
    public class GrnDataController : Controller
    {
        public string remarks { get; set; }

        [Route("api/SaveGrnSync")]
        [HttpPost]
        //[HttpGet("{dataCollect}")]
        public FunctionResponse<List<GrnMaster>> saveGrnSync([FromBody]List<GrnMaster> GrnMasterList)
        {
            try
            {
                foreach(var item in GrnMasterList)
                {
                    var vchrNo = SaveGrnMaster(item);
                    if(vchrNo != "no")
                    {
                        item.GrnMain.refNo = vchrNo;
                        item.GrnMain.IsSaved = true;
                        item.GrnMain.IsUpload = false;
                    }
                    else 
                    {
                        item.GrnMain.remarks = this.remarks;
                    }
                }
                return new FunctionResponse<List<GrnMaster>>() { status = "ok", result = GrnMasterList };
            }
            catch (Exception e)
            {
                return new FunctionResponse<List<GrnMaster>>() { status = "error", Message = e.Message };
            }
        }

        public string SaveGrnMaster(GrnMaster GrnMaster)
        {
            SqlTransaction trn;
            //LoadGrnCollect[] lgcJson;
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

                    vchrNo = GrnMaster.GrnMain.vchrNo;
                    division = GrnMaster.GrnMain.division;
                    chalanNo = GrnMaster.GrnMain.chalanNo;
                    trnDate = GrnMaster.GrnMain.trnDate.ToString("MM/dd/yyyy");
                    trnAc = GrnMaster.GrnMain.trnAc;
                    ParAc = GrnMaster.GrnMain.ParAc;
                    trnMode = GrnMaster.GrnMain.trnMode;
                    refOrdBill = GrnMaster.GrnMain.refOrdBill;
                    remarks = GrnMaster.GrnMain.remarks;
                    wareHouse = GrnMaster.GrnMain.wareHouse;
                    isTaxInvoice = GrnMaster.GrnMain.isTaxInvoice;
                    userName = GrnMaster.GrnMain.userName;

                    foreach(var item in GrnMaster.GrnProdList)
                    {
                        mcode.Add(item.mcode);
                        barcode.Add(item.barcode);
                        quantity.Add(item.quantity);
                        rate.Add(item.rate);
                        expDate.Add(item.expDate.ToString("MM/dd/yyyy"));
                        unit.Add(item.unit);
                    }                
                    
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
                        //cmd.Parameters.AddWithValue("@ISTEMP", GlobalClass.LATEGRNPOSTING);
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
                    GlobalClass.writeErrorToExternalFile(e.Message,"SaveGrnMaster");
                    this.remarks = e.Message;
                    trn.Rollback();
                    return "no";
                }
            }
        }
        
    }
}