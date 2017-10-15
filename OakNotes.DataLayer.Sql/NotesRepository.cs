using System;
using System.Collections.Generic;
using OakNotes.Model;
using System.Data.SqlClient;

namespace OakNotes.DataLayer.Sql
{
    public class NotesRepository : INotesRepository
    {
        private readonly string _connectionString;
        private readonly UsersRepository _usersRepository;
        private readonly CategoriesRepository _categoriesRepository;

        public NotesRepository(string connectionString, UsersRepository usersRepository, CategoriesRepository categoriesRepository)
        {
            _connectionString = connectionString;
            _usersRepository = usersRepository;
            _categoriesRepository = categoriesRepository;
        }

        /// <summary>
        /// Create new note
        /// </summary>
        /// <param name="note">Note</param>
        /// <returns>Created note</returns>
        public Note Create(User owner, string title, string text)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    var note = new Note
                    {
                        Id = Guid.NewGuid(),
                        Owner = owner,
                        Title = title,
                        Text = text,
                        Created = DateTime.Now,
                        Updated = DateTime.Now
                    };

                    note.Id = Guid.NewGuid();
                    sqlCommand.CommandText = "insert into notes (id, ownerId, title, text, created, updated) values (@id, @ownerId, @title, @text, @created, @updated)";
                    sqlCommand.Parameters.AddWithValue("@id", note.Id);
                    sqlCommand.Parameters.AddWithValue("@ownerId", note.Owner.Id);
                    sqlCommand.Parameters.AddWithValue("@title", note.Title);
                    sqlCommand.Parameters.AddWithValue("@text", note.Text);
                    sqlCommand.Parameters.AddWithValue("@created", note.Created);
                    sqlCommand.Parameters.AddWithValue("@updated", note.Updated);

                    sqlCommand.ExecuteNonQuery();

                    return note;
                }
            }
        }

        /// <summary>
        /// Delete note from database
        /// </summary>
        /// <param name="id">Note id</param>
        public void Delete(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "delete from notes where id = @id";
                    sqlCommand.Parameters.AddWithValue("@id", id);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <returns>Note</returns>
        public Note Get(Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select id, ownerId, title, text, created, updated from Notes where id = @noteId";
                    sqlCommand.Parameters.AddWithValue("@noteId", noteId);

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException($"Note with id {noteId} not exist");
                        }

                        var user = _usersRepository.Get(reader.GetGuid(reader.GetOrdinal("ownerId")));
                        return new Note
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("id")),
                            Owner = user,
                            Title = reader.GetString(reader.GetOrdinal("title")),
                            Text = reader.GetString(reader.GetOrdinal("text")),
                            Created = reader.GetDateTime(reader.GetOrdinal("created")),
                            Updated = reader.GetDateTime(reader.GetOrdinal("updated")),
                            Categories = _categoriesRepository.GetNoteCategories(reader.GetGuid(reader.GetOrdinal("id"))),
                            Shares = _usersRepository.GetNoteShares(reader.GetGuid(reader.GetOrdinal("id")))
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Get notes of selected category
        /// </summary>
        /// <param name="categoryId">Category id</param>
        /// <returns>Notes of category</returns>
        public IEnumerable<Note> GetCategoryNotes(Guid categoryId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select noteId, ownerId, title, text, created, updated from NoteCategories inner join Notes on NoteCategories.NoteId = Notes.Id where categoryId = @categoryId";
                    sqlCommand.Parameters.AddWithValue("@categoryId", categoryId);

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            var user = _usersRepository.Get(reader.GetGuid(reader.GetOrdinal("ownerId")));
                            yield return new Note
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("noteId")),
                                Owner = user,
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Created = reader.GetDateTime(reader.GetOrdinal("created")),
                                Updated = reader.GetDateTime(reader.GetOrdinal("updated")),
                                Categories = _categoriesRepository.GetNoteCategories(reader.GetGuid(reader.GetOrdinal("noteId"))),
                                Shares = _usersRepository.GetNoteShares(reader.GetGuid(reader.GetOrdinal("noteId")))
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return notes, shared for selected user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Shared notes</returns>
        public IEnumerable<Note> GetSharedNotes(Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select noteId, ownerId, title, text, created, updated from shares inner join notes on shares.noteId = notes.id where userId = @userId";
                    sqlCommand.Parameters.AddWithValue("@userId", userId);

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = _usersRepository.Get(reader.GetGuid(reader.GetOrdinal("ownerId")));
                            yield return new Note
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("noteId")),
                                Owner = user,
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Created = reader.GetDateTime(reader.GetOrdinal("created")),
                                Updated = reader.GetDateTime(reader.GetOrdinal("updated")),
                                Categories = _categoriesRepository.GetNoteCategories(reader.GetGuid(reader.GetOrdinal("noteId"))),
                                Shares = _usersRepository.GetNoteShares(reader.GetGuid(reader.GetOrdinal("noteId")))
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get all user notes
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Note> GetUserNotes(Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select noteId, ownerId, title, text, created, updated from shares inner join notes on shares.noteId = notes.id where userId = @userId";
                    sqlCommand.Parameters.AddWithValue("@userId", userId);

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = _usersRepository.Get(reader.GetGuid(reader.GetOrdinal("ownerId")));
                            yield return new Note
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("noteId")),
                                Owner = user,
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Created = reader.GetDateTime(reader.GetOrdinal("created")),
                                Updated = reader.GetDateTime(reader.GetOrdinal("updated")),
                                Categories = _categoriesRepository.GetNoteCategories(reader.GetGuid(reader.GetOrdinal("noteId"))),
                                Shares = _usersRepository.GetNoteShares(reader.GetGuid(reader.GetOrdinal("noteId")))
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Share note to user
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <param name="userId">User id</param>
        public void Share(Guid noteId, Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "insert into shares (noteId, userId) values (@noteId, @userId)";
                    sqlCommand.Parameters.AddWithValue("@noteId", noteId);
                    sqlCommand.Parameters.AddWithValue("@userId", userId);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Unshare note from user
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <param name="userId">User id</param>
        public void Unshare(Guid noteId, Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "delete from shares where noteId = @noteId and userId = @userId";
                    sqlCommand.Parameters.AddWithValue("@noteId", noteId);
                    sqlCommand.Parameters.AddWithValue("@userId", userId);

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
        
        /// <summary>
        /// Update note
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <param name="title">New title</param>
        /// <param name="text">New text</param>
        public Note Update(Guid noteId, string title, string text)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "update notes set title = @title, text = @text, updated = @updated where id = @id";
                    sqlCommand.Parameters.AddWithValue("@id", noteId);
                    sqlCommand.Parameters.AddWithValue("@title", title);
                    sqlCommand.Parameters.AddWithValue("@text", text);
                    sqlCommand.Parameters.AddWithValue("@updated", DateTime.Now);

                    sqlCommand.ExecuteNonQuery();

                    return Get(noteId);
                }
            }
        }
    }
}
