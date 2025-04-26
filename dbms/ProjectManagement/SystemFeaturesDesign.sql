-- REFACTOR SQL CODE
-- DATA TYPE ConditionType 
CREATE TYPE ConditionType AS TABLE
(
    ColumnName NVARCHAR(128),    -- Tên trường
    ColumnValue NVARCHAR(MAX)    -- Giá trị điều kiện
);
GO


-- APPLICATION TRANSACTION
-- 1. PROC_DeleteTaskCascade
CREATE PROCEDURE PROC_DeleteTaskCascade
(
    @TaskId VARCHAR(20)
)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Xóa Comment liên quan đến Task
        DELETE FROM Comment
        WHERE taskId = @TaskId;

        -- Xóa Evaluation liên quan đến Task
        DELETE FROM Evaluation
        WHERE taskId = @TaskId;

        -- Xóa TaskStudent liên quan đến Task
        DELETE FROM TaskStudent
        WHERE taskId = @TaskId;

        -- Xóa FavoriteTask liên quan đến Task
        DELETE FROM FavoriteTask
        WHERE taskId = @TaskId;

        -- Xóa Task
        DELETE FROM Task
        WHERE taskId = @TaskId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 2. PROC_DeleteTeamCascade
CREATE PROCEDURE PROC_DeleteTeamCascade
(
    @TeamId VARCHAR(20)
)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Xóa JoinTeam liên quan đến Team
        DELETE FROM JoinTeam
        WHERE teamId = @TeamId;

        -- Xóa Team
        DELETE FROM Team
        WHERE teamId = @TeamId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 3. PROC_DeleteProjectCascade
CREATE PROCEDURE PROC_DeleteProjectCascade
(
    @ProjectId VARCHAR(20)
)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Xóa tất cả các Team liên quan đến Project bằng cách gọi thủ tục xóa Team lan truyền
        DECLARE @TeamId VARCHAR(20);
        DECLARE team_cursor CURSOR FOR
            SELECT teamId FROM Team WHERE projectId = @ProjectId;

        OPEN team_cursor;
        FETCH NEXT FROM team_cursor INTO @TeamId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            EXEC PROC_DeleteTeamCascade @TeamId;
            FETCH NEXT FROM team_cursor INTO @TeamId;
        END;

        CLOSE team_cursor;
        DEALLOCATE team_cursor;

        -- Xóa tất cả các Task liên quan đến Project bằng cách gọi thủ tục xóa Task lan truyền
        DECLARE @TaskId VARCHAR(20);
        DECLARE task_cursor CURSOR FOR
            SELECT taskId FROM Task WHERE projectId = @ProjectId;

        OPEN task_cursor;
        FETCH NEXT FROM task_cursor INTO @TaskId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            EXEC PROC_DeleteTaskCascade @TaskId;
            FETCH NEXT FROM task_cursor INTO @TaskId;
        END;

        CLOSE task_cursor;
        DEALLOCATE task_cursor;

        -- Xóa Meeting liên quan đến Project
        DELETE FROM Meeting
        WHERE projectId = @ProjectId;

        -- Xóa GiveUp liên quan đến Project
        DELETE FROM GiveUp
        WHERE projectId = @ProjectId;

        -- Xóa ProjectTechnology liên quan đến Project
        DELETE FROM ProjectTechnology
        WHERE projectId = @ProjectId;

        -- Xóa FavoriteProject liên quan đến Project
        DELETE FROM FavoriteProject
        WHERE projectId = @ProjectId;

        -- Xóa Project
        DELETE FROM Project
        WHERE projectId = @ProjectId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO


-- CREATE TRIGGERS
-- 1. TRIG_TeamRegisterdProject
CREATE TRIGGER TRIG_TeamRegisterdProject
ON Team
AFTER INSERT
AS
BEGIN
    UPDATE Project
    SET status = 'Registered'
    WHERE projectId IN (SELECT projectId FROM inserted);
END
GO

-- 2. TRIG_SetProjectWaiting
CREATE TRIGGER TRIG_SetProjectWaiting
ON GiveUp
AFTER INSERT
AS
BEGIN
    UPDATE Project
    SET status = 'Waiting'
    WHERE projectId IN (SELECT projectId FROM inserted);
END
GO

-- 3. TRIG_ResetProjectProcessing
CREATE TRIGGER TRIG_ResetProjectProcessing
ON GiveUp
AFTER DELETE
AS
BEGIN
    UPDATE Project
    SET status = 'Processing'
    WHERE projectId IN (SELECT projectId FROM deleted);
END
GO

