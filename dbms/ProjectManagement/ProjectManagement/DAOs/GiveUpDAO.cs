using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Database;
using ProjectManagement.Models;
using ProjectManagement.Mappers.Implement;
using ProjectManagement.Utils;
using System.Data.SqlClient;
using ProjectManagement.Enums;

namespace ProjectManagement.DAOs
{
    internal class GiveUpDAO : DBConnection
    {
        #region CHECK INFORMATIONS

        public static bool CheckIsNotEmpty(string input, string fieldName)
        {
            string sqlStr = "SELECT * FROM dbo.FUNC_IsNotEmpty(@Input, @FieldName)";
            List<SqlParameter> parameters = new List<SqlParameter>
            {
            new SqlParameter("@Input", input),
            new SqlParameter("@FieldName", fieldName)
            };
            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);
            return dataTable.Rows.Count > 0 && Convert.ToBoolean(dataTable.Rows[0]["IsValid"]);
        }

        #endregion

        public static GiveUp SelectFollowProject(string projectId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.GiveUp, [new("projectId", projectId)]);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                GiveUpMapper giveUpMapper = new GiveUpMapper();
                return giveUpMapper.MapRow(dataTable.Rows[0]);
            }
            return null;
        }
        public static void UpdateStatus(string projectId, EGiveUpStatus newStatus, EGiveUpStatus oldStatus)
        {
            DBExecution.UpdateDynamic(DBTableNames.Project,
            [
                new ("status", EnumUtil.GetDisplayName(newStatus))
            ],
            [
                new ("projectId", projectId),
                new("status", EnumUtil.GetDisplayName(oldStatus))
            ]);
        }
        public static void Insert(GiveUp giveUp)
        {
            DBExecution.InsertDynamic(DBTableNames.GiveUp, 
                [
                    new("projectId", giveUp.ProjectId),
                    new("userId", giveUp.UserId),
                    new("reason", giveUp.Reason),
                    new("createdAt", giveUp.CreatedAt.ToString()),
                    new("status", EnumUtil.GetDisplayName(giveUp.Status))
                ]);
        }
        public static void Delete(string projectId)
        {
            DBExecution.DeleteDynamic(DBTableNames.GiveUp, [new("projectId", projectId)]);
        }
    }
}
