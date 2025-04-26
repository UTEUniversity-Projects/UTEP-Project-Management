using ProjectManagement.Mappers;
using ProjectManagement.Utils;
using System.Data;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ProjectManagement.Database
{
    internal class DBExecution
    {
        private static DBConnection uDConnection = new DBConnection();

        #region SQL EXECUTION QUERY

        public static DataTable SQLExecuteQuery(string sqlStr, List<SqlParameter> parameters, string typeExecution)
        {
            SqlConnection connection = uDConnection.GetConnection();
            DataTable dataTable = new DataTable();

            try
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(sqlStr, connection))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }

                if (!string.IsNullOrEmpty(typeExecution))
                {
                    WinformControlUtil.ShowMessage("Notification", typeExecution + " successfully");
                }
            }
            catch (Exception ex)
            {
                WinformControlUtil.ShowMessage("Notification", typeExecution + " failed: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dataTable;
        }

        #endregion

        #region SQL EXECUTION NON QUERY

        public static void SQLExecuteNonQuery(string sqlStr, List<SqlParameter> parameters, string typeExecution)
        {
            SqlConnection connection = uDConnection.GetConnection();

            try
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(sqlStr, connection))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }

                    if (cmd.ExecuteNonQuery() > 0 && !string.IsNullOrEmpty(typeExecution))
                    {
                        WinformControlUtil.ShowMessage("Notification", typeExecution + " successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                WinformControlUtil.ShowMessage("Notification", typeExecution + " failed: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        #endregion

        #region CRUD OPERATION

        public static void Insert<T>(T model, string tableName)
        {
            string sqlStr = $"INSERT INTO {tableName} ({string.Join(", ", typeof(T).GetProperties().Select(p => char.ToLower(p.Name[0]) + p.Name.Substring(1)))}) " +
                            $"VALUES ({string.Join(", ", typeof(T).GetProperties().Select(p => "@" + p.Name))})";

            List<SqlParameter> parameters = DBUtil.GetSqlParameters(model);
            SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }
        public static void Insert<T>(T model, string tableName, string typeExecution)
        {
            string sqlStr = $"INSERT INTO {tableName} ({string.Join(", ", typeof(T).GetProperties().Select(p => char.ToLower(p.Name[0]) + p.Name.Substring(1)))}) " +
                            $"VALUES ({string.Join(", ", typeof(T).GetProperties().Select(p => "@" + p.Name))})";

            List<SqlParameter> parameters = DBUtil.GetSqlParameters(model);
            SQLExecuteNonQuery(sqlStr, parameters, typeExecution);
        }

        public static void Update<T>(T model, string tableName, string primaryKeyName, string primaryKeyValue)
        {
            string sqlStr = $"UPDATE {tableName} SET {string.Join(", ", typeof(T).GetProperties().Select(p => char.ToLower(p.Name[0]) + p.Name.Substring(1) + " = @" + p.Name))} " +
                            $"WHERE {primaryKeyName} = @{primaryKeyName + "KEY"}";

            List<SqlParameter> parameters = DBUtil.GetSqlParameters(model);
            parameters.Add(new SqlParameter("@" + primaryKeyName + "KEY", primaryKeyValue));

            SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }

        public static void Delete(string tableName, string primaryKeyName, object primaryKeyValue)
        {
            string sqlStr = $"DELETE FROM {tableName} WHERE {primaryKeyName} = @{primaryKeyName}";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@" + primaryKeyName, primaryKeyValue)
            };

            SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }

        #endregion

        #region EXTEND CRUD

        public static void InsertDynamic(string tableName, List<KeyValuePair<string, string>> columnValues)
        {
            // Build the SQL command string to call the stored procedure
            string sqlStr = "EXEC PROC_InsertDynamic @TableName, @ColumnsValues";

            // Define the parameters list, including @TableName
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@TableName", tableName)
            };

            // Create a DataTable to store the column-value pairs
            DataTable columnValueTable = new DataTable();
            columnValueTable.Columns.Add("ColumnName", typeof(string));
            columnValueTable.Columns.Add("ColumnValue", typeof(string));

            // Populate the DataTable with each column-value pair
            foreach (var columnValue in columnValues)
            {
                columnValueTable.Rows.Add(columnValue.Key, columnValue.Value);
            }

            // Add the DataTable as a parameter with SQL Server's custom type "ConditionType"
            SqlParameter conditionParam = new SqlParameter("@ColumnsValues", columnValueTable)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "ConditionType"  // Custom table type defined in SQL Server
            };
            parameters.Add(conditionParam);

            // Execute the SQL command using DBExecution's SQLExecuteNonQuery
            SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }

        public static void UpdateDynamic(string tableName, List<KeyValuePair<string, string>> setValues, List<KeyValuePair<string, string>> conditions)
        {
            // SQL command to execute stored procedure
            string sqlStr = "EXEC PROC_UpdateDynamic @TableName, @SetValues, @Conditions";

            // Define the parameters list
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@TableName", tableName)
            };

            // Create DataTables for SetValues and Conditions
            DataTable setValueTable = new DataTable();
            setValueTable.Columns.Add("ColumnName", typeof(string));
            setValueTable.Columns.Add("ColumnValue", typeof(string));

            DataTable conditionTable = new DataTable();
            conditionTable.Columns.Add("ColumnName", typeof(string));
            conditionTable.Columns.Add("ColumnValue", typeof(string));

            // Populate the SetValues DataTable
            foreach (var setValue in setValues)
            {
                setValueTable.Rows.Add(setValue.Key, setValue.Value);
            }

            // Populate the Conditions DataTable
            foreach (var condition in conditions)
            {
                conditionTable.Rows.Add(condition.Key, condition.Value);
            }

            // Add DataTables as structured parameters
            SqlParameter setParam = new SqlParameter("@SetValues", setValueTable)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "ConditionType"
            };
            parameters.Add(setParam);

            SqlParameter conditionParam = new SqlParameter("@Conditions", conditionTable)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "ConditionType"
            };
            parameters.Add(conditionParam);

            // Execute the SQL command using DBExecution's SQLExecuteNonQuery
            SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }

        public static void DeleteDynamic(string tableName, List<KeyValuePair<string, string>> conditions)
        {
            // Build the SQL string to execute the stored procedure
            string sqlStr = "EXEC PROC_DeleteDynamic @TableName, @Conditions";

            // Add the @TableName parameter
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@TableName", tableName)
            };

            // Create a DataTable to hold the conditions
            DataTable conditionTable = new DataTable();
            conditionTable.Columns.Add("ColumnName", typeof(string));
            conditionTable.Columns.Add("ColumnValue", typeof(string));

            // Populate the DataTable with the conditions (column name, column value)
            foreach (var condition in conditions)
            {
                conditionTable.Rows.Add(condition.Key, condition.Value);
            }

            // Add the DataTable as a parameter with SQL Server's "ConditionType" data type
            SqlParameter conditionParam = new SqlParameter("@Conditions", conditionTable)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "ConditionType"  // Custom table type defined in SQL Server
            };
            parameters.Add(conditionParam);

            // Execute the SQL command with the parameters
            SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }

        public static DataTable GetDynamic(string tableName, List<KeyValuePair<string, string>> conditions)
        {
            // Build the SQL command string to execute the stored procedure
            string sqlStr = "EXEC PROC_GetDynamic @TableName, @Conditions";

            // Define the parameters, including @TableName
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@TableName", tableName)
            };

            // Create a DataTable to store conditions in the format (ColumnName, ColumnValue)
            DataTable conditionTable = new DataTable();
            conditionTable.Columns.Add("ColumnName", typeof(string));
            conditionTable.Columns.Add("ColumnValue", typeof(string));

            // Populate the DataTable with each condition pair
            foreach (var condition in conditions)
            {
                conditionTable.Rows.Add(condition.Key, condition.Value);
            }

            // Add the condition table as a parameter of SQL Server type "ConditionType"
            SqlParameter conditionParam = new SqlParameter("@Conditions", conditionTable)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = "ConditionType"  // Custom table type defined in SQL Server
            };
            parameters.Add(conditionParam);

            // Execute the SQL command and return the result as a DataTable
            return SQLExecuteQuery(sqlStr, parameters, string.Empty);
        }

        #endregion

    }
}