-- CREATE VIEWS
-- 1. VIEW_CanRegisterdProject
CREATE VIEW VIEW_CanRegisterdProject AS
SELECT P.projectId, P.instructorId, P.topic, P.description, P.feature, P.requirement, P.maxMember, P.status, P.createdAt, P.createdBy, P.fieldId
FROM Project P
WHERE P.status IN ('Published', 'Registered')
AND NOT EXISTS (
    SELECT 1 
    FROM JoinTeam JT 
    INNER JOIN Team T ON JT.teamId = T.teamId
    WHERE T.projectId = P.projectId
);
GO

-- 2. VIEW_TaskTeam
CREATE VIEW VIEW_TaskTeam AS
SELECT T.taskId, T.title, T.description, T.projectId, TM.teamId, TM.teamName
FROM Task T
INNER JOIN Team TM ON T.projectId = TM.projectId;
GO

-- 3. VIEW_MeetingTeam
CREATE VIEW VIEW_MeetingTeam AS
SELECT M.meetingId, M.title, M.description, M.startAt, M.location, M.link, M.projectId, TM.teamId, TM.teamName
FROM Meeting M
INNER JOIN Team TM ON M.projectId = TM.projectId;
GO

-- 4. VIEW_TaskStudent
CREATE VIEW VIEW_TaskStudent AS
SELECT TS.taskId, TS.studentId, U.fullName, U.email, U.phoneNumber
FROM TaskStudent TS
INNER JOIN Users U ON TS.studentId = U.userId;
GO

-- 5. VIEW_StudentEvaluation
CREATE VIEW VIEW_StudentEvaluation AS
SELECT E.evaluationId, E.studentId, U.fullName, U.email, E.taskId, E.completionRate, E.score, E.evaluated, E.content
FROM Evaluation E
INNER JOIN Users U ON E.studentId = U.userId;
GO

-- 6. VIEW_FieldTechnology
CREATE VIEW VIEW_FieldTechnology AS
SELECT F.fieldId, F.name AS FieldName, T.technologyId, T.name AS TechnologyName
FROM FieldTechnology FT
INNER JOIN Field F ON FT.fieldId = F.fieldId
INNER JOIN Technology T ON FT.technologyId = T.technologyId;
GO


-- GENERAL FUNCTION
-- 1. FUNC_IsNotEmpty
CREATE FUNCTION FUNC_IsNotEmpty (@input NVARCHAR(MAX), @fieldName NVARCHAR(50))
RETURNS @Result TABLE
(
    IsValid BIT,
    Message NVARCHAR(100)
)
AS
BEGIN
    DECLARE @isValid BIT;
    DECLARE @message NVARCHAR(100);
    
    IF LTRIM(RTRIM(@input)) <> ''
    BEGIN
        SET @isValid = 1;  
        SET @message = @fieldName + N' is not empty.';  
    END
    ELSE
    BEGIN
        SET @isValid = 0;
        SET @message = @fieldName + N' is empty.';  
    END
    
    INSERT INTO @Result (IsValid, Message)
    VALUES (@isValid, @message);

    RETURN;
END;
GO

-- 2. FUNC_IsValidInRange
CREATE FUNCTION FUNC_IsValidInRange (
    @value REAL,
	@minValue REAL,
    @maxValue REAL,
    @fieldName NVARCHAR(50)
)
RETURNS @Result TABLE
(
    IsValid BIT,
    Message NVARCHAR(200)
)
AS
BEGIN
    DECLARE @isValid BIT;
    DECLARE @message NVARCHAR(200);


    IF @value >= @minValue AND @value <= @maxValue
    BEGIN
        SET @isValid = 1;
        SET @message = @fieldName + N' is valid (within the range).';
    END
    ELSE
    BEGIN
        SET @isValid = 0;
        SET @message = @fieldName + N' is not valid (must be between ' + CAST(@minValue AS NVARCHAR) + N' and ' + CAST(@maxValue AS NVARCHAR) + N').';
    END
    INSERT INTO @Result (IsValid, Message)
    VALUES (@isValid, @message);

    RETURN;
END;
GO

-- 3. FUNC_GetAllMonths
CREATE FUNCTION FUNC_GetAllMonths()
RETURNS @Months TABLE 
(
    MonthNumber INT, 
    MonthName NVARCHAR(20)
)
AS
BEGIN
    INSERT INTO @Months (MonthNumber, MonthName)
    VALUES 
        (1, 'January'),
        (2, 'February'),
        (3, 'March'),
        (4, 'April'),
        (5, 'May'),
        (6, 'June'),
        (7, 'July'),
        (8, 'August'),
        (9, 'September'),
        (10, 'October'),
        (11, 'November'),
        (12, 'December');

    RETURN;
END;
GO

