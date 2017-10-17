using OakNotes.DataLayer;
using OakNotes.DataLayer.Sql;
using OakNotes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OakNotes.Api.Controllers
{
    public class NotesController : ApiController
    {
        private const string _connectionString = @"Data Source=DESKTOP-8E5V1RN\SQLEXPRESS;Database=development;Trusted_connection=true";
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly INotesRepository _notesRepository;
        private readonly IUsersRepository _usersRepository;

        public NotesController()
        {
            _categoriesRepository = new CategoriesRepository(_connectionString);
            _usersRepository = new UsersRepository(_connectionString, _categoriesRepository);
            _notesRepository = new NotesRepository(_connectionString, _usersRepository, _categoriesRepository);
        }

        /// <summary>
        /// Create new Note
        /// </summary>
        /// <param name="note">Note</param>
        /// <returns>Created note</returns>
        [HttpPost]
        [Route("api/notes")]
        public Note Create([FromBody] Note note)
        {
            return _notesRepository.Create(note);
        }

        /// <summary>
        /// Get note by id
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <returns>Note</returns>
        [HttpGet]
        [Route("api/notes/{noteId}")]
        public Note Get(Guid noteId)
        {
            return _notesRepository.Get(noteId);
        }

        /// <summary>
        /// Delete note by id
        /// </summary>
        /// <param name="noteId">Note id</param>
        [HttpDelete]
        [Route("api/notes/{noteId}")]
        public void Delete(Guid noteId)
        {
            _notesRepository.Delete(noteId);
        }

        /// <summary>
        /// Update note
        /// </summary>
        /// <param name="note">Note</param>
        /// <returns>Updated note</returns>
        [HttpPut]
        [Route("api/notes")]
        public Note Update([FromBody] Note note)
        {
            return _notesRepository.Update(note);
        }

        /// <summary>
        /// Set category of note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <param name="categoryId">Category Id</param>
        [HttpPost]
        [Route("api/notes/{noteId}/categories/{categoryId}")]
        public void AssignNoteCategory(Guid noteId, Guid categoryId)
        {
            _categoriesRepository.Assign(noteId, categoryId);
        }

        /// <summary>
        /// Remove category from note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <param name="categoryId">Category id</param>
        [HttpDelete]
        [Route("api/notes/{noteId}/categories/{categoryId}")]
        public void DissociateCategoryFromNote(Guid noteId, Guid categoryId)
        {
            _categoriesRepository.Dissociate(noteId, categoryId);
        }

        /// <summary>
        /// Get all categories of note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <returns>Note categories</returns>
        [HttpGet]
        [Route("api/notes/{noteId}/categories")]
        public IEnumerable<Category> GetNoteCategories(Guid noteId)
        {
            return _categoriesRepository.GetNoteCategories(noteId);
        }

        /// <summary>
        /// Share note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <param name="userId">User id</param>
        [HttpPost]
        [Route("api/notes/{noteId}/shares/{userId}")]
        public void ShareNote(Guid noteId, Guid userId)
        {
            _notesRepository.Share(noteId, userId);
        }

        /// <summary>
        /// Unshare note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <param name="userId">User id</param>
        [HttpDelete]
        [Route("api/notes/{noteId}/shares/{userId}")]
        public void UnshareNote(Guid noteId, Guid userId)
        {
            _notesRepository.Unshare(noteId, userId);
        }

        /// <summary>
        /// Get shares of note
        /// </summary>
        /// <param name="noteId">Note id</param>
        [HttpGet]
        [Route("api/notes/{noteId}/shares")]
        public IEnumerable<User> GetShares(Guid noteId)
        {
            return _usersRepository.GetNoteShares(noteId);
        }
    }
}
