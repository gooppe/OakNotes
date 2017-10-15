using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace OakNotes.DataLayer.Sql.Tests
{
    [TestClass]
    public class NotesRepositoryTests
    {
        private const string _connectionString = @"Data Source=DESKTOP-8E5V1RN\SQLEXPRESS;Database=development;Trusted_connection=true";
        private readonly List<Guid> _tempUsers = new List<Guid>();

        [TestMethod]
        public void ShouldCreateNote()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var noteText = "Test Text";


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);
            var note = notesRepository.Create(user, noteTitle, noteText);
            var createdNote = notesRepository.Get(note.Id);

            //assert
            Assert.AreEqual(note.Title, createdNote.Title);
        }

        [TestMethod]
        public void ShouldDeleteNote()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var noteText = "Test Text";


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);
            var note = notesRepository.Create(user, noteTitle, noteText);
            notesRepository.Delete(note.Id);

            //assert
            Assert.ThrowsException<ArgumentException>(() => notesRepository.Get(note.Id));
        }

        [TestMethod]
        public void ShouldUpdateNote()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var noteText = "Test Text";
            var updatedNoteTitle = "Updated Test Note";
            var updatedNoteText = "Updated Test Text";


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);
            var note = notesRepository.Create(user, noteTitle, noteText);
            notesRepository.Update(note.Id, updatedNoteTitle, updatedNoteText);
            var createdNote = notesRepository.Get(note.Id);

            //assert
            Assert.AreEqual(updatedNoteText, createdNote.Text);
        }

        [TestMethod]
        public void ShouldShareNote()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var noteText = "Test Text";

            var secondUserName = "test2";


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            var secondUser = usersRepository.Create(secondUserName);
            _tempUsers.Add(user.Id);
            _tempUsers.Add(secondUser.Id);

            var note = notesRepository.Create(user, noteTitle, noteText);
            notesRepository.Share(note.Id, secondUser.Id);
            var sharedNotes = notesRepository.GetSharedNotes(secondUser.Id);

            //assert
            Assert.AreEqual(note.Title, sharedNotes.Single().Title);
        }

        [TestMethod]
        public void ShouldUnshareNote()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var noteText = "Test Text";

            var secondUserName = "test2";


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            var secondUser = usersRepository.Create(secondUserName);
            _tempUsers.Add(user.Id);
            _tempUsers.Add(secondUser.Id);

            var note = notesRepository.Create(user, noteTitle, noteText);
            notesRepository.Share(note.Id, secondUser.Id);
            notesRepository.Unshare(note.Id, secondUser.Id);

            var sharedNotes = notesRepository.GetSharedNotes(secondUser.Id);

            //assert
            Assert.IsTrue(!sharedNotes.Any());
        }

        [TestMethod]
        public void ShouldGetSharedNotes()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var noteText = "Test Text";

            var secondUserName = "test2";


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            var secondUser = usersRepository.Create(secondUserName);
            _tempUsers.Add(user.Id);
            _tempUsers.Add(secondUser.Id);

            var note = notesRepository.Create(user, noteTitle, noteText);
            notesRepository.Share(note.Id, secondUser.Id);
            var sharedNotes = notesRepository.GetSharedNotes(secondUser.Id);

            //assert
            Assert.AreEqual(note.Title, sharedNotes.Single().Title);
        }

        [TestMethod]
        public void ShouldGetCategoryNotes()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var noteText = "Test Text";
            var categoryNote = "Test Category";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);

            var note = notesRepository.Create(user, noteTitle, noteText);
            var category = categoriesRepository.Create(user.Id, categoryNote);
            categoriesRepository.Assign(note.Id, category.Id);

            var categoryNotes = notesRepository.GetCategoryNotes(category.Id);

            //assert
            Assert.AreEqual(note.Title, categoryNotes.Single().Title);
        }

        [TestMethod]
        public void ShouldLoadNoteCategories()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var noteText = "Test Text";
            var categoryNote = "Test Category";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);

            var note = notesRepository.Create(user, noteTitle, noteText);
            var category = categoriesRepository.Create(user.Id, categoryNote);
            categoriesRepository.Assign(note.Id, category.Id);
            note = notesRepository.Get(note.Id);

            //assert
            Assert.AreEqual(categoryNote, note.Categories.Single().Name);
        }

        [TestMethod]
        public void ShouldLoadNoteShares()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var noteText = "Test Text";

            var secondUserName = "test2";


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            var secondUser = usersRepository.Create(secondUserName);
            _tempUsers.Add(user.Id);
            _tempUsers.Add(secondUser.Id);

            var note = notesRepository.Create(user, noteTitle, noteText);
            notesRepository.Share(note.Id, secondUser.Id);
            note = notesRepository.Get(note.Id);

            //assert
            Assert.AreEqual(secondUserName, note.Shares.Single().Name);
        }

        [TestCleanup]
        public void CleanData()
        {
            foreach(var user in _tempUsers)
            {
                new UsersRepository(_connectionString, new CategoriesRepository(_connectionString)).Delete(user);
            }
        }
    }
}
