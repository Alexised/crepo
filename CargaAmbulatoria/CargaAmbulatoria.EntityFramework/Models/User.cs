using CargaAmbulatoria.EntityFramework.Enums;
using CargaAmbulatoria.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CargaAmbulatoria.EntityFramework.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : IUser
    {
        public User()
        {
            Documents = new HashSet<Document>();
        }

        [Key]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(10)]
        public string Email { get; set; }

        [Required]
        [MaxLength(80)]
        [MinLength(8)]
        public string PasswordStored { get; set; }
        public UserRoleEnum Role { get; set; }
        public UserStatusEnum Status { get; set; }

        public virtual IEnumerable<IDocument>? Documents { get; }
        public string? TokenReset { get; set; }
        public DateTime? TokenResetExpiration { get; set; }
        public int PasswordTries { get; set; }

    }
}
