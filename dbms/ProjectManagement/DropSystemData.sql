-- DROP all triggers
DROP TRIGGER TRIG_TeamRegisterdProject;
DROP TRIGGER TRIG_SetProjectWaiting;
DROP TRIGGER TRIG_ResetProjectProcessing;

-- DROP all tables
DROP TABLE FieldTechnology;
DROP TABLE FavoriteProject;
DROP TABLE FavoriteTask;
DROP TABLE FavoriteNotification;
DROP TABLE GiveUp;
DROP TABLE TaskStudent;
DROP TABLE ProjectTechnology;
DROP TABLE ViewNotification;
DROP TABLE Notification;
DROP TABLE JoinTeam;
DROP TABLE Evaluation;
DROP TABLE Comment;
DROP TABLE Meeting;
DROP TABLE Task;
DROP TABLE Team;
DROP TABLE Project;
DROP TABLE Technology;
DROP TABLE Field;
DROP TABLE Users;

-- DROP all views
DROP VIEW VIEW_CanRegisterdProject;
DROP VIEW VIEW_TaskTeam;
DROP VIEW VIEW_MeetingTeam;
DROP VIEW VIEW_TaskStudent;
DROP VIEW VIEW_StudentEvaluation;
DROP VIEW VIEW_FieldTechnology;
DROP VIEW VIEW_TaskByStudent;
DROP VIEW VIEW_TasksByProject;
DROP VIEW VIEW_TasksByTeam;
DROP VIEW VIEW_FavoriteTasks;
DROP VIEW VIEW_GiveUpDetails;
DROP VIEW VIEW_UserDetails;

-- DROP all functions
DROP FUNCTION FUNC_IsNotEmpty;
DROP FUNCTION FUNC_IsValidInRange;
DROP FUNCTION FUNC_CheckAge;
DROP FUNCTION FUNC_CheckStartDate;
DROP FUNCTION FUNC_CheckEndDate;
DROP FUNCTION FUNC_GetAllMonths;
DROP FUNCTION FUNC_GetProjectsForStudent;
DROP FUNCTION FUNC_GetMyProjects;
DROP FUNCTION FUNC_SearchRoleLecture;
DROP FUNCTION FUNC_SearchRoleStudent;
DROP FUNCTION FUNC_CountTeamsFollowState;
DROP FUNCTION FUNC_GetEvaluationByStudentIdAndYear;
DROP FUNCTION FUNC_GetProjectsGroupedByMonth;
DROP FUNCTION FUNC_GetProjectsGroupedByStatus;
DROP FUNCTION FUNC_GetEvaluationsGroupedByMonth;
DROP FUNCTION FUNC_GetProjectByLectureAndYear;
DROP FUNCTION FUNC_CalUtil;
DROP FUNCTION FUNC_CalAvgProgress;
DROP FUNCTION FUNC_GetTopField;
DROP FUNCTION FUNC_GetTopTechnology;

-- DROP all procedures
DROP PROCEDURE PROC_InsertDynamic;
DROP PROCEDURE PROC_UpdateDynamic;
DROP PROCEDURE PROC_DeleteDynamic;
DROP PROCEDURE PROC_GetDynamic;
DROP PROCEDURE PROC_UpdateFavoriteProject;
DROP PROCEDURE PROC_UpdateFavoriteNotification;
DROP PROCEDURE PROC_InsertAssignStudent;
DROP PROCEDURE PROC_DeleteTaskCascade;
DROP PROCEDURE PROC_DeleteTeamCascade;
DROP PROCEDURE PROC_DeleteProjectCascade;
DROP PROCEDURE PROC_SearchTaskByTitle;
DROP PROCEDURE PROC_CheckNonExist;
DROP PROCEDURE PROC_GetMyCompletedProjects;
DROP PROCEDURE PROC_SelectUsersByUserNameAndRole;
DROP PROCEDURE PROC_GetNotificationsByUserId;

-- DROP all data-type
DROP TYPE ConditionType;
DROP TYPE ProjectTableType;
DROP TYPE EvaluationTableType;
DROP TYPE TaskTableType;



