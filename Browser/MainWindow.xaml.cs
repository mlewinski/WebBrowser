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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Speech.Synthesis;

namespace Browser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<WebBrowser> karty = new List<WebBrowser>();
        public int count = 0;
        public bool hasChildren = false;
        public History oknoHistoria;
        public SpeechSynthesizer speechSynth = new SpeechSynthesizer();
        PromptBuilder pb = new PromptBuilder();
        public StreamReader sr;
        bool Accessibility;
        public MainWindow()
        {
            InitializeComponent();
            updateHistory();
        }
        public void updateHistory()
        {
            sr = new StreamReader(@"history.txt");
            txtAddress.Items.Clear();
            if (File.Exists("history.txt"))
            {              
                string line;
                string[] parts = new string[2];
                while ((line = sr.ReadLine()) != null)
                {
                    parts = Regex.Split(line, "@separator@");
                    //MessageBox.Show(parts[0]);
                    txtAddress.Items.Add(parts[2]);
                    txtAddress.Items.Refresh();
                }
            }
            sr.Close();
        }
        public void newCard(object sender, RoutedEventArgs e)
        {
            WebBrowser wb = new WebBrowser();
            karty.Add(wb);
            TabItem nowaKarta = new TabItem();
            this.count++;
            nowaKarta.Header = "Nowa karta";
            Grid newItem = new Grid();
            txtAddress.Text = string.Empty;
            tabKarty.SelectedIndex = count;
            newItem.Children.Add(wb);
            nowaKarta.Content = newItem;
            tabKarty.Items.Add(nowaKarta);
            tabKarty.SelectedIndex = count;
        }

        private void closeApp(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            string path;
            if ((!txtAddress.Text.Contains("://")))
            {
                path = "http://" + txtAddress.Text;
            }
            else path = txtAddress.Text;
            try
            {
                WebRequest task = WebRequest.Create(path);
                task.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse reply = (HttpWebResponse)task.GetResponse();
                string s = reply.ContentEncoding;
                Stream stream = reply.GetResponseStream();
                StreamReader sr = new StreamReader(stream, Encoding.Default);
                string text = sr.ReadToEnd();
                Regex ex = new Regex(@"(?<=<title.*>)([\s\S]*)(?=</title>)", RegexOptions.IgnoreCase);
                string title=ex.Match(text).Value.Trim();
                Window1.Title = title;
                ((TabItem)tabKarty.Items[tabKarty.SelectedIndex]).Header = ex.Match(text).Value.Trim();
                karty[tabKarty.SelectedIndex].Source = new Uri(path);
                if (!File.Exists("history.txt"))
                {
                    File.CreateText("history.txt").Close();
                }
                try
                {
                    File.AppendAllText("history.txt", DateTime.Now.ToString() + "@separator@" + title + "@separator@" + path + Environment.NewLine);
                }
                catch (Exception exs)
                {
                    MessageBox.Show(exs.Message);
                }
                this.oknoHistoria.updateList();
                updateHistory();
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1) newCard(sender, e);
            if (e.Key == Key.F2)
            {
                closeCard(sender, e);
            }
        }

        private void txtAddress_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btnGo_Click(sender, e);
        }
        private void closeCard(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = tabKarty.SelectedIndex;
                if (count == 1)
                {
                    Window1.Title = "Przeglądarka";
                    txtAddress.Text = "";
                }
                tabKarty.Items.Remove(tabKarty.Items[x]);
                karty.Remove(karty[x]);
                count--;
            }
            catch (Exception ex) { }
        }

        public void tabKarty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                Window1.Title = ((TabItem)tabKarty.Items[tabKarty.SelectedIndex]).Header.ToString();
                txtAddress.Text = karty[tabKarty.SelectedIndex].Source.ToString();
            }
            catch (Exception ex) {
                txtAddress.Text = string.Empty;
                
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                karty[tabKarty.SelectedIndex].GoBack();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                karty[tabKarty.SelectedIndex].GoForward();
            }
            catch (Exception ex)
            {

            }            
        }

        private void historyShow(object sender, RoutedEventArgs e)
        {
            if (!this.hasChildren)
            {
                this.oknoHistoria = new History(this);
                this.oknoHistoria.Show();
                this.hasChildren = true;
            }
        }

        private void Window1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                speechSynth.Dispose();
            }
            catch (Exception ex) { }
            finally
            {
                Environment.Exit(0);
            }
        }

        private void cbInwalida_Checked(object sender, RoutedEventArgs e)
        {
            this.Accessibility = true;
        }

        private void cbInwalida_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Accessibility = false;
        }
        public void say(string message)
        {
            if (this.Accessibility)
            {
                pb.ClearContent();
                pb.AppendText(message);
                speechSynth.Speak(pb);
            }
        }

        private void btnBack_MouseEnter(object sender, MouseEventArgs e)
        {
            this.say("Wstecz");
        }

        private void btnForward_MouseEnter(object sender, MouseEventArgs e)
        {
            this.say("Dalej");
        }

        private void btnGo_MouseEnter(object sender, MouseEventArgs e)
        {
            this.say("Przejdź do strony");
        }

        private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            this.say("Opcje");
        }

        private void MenuItem_MouseEnter_1(object sender, MouseEventArgs e)
        {
            this.say("Włącz ułatwienia dla niepełnosprawnych");
        }

        private void MenuItem_MouseEnter_2(object sender, MouseEventArgs e)
        {
            this.say("Plik");
        }

        private void MenuItem_MouseEnter_3(object sender, MouseEventArgs e)
        {
            this.say("Nowa karta");
        }

        private void MenuItem_MouseEnter_4(object sender, MouseEventArgs e)
        {
            this.say("Historia przeglądania");
        }

        private void MenuItem_MouseEnter_5(object sender, MouseEventArgs e)
        {
            this.say("Zamknij aplikację");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!this.hasChildren)
            {
                try
                {
                    Source wnd = new Source(this, karty[tabKarty.SelectedIndex].Source.ToString());
                    this.hasChildren = true;
                    wnd.Show();
                }
                catch (Exception)
                {
                    
                };
            }
        }

        private void MenuItem_MouseEnter_6(object sender, MouseEventArgs e)
        {
            this.say("Pokaż źródło strony");
        }

        private void btnRefresh_MouseEnter(object sender, MouseEventArgs e)
        {
            this.say("Odśwież");
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            karty[tabKarty.SelectedIndex].Refresh();
        }

        private void MenuItem_MouseEnter_7(object sender, MouseEventArgs e)
        {
            this.say("Autor");
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Marek Lewiński", "Autor", MessageBoxButton.OK, MessageBoxImage.Information);
            this.say("Autor : Marek Lewiński");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            newCard(sender, e);
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            this.say("Nowa karta");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.closeCard(sender, e);
        }

        private void Button_MouseEnter_1(object sender, MouseEventArgs e)
        {
            this.say("Zamknij kartę");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            historyShow(sender, e);
        }

        private void Button_MouseEnter_2(object sender, MouseEventArgs e)
        {
            this.say("Historia przeglądania");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (!this.hasChildren)
            {
                email wnd = new email(this);
                wnd.Show();
                this.hasChildren = true;
            }
        }
        private void Button_MouseEnter_3(object sender, MouseEventArgs e)
        {
            this.say("Wyślij email");
        }

        private void MenuItem_MouseEnter_8(object sender, MouseEventArgs e)
        {
            this.say("Ustawienia");
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (!this.hasChildren)
            {
                accessibilitySettings wnd = new accessibilitySettings(this);
                wnd.Show();
                this.hasChildren = true;
            }
        }
    }
}
