using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.DirectX.AudioVideoPlayback;

namespace VideoEncryption
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                EncryptFile(openFileDialog1.FileName, openFileDialog1.FileName + "en.enc");
            }
            else {
                System.Windows.Forms.MessageBox.Show("Please select File");

            }
        }
            private const string SKey = "_?73^?dVT3st5har3";
        private const string SaltKey = "!2S@LT&KT3st5har3EY";
        private const int Iterations = 1042; // Recommendation is >= 1000
        
        public static void EncryptFile(string srcFilename, string destFilename)
        {
            var aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            var salt = Encoding.UTF7.GetBytes(SaltKey);
            FileStream fs = new FileStream("EncryptedFile.txt", FileMode.Create, FileAccess.Write);
            var key = new Rfc2898DeriveBytes(SKey, salt, Iterations);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var dest = new FileStream(destFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                using (var cryptoStream = new CryptoStream(dest, transform, CryptoStreamMode.Write))
                {
                    using (var source = new FileStream(srcFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        source.CopyTo(cryptoStream);
                    }
                }
            }
        }
      
        
        public static void DecryptFile(string srcFilename, string destFilename)
        {
            var tempFile = Path.GetTempFileName()+".mp4";
        var aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            var salt = Encoding.UTF7.GetBytes(SaltKey);
            var key = new Rfc2898DeriveBytes(SKey, salt, Iterations);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
            if (File.Exists(@tempFile)) {
                File.Delete(tempFile);
            }
            var dest = new FileStream(tempFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read);

            var cryptoStream = new CryptoStream(dest, transform, CryptoStreamMode.Write);
                    
                try { 
     
                    using (var source = new FileStream(srcFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        source.CopyTo(cryptoStream);

                    }
                
                    Process.Start(tempFile).WaitForExit();


                }
                catch (CryptographicException exception)
                {
                    throw new ApplicationException("Decryption failed.", exception);
                }
                finally {
                try
                {

                    cryptoStream.Dispose();
                    dest.Dispose();
                    File.Delete(tempFile);
                }
                catch (Exception e) { }
                     
                    
                }
                       
                    
                    
                
            

                
            
        }
     
        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DecryptFile(openFileDialog1.FileName, openFileDialog1.FileName.Replace(".enc", ""));
            }
            else {
                System.Windows.Forms.MessageBox.Show("Please select File");
            }
            
        }
    }
}
    
    


