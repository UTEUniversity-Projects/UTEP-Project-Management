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
using ProjectManagement.Utils;

namespace ProjectManagement.DAOs
{
    internal class MeetingDAO : DBConnection
    {

        #region SELECT MEETING

        public static Meeting SelectOnly(string meetingId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Meeting, [new("meetingId", meetingId)]);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                MeetingMapper meetingMapper = new MeetingMapper();
                return meetingMapper.MapRow(dataTable.Rows[0]);
            }

            return null;

        }
        public static List<Meeting> SelectByProject(string projectId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Meeting, [new("projectId", projectId)]);

            List<Meeting> listMeetings = new List<Meeting>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    Meeting meeting = DBGetModel.GetModelFromDataRow(row, new MeetingMapper());
                    listMeetings.Add(meeting);
                }
            }

            return listMeetings;

        }

        #endregion

        #region MEETING DAO EXCUTION

        public static void Insert(Meeting meeting)
        {
            DBExecution.InsertDynamic(DBTableNames.Meeting,
           [
               new("meetingId", meeting.MeetingId),
               new("title", meeting.Title),
               new("description", meeting.Description),
               new("startAt", meeting.StartAt.ToString()),
               new("location", meeting.Location),
               new("link", meeting.Link),
               new("createdAt", meeting.CreatedAt.ToString()),
               new("createdBy", meeting.CreatedBy),
               new("projectId", meeting.ProjectId),

           ]);

        }
        public static void Delete(Meeting meeting)
        {
            DBExecution.DeleteDynamic(DBTableNames.Meeting, [new("meetingId", meeting.MeetingId)]);
        }

        public static void Update(Meeting meeting)
        {
            DBExecution.UpdateDynamic(DBTableNames.Meeting,
               [
                   new("meetingId", meeting.MeetingId),
                   new("title", meeting.Title),
                   new("description", meeting.Description),
                   new("startAt", meeting.StartAt.ToString()),
                   new("location", meeting.Location),
                   new("link", meeting.Link),
                   new("createdAt", meeting.CreatedAt.ToString()),
                   new("createdBy", meeting.CreatedBy),
                   new("projectId", meeting.ProjectId),
               ],
               [
                   new("meetingId", meeting.MeetingId)
               ]);

        }

        #endregion

    }
}
