using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    class SPBExtraction : Extractable
    {
        // Needed to show progress by ProgressbarThread
        // counts the progressed bytes
        protected long Progress;
        // byte-Length of the File-Input-Stream
        protected long ProgressEnds;

        // used for giving parameters from ExtractData to ExtractDataThread
        protected string ExtractDataFrom;
        protected string ExtractDataTo;

        public override void CountData(string file, IWin32Window owner, DataGridViewCell cell)
        {
            cell.Value = "NA";
        }

        public override void ExtractData(string fromFile, string toPath, IWin32Window owner)
        {
            ExtractDataFrom = fromFile;
            ExtractDataTo = Path.Combine(toPath, "phonebook.xml.gz");

            if (!Directory.Exists(toPath))
                Directory.CreateDirectory(toPath);

            // Open the encrpted file
            using (StreamReader sourceFile = new StreamReader(File.OpenRead(ExtractDataFrom)))
            {
                // Set the ProgressEnds to the length of the file
                ProgressEnds = sourceFile.BaseStream.Length;
                // the Progressbar has not to be closed to early
                Progress = -2;
            }

            // create Progressbar window
            Progressbar pb = new Progressbar("Extracting " + GetLabel() + "...", "Bytes");
            // set Maximum of the Progress
            pb.SetMaximum(ProgressEnds);

            // create Threads
            Thread th = new Thread(this.ExtractDataThread);
            Thread prbr = new Thread(this.ProgressBarThread);

            // Start the Threads
            prbr.Start(pb);
            th.Start();

            // show Progressbar and disable the owner
            pb.ShowDialog(owner);
        }

        public void ExtractDataThread()
        {
            // decrypt file
            string filename = KiesEncryption.Decrypt(ExtractDataFrom, BlockExtracted);
            // interprate XML

            if (File.Exists(filename) && !File.Exists(ExtractDataTo))
            {
                File.Move(filename, ExtractDataTo);
                KiesCompression.Decompress(new FileInfo(ExtractDataTo));
            }
        }

        public void BlockExtracted(int blockLength)
        {
            Progress += blockLength;
        }

        // is only for updating progressbar data
        // object:  has to be an instance of Progressbar
        private void ProgressBarThread(object pb)
        {
            Progressbar progressBar = (Progressbar)pb;

            while (Progress < ProgressEnds)
            {
                progressBar.SetValue(Progress);
                Thread.Sleep(100);
            }

            // Thread synchronisation to close the Progressbar
            if (progressBar.InvokeRequired)
                progressBar.BeginInvoke((MethodInvoker)progressBar.Close);
            else
                progressBar.Close();
        }

        public override string GetLabel()
        {
            return "SPB - Kies phonebook backup";
        }

        public override FileSelection[] GetFilematches()
        {
            return new[] {
                    new FileSelection("Kies phonebook backup (*.spb)", "*.spb")
                };
        }

        public override bool FileMatches(string filename)
        {
            return filename.ToLower(CultureInfo.InvariantCulture).EndsWith(".spb");
        }
    }
}
