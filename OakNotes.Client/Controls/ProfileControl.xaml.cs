using OakNotes.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ProfileControl.xaml
    /// </summary>
    public partial class ProfileControl : UserControl, INotifyPropertyChanged
    {
        public delegate void CategoryEventHandler(object source, CategoryEventArgs e);

        public event PropertyChangedEventHandler PropertyChanged;
        public event CategoryEventHandler OnUpdateCategory;
        public event CategoryEventHandler OnDeleteCategory;
        public event CategoryEventHandler OnCreateCategory;
        public event RoutedEventHandler OnUpdateUser;
        public event RoutedEventHandler OnLogoutUser;

        private User _user;
        public User User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                OnPropertyChanged(null);
            }
        }

        public ProfileControl()
        {
            InitializeComponent();

        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateCategoryClick(object sender, RoutedEventArgs e)
        {
            if (CategoryListView.SelectedIndex == -1)
                return;
            OnUpdateCategory?.Invoke(this, new CategoryEventArgs(CategoryListView.SelectedItem as Category));
        }

        private void DeleteCategoryClick(object sender, RoutedEventArgs e)
        {
            if (CategoryListView.SelectedIndex == -1)
                return;
            OnDeleteCategory?.Invoke(this, new CategoryEventArgs(CategoryListView.SelectedItem as Category));
        }

        private void CreateCategoryClick(object sender, RoutedEventArgs e)
        {
            OnCreateCategory?.Invoke(this, new CategoryEventArgs(null));
        }

        private void UpdateClick(object sender, RoutedEventArgs e)
        {
            OnUpdateUser?.Invoke(this, new RoutedEventArgs());
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            OnLogoutUser?.Invoke(this, new RoutedEventArgs());
        }
    }

    public class CategoryEventArgs : EventArgs
    {
        public Category Category;
        public CategoryEventArgs(Category category) => Category = category;
    }
}
