--DROP ROLE Student;
--DROP TRIGGER TRIG_AutoCreateUserLogin;
--DROP TRIGGER TRIG_AutoDeleteUserLogin;
--GO

--REVOKE INSERT ON dbo.Project FROM Student;

-- Tạo database role cho Student
USE ProjectManagement;
CREATE ROLE Student;
GO
-- Gán quyền trên TABLE cho role Student
GRANT SELECT, REFERENCES, INSERT, UPDATE ON dbo.Users TO Student;
GRANT SELECT, REFERENCES ON dbo.Field TO Student;
GRANT SELECT, REFERENCES ON dbo.Technology TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE, UPDATE ON dbo.Project TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE, UPDATE ON dbo.Team TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE, UPDATE ON dbo.Task TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE, UPDATE ON dbo.Meeting TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE ON dbo.Comment TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE, UPDATE ON dbo.Evaluation TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE ON dbo.Notification TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE ON dbo.JoinTeam TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE ON dbo.ViewNotification TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE ON dbo.ProjectTechnology TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE ON dbo.TaskStudent TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE, UPDATE ON dbo.GiveUp TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE ON dbo.FavoriteProject TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE ON dbo.FavoriteTask TO Student;
GRANT SELECT, REFERENCES, INSERT, DELETE ON dbo.FavoriteNotification TO Student;
GRANT SELECT, REFERENCES ON dbo.FieldTechnology TO Student;
GRANT REFERENCES ON TYPE::dbo.ProjectTableType TO Student;
GRANT REFERENCES ON TYPE::dbo.EvaluationTableType TO Student;
GRANT REFERENCES ON TYPE::dbo.TaskTableType TO Student;
-- Gán quyền trên VIEW cho role Student
GRANT SELECT, REFERENCES ON VIEW_CanRegisterdProject TO Student;
GRANT SELECT, REFERENCES ON VIEW_TaskTeam TO Student;
GRANT SELECT, REFERENCES ON VIEW_MeetingTeam TO Student;
GRANT SELECT, REFERENCES ON VIEW_TaskStudent TO Student;
GRANT SELECT, REFERENCES ON VIEW_StudentEvaluation TO Student;
GRANT SELECT, REFERENCES ON VIEW_FieldTechnology TO Student;
GRANT SELECT, REFERENCES ON VIEW_TaskByStudent TO Student;
GRANT SELECT, REFERENCES ON VIEW_TasksByProject TO Student;
GRANT SELECT, REFERENCES ON VIEW_TasksByTeam TO Student;
GRANT SELECT, REFERENCES ON VIEW_FavoriteTasks TO Student;
GRANT SELECT, REFERENCES ON VIEW_GiveUpDetails TO Student;
GRANT SELECT, REFERENCES ON VIEW_UserDetails TO Student;
-- Gán quyền thực thi FUNCTION và PROCEDURE cho role Student
GRANT EXECUTE TO Student; 
GRANT SELECT TO Student;
GO