-- 4. FUNC_CheckStartDate
CREATE FUNCTION FUNC_CheckStartDate (
    @startAt DATETIME,
    @fieldName NVARCHAR(50)
)
RETURNS @Result TABLE
(
    IsValid BIT,
    Message NVARCHAR(100)
)
AS
BEGIN
    DECLARE @isValid BIT;
    DECLARE @message NVARCHAR(100);

    IF @startAt > GETDATE()
    BEGIN
        SET @isValid = 1;
        SET @message = @fieldName + N' is valid (future date).';
    END
    ELSE
    BEGIN
        SET @isValid = 0;
        SET @message = @fieldName + N' is not valid (must be a future date).';
    END

    INSERT INTO @Result (IsValid, Message)
    VALUES (@isValid, @message);

    RETURN;
END;
GO

-- 5. FUNC_CheckEndDate
CREATE FUNCTION FUNC_CheckEndDate (
    @startAt DATETIME,
    @endAt DATETIME,
    @fieldName NVARCHAR(50)
)
RETURNS @Result TABLE
(
    IsValid BIT,
    Message NVARCHAR(100)
)
AS
BEGIN
    DECLARE @isValid BIT;
    DECLARE @message NVARCHAR(100);

    IF @endAt > @startAt
    BEGIN
        SET @isValid = 1;
        SET @message = @fieldName + N' is valid (end date is after start date).';
    END
    ELSE
    BEGIN
        SET @isValid = 0;
        SET @message = @fieldName + N' is not valid (end date must be after start date).';
    END

    INSERT INTO @Result (IsValid, Message)
    VALUES (@isValid, @message);

    RETURN;
END;
GO


-- GENERAL STORED PROCEDURE
-- 1. PROC_InsertDynamic
CREATE PROCEDURE PROC_InsertDynamic
(
    @TableName NVARCHAR(MAX),                   -- Tên bảng cần chèn dữ liệu
    @ColumnsValues ConditionType READONLY       -- Danh sách các cặp (Tên cột, Giá trị) để chèn
)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX) = '';
    DECLARE @Columns NVARCHAR(MAX) = '';
    DECLARE @Values NVARCHAR(MAX) = '';

    BEGIN TRY
        -- Tạo danh sách cột và giá trị từ @ColumnsValues
        SELECT
            @Columns = @Columns + QUOTENAME(ColumnName) + ', ',
            @Values = @Values + N'''' + REPLACE(ColumnValue, '''', '''''') + ''', '
        FROM @ColumnsValues;

        -- Xóa dấu phẩy cuối cùng trong danh sách
        SET @Columns = LEFT(@Columns, LEN(@Columns) - 1);
        SET @Values = LEFT(@Values, LEN(@Values) - 1);

        -- Tạo câu lệnh SQL động cho INSERT
        SET @SQL = N'INSERT INTO ' + QUOTENAME(@TableName) +
                   N' (' + @Columns + ') VALUES (' + @Values + ')';

        -- Debug: In ra câu lệnh SQL để kiểm tra
        PRINT @SQL;

        -- Thực thi câu lệnh SQL động
        EXEC sp_executesql @SQL;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 2. PROC_UpdateDynamic
