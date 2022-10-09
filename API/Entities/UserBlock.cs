using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class UserBlock
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; } 
        public AppUser BlockedUser { get; set; }
        public int BlockedUserId { get; set; }
    }
}