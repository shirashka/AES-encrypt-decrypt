using System;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES_Encrypt_Decrypt
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome! Would you like to encrypt or decrypt a file?");
            Console.WriteLine("Press uppercase E to encrypt or D to decrypt.");
            ConsoleKeyInfo cki = Console.ReadKey();
            Console.WriteLine("");

            string doCrypt = "";

            if (cki.KeyChar == 'E')
            {
                doCrypt = "encrypt";
            } else if (cki.KeyChar == 'D')
            {
                doCrypt = "decrypt"; 
            } else
            {
                Console.WriteLine("That is not a valid entry. Please type in E to encrypt or D to decrypt");
            }

            Console.WriteLine("Please type in the filepath of the file you wish to " + doCrypt + ". ");
            string startFilePath = Console.ReadLine();
            Console.WriteLine("");

            Console.WriteLine("What would you like to call your  " + doCrypt + "ed file?");
            Console.WriteLine("Please include the full filepath.");
            string endFilePath = Console.ReadLine();
            Console.WriteLine("");

            Console.WriteLine("Please type in your password. This is your secret key.");
            string secretKey = Console.ReadLine();
            Console.WriteLine("");

            Console.WriteLine("You are about to  " + doCrypt + " the file located at " + startFilePath);
            Console.WriteLine("Once  " + doCrypt + "ed, it will be saved as a separate file called " + endFilePath);
            Console.WriteLine("If this is correct, press enter to continue.");
            Console.ReadLine();

            string decryptedFilePath = Console.ReadLine();

            try
            {
                // Encrypt the file.        
                if (doCrypt == "encrypt")
                {
                    EncryptFile(startFilePath, endFilePath, secretKey);
                    Console.WriteLine("Congrats! Your file is now encrypted. Press enter to exit.");
                    Console.ReadLine();
                }
                
                // Decrypt the file. 
                if (doCrypt == "decrypt")
                {
                   
                    DecryptFile(startFilePath, endFilePath, secretKey);
                    Console.WriteLine("Congrats! Your file is now decrypted. Press enter to exit.");
                    Console.ReadLine();
                }
                
            }
            catch (IOException ex)
            {
                Console.WriteLine("Sorry, error occurred. " + ex);
            }


        }

        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }


        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public static void EncryptFile(string file, string fileEncrypted, string password)
        {
            //string file = "C:\\SampleFile.DLL";
            //string password = "abcd1234";

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            //string fileEncrypted = "C:\\SampleFileEncrypted.DLL";


            File.WriteAllBytes(fileEncrypted, bytesEncrypted);
        }


        public static void DecryptFile(string fileEncrypted, string file, string password)
        {
            //string fileEncrypted = "C:\\SampleFileEncrypted.DLL";
            //string password = "abcd1234";

            byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            //string file = "C:\\SampleFile.DLL";

            File.WriteAllBytes(file, bytesDecrypted);
        }
    }
}
