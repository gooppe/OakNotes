using OakNotes.Model;
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
    /// Логика взаимодействия для NotesControl.xaml
    /// </summary>
    public partial class NotesControl : UserControl, INotifyPropertyChanged
    {
        public delegate void NoteEventHandler(object sender, NoteEventArgs e);

        public event PropertyChangedEventHandler PropertyChanged;
        public event NoteEventHandler OnEditNote;
        public event NoteEventHandler OnCreateNote;
        public event NoteEventHandler OnDeleteNote;
        public event NoteEventHandler OnShareNote;

        private IEnumerable<Note> _notes;
        public IEnumerable<Note> Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }

        public NotesControl()
        {
            InitializeComponent();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (NotesListView.SelectedIndex == -1)
                return;
            OnEditNote?.Invoke(this, new NoteEventArgs(NotesListView.SelectedItem as Note));
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (NotesListView.SelectedIndex == -1)
                return;
            OnDeleteNote?.Invoke(this, new NoteEventArgs(NotesListView.SelectedItem as Note));
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            OnCreateNote?.Invoke(this, new NoteEventArgs(new Note()));
        }

        private void ShareClick(object sender, RoutedEventArgs e)
        {
            if (NotesListView.SelectedIndex == -1)
                return;
            OnShareNote?.Invoke(this, new NoteEventArgs(NotesListView.SelectedItem as Note));
        }
    }

    public class NoteEventArgs : EventArgs
    {
        public Note Note;
        public NoteEventArgs(Note note) => Note = note;
    }
}
