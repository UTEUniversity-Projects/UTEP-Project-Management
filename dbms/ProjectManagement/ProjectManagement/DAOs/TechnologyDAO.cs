using ProjectManagement.Database;
using ProjectManagement.Mappers.Implement;
using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.DAOs
{
    internal class TechnologyDAO
    {
        private static Technology SelectOnly(string technologyId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Technology, [new("technologyId", technologyId)]);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                TechnologyMapper fieldMapper = new TechnologyMapper();
                return fieldMapper.MapRow(dataTable.Rows[0]);
            }

            return null;
        }
        public static List<Technology> SelectListByProject(string projectId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.ProjectTechnology, [new("projectId", projectId)]);

            List<Technology> technologies = new List<Technology>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    technologies.Add(SelectOnly(row["technologyId"].ToString()));
                }
            }

            return technologies;
        }
        public static List<Technology> SelectListByField(string fieldId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.FieldTechnology, [new("fieldId", fieldId)]);

            List<Technology> technologies = new List<Technology>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    technologies.Add(SelectOnly(row["technologyId"].ToString()));
                }
            }

            return technologies;
        }

        public static void InsertProjectTechnology(string projectId, List<Technology> technologies)
        {
            technologies = technologies.Distinct().ToList();

            foreach (Technology technology in technologies)
            {
                DBExecution.InsertDynamic(DBTableNames.ProjectTechnology,
                [
                    new ("projectId", projectId),
                    new ("technologyId", technology.TechnologyId)
                ]);
            }
        }
        public static void DeleteProjectTechnology(string projectId)
        {
            DBExecution.DeleteDynamic(DBTableNames.ProjectTechnology, [new("projectId", projectId)]);
        }

        public static string GetListTechnology(string projectId)
        {
            List<Technology> technologies = SelectListByProject(projectId);
            return string.Join(", ", technologies.Select(t => t.Name));
        }
        public static Dictionary<string, int> TopTechnology()
        {
            string sqlStr = "SELECT * FROM FUNC_GetTopTechnology() ORDER BY ProjectCount DESC";

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, new List<SqlParameter>(), string.Empty);
            Dictionary<string, int> result = new Dictionary<string, int>();

            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    string technologyName = row["TechnologyName"].ToString();
                    int projectCount = Convert.ToInt32(row["ProjectCount"]);

                    result.Add(technologyName, projectCount);
                }
            }

            return result;
        }

    }
}
