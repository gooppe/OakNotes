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

namespace OakNotes.Client.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateUserWindow.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {
        public string UserName { get => UserNameTextBox.Text.Trim(); }
        public string UserLogin { get => LoginTextBox.Text.Trim(); }

        public CreateUserWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            if (UserName == string.Empty || UserLogin == string.Empty)
            {
                MessageBox.Show("Заполните все поля", "Поля не заполнены", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
        }
    }
}
