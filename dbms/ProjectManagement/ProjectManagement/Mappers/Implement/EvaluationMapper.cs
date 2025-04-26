using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Mappers.Implement
{
    internal class EvaluationMapper : IRowMapper<Evaluation>
    {
        public Evaluation MapRow(DataRow row)
        {
            string evaluationId = row["evaluationId"].ToString();
            string content = row["content"].ToString();
            double completionRate = double.Parse(row["completionRate"].ToString());
            double score = double.Parse(row["score"].ToString());
            bool evaluated = row["evaluated"].ToString() == "True" ? true : false;
            DateTime createdAt = DateTime.Parse(row["createdAt"].ToString());
            string createdBy = row["createdBy"].ToString();
            string studentId = row["studentId"].ToString();
            string taskId = row["taskId"].ToString();

            Evaluation evaluation = new Evaluation(evaluationId, content, completionRate, score,
                evaluated, createdAt, createdBy, studentId, taskId);

            return evaluation;
        }

        public DataTable MapToTableType(List<Evaluation> list)
        {
            DataTable evaluationTable = new DataTable();

            evaluationTable.Columns.Add("evaluationId", typeof(string));
            evaluationTable.Columns.Add("content", typeof(string));
            evaluationTable.Columns.Add("completionRate", typeof(float));
            evaluationTable.Columns.Add("score", typeof(float));
            evaluationTable.Columns.Add("evaluated", typeof(bool));
            evaluationTable.Columns.Add("createdAt", typeof(DateTime));
            evaluationTable.Columns.Add("createdBy", typeof(string));
            evaluationTable.Columns.Add("studentId", typeof(string));
            evaluationTable.Columns.Add("taskId", typeof(string));

            foreach (var evaluation in list)
            {
                evaluationTable.Rows.Add(
                    evaluation.EvaluationId,
                    evaluation.Content,
                    evaluation.CompletionRate,
                    evaluation.Score,
                    evaluation.Evaluated,
                    evaluation.CreatedAt,
                    evaluation.CreatedBy,
                    evaluation.StudentId,
                    evaluation.TaskId
                );
            }

            return evaluationTable;
        }
    }
}
