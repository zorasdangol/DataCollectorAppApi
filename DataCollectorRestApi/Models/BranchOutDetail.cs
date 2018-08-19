using System;
using System.Collections.Generic;
using System.Text;

namespace DataCollectorRestApi.Models
{
    public class BranchOutDetail : BranchTransferDetail
    {

    }

    public class BranchOutItem : BranchItem { 
    }


    public class BranchOutMaster
    {
        public BranchOutDetail BranchOutMain { get; set; }
        public List<BranchOutItem> BranchOutProdList { get; set; }

        public BranchOutMaster()
        {
            BranchOutMain = new BranchOutDetail();
            BranchOutProdList = new List<BranchOutItem>();
        }
    }

    //public class BranchOutDetail
    //{
    //    public int curNo { get; set; }
    //    public string vchrNo { get; set; }
    //    public string division { get; set; }
    //    public string divisionTo { get; set; }

    //    public DateTime trnDate { get; set; }
    //    public string wareHouse { get; set; }
    //    public string remarks { get; set; }

    //    public BranchOutDetail()
    //    {
    //        curNo = 0;
    //        vchrNo = "";
    //        division = "";
    //        divisionTo = "";
    //        trnDate = DateTime.Today;
    //        wareHouse = "";
    //        remarks = "";
    //    }

    //}

    //public class BranchOutItem
    //{
    //    public int ind { get; set; }
    //    public string vchrNo { get; set; }
    //    public string division { get; set; }
    //    public string divisionTo { get; set; }
    //    public string mcode { get; set; }
    //    public string barcode { get; set; }
    //    public string quantity { get; set; }
    //    public string rate { get; set; }
    //    public string userName { get; set; }
    //    public string unit { get; set; }
    //    public string billToAdd { get; set; }

    //    public string desca { get; set; }

    //    public void SetBranchOutItem(BranchOutItem BranchOutItem)
    //    {
    //        try
    //        {
    //            ind = BranchOutItem.ind;
    //            vchrNo = BranchOutItem.vchrNo;
    //            division = BranchOutItem.division;
    //            mcode = BranchOutItem.mcode;
    //            barcode = BranchOutItem.barcode;
    //            quantity = BranchOutItem.quantity;
    //            rate = BranchOutItem.rate;
    //            userName = BranchOutItem.userName;
    //            unit = BranchOutItem.unit;

    //            desca = BranchOutItem.desca;
    //        }
    //        catch { }

    //    }

    //    public void SetInitialBranchOutItem(BranchOutDetail BranchOutDetail)
    //    {
    //        try
    //        {
    //            this.vchrNo = BranchOutDetail.vchrNo;
    //            this.division = BranchOutDetail.division;
    //            this.divisionTo = BranchOutDetail.divisionTo;
    //        }
    //        catch { }
    //    }

    //}  




}
