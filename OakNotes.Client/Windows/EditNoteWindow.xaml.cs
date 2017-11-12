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
using System.Windows.Shapes;

namespace OakNotes.Client.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditNoteWindow.xaml
    /// </summary>
    public partial class EditNoteWindow : Window
    {
        public User NoteOwner;

        public String NoteTitle
        {
            get { return NoteNameTextBox.Text; }
            set { NoteNameTextBox.Text = value; }
        }

        public String NoteText
        {
            get { return NoteTextTextBox.Text; }
            set { NoteTextTextBox.Text = value; }
        }

        private ObservableCollection<ExtendedViewCategory> _categories { get; set; }

        public IEnumerable<Category> Categories
        {
            get { return _categories.Where(cat => cat.Selected).Select(cat => new Category() { Id = cat.Id, Name = cat.Name }); }
        }

        public EditNoteWindow(Window owner, Note note)
        {
            InitializeComponent();
            try
            {
                Owner = owner;
                NoteTitle = note.Title;
                NoteText = note.Text;
                _categories = new ObservableCollection<ExtendedViewCategory>(note.Owner.Categories.Select(cat =>
                new ExtendedViewCategory()
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    Selected = note.Categories != null && note.Categories.Any(nc => nc.Id == cat.Id)
                }));
                CategoriesListView.ItemsSource = _categories;
                CreatedDateLabel.Content = "Создано: " + note.Created.ToString("dd MMM yyyy HH:mm:ss");
                UpdatedDateLabel.Content = "Отредактировано: " + note.Updated.ToString("dd MMM yyyy HH:mm:ss");
            }
            catch (Exception e)
            {

            }
            
            NoteNameTextBox.Focus();
            NoteNameTextBox.CaretIndex = int.MaxValue;
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