CREATE PROCEDURE PROC_UpdateDynamic
(
    @TableName NVARCHAR(128),                  -- Tên bảng cần cập nhật
    @SetValues ConditionType READONLY,         -- Bảng các cặp (cột, giá trị) cho câu lệnh SET
    @Conditions ConditionType READONLY         -- Bảng các cặp (cột, giá trị) cho câu lệnh WHERE
)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX) = '';
    DECLARE @SetClause NVARCHAR(MAX) = '';
    DECLARE @ConditionString NVARCHAR(MAX) = '';

    BEGIN TRY
        -- Tạo chuỗi SET động từ @SetValues
        SELECT @SetClause = @SetClause + 
            QUOTENAME(ColumnName) + ' = ''' + ColumnValue + ''', '
        FROM @SetValues;
        SET @SetClause = LEFT(@SetClause, LEN(@SetClause) - 1);

        -- Tạo chuỗi WHERE động từ @Conditions
        SELECT @ConditionString = @ConditionString +
            QUOTENAME(ColumnName) + ' = ''' + ColumnValue + ''' AND '
        FROM @Conditions;
        SET @ConditionString = LEFT(@ConditionString, LEN(@ConditionString) - 4);

        -- Xây dựng câu lệnh SQL động cho lệnh UPDATE
        SET @SQL = N'UPDATE ' + QUOTENAME(@TableName) +
                   N' SET ' + @SetClause +
                   N' WHERE ' + @ConditionString;

        -- Thực thi lệnh SQL động
        EXEC sp_executesql @SQL;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 3. PROC_DeleteDynamic
CREATE PROCEDURE PROC_DeleteDynamic
(
    @TableName NVARCHAR(128),                -- Tên bảng
    @Conditions ConditionType READONLY       -- Bảng điều kiện
)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX) = '';
    DECLARE @ConditionString NVARCHAR(MAX) = '';

    BEGIN TRY
        -- Duyệt qua bảng điều kiện để tạo chuỗi điều kiện động
        SELECT @ConditionString = @ConditionString +
            QUOTENAME(ColumnName) + ' = ''' + ColumnValue + ''' AND '
        FROM @Conditions;
        SET @ConditionString = LEFT(@ConditionString, LEN(@ConditionString) - 4);

        -- Xây dựng câu lệnh SQL động để xóa
        SET @SQL = N'DELETE FROM ' + QUOTENAME(@TableName) +
                   N' WHERE ' + @ConditionString;

        -- Thực thi lệnh SQL động
        EXEC sp_executesql @SQL;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 4. PROC_GetDynamic
CREATE PROCEDURE PROC_GetDynamic
(
    @TableName NVARCHAR(128),                -- Tên bảng
    @Conditions ConditionType READONLY       -- Bảng điều kiện
)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX) = '';
    DECLARE @ConditionString NVARCHAR(MAX) = '';

    BEGIN TRY
        -- Duyệt qua bảng điều kiện để tạo chuỗi điều kiện động
        SELECT @ConditionString = @ConditionString +
            QUOTENAME(ColumnName) + ' = ''' + ColumnValue + ''' AND '
        FROM @Conditions;
        SET @ConditionString = LEFT(@ConditionString, LEN(@ConditionString) - 4);

        -- Xây dựng câu lệnh SQL động để lấy dữ liệu
        SET @SQL = N'SELECT * FROM ' + QUOTENAME(@TableName) +
                   N' WHERE ' + @ConditionString;

        -- Thực thi lệnh SQL động để lấy dữ liệu
        EXEC sp_executesql @SQL;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO


-- SYSTEM FEATURE DESIGN

-- Feature 1. REGISTER AND LOG-IN / LOG-OUT
-- A. VIEW
-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE


-- Feature 2. MANAGE PROJECT
-- A. VIEW
-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE
-- 1. FUNC_GetProjectsForStudent
CREATE FUNCTION FUNC_GetProjectsForStudent
(
    @UserId VARCHAR(20),
    @Published VARCHAR(20),
    @Registered VARCHAR(20)
)
RETURNS TABLE
AS
RETURN
(
    SELECT P.*
    FROM Project P
    WHERE P.status IN (@Published, @Registered)
      AND NOT EXISTS (
            SELECT 1 
            FROM Team T 
            WHERE T.projectId = P.projectId
              AND T.teamId IN (
                  SELECT teamId 
                  FROM JoinTeam JT 
                  WHERE JT.studentId = @UserId
              )
        )
);
GO

-- 2. FUNC_GetMyProjects
CREATE FUNCTION FUNC_GetMyProjects
(
    @UserId VARCHAR(20)
)
RETURNS TABLE
AS
RETURN
(
    SELECT P.*
    FROM Project P
    INNER JOIN Team T ON P.projectId = T.projectId
    WHERE T.teamId IN (
        SELECT teamId 
        FROM JoinTeam JT 
        WHERE JT.studentId = @UserId
    )
);
GO

-- 3. PROC_GetMyCompletedProjects
CREATE PROCEDURE PROC_GetMyCompletedProjects
(
    @UserId VARCHAR(20),
    @Completed VARCHAR(20)
)
AS
BEGIN
    BEGIN TRY
        SELECT P.*
        FROM Project P
        INNER JOIN Team T ON P.projectId = T.projectId
        WHERE T.teamId IN (
            SELECT JT.teamId 
            FROM JoinTeam JT 
            WHERE JT.studentId = @UserId
        )
        AND P.status = @Completed;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 4. FUNC_SearchRoleLecture
CREATE FUNCTION FUNC_SearchRoleLecture
(
    @UserId VARCHAR(20),
    @TopicSyntax NVARCHAR(255)
)
RETURNS TABLE
AS
RETURN
(
    SELECT *
    FROM Project
    WHERE instructorId = @UserId
    AND topic LIKE @TopicSyntax
);
GO

-- 5. FUNC_SearchRoleStudent
CREATE FUNCTION FUNC_SearchRoleStudent
(
    @TopicSyntax NVARCHAR(255),
    @Published VARCHAR(20),
    @Registered VARCHAR(20)
)
RETURNS TABLE
AS
RETURN
(
    SELECT *
    FROM Project
    WHERE status IN (@Published, @Registered)
    AND topic LIKE @TopicSyntax
);
GO

-- 6. PROC_UpdateFavoriteProject
CREATE PROCEDURE PROC_UpdateFavoriteProject
(
    @UserId VARCHAR(20),
    @ProjectId VARCHAR(20),
    @IsFavorite BIT
)
AS
BEGIN
    BEGIN TRY
        IF @IsFavorite = 1
        BEGIN
            INSERT INTO FavoriteProject (userId, projectId)
            VALUES (@UserId, @ProjectId);
        END
        ELSE
        BEGIN
            DELETE FROM FavoriteProject
            WHERE userId = @UserId AND projectId = @ProjectId;
        END
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- Feature 3. MANAGE TEAM
-- A. VIEW
-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE
-- 1. FUNC_CountTeamsFollowState
CREATE FUNCTION FUNC_CountTeamsFollowState
(
    @ProjectId VARCHAR(20),
    @Status VARCHAR(20)
)
RETURNS TABLE
AS
RETURN
(
    SELECT COUNT(*) AS NumTeams
    FROM Team
    WHERE projectId = @ProjectId AND status = @Status
);
GO


-- Feature 4. APPROVE PROJECT REGISTRATION
-- A. VIEW
-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE


-- Feature 5. MANAGE TASK
-- A. VIEW
-- 1. VIEW_TaskByStudent
CREATE VIEW VIEW_TaskByStudent AS
SELECT 
    T.taskId,
    T.title,
    T.description,
    T.startAt,
    T.endAt,
    T.progress,
    T.priority,
    T.createdAt,
    T.createdBy,
    T.projectId,
    TS.studentId
FROM 
    Task T
INNER JOIN TaskStudent TS ON T.taskId = TS.taskId;
GO

-- 2. VIEW_TasksByProject
CREATE VIEW VIEW_TasksByProject AS
SELECT 
    taskId,
    title,
    description,
    startAt,
    endAt,
    progress,
    priority,
    createdAt,
    createdBy,
    projectId
FROM 
    Task;
GO

-- 3. VIEW_TasksByTeam
CREATE VIEW VIEW_TasksByTeam AS
SELECT 
    T.taskId,
    T.title,
    T.description,
    T.projectId,
    T.startAt,
    T.endAt,
    T.progress,
    T.priority,
    T.createdAt,
    T.createdBy,
    TE.teamId,
    TE.status
FROM 
    Task T
INNER JOIN Team TE ON T.projectId = TE.projectId;
GO

-- 4. VIEW VIEW_FavoriteTasks
CREATE VIEW VIEW_FavoriteTasks AS
SELECT 
    FT.taskId,
    FT.userId
FROM 
    FavoriteTask FT
INNER JOIN Task T ON FT.taskId = T.taskId;
GO

-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE
-- 1. PROC_InsertAssignStudent
CREATE PROCEDURE PROC_InsertAssignStudent
(
    @TaskId VARCHAR(20),
    @StudentId VARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO TaskStudent (taskId, studentId)
        VALUES (@TaskId, @StudentId);
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

GO

-- 2. PROC_SearchTaskByTitle
CREATE PROCEDURE PROC_SearchTaskByTitle
(
    @ProjectId VARCHAR(20),
    @TitleSyntax NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Thực hiện lệnh SELECT
        SELECT *
        FROM Task
        WHERE projectId = @ProjectId 
        AND title LIKE @TitleSyntax
        ORDER BY createdAt DESC;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO


-- Feature 6. COMMENT IN TASK
-- A. VIEW
-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE


-- Feature 7. EVALUATION IN TASK
-- A. VIEW
-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE
-- 1. FUNC_GetEvaluationByStudentIdAndYear
CREATE FUNCTION FUNC_GetEvaluationByStudentIdAndYear
(
    @StudentId NVARCHAR(20),
	@YearSelected INT
)
RETURNS TABLE 
AS
RETURN
(
    SELECT *
    FROM Evaluation
    WHERE studentId = @StudentId AND YEAR(createdAt) = @YearSelected AND evaluated = 1
);
GO

-- Feature 8. VIEW NOTIFICATION
-- A. VIEW
-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE
-- 1. PROC_GetNotificationsByUserId
CREATE PROCEDURE PROC_GetNotificationsByUserId
(
    @UserId NVARCHAR(20)
)
AS
BEGIN
    BEGIN TRY
        SELECT TOP 100 PERCENT N.*, VN.seen
        FROM Notification N
        JOIN ViewNotification VN ON N.notificationId = VN.notificationId
        WHERE VN.userId = @UserId
        ORDER BY N.createdAt DESC;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 1. PROC_UpdateFavoriteNotification
CREATE PROCEDURE PROC_UpdateFavoriteNotification 
    @IsFavorite BIT,
    @UserId VARCHAR(20),
    @NotificationId VARCHAR(20)
AS
BEGIN
    BEGIN TRY
        IF @IsFavorite = 1
        BEGIN
            INSERT INTO FavoriteNotification (userId, notificationId)
            VALUES (@UserId, @NotificationId);
        END
        ELSE
        BEGIN
            DELETE FROM FavoriteNotification 
            WHERE userId = @UserId AND notificationId = @NotificationId;
        END
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO


-- Feature 9. MANAGE MEETING
-- A. VIEW
-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE


-- Feature 10. GIVE UP THE PROJECT
-- A. VIEW
-- 1. VIEW_GiveUpDetails
CREATE VIEW VIEW_GiveUpDetails AS
SELECT 
    G.projectId,
    G.userId,
    G.reason,
    G.createdAt,
    G.status,
    U.fullName AS UserName
FROM 
    GiveUp G
INNER JOIN Users U ON G.userId = U.userId;
GO

-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE
-- Feature 11. VIEW STATISTICS
-- A. VIEW
-- B. TRIGGER
-- C. DATA TYPE
-- 1. ProjectTableType
CREATE TYPE ProjectTableType AS TABLE (
    projectId VARCHAR(20),
    instructorId VARCHAR(20),
    topic NVARCHAR(255),
    description NTEXT,
    feature NTEXT,
    requirement NTEXT,
    maxMember INT,
    status VARCHAR(20),
    createdAt DATETIME,
	createdBy VARCHAR(20),
    fieldId VARCHAR(20)
);
GO

-- 2. EvaluationTableType
CREATE TYPE EvaluationTableType AS TABLE (
    evaluationId VARCHAR(20),
    content NTEXT,
    completionRate REAL,
    score REAL,
    evaluated BIT,
    createdAt DATETIME,
    createdBy VARCHAR(20),
    studentId VARCHAR(20),
    taskId VARCHAR(20)
);
GO

-- 3. TaskTableType
CREATE TYPE TaskTableType AS TABLE (
    taskId VARCHAR(20),
    startAt DATETIME NOT NULL,
    endAt DATETIME NOT NULL,
    title NTEXT NOT NULL,
    description NTEXT NOT NULL,
    progress REAL NOT NULL,
    priority VARCHAR(20) NOT NULL,
    createdAt DATETIME NOT NULL,
    createdBy VARCHAR(20) NOT NULL,
    projectId VARCHAR(20) NOT NULL
);
GO

CREATE PROCEDURE InsertProjectData
    @ProjectData ProjectTableType READONLY
AS
BEGIN
    -- Giả sử có bảng thực tế là ProjectTable để lưu dữ liệu
    INSERT INTO ProjectTable (projectId, instructorId, topic, description, feature, requirement, maxMember, status, createdAt, createdBy, fieldId)
    SELECT projectId, instructorId, topic, description, feature, requirement, maxMember, status, createdAt, createdBy, fieldId
    FROM @ProjectData;
END;
GO


CREATE PROCEDURE InsertEvaluationData
    @EvaluationData EvaluationTableType READONLY
AS
BEGIN
    -- Giả sử có bảng thực tế là EvaluationTable để lưu dữ liệu
    INSERT INTO EvaluationTable (evaluationId, content, completionRate, score, evaluated, createdAt, createdBy, studentId, taskId)
    SELECT evaluationId, content, completionRate, score, evaluated, createdAt, createdBy, studentId, taskId
    FROM @EvaluationData;
END;
GO

CREATE PROCEDURE InsertTaskData
    @TaskData TaskTableType READONLY
AS
BEGIN
    -- Giả sử có bảng thực tế là TaskTable để lưu dữ liệu
    INSERT INTO TaskTable (taskId, startAt, endAt, title, description, progress, priority, createdAt, createdBy, projectId)
    SELECT taskId, startAt, endAt, title, description, progress, priority, createdAt, createdBy, projectId
    FROM @TaskData;
END;
GO

-- D. FUNCTION AND S-PROCEDURE
-- 1. FUNC_GetProjectsGroupedByMonth 
CREATE FUNCTION FUNC_GetProjectsGroupedByMonth
(
    @ProjectList ProjectTableType READONLY
)
RETURNS @Result TABLE 
(
    MonthNumber INT,
    MonthName NVARCHAR(20),
    ProjectCount INT
)
AS
BEGIN
    -- Lấy danh sách các tháng từ hàm GetAllMonths
    DECLARE @Months TABLE (MonthNumber INT, MonthName NVARCHAR(20));
    INSERT INTO @Months
    SELECT * FROM FUNC_GetAllMonths();

    -- Chèn dữ liệu vào bảng kết quả
    INSERT INTO @Result (MonthNumber, MonthName, ProjectCount)
    SELECT 
        m.MonthNumber,
        m.MonthName,
        ISNULL(COUNT(p.projectId), 0) AS ProjectCount
    FROM 
        @Months m
    LEFT JOIN 
        @ProjectList p 
        ON MONTH(p.createdAt) = m.MonthNumber
    GROUP BY 
        m.MonthNumber, m.MonthName;

    RETURN;
END;
GO

-- 2. FUNC_GetProjectsGroupedByStatus
CREATE FUNCTION FUNC_GetProjectsGroupedByStatus
(
    @ProjectList ProjectTableType READONLY
)
RETURNS @Result TABLE 
(
    ProjectStatus NVARCHAR(20),
    ProjectCount INT
)
AS
BEGIN
    -- Chèn dữ liệu vào bảng kết quả
    INSERT INTO @Result (ProjectStatus, ProjectCount)
    SELECT 
        p.status,
        COUNT(p.projectId) AS ProjectCount
    FROM 
        @ProjectList p
    GROUP BY 
        p.status;

    RETURN;
END;
GO

-- 3. FUNC_GetEvaluationsGroupedByMonth
CREATE FUNCTION FUNC_GetEvaluationsGroupedByMonth
(
    @EvaluationList EvaluationTableType READONLY
)
RETURNS @Result TABLE 
(
    MonthNumber INT,
    MonthName NVARCHAR(20),
    EvaluationCount INT
)
AS
BEGIN
    -- Lấy danh sách các tháng từ hàm GetAllMonths
    DECLARE @Months TABLE (MonthNumber INT, MonthName NVARCHAR(20));
    INSERT INTO @Months
    SELECT * FROM FUNC_GetAllMonths();

    -- Chèn dữ liệu vào bảng kết quả
    INSERT INTO @Result (MonthNumber, MonthName, EvaluationCount)
    SELECT 
        m.MonthNumber,
        m.MonthName,
        COUNT(e.evaluationId) AS EvaluationCount
    FROM 
        @Months m
    LEFT JOIN 
        @EvaluationList e 
        ON MONTH(e.createdAt) = m.MonthNumber
    WHERE 
        e.evaluationId IS NULL OR e.evaluated = 1  -- Đảm bảo kiểm tra đánh giá hoặc cho phép tháng trống
    GROUP BY 
        m.MonthNumber, m.MonthName;

    RETURN;
END;
GO

-- 4. FUNC_GetProjectByLectureAndYear
CREATE FUNCTION FUNC_GetProjectByLectureAndYear
(
    @UserId VARCHAR(20),
    @YearSelected INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT projectId, instructorId, topic, description, feature, requirement, maxMember, status, createdAt, createdBy, fieldId
    FROM Project
    WHERE instructorId = @UserId
    AND YEAR(createdAt) = @YearSelected
);
GO

-- 5. FUNC_CalUtil
CREATE FUNCTION FUNC_CalUtil
(
    @PeopleId VARCHAR(20),
    @TaskList TaskTableType READONLY,
    @CalculationField VARCHAR(20) -- "completionRate" or "score"
)
RETURNS @ResultTable TABLE 
(
    AverageValue FLOAT
)
AS
BEGIN
    DECLARE @Result FLOAT = 0;
    DECLARE @Amount INT = 0;
    DECLARE @AverageValue FLOAT;

    -- Common query logic with dynamic field selection using CASE
    SELECT 
        @Result = SUM(CASE 
                        WHEN @CalculationField = 'completionRate' THEN e.completionRate 
                        WHEN @CalculationField = 'score' THEN e.score 
                        ELSE 0 
                      END),
        @Amount = COUNT(CASE 
                          WHEN @CalculationField = 'completionRate' THEN e.completionRate
                          WHEN @CalculationField = 'score' THEN e.score 
                          ELSE NULL 
                        END)
    FROM 
        @TaskList AS t
    INNER JOIN 
        Evaluation e ON t.taskId = e.TaskId
    WHERE 
        e.studentId = @PeopleId;

    -- Calculate the average based on the chosen field
    IF @Amount > 0 
        SET @AverageValue = @Result / @Amount;
    ELSE 
        SET @AverageValue = 0.0;

    -- Insert the result into the table
    INSERT INTO @ResultTable (AverageValue)
    VALUES (@AverageValue);

    RETURN;
END;
GO

-- 6. FUNC_CalAvgProgress
CREATE FUNCTION FUNC_CalAvgProgress
(
    @TaskList TaskTableType READONLY
)
RETURNS @ResultTable TABLE 
(
    AverageProgress INT
)
AS
BEGIN
    DECLARE @TotalProgress FLOAT = 0;
    DECLARE @TaskCount INT = 0;
    DECLARE @AverageProgress INT;

    -- Calculate the total progress and count the tasks
    SELECT 
        @TotalProgress = SUM(t.progress),
        @TaskCount = COUNT(*)
    FROM 
        @TaskList AS t;

    -- Calculate the average progress if there are tasks
    IF @TaskCount > 0 
        SET @AverageProgress = FLOOR(@TotalProgress / @TaskCount); -- Round down
    ELSE 
        SET @AverageProgress = 0;

    -- Insert the result into the table
    INSERT INTO @ResultTable (AverageProgress)
    VALUES (@AverageProgress);

    RETURN;
END;
GO


-- Feature 12. MANAGE PERSONNEL INFORMATION
-- A. VIEW
-- 1. VIEW_UserDetails
CREATE VIEW VIEW_UserDetails AS
SELECT *
FROM Users;
GO

-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE
-- 1. PROC_SelectUsersByUserNameAndRole
CREATE PROCEDURE PROC_SelectUsersByUserNameAndRole
(
    @UserNameSyntax NVARCHAR(50),
    @Role NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT *
        FROM Users
        WHERE userName LIKE @UserNameSyntax 
        AND role = @Role;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 2. PROC_CheckNonExist
CREATE PROCEDURE PROC_CheckNonExist
(
    @TableName NVARCHAR(50),
    @Field NVARCHAR(50),
    @Information NVARCHAR(255)
)
AS
BEGIN
    DECLARE @Sql NVARCHAR(MAX);
    DECLARE @count INT;
    DECLARE @isValid BIT;
    DECLARE @message NVARCHAR(200);

    BEGIN TRY
        -- Tạo SQL động
        SET @Sql = N'SELECT @count = COUNT(1) FROM ' + QUOTENAME(@TableName) + 
                   N' WHERE ' + QUOTENAME(@Field) + N' = @Information';

        -- Thực thi SQL động và lấy kết quả
        EXEC sp_executesql @Sql, 
            N'@Information NVARCHAR(255), @count INT OUTPUT', 
            @Information, @count OUTPUT;

        -- Kiểm tra kết quả và tạo thông báo
        IF @count = 0
        BEGIN
            SET @isValid = 1;
            SET @message = @Field + N' with value "' + @Information + N'" does not exist.';
        END
        ELSE
        BEGIN
            SET @isValid = 0;
            SET @message = @Field + N' with value "' + @Information + N'" already exists.';
        END

        SELECT @isValid AS IsValid, @message AS Message;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- 3. FUNC_CheckAge
CREATE FUNCTION FUNC_CheckAge
(
    @DateOfBirth DATE,
    @FieldName NVARCHAR(50)
)
RETURNS @Result TABLE
(
    IsValid BIT,
    Message NVARCHAR(200)
)
AS
BEGIN
    DECLARE @today DATE = GETDATE();
    DECLARE @age INT;
    DECLARE @isValid BIT;
    DECLARE @message NVARCHAR(200);

    SET @age = DATEDIFF(YEAR, @DateOfBirth, @today);

    IF @DateOfBirth > DATEADD(YEAR, -@age, @today)
    BEGIN
        SET @age = @age - 1;
    END

    IF @age >= 18
    BEGIN
        SET @isValid = 1;
        SET @message = @FieldName + N' is valid. The person is 18 years or older.';
    END
    ELSE
    BEGIN
        SET @isValid = 0;
        SET @message = @FieldName + N' is not valid. The person must be at least 18 years old.';
    END

    INSERT INTO @Result (IsValid, Message)
    VALUES (@isValid, @message);

    RETURN;
END;
GO


-- OTHER FEATURES
-- A. VIEW
-- B. TRIGGER
-- C. FUNCTION AND S-PROCEDURE

-- FUNCTION in Field DAO
CREATE FUNCTION FUNC_GetTopField()
RETURNS TABLE
AS
RETURN
(
    SELECT TOP 5 
        f.name AS FieldName, 
        COUNT(p.fieldId) AS ProjectCount
    FROM Project p
    JOIN Field f ON p.fieldId = f.fieldId
    GROUP BY f.name
);
GO

-- FUNCTION in Technology DAO
CREATE FUNCTION FUNC_GetTopTechnology()
RETURNS TABLE
AS
RETURN
(
    SELECT TOP 5 
        t.name AS TechnologyName, 
        COUNT(ft.technologyId) AS ProjectCount
    FROM FieldTechnology ft
    JOIN Technology t ON ft.technologyId = t.technologyId
    GROUP BY t.name
);
GO

