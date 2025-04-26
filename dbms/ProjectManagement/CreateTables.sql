--USE ProjectManagement;
--GO

CREATE TABLE Users (
    userId VARCHAR(20),
    userName VARCHAR(50) NOT NULL,
    fullName NVARCHAR(255) NOT NULL,
    password VARCHAR(50) NOT NULL,
    email VARCHAR(50) NOT NULL,
    phoneNumber VARCHAR(15) NOT NULL,
    dateOfBirth DATETIME NOT NULL,
    citizenCode VARCHAR(20) NOT NULL,
    university NVARCHAR(100) NOT NULL,
    faculty NVARCHAR(100) NOT NULL,
	workCode VARCHAR(20) NOT NULL,
    gender NVARCHAR(10) NOT NULL,
    avatar VARCHAR(30) NOT NULL,
	role VARCHAR(20) NOT NULL,
	joinAt DATETIME NOT NULL,
    CONSTRAINT PK_User PRIMARY KEY (userId)
);
GO

CREATE TABLE Field (
    fieldId VARCHAR(20),
    name NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Field PRIMARY KEY (fieldId)
);
GO

CREATE TABLE Technology (
    technologyId VARCHAR(20),
    name NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Technology PRIMARY KEY (technologyId)
);
GO

CREATE TABLE Project (
    projectId VARCHAR(20),
    instructorId VARCHAR(20) NOT NULL,
    topic NVARCHAR(255) NOT NULL,
    description NTEXT NOT NULL,
    feature NTEXT NOT NULL,
    requirement NTEXT NOT NULL,
    maxMember INT NOT NULL,
	status VARCHAR(20) NOT NULL,
	createdAt DATETIME NOT NULL,
	createdBy VARCHAR(20) NOT NULL,
	fieldId VARCHAR(20) NOT NULL,
    CONSTRAINT PK_Project PRIMARY KEY (projectId),
    CONSTRAINT FK_ProjectLecture FOREIGN KEY (instructorId) REFERENCES Users(userId),
	CONSTRAINT FK_ProjectField FOREIGN KEY (fieldId) REFERENCES Field(fieldId)
);
GO

CREATE TABLE Team (
    teamId VARCHAR(20),
    teamName NVARCHAR(100) NOT NULL,
    avatar VARCHAR(30) NOT NULL,
    createdAt DATETIME NOT NULL,
    createdBy VARCHAR(20) NOT NULL,
	projectId VARCHAR(20) NOT NULL,
	status VARCHAR(20) NOT NULL,
    CONSTRAINT PK_Team PRIMARY KEY (teamId),
    CONSTRAINT FK_TeamStudent FOREIGN KEY (createdBy) REFERENCES Users(userId),
	CONSTRAINT FK_TeamProject FOREIGN KEY (projectId) REFERENCES Project(projectId)
);
GO

CREATE TABLE Task (
    taskId VARCHAR(20),
	startAt DATETIME NOT NULL,
	endAt DATETIME NOT NULL,
    title NTEXT NOT NULL,
    description NTEXT NOT NULL,
    progress REAL NOT NULL,
    priority VARCHAR(20) NOT NULL,
	createdAt DATETIME NOT NULL,
	createdBy VARCHAR(20) NOT NULL,
	projectId VARCHAR(20) NOT NULL,
    CONSTRAINT PK_Task PRIMARY KEY (taskId),
	CONSTRAINT FK_TaskUser FOREIGN KEY (createdBy) REFERENCES Users(userId),
	CONSTRAINT FK_TaskProject FOREIGN KEY (projectId) REFERENCES Project(projectId)
);
GO

CREATE TABLE Meeting (
    meetingId VARCHAR(20),
    title NTEXT NOT NULL,
    description NTEXT NOT NULL,
    startAt DATETIME NOT NULL,
    location NTEXT NOT NULL,
    link TEXT NOT NULL,
	createdAt DATETIME NOT NULL,
	createdBy VARCHAR(20) NOT NULL,
	projectId VARCHAR(20) NOT NULL,
    CONSTRAINT PK_Meeting PRIMARY KEY (meetingId),
	CONSTRAINT FK_MeetingUser FOREIGN KEY (createdBy) REFERENCES Users(userId),
	CONSTRAINT FK_MeetingProject FOREIGN KEY (projectId) REFERENCES Project(projectId)
);
GO

CREATE TABLE Comment (
    commentId VARCHAR(20),
    content NTEXT NOT NULL,
	createdAt DATETIME NOT NULL,
	createdBy VARCHAR(20) NOT NULL,
	taskId VARCHAR(20) NOT NULL,
    CONSTRAINT PK_Comment PRIMARY KEY (commentId),
	CONSTRAINT FK_CommentUser FOREIGN KEY (createdBy) REFERENCES Users(userId),
	CONSTRAINT FK_CommentTask FOREIGN KEY (taskId) REFERENCES Task(taskId)
);
GO

