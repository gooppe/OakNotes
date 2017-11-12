using Newtonsoft.Json;
using OakNotes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OakNotes.Client
{
    public class ServiceClient
    {
        private readonly string _connectionString;
        private HttpClient _client;

        public ServiceClient(string connectionString)
        {
            _connectionString = connectionString;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(connectionString);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public User GetUser(Guid userId)
        {
            return _client.GetAsync($"users/{userId}").Result.Content.ReadAsAsync<User>().Result;
        }

        public User GetUser(string login)
        {
            var response = _client.GetAsync($"users/login/{login}").Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException($"Пользователя с именем \"{login}\" не существует");
            }
            return response.Content.ReadAsAsync<User>().Result;
        }

        public User CreateUser(User user)
        {
            var response = _client.PostAsJsonAsync($"users", user).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException($"Логин {user.Login} занят");
            }
            return response.Content.ReadAsAsync<User>().Result;
        }

        public Category CreateCategory(Category category, User owner)
        {
            //var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
            //return _client.PostAsync($"users/{owner.Id}/categories", content).Result.Content.ReadAsAsync<Category>().Result;
            return _client.PostAsJsonAsync($"users/{owner.Id}/categories", category).Result.Content.ReadAsAsync<Category>().Result;
        }

        public bool DeleteCategory(Category category)
        {
            return _client.DeleteAsync($"categories/{category.Id}").Result.IsSuccessStatusCode;
        }

        public Category UpdateCategory(Category category)
        {
            var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
            return _client.PutAsync($"categories", content).Result.Content.ReadAsAsync<Category>().Result;
        }

        public Note CreateUserNote(Note note)
        {
            IEnumerable<Category> categories = note.Categories;
            note = _client.PostAsJsonAsync($"notes", note).Result.Content.ReadAsAsync<Note>().Result;
            
            foreach(Category cat in categories)
            {
                AssignCategory(note, cat);
            }

            return note;
        }

        public IEnumerable<Note> GetUserNotes(User owner)
        {
            return _client.GetAsync($"users/{owner.Id}/notes").Result.Content.ReadAsAsync<IEnumerable<Note>>().Result;
        }

        public bool DeleteUserNote(Note note)
        {
            return _client.DeleteAsync($"notes/{note.Id}").Result.IsSuccessStatusCode;
        }

        public Note UpdateNote(Note note)
        {
            return _client.PutAsJsonAsync($"notes", note).Result.Content.ReadAsAsync<Note>().Result;
        }

        public bool AssignCategory(Note note, Category category)
        {
            return _client.PostAsync($"notes/{note.Id}/categories/{category.Id}", null).Result.IsSuccessStatusCode;
        }

        public bool DissociateCategory(Note note, Category category)
        {
            return _client.DeleteAsync($"notes/{note.Id}/categories/{category.Id}").Result.IsSuccessStatusCode;
        }

        public bool UnshareNote(Note note, User user)
        {
            return _client.DeleteAsync($"notes/{note.Id}/shares/{user.Id}").Result.IsSuccessStatusCode;
        }

        public bool ShareNote(Note note, User user)
        {
            return _client.PostAsync($"notes/{note.Id}/shares/{user.Id}", null).Result.IsSuccessStatusCode;
        }

        public IEnumerable<Note> GetSharedNotes(User user)
        {
            return _client.GetAsync($"users/{user.Id}/shared").Result.Content.ReadAsAsync<IEnumerable<Note>>().Result;
        }
    }
}
