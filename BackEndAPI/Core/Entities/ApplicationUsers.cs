using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEndAPI.Core.Entities
{
    public class ApplicationUsers : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        
        [NotMapped]
        public IList<string> Roles { get; set; }
    }
}
