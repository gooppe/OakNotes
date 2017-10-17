using OakNotes.Model;
using System;
using System.Collections.Generic;

namespace OakNotes.DataLayer
{
    public interface INotesRepository
    {
        Note Create(Note note);
        void Delete(Guid id);
        Note Update(Note note);
        Note Get(Guid noteId);
        void Share(Guid noteId, Guid userId);
        void Unshare(Guid noteId, Guid userId);
        IEnumerable<Note> GetUserNotes(Guid userId);
        IEnumerable<Note> GetCategoryNotes(Guid categoryId);
        IEnumerable<Note> GetSharedNotes(Guid userId);
    }
}
