using Bot.DataAccess.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MultigamingBot.Configuration;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Bot.DataAccess
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
                    cmd.Parameters.Add("@l_id", SqlDbType.NVarChar, 50).Value = Guid.NewGuid().ToString();
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

        public async Task RetrieveStoredCodes()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_config.GetSection("connectionString")))
                {
                    connection.Open();

                    var cmd = new SqlCommand("dbo.uspRetrieveStoredCodes", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    var returnData = cmd.ExecuteReader();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while running \"dbo.uspRetrieveStoredCodes\" {e.Message}");
            }
            await Task.CompletedTask;
        }

        public async Task<bool> CodeExists(string code)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_config.GetSection("connectionString")))
                using (var cmd = new SqlCommand("dbo.uspCheckCodeStatus", connection))
                {
                    connection.Open();

                    cmd.Parameters.Add("@l_code", SqlDbType.NVarChar, 50).Value = code;
                    cmd.CommandType = CommandType.StoredProcedure;

                    return ((int)cmd.ExecuteScalar()) != 0;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while running \"dbo.uspRetrieveStoredCodes\" {e.Message}");
            }
            return true;
        }

        public int AddUserToOSRSUsers(int id, string name, string gamemode) 
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_config.GetSection("connectionString")))
                using (var cmd = new SqlCommand("dbo.uspAddUserToOSRSUsers", connection))
                {
                    connection.Open();

                    cmd.Parameters.Add("@l_id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@l_gamemode", SqlDbType.NVarChar, 50).Value = gamemode;
                    cmd.Parameters.Add("@l_displayname", SqlDbType.NVarChar, 50).Value = name;
                    cmd.CommandType = CommandType.StoredProcedure;

                    return cmd.ExecuteNonQuery()/2;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while running \"dbo.uspAddUserToOSRSUsers\" {e.Message}");
            }
            return 0;
        }

        public bool UserExistsInDatabase(string name)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_config.GetSection("connectionString")))
                using (var cmd = new SqlCommand("dbo.uspUserExistsInDatabase", connection))
                {
                    connection.Open();

                    cmd.Parameters.Add("@l_displayname", SqlDbType.NVarChar, 50).Value = name;
                    cmd.CommandType = CommandType.StoredProcedure;

                    return ((int)cmd.ExecuteScalar()) != 0;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while running \"dbo.uspUserExistsInDatabase\" {e.Message}");
            }
            return true;
        }
    }
}