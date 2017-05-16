using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SBUExtractor.DataExtraction
{
    class Files : Extractable
    {
        private readonly byte[] StartSequence = {
                                            0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 
                                            0x60, 0x82, 0xAC, 0x00, 0x00, 0x00, 0x00
                                       };
        public override string GetLabel()
        {
            return "Files";
        }

        public override void CountData(string file, IWin32Window owner, DataGridViewCell cell)
        {
            cell.Value = "NA";
        }

        public override void ExtractData(string fromFile, string toPath, IWin32Window owner)
        {
            StreamReader sr = new StreamReader(File.OpenRead(fromFile));

            FindBeginOfFilePaths(sr);
            //sr.BaseStream.ReadByte();
            while (ExtractFile(sr, toPath)) ;
        }

        private bool ExtractFile(StreamReader sr, string toPath)
        {
            long position = this.GetIntNext(sr);

            // 00 00 00 00 00 XX 00
            for (int i = 0; i < 6; i++)
            {
                sr.BaseStream.ReadByte();
            }
            if (!ReadInternal(sr))
                return false;

            string path = toPath + this.GetPath(sr);
            long length = this.GetIntNext(sr);

            ReadOutFile(sr, position, path, length);

            // 00 XX XX XX XX 00 00 00 00 XX 00 00 00 00
            for (int i = 0; i < 14; i++)
            {
                sr.BaseStream.ReadByte();
            }

            return true;
        }

        private void ReadOutFile(StreamReader sr, long position, string path, long length)
        {
            Directory.CreateDirectory(path.Substring(0, path.Length - Path.GetFileName(path).Length));
            StreamWriter sw = new StreamWriter(File.OpenWrite(path));
            long oldPosition = sr.BaseStream.Position;
            sr.BaseStream.Position = position;
            for (int i = 0; i < length; i++)
            {
                sw.BaseStream.WriteByte((byte)sr.BaseStream.ReadByte());
            }
            sr.BaseStream.Position = oldPosition;
            sw.Flush();
            sw.Close();
        }

        private long GetIntNext(StreamReader sr)
        {
            List<byte> reversePosition = new List<byte>();
            while (sr.BaseStream.ReadByte() == 0x00) ;
                sr.BaseStream.Position--;
            while(sr.BaseStream.ReadByte() != 0x00)
            {
                sr.BaseStream.Position--;
                reversePosition.Add((byte)sr.BaseStream.ReadByte());
            }
            sr.BaseStream.Position--;
            //Array.Reverse(reversePosition);
            while (reversePosition[0] == 0)
                ShiftLeftArray(reversePosition.ToArray());

            if (reversePosition.Count <= 4)
            {
                while (reversePosition.Count < 4)
                    reversePosition.Add(0x00);
                return BitConverter.ToInt32(reversePosition.ToArray(), 0);
            }
            else
            {
                while (reversePosition.Count < 8)
                    reversePosition.Add(0x00);
                return BitConverter.ToInt64(reversePosition.ToArray(), 0);
            }
        }

        private void ShiftLeftArray(byte[] reversePosition)
        {
            for (int i = 0; i < reversePosition.Length-1; i++)
            {
                reversePosition[i] = reversePosition[i + 1];
            }

            reversePosition[reversePosition.Length - 1] = 0;
        }

        private string GetPath(StreamReader sr)
        {
            string path = "";
            int counter = 0;
            
            while (!sr.EndOfStream && sr.BaseStream.Position != sr.BaseStream.Length)
            {
                char b = (char) sr.BaseStream.ReadByte();
                if (b == (char)0)
                    counter++;
                else if(IsInvalidChar(b))
                {
                    counter = 0;
                }
                else
                {
                    path = path + b;
                    counter = 0;
                }

                if (counter == 4)
                    return path;
            }

            return path;
        }

        private bool IsInvalidChar(char c)
        {
            for (int i = 0; i < Path.GetInvalidPathChars().Length; i++)
            {
                if (c == Path.GetInvalidPathChars()[i])
                    return true;
            }

            return false;
        }

        private bool ReadInternal(StreamReader sr)
        {
            byte[] key = {0x49, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x65, 0x00, 0x72, 0x00, 0x6E, 0x00, 0x61, 0x00, 0x6C, 0x00, 0x3A, 0x00};

            for (int i = 0; i < key.Length; i++)
            {
                if (key[i] != (byte)sr.BaseStream.ReadByte())
                    return false;
            }

            return true;
        }

        private void FindBeginOfFilePaths(StreamReader sr)
        {
            byte[] sequence = new byte[StartSequence.Length];
            int count = 0;

            while(!sr.EndOfStream && sr.BaseStream.Position != sr.BaseStream.Length)
            {
                sequence[count] = (byte)sr.BaseStream.ReadByte();
                if (sequence[count] != StartSequence[count])
                    count = 0;
                else
                    count++;

                if (count == StartSequence.Length)
                    return;
            }
        }

        private bool EqualValues(int[] startSequence, List<int> sequence)
        {
            if (sequence.Count != startSequence.Length)
                return false;

            for (int i = 0; i < sequence.Count; i++)
            {
                if (startSequence[i] != sequence[i])
                    return false;
            }

            return true;
        }

        private bool StartsWith(int[] startSequence, List<int> sequence)
        {
            if (sequence.Count > startSequence.Length)
                return false;

            for (int i = 0; i < sequence.Count; i++)
            {
                if (startSequence[i] != sequence[i])
                    return false;
            }

            return true;
        }
    }
}
