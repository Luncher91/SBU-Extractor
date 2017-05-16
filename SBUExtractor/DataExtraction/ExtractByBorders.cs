using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    class ExtractByBorders:Extractable
    {
        // Have to be set by Constructor
        // the key-string which indicates the beginning of some data
        protected string StartKey;
        // the key-string which indicates the ending of some data
        protected string EndKey;
        // the label which will be returned by GetLabel()
        protected string Label;
        // the suffix for the ending of the file(s)
        protected string Suffix;


        // Needed to show progress by ProgressbarThread
        // counts the progressed bytes
        protected long Progress;
        // byte-Length of the File-Input-Stream
        protected long ProgressEnds;
        

        // Set by CountData and used by CountDataThread
        // will be used for showing the result of counting data
        protected DataGridViewCell CountDataCell;


        // used for giving parameters from ExtractData to ExtractDataThread
        protected string ExtractDataFrom;
        protected string ExtractDataTo;
        protected bool ExtractToSingleFile;

        public override string GetLabel()
        {
            return Label;
        }

        public override void CountData(string file, IWin32Window owner, DataGridViewCell cell)
        {
            CountDataCell = cell;

            StreamReader sbu = new StreamReader(File.OpenRead(file));
            // Sets the Length of the file
            ProgressEnds = sbu.BaseStream.Length;
            // is set to -2 because the Progressbar has not to be closed to early
            Progress = -2;
            sbu.Close();

            // creates the Thread which counts the data
            Thread th = new Thread(CountDataThread);

            // creates the Thread which updates the data for the progressbar
            Thread progressBar = new Thread(ProgressBarThread);

            // creates the Progressbar Window
            Progressbar pb = new Progressbar("Counting " + Label + "...", "Bytes");

            // Sets the Maximum of the Progressbar
            pb.SetMaximum(ProgressEnds);

            // starts both Threads
            progressBar.Start(pb);
            th.Start(file);

            // shows the Progressbar as an Dialog to disable owner
            pb.ShowDialog(owner);
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

        // counts the data and write the result to CountDataCell
        // object:  has to be the path to the SBU-File
        private void CountDataThread(object fileO)
        {
            string file = (string)fileO;
            int ret = 0;

            StreamReader sbu = new StreamReader(File.OpenRead(file));

            int counter = 0;
            while (!sbu.EndOfStream)
            {
                Progress++;
                char b = (char)sbu.Read();

                // There are 0 bytes inserted by Kies, so ignore it
                if (b == (char)0)
                    continue;

                // is this byte the next byte of the StartKey
                // to find the Startkey in the Stream
                if (b == StartKey[counter])
                    counter++;
                else
                    counter = 0;

                // Startkey was found so we found one more data
                if (counter == StartKey.Length)
                {
                    counter = 0;
                    ret++;
                }
            }

            // close the SBU file
            sbu.Close();
            // Progress is up to 100% so set Progress to ProgressEnds
            Progress = ProgressEnds;
            // return the value to the user
            CountDataCell.Value = ret;
        }

        public override void ExtractData(string fromFile, string toPath, IWin32Window owner)
        {
            ExtractDataFrom = fromFile;
            // add backslash to easely add the file name
            ExtractDataTo = toPath + "\\";

            // create directory if its not existing!
            if (!Directory.Exists(ExtractDataTo))
                Directory.CreateDirectory(ExtractDataTo);

            // Open the SBU-File
            StreamReader sbu = new StreamReader(File.OpenRead(ExtractDataFrom));

            // Set the ProgressEnds to the length of the SBU-File
            ProgressEnds = sbu.BaseStream.Length;
            // the Progressbar has not to be closed to early
            Progress = -2;
            // close sbu file
            sbu.Close();

            // create Progressbar window
            Progressbar pb = new Progressbar("Extracting " + Label + "...", "Bytes");
            // set Maximum of the Progress
            pb.SetMaximum(ProgressEnds);

            // create Threads
            Thread th = new Thread(this.ExtractDataThread);
            Thread prbr = new Thread(this.ProgressBarThread);

            // Ask for extracting to a single file
            DialogResult res = MessageBox.Show(owner, "Extract \"" + Label + "\" to single file?", "Extraction method - " + Label,
                                               MessageBoxButtons.YesNo);

            // save the answer
            ExtractToSingleFile = res == DialogResult.Yes;

            // Start the Threads
            prbr.Start(pb);
            th.Start();


            // show Progressbar and disable the owner
            pb.ShowDialog(owner);
        }

        public void ExtractDataThread()
        {
            // only used if the data will not be extracted to a single file
            int eventCounter = 1;
            // open SBU-File
            StreamReader sbu = new StreamReader(File.OpenRead(ExtractDataFrom));

            // Reads until the Startkey was there
            ReadWhile(StartKey, sbu);
            while (!sbu.EndOfStream)
            {
                // creates the file Writer 
                // if it is extracted to a single file it will get the name of the Label+Suffix
                // else the file gets the name of eventCounter + Suffix so they are counted 1 2 3 ...
                StreamWriter of = this.ExtractToSingleFile
                                      ? new StreamWriter(File.OpenWrite(this.ExtractDataTo + Label + Suffix))
                                      : new StreamWriter(File.OpenWrite(this.ExtractDataTo + eventCounter + Suffix));

                // Write the Startkey to the file because is was already read
                of.Write(StartKey);
                // Write Everything to the file until the EndKey was written
                ReadToFile(EndKey, sbu, of);
                // go to next line
                of.WriteLine();
                // flush everything
                of.Flush();
                // close the file
                of.Close();
                // Count one more event (only if it is not extracted to one file)
                eventCounter++;
                // Reads until the Startkey was there
                ReadWhile(StartKey, sbu);
            }

            // Progress is allowed to end
            Progress = ProgressEnds;
            // close SBU-File
            sbu.Close();
        }

        // Reads the sbu and writes it to of until the key was read
        // skip every 0-byte
        private void ReadToFile(string key, StreamReader sbu, StreamWriter of)
        {
            int counter = 0;
            while (!sbu.EndOfStream)
            {
                Progress++;
                char b = (char)sbu.Read();

                if (b == (char)0)
                    continue;

                if (b == key[counter])
                    counter++;
                else
                {
                    counter = 0;
                }

                of.Write(b);

                if (counter == key.Length)
                    return;
            }
            of.Flush();
        }

        // Read the file until the key was read
        // skip every 0-byte
        private void ReadWhile(string key, StreamReader sbu)
        {
            int counter = 0;
            while (!sbu.EndOfStream)
            {
                Progress++;
                char b = (char)sbu.Read();

                if (b == (char)0)
                    continue;

                if (b == key[counter])
                    counter++;
                else
                    counter = 0;

                if (counter == key.Length)
                    return;
            }
        }
    }
}
