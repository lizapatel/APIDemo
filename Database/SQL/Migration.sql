CREATE TABLE tbl_roles (
    c_roleid INT IDENTITY(1,1) PRIMARY KEY,
    c_rolename NVARCHAR(50) NOT NULL UNIQUE
);
GO
------------------------------------------------------------------------------------------------------------------------
INSERT INTO tbl_roles (c_rolename) VALUES 
('System Administrator'), 
('System User');
GO
------------------------------------------------------------------------------------------------------------------------
CREATE TABLE tbl_users (
    c_userid INT IDENTITY(1,1) PRIMARY KEY,
	c_email NVARCHAR(100) NOT NULL UNIQUE,
    c_passwordhash NVARCHAR(256) NOT NULL,
    c_firstname NVARCHAR(50) NOT NULL,
    c_lastname NVARCHAR(50) NOT NULL,
    c_contactnumber NVARCHAR(15),
    c_roleid INT NOT NULL,    
	c_createdby BIGINT,	
    c_createdat DATETIME DEFAULT GETDATE(),
	c_updatedby BIGINT,
    c_updatedat DATETIME,
    c_isactive BIT DEFAULT 1,
	FOREIGN KEY (c_roleid) REFERENCES tbl_roles(c_roleid),
	FOREIGN KEY (c_createdby) REFERENCES tbl_users(c_userid),
	FOREIGN KEY (c_updatedby) REFERENCES tbl_users(c_userid)
);
GO
------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[sp_user_AddEdit]
    @XMLInput XML,             -- Accepting the XML data
    @IsUpdate BIT,             -- Indicates if it's an update or insert
    @ResultId INT OUTPUT      -- Output parameter for the UserId
AS
BEGIN
    SET NOCOUNT ON;

    -- Declare variables to store extracted XML data
    DECLARE 
        @UserId BIGINT,
        @Email NVARCHAR(100),
        @PasswordHash NVARCHAR(256),
        @FirstName NVARCHAR(50),
        @LastName NVARCHAR(50),
        @ContactNumber NVARCHAR(15),
        @RoleId INT,
		@CreatedBy BIGINT,
		@UpdatedBy BIGINT,
        @IsActive BIT;

    -- Default ResultId to NULL
    SET @ResultId = NULL;

    -- Extract values from XML input
    SELECT 
        @UserId = @XMLInput.value('(User/c_userid)[1]', 'BIGINT'),
        @Email = @XMLInput.value('(User/c_email)[1]', 'NVARCHAR(100)'),
        @PasswordHash = @XMLInput.value('(User/c_passwordhash)[1]', 'NVARCHAR(256)'),
        @FirstName = @XMLInput.value('(User/c_firstname)[1]', 'NVARCHAR(50)'),
        @LastName = @XMLInput.value('(User/c_lastname)[1]', 'NVARCHAR(50)'),
        @ContactNumber = @XMLInput.value('(User/c_contactnumber)[1]', 'NVARCHAR(15)'),
        @RoleId = @XMLInput.value('(User/c_roleid)[1]', 'INT'),
		@CreatedBy = @XMLInput.value('(User/c_createdby)[1]', 'BIGINT'),
		@UpdatedBy = @XMLInput.value('(User/c_updatedby)[1]', 'BIGINT'),
        @IsActive = @XMLInput.value('(User/c_isactive)[1]', 'BIT');

    -- Print for debugging (optional)
    --PRINT 'Email: ' + ISNULL(@Email, '');

    -- Check if it's an update operation
    IF @IsUpdate = 1
    BEGIN
        -- Update the user
        UPDATE tbl_users
        SET 
            c_email = @Email,
           -- c_passwordhash = @PasswordHash,
            c_firstname = @FirstName,
            c_lastname = @LastName,
            c_contactnumber = @ContactNumber,
           -- c_roleid = @RoleId,
		    c_updatedby= (CASE
							WHEN @UpdatedBy = 0 THEN NULL
							ELSE @UpdatedBy
						END),
            c_updatedat = GETDATE(),
            c_isactive = @IsActive
        WHERE c_userid = @UserId;

        -- Set the output parameter to the updated UserId
        SET @ResultId = @UserId;
    END
    ELSE
    BEGIN
        -- Insert a new user
        INSERT INTO tbl_users (
            c_email,
            c_passwordhash,
            c_firstname,
            c_lastname,
            c_contactnumber,
            c_roleid,
			c_createdby,
            c_createdat,
            c_isactive
        )
        VALUES (
            @Email,
            @PasswordHash,
            @FirstName,
            @LastName,
            @ContactNumber,
            @RoleId,
			CASE
				WHEN @CreatedBy = 0 THEN NULL
				ELSE @CreatedBy
			END,
            GETDATE(),
            1  -- Assuming default is active
        );

        -- Set the output parameter to the newly inserted UserId
        SET @ResultId = SCOPE_IDENTITY();
    END
	Select @ResultId
END;
GO
------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[sp_user_Get]
    @UserId BIGINT NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 	u.c_userid, u.c_email, u.c_firstname, u.c_lastname,
           	u.c_contactnumber, u.c_roleid, r.c_rolename,
		   	u.c_createdby, cu.c_email as c_createdbyemail, u.c_updatedby, uu.c_email as c_updatedbyemail, u.c_isactive
    FROM tbl_users u
    INNER JOIN tbl_roles r ON r.c_roleid = u.c_roleid
	LEFT JOIN tbl_users cu ON cu.c_userid = u.c_createdby
	LEFT JOIN tbl_users uu ON uu.c_userid = u.c_updatedby
    WHERE @UserId IS NULL OR u.c_userid = @UserId
    ORDER BY u.c_createdat ASC;

END;
GO
------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[sp_user_delete]
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM tbl_users
    WHERE c_userid = @UserId;

	SELECT @@ROWCOUNT AS RowsAffected;

END;
GO
------------------------------------------------------------------------------------------------------------------------