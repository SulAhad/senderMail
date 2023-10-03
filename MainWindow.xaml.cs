using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Windows;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media.Animation;
using System.Windows.Media.Animation;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;

namespace mailSender
{
    public partial class MainWindow : Window
    {
        string fileDataForUser;
        public ObservableCollection<string> Data { get; set; }
        // Коллекция данных для привязки к представлению

        public object fileList { get; private set; }
        // Объект fileList, доступный только для чтения

        public MainWindow()
        {
            InitializeComponent();
            Data = new ObservableCollection<string>();
            DataContext = this;
            // Инициализация компонентов и установка контекста передачи данных

            // Регистрируем провайдера кодировки
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Регистрация провайдера кодировки, необходимого для чтения текстовых файлов с определенной кодировкой
        }

        

        private Dictionary<string, string> emailDictionary = new Dictionary<string, string>()
        {   { "1880038",  "akhad.suleymanov@lab-industries.ru" },
            { "20191816", "akhad.suleymanov@lab-industries.ru" },
            { "20191694", "akhad.suleymanov@lab-industries.ru" },
            { "20190376", "akhad.suleymanov@lab-industries.ru" },
            { "20138573", "akhad.suleymanov@lab-industries.ru" }};


        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    using (var reader = new StreamReader(openFileDialog.FileName, Encoding.GetEncoding("windows-1251")))
                    {
                        string content = reader.ReadToEnd();
                        string[] lines = content.Split(new[] { "\n" }, StringSplitOptions.None);
                        int numLines = lines.Length;
                        int linesPerChunk = 65;
                        int numChunks = (int)Math.Ceiling((double)numLines / linesPerChunk);
                        string nameFolder = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("ru-RU"));
                        string directoryPath = Path.Combine(Path.GetDirectoryName(openFileDialog.FileName), nameFolder);
                        Directory.CreateDirectory(directoryPath);
                        List<FileData> fileList = new List<FileData>();
                        DataBaseContext db = new DataBaseContext();
                        var databaseList = db.DataBases.ToList();
                        for (int i = 0; i < numChunks; i++)
                        {
                            int startIndex = i * linesPerChunk;
                            int endIndex = Math.Min(startIndex + linesPerChunk, numLines);
                            string[] chunkLines = new ArraySegment<string>(lines, startIndex, endIndex - startIndex).ToArray();
                            string chunkString = string.Join("\n", chunkLines);
                            string fileName = chunkLines[9].Trim();
                            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                            { fileName = fileName.Replace(invalidChar.ToString(), ""); }
                            fileName += ".txt";
                            string chunkFilePath = Path.Combine(directoryPath, fileName);
                            string line6 = "";
                            if (chunkLines.Length >= 6)
                            {
                                line6 = chunkLines[5];
                                line6 = new string(line6.Replace(":", "").Where(c => !char.IsWhiteSpace(c) && !char.IsLetter(c)).ToArray());
                            }
                            FileData fileData = new FileData()
                            {
                                Line = chunkFilePath,
                                Line6 = line6,
                                Message = chunkString
                            };
                            foreach (var dbItem in databaseList)
                            {
                                if (dbItem.SomeText == line6)
                                {
                                    fileData.Email = dbItem.Date;
                                    File.WriteAllText(chunkFilePath, chunkString);
                                    break;
                                }
                            }
                            fileList.Add(fileData);

                            //string message = $"Содержание файла {fileName}:\n{chunkString}";
                            //MessageBox.Show(message);
                        }
                        DataContext = new { Data = fileList };
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        /*private void FileItem_Click(object sender, MouseButtonEventArgs e)
        {

            var listViewItem = sender as ListViewItem;
            var fileData = listViewItem?.Content as FileData;
            if (fileData != null)
            {string filePath = fileData.Line;
                string fileContent = File.ReadAllText(filePath);
                MessageTextBox.Text = fileContent;
                }}*/

        private string GetMatchingEmail(string employeeId)
        {
            DataBaseContext db = new DataBaseContext();

            // Check if the employeeId exists in the database
            if (db.DataBases.Any(db => db.SomeText == employeeId))
            {
                // Retrieve the matching email from the database
                string email = db.DataBases.First(db => db.SomeText == employeeId).Date;
                return email;
            }

            return "";
        }
        // Метод получения email-адреса, соответствующего заданному идентификатору сотрудника.
        // Если email-адрес присутствует в словаре emailDictionary, возвращается соответствующее значение.
        // В противном случае возвращается пустая строка.

        private void FileItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listViewItem = (ListViewItem)sender;
            // Получение элемента ListViewItem, на котором произошло двойное нажатие мыши
            var listView = (ListView)ItemsControl.ItemsControlFromItemContainer(listViewItem);
            // Получение родительского ListView из элемента ListViewItem
            if (listView.SelectedItem != null)
            {
                FileData fileData = (FileData)listView.SelectedItem;
                // Получение выбранного элемента FileData из ListView
                fileData.OpenCommand.Execute(null);
                // Выполнение команды OpenCommand, связанной с выбранным элементом FileData
            }
        }
        // Обработчик события двойного щелчка мыши на элементе списка файлов (FileItem).
        // Получает выбранный элемент списка, выполняет команду OpenCommand для выбранного элемента.

        // Функция для замены недопустимых символов в имени файла
        public static string GetValidFileName(string fileName)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c.ToString(), "_");
            }
            return fileName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (FileData fileData in listView.SelectedItems)
                {
                    MailAddress from = new MailAddress(login.Text, login.Text);
                    MailAddress to = new MailAddress(fileData.Email);
                    MailMessage m = new MailMessage(from, to);
                    m.Subject = subject.Text;
                    m.Body = fileData.Message;
                    m.IsBodyHtml = false;
                    SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);
                    smtp.Credentials = new NetworkCredential(login.Text, pass.Password);
                    smtp.EnableSsl = true;
                    smtp.Send(m);
                }

                MessageBox.Show("Email sent successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email: " + ex.Message);
            }
        }

        private void SendEmailButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("В процессе");
        }

        private void ClickButtonAddMail(object sender, RoutedEventArgs e)
        {
            Email_Window email_Window = new();
            email_Window.ShowDialog();
        }

    }
}