-- 1. TRIG_AutoCreateUserLogin
CREATE TRIGGER TRIG_AutoCreateUserLogin
ON dbo.Users
AFTER INSERT
AS
BEGIN
    DECLARE @userEmail NVARCHAR(100), @passWord NVARCHAR(50), @role NVARCHAR(50);
    
    -- Lấy thông tin từ bản ghi vừa được chèn
    SELECT @userEmail = email, @passWord = password, @role = role FROM inserted;
    
    -- Tạo LOGIN với email làm tên đăng nhập và mật khẩu từ dữ liệu vừa chèn
    DECLARE @sqlString NVARCHAR(2000);
    SET @sqlString = 'CREATE LOGIN [' + @userEmail + '] WITH PASSWORD = ''' + @passWord + ''', CHECK_EXPIRATION = OFF, CHECK_POLICY = OFF';
    EXEC sp_executesql @sqlString;
    
    -- Tạo USER cho LOGIN vừa tạo
    SET @sqlString = 'CREATE USER [' + @userEmail + '] FOR LOGIN [' + @userEmail + ']';
    EXEC sp_executesql @sqlString;

    -- Gán quyền cho User dựa trên vai trò
    IF @role = 'Lecture'
        SET @sqlString = 'ALTER SERVER ROLE sysadmin ADD MEMBER [' + @userEmail + ']';
    ELSE
        SET @sqlString = 'ALTER ROLE Student ADD MEMBER [' + @userEmail + ']';
    EXEC sp_executesql @sqlString;
END;
GO

-- 2. TRIG_AutoDeleteUserLogin
CREATE TRIGGER TRIG_AutoDeleteUserLogin
ON dbo.Users
AFTER DELETE
AS
BEGIN
    DECLARE @userEmail NVARCHAR(100);
    
    -- Lấy email từ bản ghi vừa xóa
    SELECT @userEmail = email FROM deleted;
    
    -- Kiểm tra và ngắt kết nối người dùng nếu đang đăng nhập
    DECLARE @SessionID INT;
    SELECT @SessionID = session_id FROM sys.dm_exec_sessions WHERE login_name = @userEmail;
    IF @SessionID IS NOT NULL
    BEGIN
        DECLARE @killSql NVARCHAR(100);
        SET @killSql = 'KILL ' + CAST(@SessionID AS NVARCHAR(20));
        EXEC sp_executesql @killSql;
    END

    -- Xóa USER và LOGIN
    DECLARE @sqlString NVARCHAR(2000);
    SET @sqlString = 'DROP USER [' + @userEmail + ']';
    EXEC sp_executesql @sqlString;

    SET @sqlString = 'DROP LOGIN [' + @userEmail + ']';
    EXEC sp_executesql @sqlString;
END;
GO

---- view list server role
--SELECT * FROM sys.server_principals WHERE type = 'R';

---- view list database role
--USE ProjectManagement;
--SELECT * FROM sys.database_principals WHERE type = 'R';

---- view list LOGIN of server role
--SELECT role.name AS RoleName, member.name AS MemberName
--FROM sys.server_role_members AS srm
--JOIN sys.server_principals AS role ON srm.role_principal_id = role.principal_id
--JOIN sys.server_principals AS member ON srm.member_principal_id = member.principal_id;

---- view list USER of database role
--USE ProjectManagement;
--SELECT role.name AS RoleName, member.name AS MemberName
--FROM sys.database_role_members AS drm
--JOIN sys.database_principals AS role ON drm.role_principal_id = role.principal_id
--JOIN sys.database_principals AS member ON drm.member_principal_id = member.principal_id;

---- view list permission of server role
--SELECT 
--    sp.name AS PrincipalName,
--    sp.type_desc AS PrincipalType,
--    p.permission_name AS PermissionName,
--    p.state_desc AS PermissionState
--FROM 
--    sys.server_permissions AS p
--JOIN 
--    sys.server_principals AS sp ON p.grantee_principal_id = sp.principal_id
--WHERE 
--    sp.type = 'R'
--ORDER BY 
--    sp.name;

---- view list permission of database role
--USE ProjectManagement;
--SELECT 
--    dp.name AS RoleName,
--    dp.type_desc AS PrincipalType,
--    o.name AS ObjectName,
--    p.permission_name AS PermissionName,
--    p.state_desc AS PermissionState
--FROM 
--    sys.database_permissions AS p
--JOIN 
--    sys.objects AS o ON p.major_id = o.object_id
--JOIN 
--    sys.database_principals AS dp ON p.grantee_principal_id = dp.principal_id
--WHERE 
--    dp.type = 'R'
--ORDER BY 
--    dp.name, o.name;

---- FREEDOM
--DECLARE @RoleName NVARCHAR(128);
--DECLARE @UserName NVARCHAR(128);
--DECLARE @LoginName NVARCHAR(128);
--DECLARE @sql NVARCHAR(MAX);

------ 1. Xóa USER trong database role Student và LOGIN tương ứng
--SET @RoleName = 'Student';

--DECLARE user_cursor CURSOR FOR 
--SELECT dp.name
--FROM sys.database_principals dp
--JOIN sys.database_role_members drm ON dp.principal_id = drm.member_principal_id
--JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
--WHERE r.name = @RoleName;

--OPEN user_cursor
--FETCH NEXT FROM user_cursor INTO @UserName
--WHILE @@FETCH_STATUS = 0
--BEGIN
--    -- Xóa tất cả các quyền của User
--    SET @sql = 'REVOKE ALL PRIVILEGES ON DATABASE::[' + DB_NAME() + '] FROM ' + QUOTENAME(@UserName) + ';';
--    EXEC sp_executesql @sql;
    
--    -- Xóa USER khỏi database
--    SET @sql = 'DROP USER ' + QUOTENAME(@UserName) + ';';
--    EXEC sp_executesql @sql;
    
--    -- Xóa LOGIN tương ứng
--    SET @sql = 'DROP LOGIN ' + QUOTENAME(@UserName) + ';';
--    EXEC sp_executesql @sql;
    
--    FETCH NEXT FROM user_cursor INTO @UserName;
--END
--CLOSE user_cursor;
--DEALLOCATE user_cursor;

---- Xóa database role Student
--SET @sql = 'DROP ROLE ' + QUOTENAME(@RoleName) + ';';
--EXEC sp_executesql @sql;

---- 2. Xóa LOGIN trong server role Lecture (sysadmin) và các USER tương ứng trong từng database
--SET @RoleName = 'sysadmin';

--DECLARE login_cursor CURSOR FOR 
--SELECT sp.name
--FROM sys.server_principals sp
--JOIN sys.server_role_members srm ON sp.principal_id = srm.member_principal_id
--JOIN sys.server_principals sr ON srm.role_principal_id = sr.principal_id
--WHERE sr.name = @RoleName;

--OPEN login_cursor
--FETCH NEXT FROM login_cursor INTO @LoginName
--WHILE @@FETCH_STATUS = 0
--BEGIN
--    -- Xóa USER trong tất cả các database cho LOGIN tương ứng
--    DECLARE @dbName NVARCHAR(128);
--    DECLARE db_cursor CURSOR FOR 
--    SELECT name 
--    FROM sys.databases 
--    WHERE state_desc = 'ONLINE';

--    OPEN db_cursor
--    FETCH NEXT FROM db_cursor INTO @dbName
--    WHILE @@FETCH_STATUS = 0
--    BEGIN
--        SET @sql = 'USE ' + QUOTENAME(@dbName) + '; IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = ' + QUOTENAME(@LoginName) + ') BEGIN DROP USER ' + QUOTENAME(@LoginName) + '; END';
--        EXEC sp_executesql @sql;
        
--        FETCH NEXT FROM db_cursor INTO @dbName;
--    END
--    CLOSE db_cursor;
--    DEALLOCATE db_cursor;

--    -- Xóa LOGIN khỏi server
--    SET @sql = 'DROP LOGIN ' + QUOTENAME(@LoginName) + ';';
--    EXEC sp_executesql @sql;

--    FETCH NEXT FROM login_cursor INTO @LoginName;
--END
--CLOSE login_cursor;
--DEALLOCATE login_cursor;

---- Không xóa server role sysadmin
--GO


