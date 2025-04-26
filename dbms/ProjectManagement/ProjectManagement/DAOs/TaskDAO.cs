using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Database;
using ProjectManagement.Models;
using ProjectManagement.Mappers.Implement;
using System.Data.SqlClient;
using ProjectManagement.Enums;
using ProjectManagement.Utils;
using ProjectManagement.MetaData;
using Microsoft.VisualBasic.ApplicationServices;
using static TheArtOfDevHtmlRenderer.Adapters.RGraphicsPath;
using System.Windows.Forms;

namespace ProjectManagement.DAOs
{
    internal class TaskDAO : DBConnection
    {

        #region SELECT TASKS

        public static Tasks SelectOnly(string taskId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Task, [new("taskId", taskId)]);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                TaskMapper taskMapper = new TaskMapper();
                return taskMapper.MapRow(dataTable.Rows[0]);
            }

            return null;
        }
        public static List<Tasks> SelectListByTeam(string teamId)
        {
            DataTable getProject = DBExecution.GetDynamic(DBTableNames.Team, [new("teamId", teamId)]);

            if (getProject != null && getProject.Rows.Count > 0)
            {
                DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Task, [new("projectId", getProject.Rows[0]["projectId"].ToString())]);

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    List<Tasks> tasksList = new List<Tasks>();
                    TaskMapper taskMapper = new TaskMapper();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Tasks task = taskMapper.MapRow(row);

                        tasksList.Add(task);
                    }
                    return tasksList;
                }
            }
            return new List<Tasks>();
        }
        public static List<TaskMeta> SelectListTaskMeta(string userId, string teamId, string projectId)
        {
            List<Tasks> tasks = SelectListByTeam(teamId);
            List<string> favoriteTasks = SelectFavoriteList(userId, projectId);

            List<TaskMeta> taskMetas = new List<TaskMeta>();
            foreach (Tasks task in tasks)
            {
                taskMetas.Add(new TaskMeta(task, favoriteTasks.Contains(task.TaskId)));
            }

            return taskMetas;
        }
        public static List<Tasks> SelectListTaskByStudent(string projectId, string studentId)
        {
            DataTable getTask = DBExecution.GetDynamic(DBTableNames.TaskStudent, [new("studentId", studentId)]);

            if (getTask != null)
            {
                List<string> taskIds = new List<string>();
                foreach (DataRow row in getTask.Rows)
                {
                    taskIds.Add(row["taskId"].ToString());
                }

                List<Tasks> tasks = SelectListTaskByProject(projectId);

                List<Tasks> tasksList = tasks.Where(t => taskIds.Contains(t.TaskId)).ToList();

                return tasksList;
            }

            return new List<Tasks>();
        }
        public static List<Tasks> SelectListTaskByProject(string projectId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Task, [new("projectId", projectId)]);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                List<Tasks> tasksList = new List<Tasks>();
                TaskMapper taskMapper = new TaskMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Tasks task = taskMapper.MapRow(row);
                    tasksList.Add(task);
                }
                return tasksList.OrderByDescending(t => t.CreatedAt).ToList();
            }
            return new List<Tasks>();
        }
        private static List<string> SelectFavoriteList(string userId, string projectId)
        {
            DataTable getTask = DBExecution.GetDynamic(DBTableNames.FavoriteTask, [new("userId", userId)]);

            if (getTask != null)
            {
                List<string> taskIds = new List<string>();
                foreach (DataRow row in getTask.Rows)
                {
                    taskIds.Add(row["taskId"].ToString());
                }

                List<string> tasks = SelectListTaskByProject(projectId).Select(t => t.TaskId).ToList();

                List<string> tasksList = tasks.Where(t => taskIds.Contains(t)).ToList();

                return tasksList;
            }

            return new List<string>();
        }


        #endregion

        #region TASK DAO EXECUTION

        public static void Insert(Tasks task)
        {
            DBExecution.InsertDynamic(DBTableNames.Task,
                [
                    new("taskId", task.TaskId),
                    new("startAt", task.StartAt.ToString()),
                    new("endAt", task.EndAt.ToString()),
                    new("title", task.Title),
                    new("description", task.Description),
                    new("progress", task.Progress.ToString()),
                    new("priority", EnumUtil.GetDisplayName(task.Priority)),
                    new("createdAt", task.CreatedAt.ToString()),
                    new("createdBy", task.CreatedBy),
                    new("projectId", task.ProjectId),
                ]);
        }
        public static void InsertAssignStudent(string taskId, string studentId)
        {
            string sqlStr = "EXEC PROC_InsertAssignStudent @taskId, @studentId";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@taskId", taskId),
                new SqlParameter("@studentId", studentId)
            };

            DBExecution.SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }

        public static void DeleteTaskStudentByTaskId(string taskId)
        {
            DBExecution.DeleteDynamic(DBTableNames.TaskStudent, [new("taskId", taskId)]);
        }
        public static void DeleteFavoriteTaskByTaskId(string taskId)
        {
            DBExecution.DeleteDynamic(DBTableNames.FavoriteTask, [new("taskId", taskId)]);
        }
        public static void Delete(string taskId)
        {
            string sqlStr = "EXEC PROC_DeleteTaskCascade @taskId";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@taskId", taskId),
            };

            DBExecution.SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }
        
        public static void DeleteFollowProject(string projectId)
        {
            List<Tasks> listTasks = SelectListTaskByProject(projectId);

            foreach (Tasks task in listTasks)
            {
                Delete(task.TaskId);
            }
        }

        public static void Update(Tasks task)
        {
            DBExecution.UpdateDynamic(DBTableNames.Task,
                [
                    new("title", task.Title),
                    new("description", task.Description),
                    new("startAt", task.StartAt.ToString()),
                    new("endAt", task.EndAt.ToString()),
                    new("progress", task.Progress.ToString()),
                    new("priority", EnumUtil.GetDisplayName(task.Priority)),
                ],
                [
                    new("taskId", task.TaskId)
                ]);
        }
       
        public static void DeleteFavoritesTask(string userId, string taskId)
        {
            DBExecution.DeleteDynamic(DBTableNames.FavoriteTask, 
                [
                    new("userId", userId),
                    new("taskId", taskId)
                ]);
        }
        public static void InsertFavoritesTask(string userId, string taskId)
        {
            DBExecution.InsertDynamic(DBTableNames.FavoriteTask,
            [
                new("userId", userId),
                new("taskId", taskId)
            ]);
        }
        public static void UpdateFavorite(string userId, string taskId, bool isFavorite)
        {
            string sqlStr = string.Empty;

            if (isFavorite == false)
            {
                DeleteFavoritesTask(userId, taskId);
            }
            else
            {
                InsertFavoritesTask(userId, taskId);
            }
        }

        #endregion

        #region SEARCH TASK

        private static List<Tasks> SearchTaskTitle(string projectId, string title)
        {
            string sqlStr = "EXEC PROC_SearchTaskByTitle @ProjectId, @TitleSyntax";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", projectId),
                new SqlParameter("@TitleSyntax", title + "%")
            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                List<Tasks> tasksList = new List<Tasks>();
                TaskMapper taskMapper = new TaskMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Tasks task = taskMapper.MapRow(row);

                    tasksList.Add(task);
                }
                return tasksList;
            }
            return new List<Tasks>();
        }
        public static List<TaskMeta> SearchTaskMetaTitle(string userId, string projectId, string title)
        {
            List<Tasks> tasks = SearchTaskTitle(projectId, title);
            List<string> favoriteTasks = SelectFavoriteList(userId, projectId);

            List<TaskMeta> taskMetas = new List<TaskMeta>();
            foreach (Tasks task in tasks)
            {
                taskMetas.Add(new TaskMeta(task, favoriteTasks.Contains(task.TaskId)));
            }

            return taskMetas;
        }

        #endregion

        #region CHECK INFORMATION

        public static bool CheckIsFavoriteTask(string userId, string taskId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.FavoriteTask,
                [
                    new("userId", userId),
                    new("taskId", taskId)
                ]);

            return dataTable != null && dataTable.Rows.Count > 0;
        }

        #endregion

        #region GET MEMBERS

        public static List<Member> GetMembersByTaskId(string taskId)
        {
            string sqlStr = $"SELECT studentId FROM {DBTableNames.TaskStudent} WHERE taskId = @TaskId";
            List<SqlParameter> parameters = new List<SqlParameter> { new SqlParameter("@TaskId", taskId) };
            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            List<Member> list = new List<Member>();

            foreach (DataRow row in dataTable.Rows)
            {
                Users student = UserDAO.SelectOnlyByID(row["studentId"].ToString());
                ETeamRole role = default;
                DateTime joinAt = DateTime.Now;

                list.Add(new Member(student, role, joinAt));
            }

            return list.OrderBy(m => m.Role).ToList();
        }

        #endregion

    }
}
