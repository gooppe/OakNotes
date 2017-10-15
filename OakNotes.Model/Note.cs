using System;
using System.Collections.Generic;

namespace OakNotes.Model
{
    public class Note
    {
        public Guid Id { get; set; }

        public User Owner { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime Created { get; set; }
        
        public DateTime Updated { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<User> Shares { get; set; }
    }
}
