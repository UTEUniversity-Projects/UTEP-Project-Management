using ProjectManagement.Database;
using ProjectManagement.Mappers.Implement;
using ProjectManagement.Models;
using ProjectManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ProjectManagement.DAOs
{
    internal class EvaluationDAO : DBConnection
    {

        #region SELECT EVALUATION

        // 1.
        public static Evaluation SelectOnly(string taskId, string studentId)
        {
            List<KeyValuePair<string, string>> conditions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("taskId", taskId),
                new KeyValuePair<string, string>("studentId", studentId)
            };
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Evaluation, conditions);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                EvaluationMapper evaluationMapper = new EvaluationMapper();
                return evaluationMapper.MapRow(dataTable.Rows[0]);
            }

            return null;
        }

        // 2.
        public static List<Evaluation> SelectListByUserAndYear(string studentId, int year)
        {
            string sqlStr = string.Format("SELECT * FROM dbo.FUNC_GetEvaluationByStudentIdAndYear(@StudentId, @YearSelected)");
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@StudentId", studentId),
                new SqlParameter("@YearSelected", year)

            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            List<Evaluation> listEvaluations = new List<Evaluation>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                EvaluationMapper evaluationMapper = new EvaluationMapper();
                foreach (DataRow row in dataTable.Rows)
                {
                    Evaluation evaluation = evaluationMapper.MapRow(row);
                    listEvaluations.Add(evaluation);
                }
            }

            return listEvaluations;
        }
        #endregion

        #region EVALUATION DAO EXECUTION

        // 3.
        public static void InsertAssignStudent(string instructorId, string taskId, string studentId)
        {
            Evaluation evaluation = new Evaluation(string.Empty, 0.0D, 0.0D, false, DateTime.Now,
                instructorId, studentId, taskId);
            List<KeyValuePair<string, string>> conditions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("evaluationId", evaluation.EvaluationId),
                new KeyValuePair<string, string>("content", evaluation.Content),
                new KeyValuePair<string, string>("completionRate", evaluation.CompletionRate.ToString()),
                new KeyValuePair<string, string>("score", evaluation.Score.ToString()),
                new KeyValuePair<string, string>("evaluated", (evaluation.Evaluated ? 1 : 0).ToString()),
                new KeyValuePair<string, string>("createdAt", evaluation.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")),
                new KeyValuePair<string, string>("createdBy", evaluation.CreatedBy),
                new KeyValuePair<string, string>("studentId", evaluation.StudentId),
                new KeyValuePair<string, string>("taskId", evaluation.TaskId)
            };
            DBExecution.InsertDynamic(DBTableNames.Evaluation, conditions);
        }

        // 4.
        public static void Update(Evaluation evaluation)
        {
            List<KeyValuePair<string, string>> setValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("content", evaluation.Content),
                new KeyValuePair<string, string>("completionRate", evaluation.CompletionRate.ToString()),
                new KeyValuePair<string, string>("score", evaluation.Score.ToString()),
                new KeyValuePair<string, string>("evaluated", (evaluation.Evaluated ? 1 : 0).ToString()),
                new KeyValuePair<string, string>("createdAt", evaluation.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")),
                new KeyValuePair<string, string>("createdBy", evaluation.CreatedBy),
                new KeyValuePair<string, string>("studentId", evaluation.StudentId),
                new KeyValuePair<string, string>("taskId", evaluation.TaskId)
            };
            List<KeyValuePair<string, string>> conditions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("evaluationId", evaluation.EvaluationId)
            };

            DBExecution.UpdateDynamic(DBTableNames.Evaluation, setValues, conditions);
        }


        // 5.
        public static void DeleteByTaskId(string taskId)
        {
            DBExecution.DeleteDynamic(DBTableNames.Evaluation,
            [
                new ("taskId", taskId),
            ]);
        }
        #endregion

        #region STATISTICAL

        public static Dictionary<string, int> GroupByMonth(List<Evaluation> listEvaluations)
        {
            EvaluationMapper evaluationMapper = new EvaluationMapper();
            DataTable evaluationTable = evaluationMapper.MapToTableType(listEvaluations);

            string sqlStr = "SELECT MonthName, EvaluationCount FROM FUNC_GetEvaluationsGroupedByMonth(@EvaluationList) " +
                            "ORDER BY MonthNumber;";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    ParameterName = "@EvaluationList",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "EvaluationTableType",
                    Value = evaluationTable
                }
            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);
            Dictionary<string, int> result = new Dictionary<string, int>();

            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string monthName = row["MonthName"].ToString();
                    int evaluationCount = Convert.ToInt32(row["EvaluationCount"]);

                    result.Add(monthName, evaluationCount);
                }
            }

            return result;
        }


        #endregion

    }
}
