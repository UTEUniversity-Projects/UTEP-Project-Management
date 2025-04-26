using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Database;
using ProjectManagement.Models;
using ProjectManagement.Mappers.Implement;
using System.Data.SqlClient;
using ProjectManagement.Utils;
using ProjectManagement.Enums;
using ProjectManagement.MetaData;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProjectManagement.DAOs
{
    internal class TeamDAO : DBConnection
    {

        #region SELECT TEAM

        public static Team SelectFollowProject(string projectId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Team, [new("projectId", projectId)]);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                TeamMapper teamMapper = new TeamMapper();
                return teamMapper.MapRow(dataTable.Rows[0]);
            }

            return null;
        }
        public static List<Team> SelectList(string projectId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Team, [new("projectId", projectId)]);

            List<Team> teams = new List<Team>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                TeamMapper teamMapper = new TeamMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Team team = teamMapper.MapRow(row);
                    teams.Add(team);
                }
            }

            return teams;
        }
        public static List<Team> SelectFollowUser(string userId)
        {
            DataTable getTeam = DBExecution.GetDynamic(DBTableNames.JoinTeam, [new("studentId", userId)]);

            if (getTeam != null && getTeam.Rows.Count > 0)
            {
                List<Team> teams = new List<Team>();

                foreach (DataRow row in getTeam.Rows)
                {
                    DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Team, [new("teamId", row["teamId"].ToString())]);

                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        TeamMapper teamMapper = new TeamMapper();
                        Team team = teamMapper.MapRow(dataTable.Rows[0]);
                        teams.Add(team);
                    }
                }
                return teams;
            }

            return new List<Team>();
        }

        #endregion

        #region SELECT MEMBERS

        public static List<Member> GetMembersByTeamId(string teamId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.JoinTeam, [new("teamId", teamId)]);

            List<Member> list = new List<Member>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    Users student = UserDAO.SelectOnlyByID(row["studentId"].ToString());
                    ETeamRole role = EnumUtil.GetEnumFromDisplayName<ETeamRole>(row["role"].ToString());
                    DateTime joinAt = DateTime.Parse(row["joinAt"].ToString());

                    list.Add(new Member(student, role, joinAt));
                }
            }

            return list.OrderBy(m => m.Role).ToList();
        }

        #endregion

        #region TEAM DAO CRUD

        public static void Insert(Team team, List<Member> members)
        {
            DBExecution.InsertDynamic(DBTableNames.Team,
            [
                new ("teamId", team.TeamId),
                new ("teamName", team.TeamName),
                new ("avatar", team.Avatar),
                new ("createdAt", team.CreatedAt.ToString()),
                new ("createdBy", team.CreatedBy),
                new ("projectId", team.ProjectId),
                new ("status", EnumUtil.GetDisplayName(team.Status))
            ]);

            InsertTeamMembers(team.TeamId, members);
        }
        public static void InsertTeamMembers(string teamId, List<Member> members)
        {
            foreach (Member member in members)
            {
                DBExecution.InsertDynamic(DBTableNames.JoinTeam,
                [
                    new ("teamId", teamId),
                    new ("studentId", member.User.UserId),
                    new ("role", EnumUtil.GetDisplayName(member.Role)),
                    new ("joinAt", member.JoinAt.ToString())
                ]);
            }
        }

        public static void UpdateTeamStatus(string teamId, ETeamStatus status)
        {
            DBExecution.UpdateDynamic(DBTableNames.Team,
            [
                new("status", EnumUtil.GetDisplayName(status))
            ],
            [
                new("teamId", teamId)
            ]);
        }

        public static void Delete(string teamId)
        {
            string sqlStr = "EXEC PROC_DeleteTeamCascade @TeamId";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@TeamId", teamId),
            };

            DBExecution.SQLExecuteNonQuery(sqlStr, parameters, string.Empty);
        }
        public static void DeleteListTeam(List<Team> teams)
        {
            foreach (Team team in teams)
            {
                Delete(team.TeamId);
            }
        }
        public static void DeleteFollowProject(string projectId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Team, [new("projectId", projectId)]);

            foreach (DataRow row in dataTable.Rows)
            {
                Delete(row["teamId"].ToString());
            }
        }

        #endregion

        #region TEAM DAO UTIL

        public static int CountTeamFollowState(string projectId, EProjectStatus status)
        {
            string sqlStr = "SELECT * FROM FUNC_CountTeamsFollowState(@ProjectId, @Status)";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", projectId),
                new SqlParameter("@Status", EnumUtil.GetDisplayName(status))
            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);

            int num = 0;
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                int.TryParse(dataTable.Rows[0]["NumTeams"].ToString(), out num);
            }

            return num;
        }

        #endregion

    }
}
