using ProjectManagement.Enums;
using ProjectManagement.Models;
using ProjectManagement.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Mappers.Implement
{
    internal class TaskMapper : IRowMapper<Tasks>
    {
        public Tasks MapRow(DataRow row)
        {
            string taskId = row["taskId"].ToString();
            DateTime startAt = DateTime.Parse(row["startAt"].ToString());
            DateTime endAt = DateTime.Parse(row["endAt"].ToString());
            string title = row["title"].ToString();
            string description = row["description"].ToString();
            double progress = double.Parse(row["progress"].ToString());
            ETaskPriority priority = EnumUtil.GetEnumFromDisplayName<ETaskPriority>(row["priority"].ToString());
            DateTime createdAt = DateTime.Parse(row["createdAt"].ToString());
            string createdBy = row["createdBy"].ToString();
            string projectId = row["projectId"].ToString();

            Tasks task = new Tasks(taskId, startAt, endAt, title, description, progress, priority, createdAt, createdBy, projectId);
            return task;
        }

        public DataTable MapToTableType(List<Tasks> list)
        {
            DataTable taskTable = new DataTable();

            taskTable.Columns.Add("taskId", typeof(string));
            taskTable.Columns.Add("startAt", typeof(DateTime));
            taskTable.Columns.Add("endAt", typeof(DateTime));
            taskTable.Columns.Add("title", typeof(string));
            taskTable.Columns.Add("description", typeof(string));
            taskTable.Columns.Add("progress", typeof(float));
            taskTable.Columns.Add("priority", typeof(string));
            taskTable.Columns.Add("createdAt", typeof(DateTime));
            taskTable.Columns.Add("createdBy", typeof(string));
            taskTable.Columns.Add("projectId", typeof(string));

            foreach (var task in list)
            {
                taskTable.Rows.Add(
                    task.TaskId,
                    task.StartAt,
                    task.EndAt,
                    task.Title,
                    task.Description,
                    task.Progress,
                    task.Priority,
                    task.CreatedAt,
                    task.CreatedBy,
                    task.ProjectId
                );
            }

            return taskTable;
        }

    }
}
