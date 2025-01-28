using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Encryter
{
    public partial class Crypt : Form
    {
        private BackgroundWorker backgroundWorker;
        private Stopwatch stopwatch;

        public Crypt()
        {
            InitializeComponent();

            this.CenterToScreen();

            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            stopwatch = new Stopwatch();
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                StartBackgroundWorker(isEncrypt: true);
            }
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                StartBackgroundWorker(isEncrypt: false);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtFilePath.Text) || !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Шлях до файлу некоректний або файл не існує.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtKey.Text))
            {
                MessageBox.Show("Ключ не може бути порожнім.");
                return false;
            }

            return true;
        }

        private void StartBackgroundWorker(bool isEncrypt)
        {
            btnEncrypt.Enabled = btnDecrypt.Enabled = false;
            progressBar.Value = 0;
            stopwatch.Reset();
            stopwatch.Start();

            backgroundWorker.RunWorkerAsync(new { IsEncrypt = isEncrypt });
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            dynamic args = e.Argument;
            bool isEncrypt = args.IsEncrypt;

            string inputFile = txtFilePath.Text;
            string outputFile = inputFile + (isEncrypt ? ".enc" : ".dec");
            string key = txtKey.Text;

            try
            {
                if (isEncrypt)
                {
                    EncryptFile(inputFile, outputFile, key);
                }
                else
                {
                    DecryptFile(inputFile, outputFile, key);
                }

                e.Result = new { OutputFile = outputFile, IsEncrypt = isEncrypt };
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void EncryptFile(string inputFile, string outputFile, string key)
        {
            using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                byte[] checksumBytes = BitConverter.GetBytes(CalculateChecksum(inputStream));
                inputStream.Seek(0, SeekOrigin.Begin);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = new byte[16];

                    outputStream.Write(checksumBytes, 0, checksumBytes.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(
                        outputStream,
                        aes.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        CopyStreamWithProgress(inputStream, cryptoStream);
                    }
                }
            }
        }

        private void DecryptFile(string inputFile, string outputFile, string key)
        {
            using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));

                byte[] checksumBytes = new byte[sizeof(long)];
                inputStream.Read(checksumBytes, 0, checksumBytes.Length);
                long originalChecksum = BitConverter.ToInt64(checksumBytes, 0);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = new byte[16];

                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        try
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(
                                inputStream,
                                aes.CreateDecryptor(),
                                CryptoStreamMode.Read))
                            {
                                cryptoStream.CopyTo(decryptedStream);
                            }

                            decryptedStream.Seek(0, SeekOrigin.Begin);
                            long calculatedChecksum = CalculateChecksum(decryptedStream);

                            if (calculatedChecksum != originalChecksum)
                            {
                                throw new InvalidOperationException("Контрольна сума не співпадає. Пароль невірний або файл пошкоджено.");
                            }

                            decryptedStream.Seek(0, SeekOrigin.Begin);
                            using (FileStream outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                            {
                                decryptedStream.CopyTo(outputStream);
                            }
                        }
                        catch (CryptographicException)
                        {
                            throw new InvalidOperationException("Контрольна сума не співпадає. Пароль невірний або файл пошкоджено.");
                        }
                    }
                }
            }
        }

        private long CalculateChecksum(Stream stream)
        {
            long checksum = 0;
            byte[] buffer = new byte[1024];
            int bytesRead;

            stream.Seek(0, SeekOrigin.Begin);
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    checksum += buffer[i];
                }
            }

            return checksum;
        }

        private void CopyStreamWithProgress(Stream input, Stream output)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            long totalBytesRead = 0;
            long totalBytes = input.Length;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;

                int progress = (int)((double)totalBytesRead / totalBytes * 100);
                backgroundWorker.ReportProgress(progress);

                if (backgroundWorker.CancellationPending)
                {
                    throw new OperationCanceledException();
                }
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stopwatch.Stop();
            btnEncrypt.Enabled = btnDecrypt.Enabled = true;

            if (e.Cancelled)
            {
                MessageBox.Show("Операція була скасована.");
            }
            else if (e.Error != null)
            {
                MessageBox.Show($"Сталася помилка: {e.Error.Message}");
            }
            else if (e.Result is Exception ex)
            {
                MessageBox.Show($"Сталася помилка: {ex.Message}");
            }
            else
            {
                dynamic result = e.Result;
                string outputFile = result.OutputFile;
                bool isEncrypt = result.IsEncrypt;

                if (isEncrypt)
                {
                    using (FileStream stream = new FileStream(outputFile, FileMode.Open, FileAccess.Read))
                    {
                        long checksum = CalculateChecksum(stream);
                        MessageBox.Show($"Файл успішно зашифровано: {outputFile}\nКонтрольна сума: {checksum}\nЧас виконання: {stopwatch.Elapsed}");
                    }
                }
                else
                {
                    MessageBox.Show($"Файл успішно розшифровано: {outputFile}\nЧас виконання: {stopwatch.Elapsed}");
                }
            }
        }
    }
}
