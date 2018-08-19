using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataCollectorRestApi.Models;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace DataCollectorRestApi.Controllers
{
    [Produces("application/json")]
    
    public class BranchInDataController : Controller
    {
        public string remarks { get; set; }

        [Route("api/SaveBranchInSync")]
        [HttpPost]
        //[HttpGet("{dataCollect}")]
        public FunctionResponse<List<BranchInMaster>> saveBranchInSync([FromBody]List<BranchInMaster> BranchInMasterList)
        {
            try
            {
                foreach (var item in BranchInMasterList)
                {
                    var vchrNo = SaveBranchInMaster(item);
                    if (vchrNo == "success")
                    {
                        //item.BranchInMain.refNo = vchrNo;
                        item.BranchInMain.IsSaved = true;
                        item.BranchInMain.IsUpload = false;
                    }
                    else if(vchrNo == "no")
                    {
                        item.BranchInMain.IsSaved = false;
                        item.BranchInMain.IsUpload = false;
                        item.BranchInMain.remarks = this.remarks;
                    }
                }
                return new FunctionResponse<List<BranchInMaster>>() { status = "ok", result = BranchInMasterList };
            }
            catch (Exception e)
            {
                return new FunctionResponse<List<BranchInMaster>>() { status = "error", Message = e.Message };
            }
        }
        
        [Route("api/getBranchInProd")]
        [HttpPost]
        //[HttpGet("{dataCollect}")]
        public FunctionResponse<List<BranchInItem>> LoadBranchInProd([FromBody]BranchInDetail BranchInDetail)
        {
            try
            {
                List<BranchInItem> BranchInItemList = new List<BranchInItem>();
                using (SqlConnection conn = new SqlConnection(GlobalClass.DataConnectionString))
                {
                    var found = new List<BranchInDetail>(conn.Query<BranchInDetail>("Select  * from RMD_TRNMain where vchrno = '" + BranchInDetail.chalanNo + "' and billTo = '" + BranchInDetail.billToAdd + "' and billToAdd = '" + BranchInDetail.billTo + "'" ));
                    if(found.Count != 0)
                    {                        
                        BranchInItemList = new List<BranchInItem>(conn.Query<BranchInItem>("select * from RMD_TRNPROD where vchrno = '" + BranchInDetail.chalanNo + "' and division = '" + BranchInDetail.billToAdd + "'" ));                        
                    }
                }
                return new FunctionResponse<List<BranchInItem>>() { status = "ok", result = BranchInItemList };
            }
            catch (Exception e)
            {
                return new FunctionResponse<List<BranchInItem>>() { status = "error", Message = e.Message };
            }
        }


        public string SaveBranchInMaster(BranchInMaster BranchInMaster)
        {
            SqlTransaction trn;

            //LoadBranchTransfer[] lbtJson;
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

            //lbtJson = bOutDataCollect;
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

                    var branchTransferMain = BranchInMaster.BranchInMain;

                    vchrNo = branchTransferMain.vchrNo;
                    division = branchTransferMain.division;
                    chalanNo = branchTransferMain.chalanNo.ToUpper();
                    trnDate = branchTransferMain.trnDate.ToString("MM/dd/yyyy");
                    trnAc = branchTransferMain.trnAc;
                    ParAc = branchTransferMain.ParAc;
                    trnMode = branchTransferMain.trnMode;
                    refOrdBill = branchTransferMain.refOrdBill;
                    remarks = branchTransferMain.remarks;
                    wareHouse = branchTransferMain.wareHouse;
                    isTaxInvoice = branchTransferMain.isTaxInvoice;
                    userName = branchTransferMain.userName;
                    billTo = branchTransferMain.billTo;
                    billToAdd = branchTransferMain.billToAdd;

                    foreach (var lbt in BranchInMaster.BranchInProdList)
                    {
                        mcode.Add(lbt.mcode);
                        barcode.Add(lbt.barcode);
                        quantity.Add(lbt.quantity);
                        rate.Add(lbt.rate);
                        expDate.Add(lbt.expDate.ToString("MM/dd/yyyy"));
                        unit.Add(lbt.unit);
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
                    var ins = cmd.ExecuteNonQuery();


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
                        cmd.Parameters.AddWithValue("@DIVISION", billToAdd);
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
                    GlobalClass.writeErrorToExternalFile(SqlEx.Message, "SaveGrnMaster");
                    this.remarks = SqlEx.Message;

                    if (trn.Connection != null)
                        trn.Rollback();
                    return "no";
                }
            }
        }
    }
}