using OakNotes.Model;
using System;
using System.Collections.Generic;

namespace OakNotes.DataLayer
{
    public interface ICategoriesRepository
    {
        Category Create(Guid userId, string name);
        Category Update(Guid categoryId, string name);
        Category Get(Guid categoryId);
        void Delete(Guid id);
        void Assign(Guid noteId, Guid categoryId);
        void Dissociate(Guid noteId, Guid categoryId);
        IEnumerable<Category> GetUserCategories(Guid userId);
        IEnumerable<Category> GetNoteCategories(Guid noteId);
    }
}
