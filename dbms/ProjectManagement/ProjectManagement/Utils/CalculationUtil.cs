using Microsoft.VisualBasic.ApplicationServices;
using ProjectManagement.DAOs;
using ProjectManagement.Database;
using ProjectManagement.Mappers.Implement;
using ProjectManagement.Models;
using System.Data;
using System.Data.SqlClient;

namespace ProjectManagement.Process
{
    internal class CalculationUtil
    {
        public static double CalUtil(string peopleId, List<Tasks> listTasks, string field)
        {
            if (listTasks.Count == 0 || listTasks == null) return 0;
            TaskMapper taskMapper = new TaskMapper();
            DataTable taskTable = taskMapper.MapToTableType(listTasks);
            string sqlStr = "SELECT * FROM FUNC_CalUtil(@PeopleId, @TaskList, @CalculationField)";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                 new SqlParameter("@PeopleId", peopleId),
                 new SqlParameter
                 {
                    ParameterName = "@TaskList",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "TaskTableType",
                    Value = taskTable
                 },
                 new SqlParameter("@CalculationField", field)
            };
            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);
            double avg = (double)dataTable.Rows[0]["AverageValue"];
            return avg;
        }
        public static int CalAvgProgress(List<Tasks> listTasks)
        {
            if (listTasks == null)
                return 0;

            TaskMapper taskMapper = new TaskMapper();
            DataTable taskTable = taskMapper.MapToTableType(listTasks);
            string sqlStr = "SELECT * FROM FUNC_CalAvgProgress(@TaskList)";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                 new SqlParameter
                 {
                    ParameterName = "@TaskList",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "TaskTableType",
                    Value = taskTable
                 }
            };
            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);
            int avg = (int)dataTable.Rows[0]["AverageProgress"];
            return avg;
        }
    }
}
