using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using OakNotes.Model;

namespace OakNotes.DataLayer.Sql.Tests
{
    [TestClass]
    public class CategoriesRepositoryTests
    {
        private const string _connectionString = @"Data Source=DESKTOP-8E5V1RN\SQLEXPRESS;Database=development;Trusted_connection=true";
        private readonly List<Guid> _tempUsers = new List<Guid>();

        [TestMethod]
        public void ShouldCreateCategory()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };
            var category = new Category()
            {
                Name = "test_cat"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var userRepository = new UsersRepository(_connectionString, categoriesRepository);

            var createdUser = userRepository.Create(user);
            _tempUsers.Add(createdUser.Id);
            var createdCategory = categoriesRepository.Create(category, user.Id);
            var selectedCategory = categoriesRepository.Get(createdCategory.Id);

            //assert
            Assert.AreEqual(selectedCategory.Name, createdCategory.Name);
        }

        [TestMethod]
        public void ShouldDeleteCategory()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };
            var category = new Category()
            {
                Name = "test_cat"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var userRepository = new UsersRepository(_connectionString, categoriesRepository);

            var createdUser = userRepository.Create(user);
            _tempUsers.Add(createdUser.Id);
            var createdCategory = categoriesRepository.Create(category, user.Id);

            categoriesRepository.Delete(createdCategory.Id);

            //assert
            Assert.ThrowsException<ArgumentException>(()=>categoriesRepository.Get(createdCategory.Id));
        }

        [TestMethod]
        public void ShouldAssignCategoryToNote()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };
            var category = new Category()
            {
                Name = "test_cat"
            };
            var note = new Note()
            {
                Title = "test_note",
                Text = "test_text",
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(user.Id);
            note.Owner = user;
            var createdNote = notesRepository.Create(note);
            var createdCategory = categoriesRepository.Create(category, createdUser.Id);
            categoriesRepository.Assign(note.Id, category.Id);

            var selectedCategories = categoriesRepository.GetNoteCategories(note.Id);
            
            //assert
            Assert.AreEqual(category.Name, selectedCategories.Single().Name);
        }

        [TestMethod]
        public void ShouldDissociateCategoryNote()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };
            var category = new Category()
            {
                Name = "test_cat"
            };
            var note = new Note()
            {
                Title = "test_note",
                Text = "test_text",
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(user.Id);
            note.Owner = createdUser; 
            var createdNote = notesRepository.Create(note);
            var createdCategory = categoriesRepository.Create(category, createdUser.Id);
            categoriesRepository.Assign(createdNote.Id, createdCategory.Id);
            categoriesRepository.Dissociate(createdNote.Id, createdCategory.Id);

            var selectedCategories = categoriesRepository.GetNoteCategories(createdNote.Id);

            //assert
            Assert.IsTrue(!selectedCategories.Any());
        }

        [TestMethod]
        public void ShouldGetNoteCategories()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };
            var category = new Category()
            {
                Name = "test_cat"
            };
            var note = new Note()
            {
                Title = "test_note",
                Text = "test_text",
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(user.Id);
            note.Owner = createdUser;
            var createdNote = notesRepository.Create(note);
            var createdCategory = categoriesRepository.Create(category, createdUser.Id);
            categoriesRepository.Assign(createdNote.Id, createdCategory.Id);

            var selectedCategories = categoriesRepository.GetNoteCategories(createdNote.Id);

            //assert
            Assert.AreEqual(category.Name, selectedCategories.Single().Name);
        }

        [TestMethod]
        public void ShouldGetUserCategories()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };
            var category = new Category()
            {
                Name = "test_cat"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(user.Id);
            var createdCategory = categoriesRepository.Create(category, createdUser.Id);

            var userCategories = categoriesRepository.GetUserCategories(createdUser.Id);

            //assert
            Assert.AreEqual(category.Name, userCategories.Single().Name);
        }

        [TestMethod]
        public void ShouldUpdateCategory()
        {
            //arrange
            var user = new User()
            {
                Name = "test"
            };
            var category = new Category()
            {
                Name = "test_cat"
            };

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);

            var createdUser = usersRepository.Create(user);
            _tempUsers.Add(user.Id);
            var createdCategory = categoriesRepository.Create(category, createdUser.Id);
            createdCategory.Name = "Changed Name";
            categoriesRepository.Update(createdCategory);

            var updatedCategory = categoriesRepository.Get(category.Id);

            //assert
            Assert.AreEqual("Changed Name", updatedCategory.Name);
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
