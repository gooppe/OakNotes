using System;
using OakNotes.Model;
using System.Collections.Generic;

namespace OakNotes.DataLayer
{
    public interface IUsersRepository
    {
        User Create(string name);
        User Get(Guid id);
        void Delete(Guid id);
        IEnumerable<User> GetNoteShares(Guid noteId);
    }
}
