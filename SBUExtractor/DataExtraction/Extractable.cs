using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    /**
     * You have to inherit from this class if you want to implement additional extractions of data
     */
    public abstract class Extractable
    {
        // labels the data-extraction method for the user
        public abstract string GetLabel();

        // Counts the parts of the data
        // owner:   gives the Main window for disabling at loading
        // cell:    gives the cell in which the result is shown in
        // file:    gives the path of the SBU-File
        public abstract void CountData(string file, IWin32Window owner, DataGridViewCell cell);

        // extracts the data
        // fromFile:    the path to the SBU-File
        // toFile:      the path in which the data will be extracted in
        // owner:       the Main window which has to be disabled while loading
        public abstract void ExtractData(string fromFile, string toPath, IWin32Window owner);

        public virtual FileSelection[] GetFilematches()
        {
            return new[] { new FileSelection("Kies backup file (*.sbu)", "*.sbu") };
        }

        public virtual bool FileMatches(string filename)
        {
            return filename.ToLower(CultureInfo.InvariantCulture).EndsWith(".sbu");                
        }

        public class FileSelection
        {
            public string ExtensionMatch;
            public string Description;

            public FileSelection(string description, string extensionFilter)
            {
                Description = description;
                ExtensionMatch = extensionFilter;
            }

            public override string ToString()
            {
                return Description + "|" + ExtensionMatch;
            }
        }
    }
}
