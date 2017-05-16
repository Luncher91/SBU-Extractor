using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    class Jpeg : Extractable
    {
        private long Progress;
        private long ProgressEnds;
        private string ExtractDataFrom;
        private string ExtractDataTo;

        public override string GetLabel()
        {
            return "Pictures(JpegOnlyButAll)";
        }

        public override void CountData(string file, IWin32Window owner, DataGridViewCell cell)
        {
            cell.Value = "NA";
        }

        public override void ExtractData(string fromFile, string toPath, IWin32Window owner)
        {
            ExtractDataFrom = fromFile;
            // add backslash to easely add the file name
            ExtractDataTo = toPath + "\\";

            // create directory if its not existing!
            if (!Directory.Exists(ExtractDataTo))
                Directory.CreateDirectory(ExtractDataTo);

            StreamReader sr = new StreamReader(File.OpenRead(ExtractDataFrom));
            ProgressEnds = sr.BaseStream.Length;
            // is set to -2 because the Progressbar has not to be closed to early
            Progress = -2;
            sr.Close();

            // creates the Thread which updates the data for the progressbar
            Thread progressBar = new Thread(ProgressBarThread);

            Thread extractionThread = new Thread(this.ExtractDataThread);

            // creates the Progressbar Window
            Progressbar pb = new Progressbar("Extracting " + this.GetLabel() + "...", "Bytes");

            // Sets the Maximum of the Progressbar
            pb.SetMaximum(ProgressEnds);

            // starts both Threads
            progressBar.Start(pb);
            extractionThread.Start();

            // show Progressbar and disable the owner
            pb.ShowDialog(owner);
        }

        private void ExtractDataThread()
        {
            StreamReader sr = new StreamReader(File.OpenRead(ExtractDataFrom));
            int pictureNumber = 0;
            long lastStart = 0;
            string file = "";

            while (sr.BaseStream.Position < sr.BaseStream.Length)
            {
                file = ExtractDataTo + pictureNumber + ".jpg";
                ReadToStart(sr);
                lastStart = sr.BaseStream.Position;
                WriteToFile(sr, file);
                try
                {
                    new Bitmap(file);
                }
                catch (Exception)
                {
                    sr.BaseStream.Position = lastStart + 2;
                    while (File.Exists(file))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                Progress = sr.BaseStream.Position - 2;
                pictureNumber++;
            }

            sr.Close();
            Progress = ProgressEnds;
        }

        readonly byte[] start = { 0xFF, 0xD8 };
        readonly byte[] stop = { 0xFF, 0xD9 };

        private void WriteToFile(StreamReader sr, string s)
        {
            StreamWriter sw = new StreamWriter(File.OpenWrite(s));

            byte[] buffer = new byte[2];
            int counter = 1;
            while(sr.BaseStream.Position < sr.BaseStream.Length)
            {
                buffer[0] = buffer[1];
                buffer[1] = (byte) sr.BaseStream.ReadByte();
                sw.BaseStream.WriteByte(buffer[1]);

                if (buffer[0] == start[0] && buffer[1] == start[1])
                    counter++;

                if (buffer[0] == stop[0] && buffer[1] == stop[1])
                {
                    counter--;
                    if (counter > 5)
                    {
                        sw.Flush();
                        sw.Close();
                        return;
                    }
                }

                if(counter == 0)
                {
                    sw.Flush();
                    sw.Close();
                    return;
                }
            }

            sw.Flush();
            sw.Close();
        }

        private void ReadToStart(StreamReader sr)
        {
            byte[] buffer = new byte[2];
            while(sr.BaseStream.Position < sr.BaseStream.Length)
            {
                buffer[0] = buffer[1];
                buffer[1] = (byte) sr.BaseStream.ReadByte();
                if (buffer[0] == start[0] && buffer[1] == start[1])
                {
                    sr.BaseStream.Position -= 2;
                    return;
                }
            }
        }

        // is only for updating progressbar data
        // object:  has to be an instance of Progressbar
        private void ProgressBarThread(object pb)
        {
            Progressbar progressBar = (Progressbar)pb;

            while (Progress < ProgressEnds)
            {
                if(Progress > 0)
                    progressBar.SetValue(Progress);
                Thread.Sleep(1000);
            }

            // Thread synchronisation to close the Progressbar
            if (progressBar.InvokeRequired)
                progressBar.BeginInvoke((MethodInvoker)progressBar.Close);
            else
                progressBar.Close();
        }
    }
}
