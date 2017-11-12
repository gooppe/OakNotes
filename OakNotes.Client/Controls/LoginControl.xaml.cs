using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace OakNotes.Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl, INotifyPropertyChanged
    {
        public delegate void LoginEventHandler(object source, LoginEventArgs e);

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public event LoginEventHandler LoginClick;
        public event PropertyChangedEventHandler PropertyChanged;
        public event RoutedEventHandler RegisterClick;

        public LoginControl()
        {
            InitializeComponent();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            LoginClick?.Invoke(this, new LoginEventArgs(UserName.Text, Password.Password));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RegisterButtonClick(object sender, RoutedEventArgs e)
        {
            RegisterClick?.Invoke(this, new RoutedEventArgs());
        }
    }

    public class LoginEventArgs : EventArgs
    {
        public string Login;
        public string Password;

        public LoginEventArgs(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
