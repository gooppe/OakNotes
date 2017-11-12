using OakNotes.Model;
using System;
using System.Collections.Generic;

namespace OakNotes.DataLayer
{
    public interface ICategoriesRepository
    {
        Category Create(Category category, Guid userId);
        Category Update(Category category);
        Category Get(Guid categoryId);
        void Delete(Guid id);
        void Assign(Guid noteId, Guid categoryId);
        void Dissociate(Guid noteId, Guid categoryId);
        IEnumerable<Category> GetUserCategories(Guid userId);
        IEnumerable<Category> GetNoteCategories(Guid noteId);
    }
}
