using OakNotes.DataLayer;
using OakNotes.DataLayer.Sql;
using OakNotes.Model;
using OakNotes.Logger;
using System;
using System.Web.Http;
using OakNotes.Api.Filters;

namespace OakNotes.Api.Controllers
{
    [RepositoryExceptionFilter]
    public class CategoriesController : ApiController
    {
        private const string _connectionString = @"Data Source=DESKTOP-8E5V1RN\SQLEXPRESS;Database=development;Trusted_connection=true";
        private readonly ICategoriesRepository _categoriesRepository;

        public CategoriesController()
        {
            _categoriesRepository = new CategoriesRepository(_connectionString);
        }
        
        /// <summary>
        /// Delete user category
        /// </summary>
        /// <param name="categoryId">Category id</param>
        [HttpDelete]
        [Route("api/categories/{categoryId}")]
        public void Delete(Guid categoryId)
        {
            Log.Intance.Info("Удаляется категория с id: {0}", categoryId);
            _categoriesRepository.Delete(categoryId);
        }

        /// <summary>
        /// Get category from id
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns>Category</returns>
        [HttpGet]
        [Route("api/categories/{categoryId}")]
        public Category Get(Guid categoryId)
        {
            Log.Intance.Info("Возвращается категория с id: {id}");
            return _categoriesRepository.Get(categoryId);
        }

        /// <summary>
        /// Update category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Category</returns>
        [HttpPut]
        [Route("api/categories")]
        public Category Update([FromBody] Category category)
        {
            Log.Intance.Info("Обновляется категория с id: {0}", category.Id);
            return _categoriesRepository.Update(category);
        }
    }
}
