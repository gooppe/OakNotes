using OakNotes.DataLayer;
using OakNotes.DataLayer.Sql;
using OakNotes.Model;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace OakNotes.Api.Controllers
{
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
            return _categoriesRepository.Update(category);
        }
    }
}
