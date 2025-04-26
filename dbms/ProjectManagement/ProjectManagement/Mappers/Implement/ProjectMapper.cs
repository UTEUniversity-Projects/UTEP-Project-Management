using System.Data;
using ProjectManagement.Models;
using ProjectManagement.Process;
using ProjectManagement.Enums;
using ProjectManagement.Utils;

namespace ProjectManagement.Mappers.Implement
{
    internal class ProjectMapper : IRowMapper<Project>
    {
        public Project MapRow(DataRow row)
        {
            string projectId = row["projectId"].ToString();
            string instructorId = row["instructorId"].ToString();
            string topic = row["topic"].ToString();
            string description = row["description"].ToString();
            string feature = row["feature"].ToString();
            string requirement = row["requirement"].ToString();
            int maxMember = int.Parse(row["maxMember"].ToString());
            EProjectStatus status = EnumUtil.GetEnumFromDisplayName<EProjectStatus>(row["status"].ToString());
            DateTime createdAt = DateTime.Parse(row["createdAt"].ToString());
            string createdBy = row["createdBy"].ToString();
            string fieldId = row["fieldId"].ToString();

            Project project = new Project(projectId, instructorId, topic, description, feature, requirement, maxMember,
                status, createdAt, createdBy, fieldId);

            return project;
        }

        public DataTable MapToTableType(List<Project> list)
        {
            DataTable projectTable = new DataTable();
            projectTable.Columns.Add("projectId", typeof(string));
            projectTable.Columns.Add("instructorId", typeof(string));
            projectTable.Columns.Add("topic", typeof(string));
            projectTable.Columns.Add("description", typeof(string));
            projectTable.Columns.Add("feature", typeof(string));
            projectTable.Columns.Add("requirement", typeof(string));
            projectTable.Columns.Add("maxMember", typeof(int));
            projectTable.Columns.Add("status", typeof(string));
            projectTable.Columns.Add("createdAt", typeof(DateTime));
            projectTable.Columns.Add("createdBy", typeof(string));
            projectTable.Columns.Add("fieldId", typeof(string));

            foreach (var project in list)
            {
                projectTable.Rows.Add(
                    project.ProjectId, project.InstructorId, project.Topic, project.Description,
                    project.Feature, project.Requirement, project.MaxMember, EnumUtil.GetDisplayName(project.Status),
                    project.CreatedAt, project.FieldId);
            }
            return projectTable;
        }
    }
}
