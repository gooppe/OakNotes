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
    /// Логика взаимодействия для EditCategory.xaml
    /// </summary>
    public partial class EditCategoryWindow : Window
    {
        public String CategoryName { get => CategoryNameTextBox.Text; }

        public EditCategoryWindow(Category category, Window owner)
        {
            InitializeComponent();
            CategoryNameTextBox.Text = category.Name;
            CategoryNameTextBox.SelectAll();
            CategoryNameTextBox.Focus();
            Owner = owner;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
