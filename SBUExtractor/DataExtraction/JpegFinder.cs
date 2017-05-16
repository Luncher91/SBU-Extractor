using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    public class JpegFinder : Extractable
    {
        List<long> starts = new List<long>();
        List<long> ends = new List<long>();

        readonly byte[] JpegStart = { 0xFF, 0xD8 };
        readonly byte[] JpegEnd = { 0xFF, 0xD9 };

        private long Progress;
        private long ProgressEnds;

        public override string GetLabel()
        {
            return "SystematicalJpegFinder";
        }

        public override void CountData(string file, IWin32Window owner, DataGridViewCell cell)
        {
            ExtractDataFrom = file;
            this.ScanForStartEnds(owner);
            cell.Value = "Max " + ((long)starts.Count*ends.Count);
        }

        public override void ExtractData(string fromFile, string toPath, IWin32Window owner)
        {
            ExtractDataFrom = fromFile;
            ExtractDataTo = toPath + "\\";
            this.ScanForStartEnds(owner);
            this.ExtractDataOfPositions(owner);
        }

        private void ExtractDataOfPositions(IWin32Window owner)
        {
            ProgressEnds = starts.Count*ends.Count;
            // is set to -2 because the Progressbar has not to be closed to early
            Progress = -2;

            // creates the Thread which updates the data for the progressbar
            Thread progressBar = new Thread(ProgressBarThread);

            Thread extractionThread = new Thread(this.ExtractThread);

            // creates the Progressbar Window
            Progressbar pb = new Progressbar("Extracting Jpegs (" + this.GetLabel() + ")...", "Possibilities");

            // Sets the Maximum of the Progressbar
            pb.SetMaximum(ProgressEnds);

            // starts both Threads
            progressBar.Start(pb);
            extractionThread.Start();

            // show Progressbar and disable the owner
            pb.ShowDialog(owner);
        }

        private void ExtractThread()
        {
            StreamReader sr = new StreamReader(File.OpenRead(ExtractDataFrom));

            long pictureCount = 0;

            for (int i = 0; i < starts.Count; i++)
            {
                for (int j = 0; j < ends.Count; j++)
                {
                    Progress++;

                    if(starts[i] >= ends[j])
                        continue;

                    string file = ExtractDataTo + pictureCount + ".jpg";
                    ExtractFile(sr, file, starts[i], ends[j]);
                    CheckForJpeg(file);
                    pictureCount++;
                }
            }

            sr.Close();
            Progress = ProgressEnds;
        }

        private void CheckForJpeg(string file)
        {
            try
            {
                new Bitmap(file);
            }
            catch (Exception)
            {
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
        }

        private void ExtractFile(StreamReader reader, string file, long start, long last)
        {
            // create directory if its not existing!
            if (!Directory.Exists(ExtractDataTo))
                Directory.CreateDirectory(ExtractDataTo);

            long oldPosition = reader.BaseStream.Position;
            StreamWriter sw = new StreamWriter(File.OpenWrite(file));
            char[] buffer = new char[last - start];
            reader.BaseStream.Position = start;
            reader.ReadBlock(buffer, 0, (int) (last - start));
            sw.Write(buffer);
            sw.Flush();
            sw.Close();
            reader.BaseStream.Position = oldPosition;
        }

        private string ExtractDataFrom;
        private string ExtractDataTo;

        private void ScanForStartEnds(IWin32Window owner)
        {
            StreamReader sr = new StreamReader(File.OpenRead(ExtractDataFrom));
            ProgressEnds = sr.BaseStream.Length;
            // is set to -2 because the Progressbar has not to be closed to early
            Progress = -2;
            sr.Close();

            // creates the Thread which updates the data for the progressbar
            Thread progressBar = new Thread(ProgressBarThread);

            Thread extractionThread = new Thread(this.ScanThread);

            // creates the Progressbar Window
            Progressbar pb = new Progressbar("Finding Jpegs (" + this.GetLabel() + ")...", "Bytes");

            // Sets the Maximum of the Progressbar
            pb.SetMaximum(ProgressEnds);

            // starts both Threads
            progressBar.Start(pb);
            extractionThread.Start();

            // show Progressbar and disable the owner
            pb.ShowDialog(owner);
        }

        private void ScanThread()
        {
            StreamReader sr = new StreamReader(File.OpenRead(ExtractDataFrom));

            byte[] buffer = new byte[2];
            while (sr.BaseStream.Position < sr.BaseStream.Length)
            {
                buffer[0] = buffer[1];
                buffer[1] = (byte)sr.BaseStream.ReadByte();
                if (buffer[0] == JpegStart[0] && buffer[1] == JpegStart[1])
                    starts.Add(sr.BaseStream.Position-2);
                
                if(buffer[0] == JpegEnd[0] && buffer[1] == JpegEnd[1])
                    ends.Add(sr.BaseStream.Position);

                Progress = sr.BaseStream.Position - 2;
            }

            sr.Close();
            Progress = ProgressEnds;
        }

        // is only for updating progressbar data
        // object:  has to be an instance of Progressbar
        private void ProgressBarThread(object pb)
        {
            Progressbar progressBar = (Progressbar)pb;

            while (Progress < ProgressEnds)
            {
                if (Progress > 0)
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