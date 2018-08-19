using System;
using System.Collections.Generic;
using System.Text;

namespace DataCollectorRestApi.Models
{
    public class BranchTransferDetail 
    {
        public int curNo { get; set; }
        public string vchrNo { get; set; }
        public string division { get; set; }
        public string billTo { get; set; }
        //public string divisionFrom { get; set; }
        public string billToAdd { get; set; }

        public DateTime trnDate { get; set; }
        public string wareHouse { get; set; }
        public string remarks { get; set; }

        private bool _IsUpload;
        public bool IsUpload
        {
            get { return _IsUpload; }
            set { _IsUpload = value; }
        }

        public bool IsSaved { get; set; }
        public string refNo { get; set; }

        public string userName { get; set; }

        public string chalanNo { get; set; }
        public string trnAc { get; set; }
        public string ParAc { get; set; }
        public string trnMode { get; set; }
        public string refOrdBill { get; set; }
        public string isTaxInvoice { get; set; }



        public BranchTransferDetail()
        {
            curNo = 0;
            vchrNo = "";
            division = "";
            billTo = "";
            //divisionFrom = "";
            billToAdd = "";
            trnDate = DateTime.Today;
            wareHouse = "";
            remarks = "";

            IsUpload = false;
            IsSaved = false;
            userName = "";
            refNo = "";

            chalanNo = "";
            trnAc = "";
            ParAc = "";
            trnMode = "Cash";
            refOrdBill = "";
            isTaxInvoice = "";




        }
    }

    public class BranchItem
    {
        public int ind { get; set; }
        public string vchrNo { get; set; }
        public string division { get; set; }
        public string billTo { get; set; }
        //public string divisionFrom { get; set; }
        public string mcode { get; set; }
        public string barcode { get; set; }
        public string quantity { get; set; }
        public string rate { get; set; }
        public string userName { get; set; }
        public string unit { get; set; }
        public string billToAdd { get; set; }

        public string desca { get; set; }

        public DateTime expDate { get; set; }


        public BranchItem()
        {
            ind = 0;
            vchrNo = "";
            division = "";
            billTo = "";
            //divisionFrom = "";
            mcode = "";
            barcode = "";
            quantity = "0";
            rate = "0.0";
            userName = "";
            unit = "";
            billToAdd = "";
            desca = "";
            expDate = DateTime.Today;

        }

        public void SetBranchItem(BranchItem BranchItem)
        {
            try
            {
                ind = BranchItem.ind;
                vchrNo = BranchItem.vchrNo;
                division = BranchItem.division;
                billTo = BranchItem.billTo;
                // divisionFrom = BranchItem.divisionFrom;
                mcode = BranchItem.mcode;
                barcode = BranchItem.barcode;
                quantity = BranchItem.quantity;
                rate = BranchItem.rate;
                userName = BranchItem.userName;
                unit = BranchItem.unit;

                desca = BranchItem.desca;
                expDate = BranchItem.expDate;
            }
            catch { }
        }

        public void SetInitialBranchItem(BranchTransferDetail BranchTransferDetail)
        {
            try
            {
                this.vchrNo = BranchTransferDetail.vchrNo;
                this.division = BranchTransferDetail.division;
                this.billTo = BranchTransferDetail.billTo;
                //this.divisionFrom = BranchTransferDetail.divisionFrom;
                this.billToAdd = BranchTransferDetail.billToAdd;
            }
            catch { }
        }
    }
}
