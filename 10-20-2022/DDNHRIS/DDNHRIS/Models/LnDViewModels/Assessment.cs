using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDNHRIS.Models.LnDViewModels
{
    public class LNDCompetency
    {
        public string comptCode { get; set; }
        public string comptName { get; set; }
        public string comptDesc { get; set; }
        public string comptGroupCode { get; set; }
        public string comptTool { get; set; }
        public List<LNDIndicators> KBIList { get; set; }
        public int orderNo { get; set; }
    }

    public class LNDIndicators
    {
        public int itemNo { get; set; }
        public string header { get; set; }
        public string KBICode { get; set; }
        public string KBI { get; set; }
        public string comptLevel { get; set; }
        public string basic { get; set; }
        public string intermediate { get; set; }
        public string advance { get; set; }
        public string superior { get; set; }
        public int standard { get; set; }
        public int answerSA { get; set; }
        public int answerDS { get; set; }
        public int answerPG { get; set; }

        public int tag { get; set; }
        public int orderNo { get; set; }
    }
}