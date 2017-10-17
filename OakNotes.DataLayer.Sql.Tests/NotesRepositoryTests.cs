using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using OakNotes.Model;

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
            var user = new User()
            {
                Name = "test"
            };

            var note = new Note()
            {
                Title = "Test note",
                Text = "Test text"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(createdUser.Id);
            note.Owner = createdUser;
            var createdNote = notesRepository.Create(note);
            var selectedNote = notesRepository.Get(createdNote.Id);

            //assert
            Assert.AreEqual(note.Title, selectedNote.Title);
        }

        [TestMethod]
        public void ShouldDeleteNote()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };

            var note = new Note()
            {
                Title = "Test note",
                Text = "Test text"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(createdUser.Id);
            note.Owner = createdUser;
            var createdNote = notesRepository.Create(note);
            notesRepository.Delete(createdNote.Id);

            //assert
            Assert.ThrowsException<ArgumentException>(() => notesRepository.Get(createdNote.Id));
        }

        [TestMethod]
        public void ShouldUpdateNote()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };

            var note = new Note()
            {
                Title = "Test note",
                Text = "Test text"
            };

            var updatedNote = new Note()
            {
                Title = "Updated Test note",
                Text = "Updated Test text"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(createdUser.Id);
            note.Owner = createdUser;
            var createdNote = notesRepository.Create(note);
            updatedNote.Id = createdNote.Id;
            notesRepository.Update(updatedNote);
            var selectedNote = notesRepository.Get(updatedNote.Id);

            //assert
            Assert.AreEqual(updatedNote.Text, selectedNote.Text);
        }

        [TestMethod]
        public void ShouldShareNote()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };

            var note = new Note()
            {
                Title = "Test note",
                Text = "Test text"
            };

            var secondUser = new User()
            {
                Name = "second user"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            var createdSecondUser = usersRepository.Create(secondUser);
            _tempUsers.Add(createdUser.Id);
            _tempUsers.Add(createdSecondUser.Id);
            note.Owner = user;
            var createdNote = notesRepository.Create(note);
            notesRepository.Share(createdNote.Id, createdSecondUser.Id);
            var sharedNotes = notesRepository.GetSharedNotes(createdSecondUser.Id);

            //assert
            Assert.AreEqual(note.Title, sharedNotes.Single().Title);
        }

        [TestMethod]
        public void ShouldUnshareNote()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };

            var note = new Note()
            {
                Title = "Test note",
                Text = "Test text"
            };

            var secondUser = new User()
            {
                Name = "second user"
            };


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            var createdSecondUser = usersRepository.Create(secondUser);
            _tempUsers.Add(createdUser.Id);
            _tempUsers.Add(createdSecondUser.Id);

            note.Owner = createdUser;
            var createdNote = notesRepository.Create(note);
            notesRepository.Share(createdNote.Id, createdSecondUser.Id);
            notesRepository.Unshare(createdNote.Id, createdSecondUser.Id);

            var sharedNotes = notesRepository.GetSharedNotes(createdSecondUser.Id);

            //assert
            Assert.IsTrue(!sharedNotes.Any());
        }

        [TestMethod]
        public void ShouldGetSharedNotes()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };

            var note = new Note()
            {
                Title = "Test note",
                Text = "Test text"
            };

            var secondUser = new User()
            {
                Name = "second user"
            };


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            var createdSecondUser = usersRepository.Create(secondUser);
            _tempUsers.Add(createdUser.Id);
            _tempUsers.Add(createdSecondUser.Id);

            note.Owner = createdUser;
            var createdNote = notesRepository.Create(note);
            notesRepository.Share(createdNote.Id, createdSecondUser.Id);
            var sharedNotes = notesRepository.GetSharedNotes(createdSecondUser.Id);

            //assert
            Assert.AreEqual(note.Title, sharedNotes.Single().Title);
        }

        [TestMethod]
        public void ShouldGetCategoryNotes()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };

            var note = new Note()
            {
                Title = "Test note",
                Text = "Test text"
            };

            var category = new Category()
            {
                Name = "Test Category"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(createdUser.Id);

            note.Owner = createdUser;
            var createdNote = notesRepository.Create(note);
            var createdCategory = categoriesRepository.Create(category, createdUser.Id);
            categoriesRepository.Assign(createdNote.Id, createdCategory.Id);

            var categoryNotes = notesRepository.GetCategoryNotes(createdCategory.Id);

            //assert
            Assert.AreEqual(note.Title, categoryNotes.Single().Title);
        }

        [TestMethod]
        public void ShouldLoadNoteCategories()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };

            var note = new Note()
            {
                Title = "Test note",
                Text = "Test text"
            };

            var category = new Category()
            {
                Name = "Test Category"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(createdUser.Id);

            note.Owner = createdUser;
            var createdNote = notesRepository.Create(note);
            var createdCategory = categoriesRepository.Create(category, createdUser.Id);
            categoriesRepository.Assign(createdNote.Id, createdCategory.Id);
            note = notesRepository.Get(createdNote.Id);

            //assert
            Assert.AreEqual(category.Name, note.Categories.Single().Name);
        }

        [TestMethod]
        public void ShouldLoadNoteShares()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };

            var note = new Note()
            {
                Title = "Test note",
                Text = "Test text"
            };

            var secondUser = new User()
            {
                Name = "second user"
            };


            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            var createdSecondUser = usersRepository.Create(secondUser);
            _tempUsers.Add(createdUser.Id);
            _tempUsers.Add(createdSecondUser.Id);

            note.Owner = createdUser;
            var createdNote = notesRepository.Create(note);
            notesRepository.Share(createdNote.Id, createdSecondUser.Id);
            note = notesRepository.Get(createdNote.Id);

            //assert
            Assert.AreEqual(secondUser.Name, note.Shares.Single().Name);
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
