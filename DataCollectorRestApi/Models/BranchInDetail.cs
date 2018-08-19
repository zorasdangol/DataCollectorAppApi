using System;
using System.Collections.Generic;
using System.Text;

namespace DataCollectorRestApi.Models
{
    public class BranchInDetail : BranchTransferDetail
    {
    }

    public class BranchInItem : BranchItem
    {
    }

    public class BranchInMaster
    {
        public BranchInDetail BranchInMain { get; set; }
        public List<BranchInItem> BranchInProdList { get; set; }

        public BranchInMaster()
        {
            BranchInMain = new BranchInDetail();
            BranchInProdList = new List<BranchInItem>();
        }
    }

    public class BranchInSummary: BranchItem
    {
        public int enteredQuantity { get; set; }
        public int difference { get; set; }
    }
    
}

