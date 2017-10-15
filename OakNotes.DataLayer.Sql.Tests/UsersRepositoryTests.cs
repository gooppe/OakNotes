using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using OakNotes.Model;
using System.Linq;

namespace OakNotes.DataLayer.Sql.Tests
{
    [TestClass]
    public class UsersRepositoryTests
    {
        private const string _connectionString = @"Data Source=DESKTOP-8E5V1RN\SQLEXPRESS;Database=development;Trusted_connection=true";
        private readonly List<Guid> _tempUsers = new List<Guid>();

        [TestMethod]
        public void ShouldCreateAndGetUser()
        {
            //arrange
            var user = new User
            {
                Name = "test"
            };

            //act
            var userRepository = new UsersRepository(_connectionString, new CategoriesRepository(_connectionString));
            user = userRepository.Create(user.Name);
            _tempUsers.Add(user.Id);
            var createdUser = userRepository.Get(user.Id);

            //assert
            Assert.AreEqual(user.Name, createdUser.Name);
        }

        [TestMethod]
        public void ShouldDeleteUser()
        {
            //arrange
            string userName = "test";

            //act
            var userRepository = new UsersRepository(_connectionString, new CategoriesRepository(_connectionString));
            var user = userRepository.Create(userName);
            _tempUsers.Add(user.Id);
            userRepository.Delete(user.Id);
            
            //assert
            Assert.ThrowsException<ArgumentException>(()=>userRepository.Get(user.Id));
        }

        [TestMethod]
        public void ShouldCreateUserAndAddCategory()
        {
            //arrange
            string userName = "test";
            string category = "MyFirstCategory";

            //act
            var categoriesRepository = new CategoriesRepository(_connectionString);
            var usersRepository = new UsersRepository(_connectionString, categoriesRepository);

            var createdUser = usersRepository.Create(userName);
            _tempUsers.Add(createdUser.Id);
            var createdCategory = categoriesRepository.Create(createdUser.Id, category);
            createdUser = usersRepository.Get(createdUser.Id);

            //assert
            Assert.AreEqual(createdCategory.Name, createdUser.Categories.Single().Name);
        }

        [TestCleanup]
        public void CleanData()
        {
            foreach (var user in _tempUsers)
            {
                new UsersRepository(_connectionString, new CategoriesRepository(_connectionString)).Delete(user);
            }
        }
    }
}
