using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DataAccess
{
    public interface IHttpDataAccess
    {
        T HttpClientPostJson<T>(string address, string content, Dictionary<string, string>? headers = null);
        T HttpClientGetJson<T>(string address, Dictionary<string, string>? headers = null);
    }
}
