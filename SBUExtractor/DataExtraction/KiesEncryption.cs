using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SBUExtractor.DataExtraction
{
    internal class KiesEncryption
    {
        public static readonly byte[] SAMSUNG_BACKUP_AES_IV  = Encoding.UTF8.GetBytes("afie,crywlxoetka");
        public static readonly byte[] SAMSUNG_BACKUP_AES_KEY = Encoding.UTF8.GetBytes("epovviwlx,dirwq;sor0-fvksz,erwog");

        internal static string Decrypt(string fromFile)
        {
            string aimingFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".smm.xml";

            using (var inputFile = File.OpenRead(fromFile))
            {
                var cryptoAlgorithmn = GetCryptoAlgorithm().CreateDecryptor(SAMSUNG_BACKUP_AES_KEY, SAMSUNG_BACKUP_AES_IV);
                
                using (var outputFile = File.OpenWrite(aimingFile))
                {
                    byte[] input = new byte[256];
                    byte[] output = new byte[input.Length];
                    int readBytes = inputFile.Read(input, 0, input.Length);
                    while (readBytes > 0)
                    {
                        int transformed = cryptoAlgorithmn.TransformBlock(input, 0, readBytes, output, 0);
                        outputFile.Write(output, 0, transformed);
                        readBytes = inputFile.Read(input, 0, 16);
                        readBytes = inputFile.Read(input, 0, input.Length);
                    }
                }
            }
            return aimingFile;
        }

        private static RijndaelManaged GetCryptoAlgorithm()
        {
            RijndaelManaged algorithm = new RijndaelManaged();
            //set the mode, padding and block size
            algorithm.Padding = PaddingMode.PKCS7;
            algorithm.Mode = CipherMode.CBC;
            algorithm.KeySize = 256;
            algorithm.BlockSize = 128;
            return algorithm;
        }
    }
}