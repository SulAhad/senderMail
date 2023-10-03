using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mailSender
{
    public class FileData
    {
        public string Line { get; set; }
        public ICommand OpenCommand { get; set; }
        public string Path { get; internal set; }
        public string Line6 { get; internal set; }
        public string Email { get; set; }
        public string Message { get; set; }

        public FileData()
        {
            OpenCommand = new RelayCommand(OpenFile);
        }

        private void OpenFile()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Line,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Обработка ошибки открытия файла
            }
        }

        public static explicit operator FileData(string v)
        {
            throw new NotImplementedException();
        }
    }
}
