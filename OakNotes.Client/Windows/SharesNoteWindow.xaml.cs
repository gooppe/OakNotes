using OakNotes.Model;
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
    /// Логика взаимодействия для SharesNoteWindow.xaml
    /// </summary>
    public partial class SharesNoteWindow : Window
    {
        public string ShareUserName
        {
            get => UserNameTextBox.Text.Trim();
        }

        public User RemovedUser
        {
            get;
            private set;
        } = null;

        public SharesNoteWindow(Window owner, IEnumerable<User> users)
        {
            InitializeComponent();
            Owner = owner;
            SharesListView.ItemsSource = users;
        }
        
        private void RemoveShareClick(object sender, RoutedEventArgs e)
        {
            RemovedUser = SharesListView.SelectedItem as User;
            DialogResult = true;
        }

        private void ShareClick(object sender, RoutedEventArgs e)
        {
            if (ShareUserName == string.Empty)
            {
                MessageBox.Show(this, "Введите логин пользователя", "Не введено имя пользователя", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
