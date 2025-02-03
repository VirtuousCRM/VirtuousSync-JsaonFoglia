using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sync
{
    public class DapperService
    {
        private readonly string _connectionString;
        public DapperService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> InsertContacts(List<AbbreviatedContact> abbreviatedContacts)
        {
            string sql = "INSERT INTO dbo.Contacts (Id, Name, ContactType, ContactName, Address, Email, Phone) values (@Id, @Name, @ContactType, @ContactName, @Address, @Email, @Phone)";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var inserted = await connection.ExecuteAsync(sql, abbreviatedContacts);

                return inserted;
            }
        }
    }
}
