using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SBUExtractor.DataExtraction
{
    internal class KiesEncryption
    {
        /**
         * Key and IV were extracted from http://systools.losthost.org/?misc#kiesconv
         * http://systools.losthost.org/files/kiescsrc.zip 
         * which is a C impelmentation to unpack calendar and contacts
         */
        public static readonly byte[] SAMSUNG_BACKUP_AES_IV  = Encoding.UTF8.GetBytes("afie,crywlxoetka");
        public static readonly byte[] SAMSUNG_BACKUP_AES_KEY = Encoding.UTF8.GetBytes("epovviwlx,dirwq;sor0-fvksz,erwog");

        internal static string Decrypt(string fromFile)
        {
            string aimingFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".smm.xml";

            using (var inputFile = File.OpenRead(fromFile))
            {
                using (var outputFile = File.OpenWrite(aimingFile))
                {
                    byte[] input = new byte[272];
                    byte[] output = new byte[input.Length];
                    int readBytes = 0;

                    readBytes = inputFile.Read(input, 0, input.Length);

                    while (readBytes > 0)
                    {
                        // the decryptor has to be reset for each block
                        var cryptoAlgorithmn = GetCryptoAlgorithm().CreateDecryptor(SAMSUNG_BACKUP_AES_KEY, SAMSUNG_BACKUP_AES_IV);
                        int transformed = cryptoAlgorithmn.TransformBlock(input, 0, readBytes, output, 0);
                        outputFile.Write(output, 0, transformed);

                        // drop sixteen 0x10 bytes
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
            algorithm.KeySize = SAMSUNG_BACKUP_AES_KEY.Length*8;
            algorithm.BlockSize = SAMSUNG_BACKUP_AES_IV.Length*8;
            return algorithm;
        }
    }
}