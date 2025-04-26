using System.Data.SqlClient;

namespace ProjectManagement.Database
{
    public class DBConnection
    {
        private string defaultConnectionString = "Data Source=DESKTOP-4FIVTNT\\SQLEXPRESS;Initial Catalog=ProjectManagement;Integrated Security=True;Encrypt=False";
        private SqlConnection connection = new(Properties.Settings.Default.conStr);

        public void SetConnection(string email, string password)
        {
            string connectionString = $"Data Source=DESKTOP-4FIVTNT\\SQLEXPRESS;Initial Catalog=ProjectManagement;User ID={email};Password={password};Encrypt=False;";
            this.connection = new SqlConnection(connectionString);
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }

        public void SetDefaultConnection() 
        {
            connection = new SqlConnection(defaultConnectionString);
        }
    }
}
