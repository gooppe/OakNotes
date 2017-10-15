using OakNotes.Model;
using System;
using System.Collections.Generic;

namespace OakNotes.DataLayer
{
    public interface INotesRepository
    {
        Note Create(User owner, string title, string texte);
        void Delete(Guid id);
        Note Update(Guid noteId, string title, string text);
        Note Get(Guid noteId);
        void Share(Guid noteId, Guid userId);
        void Unshare(Guid noteId, Guid userId);
        IEnumerable<Note> GetUserNotes(Guid userId);
        IEnumerable<Note> GetCategoryNotes(Guid categoryId);
        IEnumerable<Note> GetSharedNotes(Guid userId);
    }
}
