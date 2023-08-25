using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DataAccess
{
    public interface IHttpDataAccess
    {
        string HttpClientPost(string address, string content, Dictionary<string, string>? headers = null);
        string HttpClientGet(string address, Dictionary<string, string>? headers = null);
        T DeserializeJson<T>(string json);
    }
}
