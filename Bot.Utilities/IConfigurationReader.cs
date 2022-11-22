using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultigamingBot.Configuration
{
    public interface IConfigurationReader
    {
        string GetSection(string section);
    }
}
