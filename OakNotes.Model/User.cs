using System;
using System.Collections.Generic;

namespace OakNotes.Model
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }
}
