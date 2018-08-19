
using System;
using System.Collections.Generic;
using System.Text;

namespace DataCollectorRestApi.Models
{
    public class GrnMain 
    {
        public string vchrNo { get; set; }
        public string division { get; set; }
        public string chalanNo { get; set; }
        public DateTime trnDate { get; set; }
        public string trnAc { get; set; }
        public string ParAc { get; set; }
        public string trnMode { get; set; }
        public string refOrdBill { get; set; }
        public string remarks { get; set; }
        public string wareHouse { get; set; }
        public string isTaxInvoice { get; set; }

        public int curNo { get; set; }
        
        public string supplierName { get; set; }
        public string desca { get; set; }

        public string batchNo { get; set; }
        public string locationName { get; set; }
        public int sessionId { get; set; }
        public string userName { get; set; }

        private bool _IsUpload;
        public bool IsUpload
        {
            get { return _IsUpload; }
            set { _IsUpload = value;}
        }

        public bool IsSaved { get; set; }
        public string refNo { get; set; }

        public GrnMain()
        {
            vchrNo = "";
            division = "";
            chalanNo = "";
            trnDate = DateTime.Today;
            trnAc = "";
            ParAc = "";
            trnMode = "Cash";
            refOrdBill = "";
            remarks = "";
            wareHouse = "";
            isTaxInvoice = "";

            curNo = 0;
            
            supplierName = "";
            desca = "";

            batchNo = "";
            locationName = "";
            sessionId = 0;
            IsUpload = false;
            IsSaved = false;
            userName = "";
            refNo = "";
        }     

    }

    public class GrnProd
    {
        public int ind { get; set; }
        public string vchrNo { get; set; }
        public string division { get; set; }

        public string mcode { get; set; }
        public string barcode { get; set; }
        public string quantity { get; set; }
        public string rate { get; set; }
        public DateTime expDate { get; set; }
        public string userName { get; set; }
        public string unit { get; set; }

        public string desca { get; set; }
        public string supplierName { get; set; }

        public string batchNo { get; set; }
        public string locationName { get; set; }
        public int sessionId { get; set; }
        
        public GrnProd()
        {
            ind = 0;
            vchrNo = "";
            division = "";
            mcode = "";
            barcode = "";
            quantity = "0";
            rate = "0.0";
            expDate = DateTime.Today;
            userName = "";
            unit = "";
            batchNo = "";
            locationName = "";
            sessionId = 0;
            desca = "";
            supplierName = "";

        }

        public void SetGrnEntry(GrnProd GrnMain)
        {
            try
            {
                ind = GrnMain.ind;
                vchrNo = GrnMain.vchrNo;
                division = GrnMain.division;
                mcode = GrnMain.mcode;
                barcode = GrnMain.barcode;
                quantity = GrnMain.quantity;
                rate = GrnMain.rate;
                expDate = GrnMain.expDate;
                userName = GrnMain.userName;
                unit = GrnMain.unit;
                batchNo = GrnMain.batchNo;
                locationName = GrnMain.locationName;
                sessionId = GrnMain.sessionId;
                desca = GrnMain.desca;
                supplierName = GrnMain.supplierName;                
            }
            catch { }
        }

        public void SetInitialGrnData(GrnMain GrnMain)
        {
            try
            {
                this.vchrNo = GrnMain.vchrNo;
                this.division = GrnMain.division;
                this.desca = GrnMain.desca;
                this.supplierName = GrnMain.supplierName;
            }
            catch { }
        }
    }

    public class GrnMaster
    {
        public GrnMain GrnMain { get; set; }
        public List<GrnProd> GrnProdList { get; set; }

        public GrnMaster()
        {
            GrnMain = new GrnMain();
            GrnProdList = new List<GrnProd>();
        }
    }
}
