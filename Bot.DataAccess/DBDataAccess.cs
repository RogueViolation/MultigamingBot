using Bot.DataAccess.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MultigamingBot.Configuration;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Bot.DataObjects;

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

                    _logger.LogInformation($"Adding redemption atempt to DB. Params: Code {code}, Data1 {dataField1}, Data2 {dataField2}, Data3 {dataField3}, Status {status}, Source {source}");
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
                    _logger.LogInformation($"Adding user to OSRS User DB. Params: ID {id}, Name {name}, Gamemode {gamemode}");
                    return cmd.ExecuteNonQuery()/2; //Runs 2 inserts in DB therefore we divide the result by 2 since 1 user gets 2 rows inserted.
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while running \"dbo.uspAddUserToOSRSUsers\" {e.Message}");
            }
            return 0;
        }

        //Accepts the user ID, Username and Gamemode. Method checks if the user exists in the database and whether or not user has changed their username.
        //If the user is in the database, but has changed their username it returns -99 to indicate so. Other than that it returns 0 (doesn't exist) or 1 (exists).
        //DB returns a data row based on the UserID which is unique for every user and is set as PK in DB so should not ever return more than 1 record.
        public int ExactUserExistsInDatabase(int id, string name, string gamemode)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_config.GetSection("connectionString")))
                using (var cmd = new SqlCommand("dbo.uspUserExistsInDatabase", connection))
                using (var dataAdapter = new SqlDataAdapter())
                using (var dt = new DataTable())
                {
                    connection.Open();

                    cmd.Parameters.Add("@l_userid", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@l_username", SqlDbType.NVarChar, 50).Value = name;
                    cmd.Parameters.Add("@l_gamemode", SqlDbType.NVarChar, 50).Value = gamemode;
                    cmd.CommandType = CommandType.StoredProcedure;
                    dataAdapter.SelectCommand = cmd;
                    _logger.LogInformation($"Checking if user with parameters exists in the database. Params: ID {id}, Name {name}, Gamemode {gamemode}");
                    dataAdapter.Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return 1; //Exists
                    }
                    if (dt.Rows[0]["Username"].ToString() != name || dt.Rows[0]["Gamemode"].ToString() != gamemode)
                    {
                        return -99; //Exists under different name
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while running \"dbo.uspUserExistsInDatabase\" {e.Message}");
            }
            return 0; //Doesn't exist
        }

        //Will always fetch because of logic that runs this
        public OSRSUserBasic? GetOSRSBasicUserData (string name)
        {
            var userData = new OSRSUserBasic();
            try
            {
                using (SqlConnection connection = new SqlConnection(_config.GetSection("connectionString")))
                using (var cmd = new SqlCommand("dbo.uspGetUserInfoByName", connection))
                {
                    connection.Open();

                    cmd.Parameters.Add("@l_displayname", SqlDbType.NVarChar, 50).Value = name;
                    cmd.CommandType = CommandType.StoredProcedure;

                    _logger.LogInformation($"Getting basic OSRS User data. Params: Name {name}");
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        userData.Id = reader.GetInt32(0);
                        userData.UserName = reader.GetString(1);
                        userData.GameMode = reader.GetString(2);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while running \"dbo.uspUserExistsInDatabase\" {e.Message}");
            }
            return userData;
        }
    }
}