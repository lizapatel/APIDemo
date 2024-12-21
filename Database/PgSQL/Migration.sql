CREATE TABLE tbl_roles (
    c_roleid SERIAL PRIMARY KEY,
    c_rolename VARCHAR(50) NOT NULL UNIQUE
);
------------------------------------------------------------------------------------------------------------------------
INSERT INTO tbl_roles (c_rolename) VALUES 
('System Administrator'), 
('System User');
------------------------------------------------------------------------------------------------------------------------
--DROP TABLE IF EXISTS tbl_users;
--TRUNCATE TABLE tbl_users RESTART IDENTITY;

--ALTER SEQUENCE tbl_users_c_userid_seq RESTART WITH 1;

CREATE TABLE tbl_users (
    c_userid BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    c_email VARCHAR(100) NOT NULL UNIQUE,
    c_passwordhash VARCHAR(256) NOT NULL,
    c_firstname VARCHAR(50) NOT NULL,
    c_lastname VARCHAR(50) NOT NULL,
    c_contactnumber VARCHAR(15),
    c_roleid INT NOT NULL,
	c_createdby BIGINT,
    c_createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	c_updatedby BIGINT,
    c_updatedat TIMESTAMP,
    c_isactive BOOLEAN DEFAULT TRUE,
    CONSTRAINT fk_role FOREIGN KEY (c_roleid) REFERENCES tbl_roles (c_roleid),
	CONSTRAINT fk_createdby FOREIGN KEY (c_createdby) REFERENCES tbl_users (c_userid),
	CONSTRAINT fk_updatedby FOREIGN KEY (c_updatedby) REFERENCES tbl_users (c_userid)
);
------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_user_addedit(json, boolean)

-- DROP FUNCTION IF EXISTS public.fn_user_addedit(json, boolean);

CREATE OR REPLACE FUNCTION public.fn_user_addedit(
	p_input_json json,
	p_is_update boolean)
    RETURNS bigint
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    p_user_id bigint;
BEGIN
    -- Check if it is an UPDATE operation
    IF p_is_update THEN
        -- Perform UPDATE
		UPDATE public.tbl_users
		SET c_email= p_input_json ->> 'c_email', 
			--c_passwordhash= p_input_json ->> 'c_passwordhash',  
			c_firstname= p_input_json ->> 'c_firstname', 
			c_lastname= p_input_json ->> 'c_lastname', 
			c_contactnumber= p_input_json ->> 'c_contactnumber', 
			--c_roleid= p_input_json ->> 'c_roleid',
			c_updatedby= (CASE
							WHEN (p_input_json ->> 'c_updatedby')::bigint = 0 THEN NULL
							ELSE (p_input_json ->> 'c_updatedby')::bigint
						END),
			c_updatedat= NOW(), 
			c_isactive= (p_input_json ->> 'c_isactive')::BOOLEAN
		WHERE c_userid = (p_input_json ->> 'c_userid')::bigint
        RETURNING c_userid INTO p_user_id;
    ELSE
        -- Perform INSERT
        INSERT INTO public.tbl_users(
					c_email, c_passwordhash, c_firstname, c_lastname,c_contactnumber, 
					c_roleid, c_createdby, c_createdat, c_isactive)
		VALUES (
            p_input_json ->> 'c_email',
            p_input_json ->> 'c_passwordhash',
            p_input_json ->> 'c_firstname',
            p_input_json ->> 'c_lastname',
            p_input_json ->> 'c_contactnumber',
			(p_input_json ->> 'c_roleid')::INT,
			CASE
				WHEN (p_input_json ->> 'c_createdby')::bigint = 0 THEN NULL
				ELSE (p_input_json ->> 'c_createdby')::bigint
			END,
            NOW(),
            (p_input_json ->> 'c_isactive')::BOOLEAN
        )
        RETURNING c_userid INTO p_user_id;
    END IF;
    RETURN p_user_id;
END;
$BODY$;
------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_user_get(bigint)

-- DROP FUNCTION IF EXISTS public.fn_user_get(bigint);

CREATE OR REPLACE FUNCTION public.fn_user_get(
	p_user_id bigint DEFAULT NULL::bigint)
    RETURNS TABLE(c_userid bigint, c_email character varying, c_firstname character varying, c_lastname character varying, c_contactnumber character varying, c_roleid integer, c_rolename character varying, c_createdby bigint, c_createdbyemail character varying, c_updatedby bigint, c_updatedbyemail character varying, c_isactive boolean) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT 	u.c_userid, u.c_email, u.c_firstname, u.c_lastname,
           	u.c_contactnumber, u.c_roleid, r.c_rolename,
		   	u.c_createdby, cu.c_email, u.c_updatedby, uu.c_email, u.c_isactive
    FROM tbl_users u
    INNER JOIN tbl_roles r ON r.c_roleid = u.c_roleid
	LEFT JOIN tbl_users cu ON cu.c_userid = u.c_createdby
	LEFT JOIN tbl_users uu ON uu.c_userid = u.c_updatedby
    WHERE p_user_id IS NULL OR u.c_userid = p_user_id
    ORDER BY u.c_createdat ASC;
END;
$BODY$;

------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_user_delete(bigint)

-- DROP FUNCTION IF EXISTS public.fn_user_delete(bigint);

CREATE OR REPLACE FUNCTION public.fn_user_delete(
	p_user_id bigint)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_deleted_count integer;
BEGIN
    -- Perform the DELETE operation
    DELETE FROM tbl_users
    WHERE c_userid = p_user_id
    RETURNING 1 INTO v_deleted_count;  -- Capture the number of deleted rows

    -- Return the count of affected rows
    RETURN v_deleted_count;
END;
$BODY$;
------------------------------------------------------------------------------------------------------------------------
