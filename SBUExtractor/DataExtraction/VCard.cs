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
