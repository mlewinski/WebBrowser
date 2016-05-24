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
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace Browser
{
    /// <summary>
    /// Interaction logic for email.xaml
    /// </summary>
    public partial class email : Window
    {
        MainWindow parent;
        public email(MainWindow parent)
        {
            this.parent = parent;
            InitializeComponent();
            txtPass.PasswordChar = 'a';
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.parent.hasChildren = false;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            parent.say("Wyślij wiadomość");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MailAddress To, From, Cc = null;
            SmtpClient client;
            try
            {
                From = new MailAddress(txtFrom.Text);
            }
            catch{
                parent.say("Nieprawidłowy adres nadawcy");
                MessageBox.Show("Nieprawidłowy adres nadawcy!");
                txtTo.Text = string.Empty;
                return;
            }
            try
            {
                To = new MailAddress(txtTo.Text);
            }
            catch
            {
                parent.say("Nieprawidłowy adres odbiorcy");
                MessageBox.Show("Nieprawidłowy adres odbiorcy");
                txtTo.Text = string.Empty;
                return;
            }
            if (txtCc.Text != String.Empty)
            {
                if (txtCc.Text.Trim().Length > 0)
                {
                    try
                    {
                        Cc = new MailAddress(txtCc.Text);
                    }
                    catch
                    {
                        parent.say("Nieprawidłowe adresy odbiorców");
                        MessageBox.Show("Nieprawidłowe adresy odbiorców");
                        txtCc.Text = string.Empty;
                        return;
                    }
                }
            }
            MailMessage message = new MailMessage(From, To);
            message.Subject = txtSubject.Text;
            message.Body = txtBody.Text;
            message.IsBodyHtml = true;
            if (Cc != null) message.CC.Add(Cc);
            try
            {
                client = new SmtpClient(txtServer.Text);
                if (txtPort != null) client.Port = int.Parse(txtPort.Text);
                if (txtUser.Text != String.Empty && txtPass.Password != String.Empty)
                {
                    client.Credentials = new NetworkCredential(txtUser.Text, txtPass.Password);
                }
                client.EnableSsl = true;
                client.Send(message);
            }
            catch
            {
                parent.say("Brak połączenia z serwerem!");
                MessageBox.Show("Brak połączenia z serwerem!");
            }
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
