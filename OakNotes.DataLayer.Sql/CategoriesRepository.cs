using System;
using System.Collections.Generic;
using OakNotes.Model;
using System.Data.SqlClient;

namespace OakNotes.DataLayer.Sql
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly string _connectionString;

        public CategoriesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Create new category of notes for user
        /// </summary>
        /// <param name="userId">Category owner</param>
        /// <param name="name">Name of category</param>
        /// <returns></returns>
        public Category Create(Guid userId, string name)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };

                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "insert into categories (id, userid, name) values (@id, @userid, @name)";
                    sqlCommand.Parameters.AddWithValue("@id", category.Id);
                    sqlCommand.Parameters.AddWithValue("@userid", userId);
                    sqlCommand.Parameters.AddWithValue("@name", category.Name);

                    sqlCommand.ExecuteNonQuery();
                }

                return category;
            }
        }

        /// <summary>
        /// Delete category from database
        /// </summary>
        /// <param name="id">Category Id</param>
        public void Delete(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "delete from categories where id = @id";
                    sqlCommand.Parameters.AddWithValue("@id", id);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get category
        /// </summary>
        /// <param name="categoryId">Category id</param>
        /// <returns>Category</returns>
        public Category Get(Guid categoryId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select id, name from categories where id = @id";
                    sqlCommand.Parameters.AddWithValue("@id", categoryId);

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException($"Category with id {categoryId} not exists");
                        }

                        return new Category
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name"))
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Assign category to note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <param name="categoryId">Category Iid</param>
        public void Assign(Guid noteId, Guid categoryId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "insert into noteCategories (categoryId, noteId) values (@categoryId, @noteId)";
                    sqlCommand.Parameters.AddWithValue("@categoryId", categoryId);
                    sqlCommand.Parameters.AddWithValue("@noteId", noteId);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Dissociate category from note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <param name="categoryId">Category id</param>
        public void Dissociate(Guid noteId, Guid categoryId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "delete from noteCategories where categoryId = @categoryId and noteId = @noteId";
                    sqlCommand.Parameters.AddWithValue("@categoryId", categoryId);
                    sqlCommand.Parameters.AddWithValue("@noteId", noteId);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get categories, assigned to note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <returns></returns>
        public IEnumerable<Category> GetNoteCategories(Guid noteId)
        {
            using (var sqlConnetion = new SqlConnection(_connectionString))
            {
                sqlConnetion.Open();
                using (var sqlCommand = sqlConnetion.CreateCommand())
                {
                    sqlCommand.CommandText = "select categoryId, userId, name from noteCategories inner join categories on noteCategories.categoryId = categories.id where noteId = @noteId";
                    sqlCommand.Parameters.AddWithValue("@noteId", noteId);

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            yield return new Category
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("categoryId")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get all categories of user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns></returns>
        public IEnumerable<Category> GetUserCategories(Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select id, name from categories where userId = @userId";
                    sqlCommand.Parameters.AddWithValue("@userId", userId);

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new Category
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update category
        /// </summary>
        /// <param name="categoryId">Category id</param>
        /// <param name="name">Category name</param>
        /// <returns>Updated category</returns>
        public Category Update(Guid categoryId, string name)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "update categories set name = @name where id = @id";
                    sqlCommand.Parameters.AddWithValue("@id", categoryId);
                    sqlCommand.Parameters.AddWithValue("@name", name);

                    sqlCommand.ExecuteNonQuery();
                }
            }

            return Get(categoryId);
        }
    }
}