CREATE TABLE Evaluation (
    evaluationId VARCHAR(20),
    content NTEXT NOT NULL,
    completionRate REAL NOT NULL,
    score REAL NOT NULL,
    evaluated BIT NOT NULL,
	createdAt DATETIME NOT NULL,
	createdBy VARCHAR(20) NOT NULL,
	studentId VARCHAR(20) NOT NULL,
	taskId VARCHAR(20) NOT NULL,
    PRIMARY KEY (evaluationId),
	CONSTRAINT PK_Evaluation FOREIGN KEY (createdBy) REFERENCES Users(userId),
	CONSTRAINT FK_EvaluationStudent FOREIGN KEY (studentId) REFERENCES Users(userId),
	CONSTRAINT FK_EvaluationTask FOREIGN KEY (taskId) REFERENCES Task(taskId)
);
GO

CREATE TABLE Notification (
    notificationId VARCHAR(20),
    title NTEXT NOT NULL,
    content NTEXT NOT NULL,
    type VARCHAR(20) NOT NULL,
    createdAt DATETIME NOT NULL,
    CONSTRAINT PK_Notification PRIMARY KEY (notificationId)
);
GO

CREATE TABLE JoinTeam (
	teamId VARCHAR(20),
	studentId VARCHAR(20),
	role VARCHAR(20) NOT NULL,
	joinAt DATETIME NOT NULL,
	CONSTRAINT PK_JoinTeam PRIMARY KEY (teamId, studentId),
	CONSTRAINT FK_JT_Team FOREIGN KEY (teamId) REFERENCES Team(teamId),
	CONSTRAINT FK_JT_Student FOREIGN KEY (studentId) REFERENCES Users(userId)
);
GO

CREATE TABLE ViewNotification (
    userId VARCHAR(20),
    notificationId VARCHAR(20),
	seen BIT NOT NULL,
    CONSTRAINT PK_ViewNotification PRIMARY KEY (userId, notificationId),
    CONSTRAINT FK_VN_User FOREIGN KEY (userId) REFERENCES Users(userId),
    CONSTRAINT FK_VN_Notification FOREIGN KEY (notificationId) REFERENCES Notification(notificationId)
);
GO

CREATE TABLE ProjectTechnology (
    projectId VARCHAR(20),
    technologyId VARCHAR(20),
    CONSTRAINT PK_ProjectTechnology PRIMARY KEY (projectId, technologyId),
    CONSTRAINT FK_PT_Project FOREIGN KEY (projectId) REFERENCES Project(projectId),
    CONSTRAINT FK_PT_Technology FOREIGN KEY (technologyId) REFERENCES Technology(technologyId)
);
GO

CREATE TABLE TaskStudent (
    taskId VARCHAR(20),
    studentId VARCHAR(20),
    CONSTRAINT PK_TaskStudent PRIMARY KEY (taskId, studentId),
    CONSTRAINT FK_TS_Task FOREIGN KEY (taskId) REFERENCES Task(taskId),
    CONSTRAINT FK_TS_Student FOREIGN KEY (studentId) REFERENCES Users(userId)
);
GO

CREATE TABLE GiveUp (
	projectId VARCHAR(20),
	userId VARCHAR(20),
	reason NTEXT NOT NULL,
	createdAt DATETIME NOT NULL,
	status VARCHAR(20) NOT NULL,
	CONSTRAINT PK_GaveUp PRIMARY KEY (projectId),
	CONSTRAINT FK_GU_Project FOREIGN KEY (projectId) REFERENCES Project(projectId),
	CONSTRAINT FK_GU_User FOREIGN KEY (userId) REFERENCES Users(userId)
);
GO

CREATE TABLE FavoriteProject (
	userId VARCHAR(20),
	projectId VARCHAR(20),
	CONSTRAINT PK_FavouriteProject PRIMARY KEY (userId, projectId),
	CONSTRAINT FK_FP_User FOREIGN KEY (userId) REFERENCES Users(userId),
	CONSTRAINT FK_FP_Project FOREIGN KEY (projectId) REFERENCES Project(projectId)
);
GO

CREATE TABLE FavoriteTask (
	userId VARCHAR(20),
	taskId VARCHAR(20),
	CONSTRAINT PK_FavouriteTask PRIMARY KEY (userId, taskId),
	CONSTRAINT FK_FT_User FOREIGN KEY (userId) REFERENCES Users(userId),
	CONSTRAINT FK_FT_Task FOREIGN KEY (taskId) REFERENCES Task(taskId)
);
GO

CREATE TABLE FavoriteNotification (
	userId VARCHAR(20),
	notificationId VARCHAR(20),
	CONSTRAINT PK_FavouriteNotification PRIMARY KEY (userId, notificationId),
	CONSTRAINT FK_FN_User FOREIGN KEY (userId) REFERENCES Users(userId),
	CONSTRAINT FK_FN_Notification FOREIGN KEY (notificationId) REFERENCES Notification(notificationId)
);
GO

CREATE TABLE FieldTechnology (
    fieldId VARCHAR(20),
    technologyId VARCHAR(20),
    CONSTRAINT PK_FieldTechnology PRIMARY KEY (fieldId, technologyId),
    CONSTRAINT FK_FT_Field FOREIGN KEY (fieldId) REFERENCES Field(fieldId),
    CONSTRAINT FK_FT_Technology FOREIGN KEY (technologyId) REFERENCES Technology(technologyId)
);
GO