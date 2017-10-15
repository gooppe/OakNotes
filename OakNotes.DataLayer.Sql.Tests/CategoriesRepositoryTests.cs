using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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
            var userName = "test";
            var categoryName = "test_cat";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var userRepository = new UsersRepository(_connectionString, categoriesRepository);

            var createdUser = userRepository.Create(userName);
            _tempUsers.Add(createdUser.Id);
            var createdCategory = categoriesRepository.Create(createdUser.Id, categoryName);
            var selectedCategory = categoriesRepository.Get(createdCategory.Id);

            //assert
            Assert.AreEqual(selectedCategory.Name, createdCategory.Name);
        }

        [TestMethod]
        public void ShouldDeleteCategory()
        {
            //arrange
            var userName = "test";
            var categoryName = "test_cat";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var userRepository = new UsersRepository(_connectionString, categoriesRepository);

            var createdUser = userRepository.Create(userName);
            _tempUsers.Add(createdUser.Id);
            var createdCategory = categoriesRepository.Create(createdUser.Id, categoryName);

            categoriesRepository.Delete(createdCategory.Id);

            //assert
            Assert.ThrowsException<ArgumentException>(()=>categoriesRepository.Get(createdCategory.Id));
        }

        [TestMethod]
        public void ShouldAssignCategoryToNote()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var categoryName = "Test category";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);
            var note = notesRepository.Create(user, noteTitle, string.Empty);
            var category = categoriesRepository.Create(user.Id, categoryName);
            categoriesRepository.Assign(note.Id, category.Id);

            var selectedCategories = categoriesRepository.GetNoteCategories(note.Id);
            
            //assert
            Assert.AreEqual(category.Name, selectedCategories.Single().Name);
        }

        [TestMethod]
        public void ShouldDissociateCategoryNote()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var categoryName = "Test category";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);
            var note = notesRepository.Create(user, noteTitle, string.Empty);
            var category = categoriesRepository.Create(user.Id, categoryName);
            categoriesRepository.Assign(note.Id, category.Id);
            categoriesRepository.Dissociate(note.Id, category.Id);

            var selectedCategories = categoriesRepository.GetNoteCategories(note.Id);

            //assert
            Assert.IsTrue(!selectedCategories.Any());
        }

        [TestMethod]
        public void ShouldGetNoteCategories()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var categoryName = "Test category";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);
            var notesRepository = new Sql.NotesRepository(_connectionString, usersRepository, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);
            var note = notesRepository.Create(user, noteTitle, string.Empty);
            var category = categoriesRepository.Create(user.Id, categoryName);
            categoriesRepository.Assign(note.Id, category.Id);

            var selectedCategories = categoriesRepository.GetNoteCategories(note.Id);

            //assert
            Assert.AreEqual(category.Name, selectedCategories.Single().Name);
        }

        [TestMethod]
        public void ShouldGetUserCategories()
        {
            //arrange
            var userName = "test";
            var categoryName = "Test category";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);
            var category = categoriesRepository.Create(user.Id, categoryName);

            var userCategories = categoriesRepository.GetUserCategories(user.Id);

            //assert
            Assert.AreEqual(category.Name, userCategories.Single().Name);
        }

        [TestMethod]
        public void ShouldUpdateCategory()
        {
            //arrange
            var userName = "test";
            var noteTitle = "Test Note";
            var categoryName = "Test category";
            var changedCategoryName = "Changed name";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);

            var user = usersRepository.Create(userName);
            _tempUsers.Add(user.Id);
            var category = categoriesRepository.Create(user.Id, categoryName);
            categoriesRepository.Update(category.Id, changedCategoryName);

            var updatedCategory = categoriesRepository.Get(category.Id);

            //assert
            Assert.AreEqual(changedCategoryName, updatedCategory.Name);
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
