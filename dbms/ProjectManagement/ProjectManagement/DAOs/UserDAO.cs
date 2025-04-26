using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.ApplicationServices;
using ProjectManagement.Database;
using ProjectManagement.Models;
using ProjectManagement.Enums;
using ProjectManagement.Mappers.Implement;
using System.Data.SqlClient;
using ProjectManagement.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace ProjectManagement.DAOs
{
    internal class UserDAO : DBConnection
    {

        #region SELECT USER

        public static List<Users> SelectListByUserName(string userName, EUserRole role)
        {
            string sqlStr = "EXEC PROC_SelectUsersByUserNameAndRole @UserNameSyntax, @Role";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserNameSyntax", userName + "%"),
                new SqlParameter("@Role", EnumUtil.GetDisplayName(role))
            };

            DataTable dataTable = DBExecution.SQLExecuteQuery(sqlStr, parameters, string.Empty);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                List<Users> usersList = new List<Users>();
                UserMapper userMapper = new UserMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Users user = userMapper.MapRow(row);

                    usersList.Add(user);
                }
                return usersList;
            }
            return new List<Users>();
        }
        public static Users SelectOnlyByID(string userId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.User, [new("userId", userId)]);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                UserMapper userMapper = new UserMapper();
                return userMapper.MapRow(dataTable.Rows[0]);
            }
            return null;
        }
        public static Users SelectOnlyByEmailAndPassword(string email, string password)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.User, [new("email", email), new ("password", password)]);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                UserMapper userMapper = new UserMapper();
                return userMapper.MapRow(dataTable.Rows[0]);
            }
            return null;
        }        

        #endregion

        #region SELECT LIST ID USER

        public static List<string> SelectListId(EUserRole role)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.User, [new("role", EnumUtil.GetDisplayName(role))]);

            List<string> list = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(row["userId"].ToString());
            }

            return list;
        }

        #endregion

        #region USER DAO EXECUTION

        public static void Insert(Users user)
        {
            DBExecution.InsertDynamic(DBTableNames.User,
                [
                    new("userId", user.UserId),
                    new("userName", user.UserName),
                    new("fullName", user.FullName),
                    new("password", user.Password),
                    new("email", user.Email),
                    new("phoneNumber", user.PhoneNumber),
                    new("dateOfBirth", user.DateOfBirth.ToString()),
                    new("citizenCode", user.CitizenCode),
                    new("university", user.University),
                    new("faculty", user.Faculty),
                    new("workCode", user.WorkCode),
                    new("gender", EnumUtil.GetDisplayName(user.Gender)),
                    new("avatar", user.Avatar),
                    new("role", EnumUtil.GetDisplayName(user.Role)),
                    new("joinAt", user.JoinAt.ToString())
                ]);
        }
        public static void Update(Users user)
        {
            DBExecution.UpdateDynamic(DBTableNames.User, 
                [
                    new("userName", user.UserName), 
                    new("fullName", user.FullName), 
                    new("citizenCode", user.CitizenCode), 
                    new("dateOfBirth", user.DateOfBirth.ToString()), 
                    new("phoneNumber", user.PhoneNumber), 
                    new("email", user.Email), 
                    new("gender", EnumUtil.GetDisplayName(user.Gender))], 
                [
                    new("userId", user.UserId)
                ]);
        }


        #endregion
        
    }
}
