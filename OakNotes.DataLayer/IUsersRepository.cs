using System;
using OakNotes.Model;
using System.Collections.Generic;

namespace OakNotes.DataLayer
{
    public interface IUsersRepository
    {
        User Create(User user);
        User Get(Guid id);
        User Get(string login);
        void Delete(Guid id);
        IEnumerable<User> GetNoteShares(Guid noteId);
    }
}
