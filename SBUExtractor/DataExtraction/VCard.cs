using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    class VCard:ExtractByBorders
    {
        // used for extracting Contacts
        public VCard()
        {
            StartKey = "BEGIN:VCARD";
            EndKey = "END:VCARD";
            Label = "Contacts";
            Suffix = ".vcf";
        }
    }
}
