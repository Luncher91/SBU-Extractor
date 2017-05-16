using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    class FilesNew : Extractable
    {
        struct FileData
        {
            public long FileStart;
            public string Location;
            public ushort PathLength;
            public string Path;
            public ushort FileNameLength;
            public string FileName;
            public uint Size;
            public long UnknowenLong;
            public ushort UnknowenShort;
            public uint NullInt;
            public uint UnknowenInt;
        }

        /* Keywords for the beginning of path
         * internal:
         * Internal:
         * external:
         * External:
         */

        private readonly byte[][] keywords = new []{
                                            new byte[]
                                                {
                                                    (byte) 'i', 0x00,
                                                    (byte) 'n', 0x00,
                                                    (byte) 't', 0x00,
                                                    (byte) 'e', 0x00,
                                                    (byte) 'r', 0x00,
                                                    (byte) 'n', 0x00,
                                                    (byte) 'a', 0x00,
                                                    (byte) 'l', 0x00,
                                                    (byte) ':', 0x00
                                                },

                                            new byte[]
                                                {
                                                    (byte) 'I', 0x00,
                                                    (byte) 'n', 0x00,
                                                    (byte) 't', 0x00,
                                                    (byte) 'e', 0x00,
                                                    (byte) 'r', 0x00,
                                                    (byte) 'n', 0x00,
                                                    (byte) 'a', 0x00,
                                                    (byte) 'l', 0x00,
                                                    (byte) ':', 0x00
                                                },

                                            new byte[]
                                                {
                                                    (byte) 'e', 0x00,
                                                    (byte) 'x', 0x00,
                                                    (byte) 't', 0x00,
                                                    (byte) 'e', 0x00,
                                                    (byte) 'r', 0x00,
                                                    (byte) 'n', 0x00,
                                                    (byte) 'a', 0x00,
                                                    (byte) 'l', 0x00,
                                                    (byte) ':', 0x00
                                                },

                                            new byte[]
                                                {
                                                    (byte) 'E', 0x00,
                                                    (byte) 'x', 0x00,
                                                    (byte) 't', 0x00,
                                                    (byte) 'e', 0x00,
                                                    (byte) 'r', 0x00,
                                                    (byte) 'n', 0x00,
                                                    (byte) 'a', 0x00,
                                                    (byte) 'l', 0x00,
                                                    (byte) ':', 0x00
                                                }
                                        };

        private long Progress;
        private long ProgressEnds;
        private string ExtractDataFrom;
        private string ExtractDataTo;

        private readonly List<FileData> dataList = new List<FileData>();

        private DataGridViewCell countDataCell;

        public override string GetLabel()
        {
            return "Files (new Version)";
        }

        public override void CountData(string file, IWin32Window owner, DataGridViewCell cell)
        {
            dataList.Clear();

            this.countDataCell = cell;
            ExtractDataFrom = file;

            StreamReader sr = new StreamReader(File.OpenRead(ExtractDataFrom));
            ProgressEnds = sr.BaseStream.Length + 2;
            // is set to -2 because the Progressbar has not to be closed to early
            Progress = 0;
            sr.Close();

            // creates the Thread which updates the data for the progressbar
            Thread progressBar = new Thread(ProgressBarThread);

            Thread extractionThread = new Thread(this.ExtractDataStructureThread);

            // creates the Progressbar Window
            Progressbar pb = new Progressbar("Counting " + this.GetLabel() + "...", "Bytes");

            // Sets the Maximum of the Progressbar
            pb.SetMaximum(ProgressEnds);

            // starts both Threads
            progressBar.Start(pb);
            extractionThread.Start();

            // show Progressbar and disable the owner
            pb.ShowDialog(owner);
        }

        public override void ExtractData(string fromFile, string toPath, IWin32Window owner)
        {
            if(fromFile != ExtractDataFrom)
                this.CountData(fromFile, owner, null);

            ExtractDataFrom = fromFile;
            // add backslash to easely add the file name
            ExtractDataTo = toPath + "\\";

            // create directory if its not existing!
            if (!Directory.Exists(ExtractDataTo))
                Directory.CreateDirectory(ExtractDataTo);

            ProgressEnds = dataList.Count + 1;
            // is set to -2 because the Progressbar has not to be closed to early
            Progress = 0;

            // creates the Thread which updates the data for the progressbar
            Thread progressBar = new Thread(ProgressBarThread);

            Thread extractionThread = new Thread(this.ExtractDataThread);

            // creates the Progressbar Window
            Progressbar pb = new Progressbar("Extracting " + this.GetLabel() + "...", "Files");

            // Sets the Maximum of the Progressbar
            pb.SetMaximum(ProgressEnds);

            // starts both Threads
            progressBar.Start(pb);
            extractionThread.Start();

            // show Progressbar and disable the owner
            pb.ShowDialog(owner);

            
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

        private void ExtractDataStructureThread()
        {
            StreamReader sr = new StreamReader(ExtractDataFrom);
            Stream stream = sr.BaseStream;

            while (stream.Position < stream.Length)
            {
                // find beginning
                // maybe the only solution is to find "internal:" with 0 byte after every char
                // if internal was found and Position x then is the Start of the FileIformation x - 10
                long startPosition = this.FindInStream(stream, keywords);

                if (startPosition == -1)
                    break; // End Of File

                stream.Position = startPosition - 10;

                // Reading Information of an File of 32 (10 before Path and 22 after) Bytes + 2*PathLength
                FileData d = new FileData();
                byte[] buffer = new byte[8];

                stream.Read(buffer, 0, 8);
                d.FileStart = this.ExtractLong(buffer);

                stream.Read(buffer, 0, 2);
                d.PathLength = (ushort)this.ExtractLong(buffer, 2);

                d.Path = this.ExtractString(stream, d.PathLength); // Bsp: internal:\DCIM\Camera\

                d.Location = d.Path.Substring(0, 9); // internal: (or external:)
                d.Path = d.Path.Substring(9);

                stream.Read(buffer, 0, 2);
                d.FileNameLength = (ushort)this.ExtractLong(buffer, 2);
                d.FileName = this.ExtractString(stream, d.FileNameLength);

                sr.BaseStream.Read(buffer, 0, 4);
                d.NullInt = (uint)this.ExtractLong(buffer, 4);

                sr.BaseStream.Read(buffer, 0, 4);
                d.Size = (uint)this.ExtractLong(buffer, 4);

                sr.BaseStream.Read(buffer, 0, 4);
                d.UnknowenInt = (uint)this.ExtractLong(buffer, 4);

                sr.BaseStream.Read(buffer, 0, 8);
                d.UnknowenLong = this.ExtractLong(buffer);

                sr.BaseStream.Read(buffer, 0, 2);
                d.UnknowenShort = (ushort)this.ExtractLong(buffer, 2);

                sr.BaseStream.Read(buffer, 0, 4);
                d.NullInt = (uint)this.ExtractLong(buffer, 4);

                this.dataList.Add(d);

                Progress = stream.Position;
            }

            if(this.countDataCell != null)
                this.countDataCell.Value = this.dataList.Count;

            Progress = ProgressEnds;
        }

        private void ExtractDataThread()
        {
            StreamReader sr = new StreamReader(ExtractDataFrom);
            Stream stream = sr.BaseStream;

            for (int i = 0; i < this.dataList.Count; i++)
            {
                this.ExtractFileFromStream(this.dataList[i], stream, ExtractDataTo);
                Progress = i;
            }

            Progress = ProgressEnds;
        }

        private long FindInStream(Stream stream, byte[][] bytes)
        {
            long[] progress = new long[bytes.Length];
            while(this.AllSmallerThan(progress, bytes))
            {
                int b = stream.ReadByte();

                if (b == -1)
                    return -1;

                for (int i = 0; i < bytes.Length; i++)
                {
                    if ((byte) b == bytes[i][progress[i]])
                        progress[i]++;
                    else
                        progress[i] = 0;
                }

                Progress = stream.Position;
            }


            return stream.Position - bytes[this.getFitterIndex(progress, bytes)].Length;
        }

        private long getFitterIndex(long[] progress, byte[][] bytes)
        {
            for (int i = 0; i < progress.Length; i++)
            {
                if (progress[i] >= bytes[i].Length)
                    return i;
            }

            return -1;
        }

        private bool AllSmallerThan(long[] progress, byte[][] bytes)
        {
            return this.getFitterIndex(progress, bytes) == -1;
        }

        public static string RemoveInvalidFileCharacters(string filename, string replaceChar)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(filename, replaceChar);
        }

        private void ExtractFileFromStream(FileData fileData, Stream sr, string toPath)
        {
            // Buffer
            const int bufferSize = 64;

            long positionSave = sr.Position;
            sr.Position = fileData.FileStart;
            string finalPath = toPath + "\\" + fileData.Location.TrimEnd(':') + "\\" + fileData.Path;

            // remove illegal chars
            string invalid = new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                finalPath = finalPath.Replace(c.ToString(), "\\");
            }

            finalPath = finalPath.TrimEnd('\\') + "\\";

            while(!Directory.Exists(finalPath))
                try
                {
                    Directory.CreateDirectory(finalPath);
                }
                catch (IOException)
                {
                    finalPath = finalPath.TrimEnd('\\') + "-Directory" + "\\";
                }
            
            StreamWriter sw = null;
            
            try
            {
                sw = new StreamWriter(finalPath + RemoveInvalidFileCharacters(fileData.FileName, "a"));
            }
            catch(Exception e)
            {
                MessageBox.Show("There went something wrong!\n" + e.Message + "\n" + e.StackTrace);
            }

            int readData = 0;
            for (long i = 0; i < fileData.Size; i += readData)
            {
                int dataHaveToRead = (int)Math.Min(bufferSize, (fileData.Size - i));
                
                byte[] buffer = new byte[dataHaveToRead];
                readData = sr.Read(buffer, 0, dataHaveToRead);
                sw.BaseStream.Write(buffer, 0, readData);

                Progress = sr.Position;
            }

            sw.Flush();
            sw.Close();

            sr.Position = positionSave;
        }

        private string ExtractString(Stream sr, ushort pathLength)
        {
            string ret = "";
            for (int i = 0; i < pathLength*2; i++)
            {
                byte b = (byte)sr.ReadByte();
                if (b != 0)
                    ret += (char)b;
            }
            return ret;
        }

        private long ExtractLong(byte[] bytes, int length = 8)
        {
            long ret = 0;
            for (int i = 0; i < bytes.Length && i < length; i++)
            {
                ret |= ((long)bytes[i]) << ((i)*8);
            }
            return ret;
        }
    }
}
