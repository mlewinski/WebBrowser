using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Browser
{
    /// <summary>
    /// Interaction logic for Source.xaml
    /// </summary>
    public partial class Source : Window
    {
        public MainWindow parent;
        public Source(MainWindow mw,string source)
        {
            this.parent = mw;
            InitializeComponent();
            this.UpdateSource(source);
        }

        public void UpdateSource(string source)
        {
            try
            {
                rtbSource.Document.Blocks.Clear();
                WebRequest task = WebRequest.Create(source);
                task.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse reply = (HttpWebResponse)task.GetResponse();
                string s = reply.ContentEncoding;
                Stream stream = reply.GetResponseStream();
                StreamReader sr = new StreamReader(stream, Encoding.Default);
                string text = sr.ReadToEnd();
                Regex ex = new Regex(@"(?<=<title.*>)([\s\S]*)(?=</title>)", RegexOptions.IgnoreCase);
                string title = ex.Match(text).Value.Trim();
                this.Title = title;
                rtbSource.AppendText(text);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            parent.hasChildren = false;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
