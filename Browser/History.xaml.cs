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
using System.Text.RegularExpressions;
using System.Threading;

namespace Browser
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Window
    {
        MainWindow parent;
        public List<historyItem> lista;
        StreamReader sr;
        public History(MainWindow mw)
        {
            this.parent = mw;
            InitializeComponent();
            updateList();
        }

        private void Historia_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            parent.hasChildren = false;
        }

        private void lbHistoria_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            parent.newCard(sender, e);
            parent.karty[parent.count-1].Source = new Uri(lista[lbHistoria.SelectedIndex].path);
            ((TabItem)parent.tabKarty.Items[parent.count - 1]).Header = lista[lbHistoria.SelectedIndex].title;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("history.txt"))
            {
                File.WriteAllText("history.txt", String.Empty);
                Thread.Sleep(100);
                updateList();
            }
            else MessageBox.Show("Nie znaleziono pliku historii przeglądania! Historia przeglądania jest pusta", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void updateList()
        {
            sr = new StreamReader(@"history.txt");
            if (File.Exists("history.txt"))
            {
                this.lista = new List<historyItem>();                
                string line;
                string[] parts = new string[2];
                while ((line = sr.ReadLine()) != null)
                {
                    parts = Regex.Split(line, "@separator@");
                    //MessageBox.Show(parts[0]);
                    lista.Add(new historyItem(parts[0], parts[1], parts[2]));
                }
            }
            else MessageBox.Show("Nie znaleziono pliku historii przeglądania! Historia przeglądania jest pusta", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
            this.lbHistoria.ItemsSource = this.lista;
            sr.Close();
            lbHistoria.Items.Refresh();
        }

        private void Historia_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }

    public class historyItem
    {
        public string title { get; set; }
        public string path { get; set; }
        public string data { get; set; }
        public historyItem(string data,string title, string path)
        {
            this.data = data;
            this.title = title;
            this.path = path;
        }
    }
}
