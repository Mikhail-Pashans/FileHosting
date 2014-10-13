using FileHosting.Database.Models;
using System.Collections.Generic;

namespace FileHosting.Domain.Models
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}