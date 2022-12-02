using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DBDataAccess
{
    public interface IDBDataAccess
    {
        Task ExecuteCodeRedeemedProcedure(string code, string? dataField1, string? dataField2, string? dataField3, bool status, string? date, ulong source);

        Task RetrieveStoredCodes();
    }
}
