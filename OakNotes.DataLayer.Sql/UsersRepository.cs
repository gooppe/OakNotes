using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using OakNotes.Model;

namespace OakNotes.DataLayer.Sql
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;
        private readonly ICategoriesRepository _categoriesRepository;

        public UsersRepository(string connectionString, ICategoriesRepository categoriesRepository)
        {
            _connectionString = connectionString;
            _categoriesRepository = categoriesRepository;
        }

        /// <summary>
        /// Insert new user into database
        /// </summary>
        /// <param name="user">New User</param>
        /// <returns></returns>
        public User Create(User user)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    user.Id = Guid.NewGuid();
                        
                    command.CommandText = "insert into users (id, name) values (@id, @name)";
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@name", user.Name);
                    command.ExecuteNonQuery();
                    return user;
                }
            }
        }

        /// <summary>
        /// Delete user from database
        /// </summary>
        /// <param name="id">User id</param>
        public void Delete(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from users where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get user from database
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User</returns>
        public User Get(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select id, name from users where id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"User with id {id} not found");

                        var user = new User
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name"))
                        };

                        user.Categories = _categoriesRepository.GetUserCategories(user.Id);
                        return user;
                    }
                }
            }
        }

        /// <summary>
        /// Get Shares of note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <returns>Shares of note</returns>
        public IEnumerable<User> GetNoteShares(Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select userId, name from shares inner join users on shares.userid = users.id where noteId = @noteId";
                    sqlCommand.Parameters.AddWithValue("@noteId", noteId);

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var user = new User
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("userId")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            };

                            user.Categories = _categoriesRepository.GetUserCategories(user.Id);

                            yield return user; 
                        }
                    }
                }
            }
        }
    }
}