--DROP PROCEDURE PROC_DeleteByKey;
--DROP PROCEDURE PROC_GetByKey;
--DROP PROCEDURE PROC_UpdateGiveUpStatus;
------
--DROP FUNCTION FUNC_GetFavoriteProjects;
--DROP FUNCTION FUNC_CheckIsFavoriteProject;
--DROP FUNCTION FUNC_CheckIsFavoriteTask;
--DROP FUNCTION FUNC_GetTaskIdsByProjectId;
--DROP FUNCTION FUNC_GetTasksByTeamId;
--DROP FUNCTION FUNC_SelectTeamsByUserId;
--DROP FUNCTION FUNC_GetMembersByTeamId;
--DROP FUNCTION FUNC_GetTasksByProjectAndStudent;
--DROP FUNCTION FUNC_GetFavoriteTasksByProjectIdAndUserId;
--DROP FUNCTION FUNC_GetMeetingsByProjectId;
--DROP FUNCTION FUNC_SelectUserIdsByRole;
--DROP FUNCTION FUNC_SelectAllFields;
------
--DROP FUNCTION FUNC_GetMyCompletedProjects;
--DROP FUNCTION FUNC_SelectUsersByUserNameAndRole;
--DROP FUNCTION FUNC_GetNotificationsByUserId;

--DROP VIEW VIEW_CommentWithUser;
--DROP TRIGGER TRIG_DeleteTechnologyBeforeUpdateProject;
--DROP FUNCTION FUNC_SelectUserByEmailAndPassword;
--DROP FUNCTION FUNC_SelectGiveUpByProjectId;
--DROP FUNCTION FUNC_GetEvaluationByStudentId;
--DROP FUNCTION FUNC_GetFavoriteNotifications;
--DROP FUNCTION FUNC_GetTaskById;
--DROP FUNCTION FUNC_SelectUserById;
--DROP FUNCTION FUNC_GetMeetingById;
--DROP FUNCTION FUNC_ViewComment;
--DROP FUNCTION FUNC_GetTeamIdsByProjectId;
--DROP FUNCTION FUNC_SelectFieldById;
--DROP FUNCTION FUNC_SelectTechnologiesByProject;
--DROP FUNCTION FUNC_SelectTechnologiesByField;
--DROP FUNCTION FUNC_GetProjectIdByTeamId;
--DROP FUNCTION FUNC_GetEvaluation;
--DROP FUNCTION FUNC_ViewComment;
--DROP PROCEDURE PROC_DeleteEvaluationByTaskId;
--DROP PROCEDURE PROC_DeleteFavoriteTaskByTaskId;
--DROP PROCEDURE PROC_DeleteGiveUpByProjectId;
--DROP PROCEDURE PROC_DeleteNotification;
--DROP PROCEDURE PROC_DeleteTaskByTaskId;
--DROP PROCEDURE PROC_DeleteTaskStudentByTaskId;
--DROP PROCEDURE PROC_CreateComment;
--DROP PROCEDURE PROC_DeleteMeeting;
--DROP PROCEDURE PROC_CreateMeeting;
--DROP PROCEDURE PROC_UpdateMeeting;
--DROP PROCEDURE PROC_UpdateUser;
--DROP PROCEDURE PROC_UpdateTask;
--DROP PROCEDURE PROC_InsertUser;
--DROP PROCEDURE PROC_InsertGiveUp;
--DROP PROCEDURE PROC_AddTask;
--DROP PROCEDURE PROC_DeleteFavoriteTask;
--DROP PROCEDURE PROC_InsertFavoriteTask;
--DROP PROCEDURE PROC_DeleteProject;
--DROP PROCEDURE PROC_DeleteTeam;
--DROP PROCEDURE PROC_UpdateProjectStatus;
--DROP PROCEDURE PROC_UpdateProject;
--DROP PROCEDURE PROC_InsertProject;
--DROP PROCEDURE PROC_InsertProjectTechnology;
--DROP PROCEDURE PROC_InsertTeam;
--DROP PROCEDURE PROC_InsertTeamMember;
--DROP PROCEDURE PROC_UpdateTeamStatus;
--DROP PROCEDURE PROC_AddEvaluation;
--DROP PROCEDURE PROC_UpdateEvaluation;
--DROP PROCEDURE PROC_AddNotification;
--DROP PROCEDURE PROC_AddViewNotification;
--DROP PROCEDURE PROC_UpdateViewNotification;
--DROP PROCEDURE PROC_AddTask;
--DROP PROCEDURE PROC_CreateComment;


