using data.domain.Models;
using data.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data.RemoteData.RemoteDatabase.DAO;

public class AllPresence
{
    public required string GroupName { get; set; }
    public required UsersForPresence Users { get; set; }
}
