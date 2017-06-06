using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    class SMMExtraction : Extractable
    {
        public override void CountData(string file, IWin32Window owner, DataGridViewCell cell)
        {
            cell.Value = "NA";
        }

        public override void ExtractData(string fromFile, string toPath, IWin32Window owner)
        {
            // decrypt file
            string filename = KiesEncryption.Decrypt(fromFile);
            // interprate XML

            string destinationFilename = Path.Combine(toPath, "memos.xml");
            if (File.Exists(filename) && !File.Exists(destinationFilename))
            {
                if (!Directory.Exists(toPath))
                    Directory.CreateDirectory(toPath);

                File.Move(filename, destinationFilename);
            }
        }

        public override string GetLabel()
        {
            return "SMM - Kies memo backup";
        }

        public override FileSelection[] GetFilematches()
        {
            return new[] {
                    new FileSelection("Kies memo backup (*.smm)", "*.smm")
                };
        }

        public override bool FileMatches(string filename)
        {
            return filename.ToLower(CultureInfo.InvariantCulture).EndsWith(".smm");
        }
    }
}
