using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MultigamingBot.Configuration;
using System.Data;

namespace Bot.DBDataAccess
{
    public class DBDataAccess : IDBDataAccess
    {
        private readonly ILogger _logger;
        private readonly IConfigurationReader _config;
        public DBDataAccess(ILogger<DBDataAccess> logger, IConfigurationReader config)
        {
            _logger = logger;
            _config = config;
        }
        public async Task ExecuteCodeRedeemedProcedure(string code, string? dataField1, string? dataField2, string? dataField3, bool status, string? date, ulong source)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_config.GetSection("connectionString")))
                {
                    connection.Open();

                    //@l_code nvarchar(50), 
                    //@l_code_content1 nvarchar(50), 
                    //@l_code_content2 nvarchar(50), 
                    //@l_code_content3 nvarchar(50), 
                    //@l_status bit,
                    //@l_date_redeem nvarchar(50), 
                    //@l_code_source nvarchar(50)
                    var cmd = new SqlCommand("dbo.uspAddCode", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@l_code", SqlDbType.NVarChar, 50).Value = code;
                    cmd.Parameters.Add("@l_code_content1", SqlDbType.NVarChar, 50).Value = dataField1;
                    cmd.Parameters.Add("@l_code_content2", SqlDbType.NVarChar, 50).Value = dataField2;
                    cmd.Parameters.Add("@l_code_content3", SqlDbType.NVarChar, 50).Value = dataField3;
                    cmd.Parameters.Add("@l_status", SqlDbType.Bit).Value = status ? 1 : 0;
                    cmd.Parameters.Add("@l_date_redeem", SqlDbType.NVarChar, 50).Value = date;
                    cmd.Parameters.Add("@l_code_source", SqlDbType.NVarChar, 50).Value = source;

                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e){
                _logger.LogError($"Error occured while running \"dbo.uspAddCode\" {e.Message}");
            }
            await Task.CompletedTask;
        }
    }
}