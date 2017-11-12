using OakNotes.Client.Controls;
using OakNotes.Client.Windows;
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

namespace OakNotes.Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ServiceClient _apiClient;
        public event PropertyChangedEventHandler PropertyChanged;

        private User _user;
        private IEnumerable<Note> _notes;
        private IEnumerable<Note> _sharedNotes;

        private bool _authenticated = false;
        public bool Authenticated { get { return _authenticated; } private set { _authenticated = value; OnPropertyChanged(nameof(Authenticated)); } }
        public bool ShowError { get; set; } = false;
        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                ShowError = value.Trim() != string.Empty;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(ShowError));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _apiClient = new ServiceClient("http://localhost:63960/api/");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateUser()
        {
            if (Authenticated)
            {
                _user = _apiClient.GetUser(_user.Id);
                _notes = _apiClient.GetUserNotes(_user);
                _sharedNotes = _apiClient.GetSharedNotes(_user);
                ProfilePanel.User = _user;
                NotesPanel.Notes = _notes.Concat(_sharedNotes);
            }
        }

        private void DisplayError(string Error)
        {
            ErrorMessage = Error;
        }

        private void ClearError()
        {
            ErrorMessage = string.Empty;
        }

        private void ClearError(object sender, RoutedEventArgs e)
        {
            ClearError();
        }

        private void LoginClick(object sender, LoginEventArgs e)
        {
            try
            {
                //Guid id = Guid.Parse(e.Login);
                _user = _apiClient.GetUser(e.Login.Trim());
                _notes = _apiClient.GetUserNotes(_user);
                _sharedNotes = _apiClient.GetSharedNotes(_user);

                ProfilePanel.User = _user;
                NotesPanel.Notes = _notes.Concat(_sharedNotes);
                Authenticated = true;
                ClearError();
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void OnUpdateCategory(object source, CategoryEventArgs e)
        {
            EditCategoryWindow categoryWindow = new EditCategoryWindow(e.Category, this);
            if (categoryWindow.ShowDialog() == true)
            {
                e.Category.Name = categoryWindow.CategoryName;
                _apiClient.UpdateCategory(e.Category);
                UpdateUser();
            }
        }

        private void OnCreateCategory(object source, CategoryEventArgs e)
        {
            EditCategoryWindow categoryWindow = new EditCategoryWindow(new Category(), this);
            if (categoryWindow.ShowDialog() == true)
            {
                _apiClient.CreateCategory(new Category() { Name = categoryWindow.CategoryName }, _user);
                UpdateUser();
            }
        }

        private void OnDeleteCategory(object source, CategoryEventArgs e)
        {
            var result = MessageBox.Show(this, $"Вы действительно хотите удалить категорию {e.Category.Name}?", "Удалить категорию", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                _apiClient.DeleteCategory(e.Category);
                UpdateUser();
            }
        }

        private void OnEditNote(object sender, NoteEventArgs e)
        {
            if (e.Note == null)
                return;
            EditNoteWindow editNote = new EditNoteWindow(this, e.Note);
            IEnumerable<Category> oldCategories = e.Note.Categories;
            if (editNote.ShowDialog() == true)
            {
                IEnumerable<Category> newCategories = editNote.Categories;
                foreach(var cat in e.Note.Owner.Categories)
                {
                    if (oldCategories.Any(c => c.Id == cat.Id) && !newCategories.Any(c => c.Id == cat.Id))
                    {
                        _apiClient.DissociateCategory(e.Note, cat);
                    }
                    else if (!oldCategories.Any(c => c.Id == cat.Id) && newCategories.Any(c => c.Id == cat.Id))
                    {
                        _apiClient.AssignCategory(e.Note, cat);
                    }
                }
                Note note = new Note() { Owner = _user, Id = e.Note.Id, Title = editNote.NoteTitle, Text = editNote.NoteText};
                _apiClient.UpdateNote(note);
                UpdateUser();
            }
        }

        private void OnCreateNote(object sender, NoteEventArgs e)
        {

            EditNoteWindow editNote = new EditNoteWindow(this, new Note() { Owner = _user });
            if (editNote.ShowDialog() == true)
            {
                Note note = new Note() { Owner = _user, Title = editNote.NoteTitle, Text = editNote.NoteText, Categories = editNote.Categories };
                _apiClient.CreateUserNote(note);
                UpdateUser();
            }
        }

        private void OnDeleteNote(object sender, NoteEventArgs e)
        {
            var messageBoxResult = MessageBox.Show(this, $"Удалить заметку \"{e.Note.Title}\"?", "Удаление заметки", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.OK)
            {
                _apiClient.DeleteUserNote(e.Note);
                UpdateUser();
            }
        }

        private void OnShareNote(object sender, NoteEventArgs e)
        {
            SharesNoteWindow sharesNoteWindow = new SharesNoteWindow(this, e.Note.Shares);
            if (sharesNoteWindow.ShowDialog() == true)
            {
                if (sharesNoteWindow.RemovedUser != null)
                {
                    var dialogres = MessageBox.Show($"Вы действительно хотите убрать доступ у пользователя {sharesNoteWindow.RemovedUser.Login} к заметке \"{ e.Note.Title }\"?", "Удаление доступа", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (dialogres == MessageBoxResult.Yes)
                    {
                        _apiClient.UnshareNote(e.Note, sharesNoteWindow.RemovedUser);
                        UpdateUser();
                    }
                    OnShareNote(sender, new NoteEventArgs(_notes.Single(n => n.Id == e.Note.Id)));
                    return;
                }

                if (sharesNoteWindow.ShareUserName != string.Empty)
                {
                    User shareUser;
                    try
                    {
                        shareUser = _apiClient.GetUser(sharesNoteWindow.ShareUserName);
                        _apiClient.ShareNote(e.Note, shareUser);
                        UpdateUser();
                        OnShareNote(sender, new NoteEventArgs(_notes.Single(n => n.Id == e.Note.Id)));
                        return;
                    }
                    catch
                    {
                        MessageBox.Show(this, $"Пользователя c логином {sharesNoteWindow.ShareUserName} не существует. Введите коректное имя пользователя", "Введите верное имя пользователя", MessageBoxButton.OK, MessageBoxImage.Error);
                        OnShareNote(sender, e);
                        return;
                    }
                    
                }
            }
        }

        private void OnUpdateUser(object sender, RoutedEventArgs e)
        {
            UpdateUser();
        }

        private void OnLogoutUser(object sender, RoutedEventArgs e)
        {
            Authenticated = false;
            LoginPanel.UserName.SelectAll();
            LoginPanel.UserName.Focus();
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            CreateUserWindow createUserWindow = new CreateUserWindow(this);
            if (createUserWindow.ShowDialog() == true)
            {
                try
                {
                    _user = _apiClient.CreateUser(new User() { Name = createUserWindow.UserName, Login = createUserWindow.UserLogin });
                    
                    Authenticated = true;
                    ClearError();
                    UpdateUser();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Логин занят", MessageBoxButton.OK, MessageBoxImage.Error);
                    RegisterClick(sender, e);
                    return;
                }
            }
        }
    }
}
