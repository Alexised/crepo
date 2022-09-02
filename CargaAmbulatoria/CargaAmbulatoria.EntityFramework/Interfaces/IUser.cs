using CargaAmbulatoria.EntityFramework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargaAmbulatoria.EntityFramework.Interfaces
{
    public interface IUser
    {
        string UserId { get; }
        string Name { get; set; }
        string Email { get; set; }
        string PasswordStored { get; set; }
        UserRoleEnum Role { get; set; }

        UserStatusEnum Status { get; set; }

    }
}
