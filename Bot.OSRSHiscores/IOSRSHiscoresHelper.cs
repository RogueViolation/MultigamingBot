using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.OSRSHiscores
{
    public interface IOSRSHiscoresHelper
    {
        Task<int> TryAddUserToOSRSUsers(string username);
    }
}
