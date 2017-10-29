using OakNotes.DataLayer;
using OakNotes.DataLayer.Sql;
using OakNotes.Model;
using System;
using System.Collections.Generic;
using System.Web.Http;
using OakNotes.Logger;
using OakNotes.Api.Filters;

namespace OakNotes.Api.Controllers
{
    [RepositoryExceptionFilter]
    public class UsersController : ApiController
    {
        private const string _connectionString = @"Data Source=DESKTOP-8E5V1RN\SQLEXPRESS;Database=development;Trusted_connection=true";
        private readonly IUsersRepository _usersRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly INotesRepository _notesRepository;

        public UsersController()
        {
            _categoriesRepository = new CategoriesRepository(_connectionString);
            _usersRepository = new UsersRepository(_connectionString, _categoriesRepository);
            _notesRepository = new NotesRepository(_connectionString, _usersRepository, _categoriesRepository);
        }

        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User</returns>
        [HttpGet]
        [Route("api/users/{id}")]
        public User Get(Guid id)
        {
            Log.Intance.Info("Возвращается пользователь c id: {0}", id);
            return _usersRepository.Get(id);
        }

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Created User</returns>
        [HttpPost]
        [Route("api/users")]
        public User Post([FromBody] User user)
        {
            Log.Intance.Info("Создание пользователя с именем: {0}", user.Name);
            return _usersRepository.Create(user);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id">User id</param>
        [HttpDelete]
        [Route("api/users/{id}")]
        public void Delete(Guid id)
        {
            Log.Intance.Info("Удаляется пользователь с id: {0}", id);
            _usersRepository.Delete(id);
        }

        /// <summary>
        /// Get user categories
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User categories</returns>
        [HttpGet]
        [Route("api/users/{id}/categories")]
        public IEnumerable<Category> GetUserCategories(Guid id)
        {
            Log.Intance.Info("Возвращается список категорий пользователя с id: {0}", id);
            return _categoriesRepository.GetUserCategories(id);
        }

        /// <summary>
        /// Create user category
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="category">Category id</param>
        /// <returns>Created category</returns>
        [HttpPost]
        [Route("api/users/{userId}/categories")]
        public Category Create(Guid userId, Category category)
        {
            Log.Intance.Info("Создается категория: \"{0}\" для пользователя с id: {1}", category.Name, userId);
            return _categoriesRepository.Create(category, userId);
        }

        /// <summary>
        /// Get all notes, shared for user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Notes</returns>
        [HttpGet]
        [Route("api/users/{userId}/shared")]
        public IEnumerable<Note> GetSharedNotes(Guid userId)
        {
            Log.Intance.Info("Возвращается список записей пользователя с id: {0}", userId);
            return _notesRepository.GetSharedNotes(userId);
        }

        /// <summary>
        /// Get user notes
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Notes</returns>
        [HttpGet]
        [Route("api/users/{userId}/notes")]
        public IEnumerable<Note> GetUserNotes(Guid userId)
        {
            Log.Intance.Info("Возвращается список записей пользователя c id: {0}", userId);
            return _notesRepository.GetUserNotes(userId);
        }
    }
}
