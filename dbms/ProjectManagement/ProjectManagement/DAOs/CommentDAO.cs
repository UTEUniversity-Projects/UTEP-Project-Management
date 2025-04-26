using Microsoft.VisualBasic.ApplicationServices;
using ProjectManagement.Database;
using ProjectManagement.Mappers.Implement;
using ProjectManagement.Models;
using ProjectManagement.Utils;
using System.Data;
using System.Data.SqlClient;

namespace ProjectManagement.DAOs
{
    internal class CommentDAO : DBConnection
    {
        public static List<Comment> SelectList(string taskId)
        {
            DataTable dataTable = DBExecution.GetDynamic(DBTableNames.Comment, [new("taskId", taskId)]);

            List<Comment> comments = new List<Comment>();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                CommentMapper commentMapper = new CommentMapper();

                foreach (DataRow row in dataTable.Rows)
                {
                    Comment comment = commentMapper.MapRow(row);
                    comments.Add(comment);
                }
            }

            return comments;
        }
        public static void Insert(Comment comment)
        {
            DBExecution.InsertDynamic(DBTableNames.Comment,
            [
                new("commentId", comment.CommentId),
                new("content", comment.Content),
                new("createdAt", comment.CreatedAt.ToString()),
                new("createdBy", comment.CreatedBy),
                new("taskId", comment.TaskId)
            ]);

        }
        public static void DeleteByTaskId(string taskId)
        {
            DBExecution.DeleteDynamic(DBTableNames.Comment,
           [
               new("taskId", taskId),
           ]);
        }
    }
}
