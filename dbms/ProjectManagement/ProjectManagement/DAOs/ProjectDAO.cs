using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ProjectManagement.Database;
using ProjectManagement.Models;
using ProjectManagement.Enums;
using ProjectManagement.Utils;
using ProjectManagement.Mappers.Implement;
using System.Data.SqlClient;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.VisualBasic.ApplicationServices;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProjectManagement.DAOs
{
    internal class ProjectDAO : DBConnection
    {

        #region SELECT PROJECT

        public static Project SelectOnly(string projectId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Project, [new("projectId", projectId)]);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ProjectMapper projectMapper = new ProjectMapper();
                return projectMapper.MapRow(dataTable.Rows[0]);
            }

            return null;
        }
        public static Project SelectFollowTeam(string teamId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Team, [new("teamId", teamId)]);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                return SelectOnly(dataTable.Rows[0]["projectId"].ToString());
            }

            return new Project();
        }

        #endregion

        #region SELECT PROJECT FOLLOW ROLE

        public static List<Project> SelectListRoleLecture(string userId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Project, [new("instructorId", userId)]);

            List<Project> projectList = new List<Project>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ProjectMapper projectMapper = new ProjectMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Project project = projectMapper.MapRow(row);
                    projectList.Add(project);
                }
            }

            return projectList;
        }
        public static List<Project> SelectByLectureAndYear(string userId, int year)
        {
            string sqlStr = "SELECT * FROM FUNC_GetProjectByLectureAndYear(@UserId, @YearSelected)";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@YearSelected", year)
            };
            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            List<Project> listProjects = new List<Project>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ProjectMapper projectMapper = new ProjectMapper();
                foreach (DataRow row in dataTable.Rows)
                {
                    Project project = projectMapper.MapRow(row);
                    listProjects.Add(project);
                }
            }

            return listProjects;
        }
        public static List<Project> SelectListRoleStudent(string userId)
        {
            string sqlStr = "SELECT * FROM FUNC_GetProjectsForStudent(@UserId, @Published, @Registered)";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Published", EnumUtil.GetDisplayName(EProjectStatus.PUBLISHED)),
                new SqlParameter("@Registered", EnumUtil.GetDisplayName(EProjectStatus.REGISTERED))
            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            List<Project> projectList = new List<Project>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ProjectMapper projectMapper = new ProjectMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Project project = projectMapper.MapRow(row);
                    projectList.Add(project);
                }
            }

            return projectList;
        }
        public static List<Project> SelectListModeMyProjects(string userId)
        {
            string sqlStr = "SELECT * FROM FUNC_GetMyProjects(@UserId)";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            List<Project> projectList = new List<Project>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ProjectMapper projectMapper = new ProjectMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Project project = projectMapper.MapRow(row);
                    projectList.Add(project);
                }
            }

            return projectList;
        }
        public static List<Project> SelectListModeMyCompletedProjects(string userId)
        {
            string sqlStr = "EXEC PROC_GetMyCompletedProjects @UserId, @Completed";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Completed", EnumUtil.GetDisplayName(EProjectStatus.COMPLETED))
            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            List<Project> projectList = new List<Project>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ProjectMapper projectMapper = new ProjectMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Project project = projectMapper.MapRow(row);
                    projectList.Add(project);
                }
            }

            return projectList;
        }

        #endregion

        #region SEARCH PROJECT

        public static List<Project> SearchRoleLecture(string userId, string topic)
        {
            string sqlStr = "SELECT * FROM FUNC_SearchRoleLecture(@UserId, @TopicSyntax)";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@TopicSyntax", topic + "%")
            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            List<Project> projectList = new List<Project>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ProjectMapper projectMapper = new ProjectMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Project project = projectMapper.MapRow(row);
                    projectList.Add(project);
                }
            }

            return projectList;
        }
        public static List<Project> SearchRoleStudent(string topic)
        {
            string sqlStr = "SELECT * FROM FUNC_SearchRoleStudent(@TopicSyntax, @Published, @Registered)";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@TopicSyntax", topic + "%"),
                new SqlParameter("@Published", EnumUtil.GetDisplayName(EProjectStatus.PUBLISHED)),
                new SqlParameter("@Registered", EnumUtil.GetDisplayName(EProjectStatus.REGISTERED))
            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            List<Project> projectList = new List<Project>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ProjectMapper projectMapper = new ProjectMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Project project = projectMapper.MapRow(row);
                    projectList.Add(project);
                }
            }

            return projectList;
        }

        #endregion

        #region PROJECT DAO CRUD

        public static void Insert(Project project, List<Technology> technologies)
        {
            DBExecution.InsertDynamic(DBTableNames.Project,
            [
                new ("projectId", project.ProjectId),
                new ("instructorId", project.InstructorId),
                new ("topic", project.Topic),
                new ("description", project.Description),
                new ("feature", project.Feature),
                new ("requirement", project.Requirement),
                new ("maxMember", project.MaxMember.ToString()),
                new ("status", EnumUtil.GetDisplayName(project.Status)),
                new ("createdAt", project.CreatedAt.ToString()),
                new ("createdBy", project.CreatedBy),
                new ("fieldId", project.FieldId)
            ]);

            TechnologyDAO.InsertProjectTechnology(project.ProjectId, technologies);
        }
        
        public static void Update(Project project, List<Technology> technologies)
        {
            TechnologyDAO.DeleteProjectTechnology(project.ProjectId);

            DBExecution.UpdateDynamic(DBTableNames.Project,
            [
                new ("instructorId", project.InstructorId),
                new ("topic", project.Topic),
                new ("description", project.Description),
                new ("feature", project.Feature),
                new ("requirement", project.Requirement),
                new ("maxMember", project.MaxMember.ToString()),
                new ("status", EnumUtil.GetDisplayName(project.Status)),
                new ("createdAt", project.CreatedAt.ToString()),
                new ("createdBy", project.CreatedBy),
                new ("fieldId", project.FieldId)
            ],
            [
                new("projectId", project.ProjectId)
            ]);

            TechnologyDAO.InsertProjectTechnology(project.ProjectId, technologies);
        }
        public static void UpdateStatus(Project project, EProjectStatus status)
        {
            DBExecution.UpdateDynamic(DBTableNames.Project,
            [
                new("status", EnumUtil.GetDisplayName(status))
            ],
            [
                new("projectId", project.ProjectId)
            ]);
        }
        public static void UpdateFavorite(string userId, string projectId, bool isFavorite)
        {
            string sqlStr = "EXEC PROC_UpdateFavoriteProject @UserId, @ProjectId, @IsFavorite";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@ProjectId", projectId),
                new SqlParameter("@IsFavorite", isFavorite)
            };

            DBExecution.SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }

        public static void Delete(string projectId)
        {
            string sqlStr = "EXEC PROC_DeleteProjectCascade @ProjectId";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", projectId),
            };

            DBExecution.SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }

        #endregion

        #region PROJECT DAO UTIL

        public static List<string> SelectFavoriteList(string userId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.FavoriteProject, [new("userId", userId)]);

            List<string> favoriteProjects = new List<string>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    favoriteProjects.Add(row["projectId"].ToString());
                }
            }

            return favoriteProjects;
        }

        #endregion

        #region CHECK INFORMATION

        public static bool CheckIsFavoriteProject(string userId, string projectId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.FavoriteProject,
                [
                    new("userId", userId),
                    new("projectId", projectId)
                ]);

            return dataTable != null && dataTable.Rows.Count > 0;
        }

        #endregion

        #region STATISTICAL

        public static Dictionary<string, int> GroupedByMonth(List<Project> listProjects)
        {
            ProjectMapper projectMapper = new ProjectMapper();
            DataTable projectTable = projectMapper.MapToTableType(listProjects);
            string sqlStr = "SELECT MonthName, ProjectCount FROM dbo.FUNC_GetProjectsGroupedByMonth(@ProjectList)" +
                            "ORDER BY MonthNumber;";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                 new SqlParameter
                 {
                    ParameterName = "@ProjectList",
                    SqlDbType = SqlDbType.Structured, 
                    TypeName = "ProjectTableType", 
                    Value = projectTable
                 }
            };
            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            Dictionary<string, int> result = new Dictionary<string, int>();
            if (dataTable.Rows.Count > 0)
            {
                foreach(DataRow row in dataTable.Rows)
                {
                    string monthName = row["MonthName"].ToString();
                    int projectCount = Convert.ToInt32(row["ProjectCount"]);

                    result.Add(monthName, projectCount);
                }
            }
            return result;
        }
        public static Dictionary<EProjectStatus, int> GroupedByStatus(List<Project> listProjects)
        {
            ProjectMapper projectMapper = new ProjectMapper();
            DataTable projectTable = projectMapper.MapToTableType(listProjects);
            string sqlStr = "SELECT * FROM dbo.FUNC_GetProjectsGroupedByStatus(@ProjectList)";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                 new SqlParameter
                 {
                    ParameterName = "@ProjectList",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "ProjectTableType",
                    Value = projectTable
                 }
            };
            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            Dictionary<EProjectStatus, int> result = new Dictionary<EProjectStatus, int>();

            foreach (DataRow row in dataTable.Rows)
            {
                EProjectStatus status = EnumUtil.GetEnumFromDisplayName<EProjectStatus>(row["ProjectStatus"].ToString());
                int projectCount = Convert.ToInt32(row["ProjectCount"]);

                result.Add(status, projectCount);
            }
            return result;
        }
        public static Dictionary<string, int> GetTopField(List<Project> listProjects)
        {
            ProjectMapper projectMapper = new ProjectMapper();
            DataTable projectTable = projectMapper.MapToTableType(listProjects);
            string sqlStr = "SELECT * FROM FUNC_GetTopFieldsByProjects(@ProjectList) ORDER BY ProjectCount DESC";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
                 new SqlParameter
                 {
                    ParameterName = "@ProjectList",
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "ProjectTableType",
                    Value = projectTable
                 }
            };
            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            MessageBox.Show(dataTable.Rows.Count.ToString());

            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (DataRow row in dataTable.Rows)
            {
                string fieldName = row["FieldName"].ToString();
                int projectCount = Convert.ToInt32(row["ProjectCount"]);
                MessageBox.Show($"FieldName: {row["FieldName"]}, ProjectCount: {row["ProjectCount"]}");
                result.Add(fieldName, projectCount);
            }
            return result;
        }
        #endregion

    }
}
