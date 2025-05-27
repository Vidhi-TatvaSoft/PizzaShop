-----------------------get section list for tables module------------------------------
CREATE OR REPLACE FUNCTION GetSectionList()
returns json as $$
declare sectionList json;
BEGIN
	select Json_agg(row_to_json(list)) into sectionList
	from(
		select 
			s.section_id as "SectionId",
			s.section_name as "SectionName",
			(
			select  Count(ti.table_id)  
				from tables as ti
				where  ti.isdelete = false AND ti.status = 'Available' AND ti.section_id = s.section_id
			)as  "AvailableCount",
			(
			select  Count(tm.table_id)  
				from sections as sm
				left join tables as tm on sm.section_id = tm.section_id
				where  tm.isdelete = false AND tm.status = 'Running' AND tm.section_id = s.section_id
			)as  "RunningCount",
			(
			select  Count(te.table_id)  
				from sections as se
				left join tables as te on se.section_id = te.section_id
				where  te.isdelete = false AND te.status = 'Assigned' AND te.section_id = s.section_id
			)as  "AssignedCount"
			from sections as s
			where s.isdelete = false
			order By s.section_id
	)list;
	RETURN sectionList;
	END;
$$ LANGUAGE plpgsql;

select GetSectionList()



--------------------------get table details by section --------------------------------------
CREATE OR REPLACE FUNCTION GetTableDetailsBySection(inputSectionId bigint)
returns json as $$
declare sectionList json;
BEGIN
	select Json_agg(row_to_json(list)) into sectionList
	from(
		select 
			t.section_id as "SectionId",
			t.table_id as "TableId",
			t.table_name as "TableName",
			t.capacity as "Capacity",
			t.status as "Status",
			CASE 
				WHEN t.status IN ('Running', 'Assigned') THEN 
				COALESCE(
					(SELECT a.created_at 
					 FROM assigntable as a 
					 WHERE a.table_id = t.table_id 
					 AND a.isdelete = false 
					 LIMIT 1), CURRENT_TIMESTAMP)
			ELSE CURRENT_TIMESTAMP
			END AS "Totaltime",
			CASE 
		        WHEN t.status = 'Running' THEN 
		            COALESCE(
		                (SELECT o.total_amount 
		                 FROM assigntable a 
		                 LEFT JOIN orders o ON a.order_id = o.order_id 
		                 WHERE a.table_id = t.table_id 
		                 AND a.isdelete = false 
		                 LIMIT 1), 0)
		        ELSE 0
    		END AS "TotalSpend"
			from tables as t
			where t.isdelete = false AND t.section_id =inputSectionId
	)list;
	RETURN sectionList;
	END;
$$ LANGUAGE plpgsql;

select GetTableDetailsBySection(1)


----------------------------------------------AddEditCustomer ----------------------------------------------
CREATE OR REPLACE PROCEDURE AddEditCustomer
    (inpEmail TEXT,
     inpCustomerName TEXT,
     inpCustomerNo BIGINT,
	 ModifiedBy BIGINT)
LANGUAGE plpgsql
AS $$
DECLARE
    IsCustomerPresent BOOLEAN;
BEGIN
    -- Check if a customer with the given email already exists
    SELECT EXISTS (
        SELECT 1 
        FROM customers c 
        WHERE c.email = inpEmail
    ) INTO IsCustomerPresent;

    -- If customer exists, update; otherwise, insert
    IF IsCustomerPresent THEN
        UPDATE public.customers
        SET customer_name = inpCustomerName,
            phoneno = inpCustomerNo,
            email = inpEmail,
            modified_at = NOW(),
            modified_by = ModifiedBy
        WHERE email = inpEmail AND isdelete = FALSE;
    ELSE
        INSERT INTO public.customers (customer_name, phoneno, email, created_by)
        VALUES (inpCustomerName, inpCustomerNo, inpEmail, ModifiedBy);
    END IF;
END;
$$;


------------------------------------- AddEditCustomerToWaitingList -------------------------------------
create or replace procedure AddEditCustomerToWaitingList
	(inpEmail TEXT,
	inpWaitingId BIGINT, 
     inpNoOfPerson INTEGER,
     inpSectionId BIGINT,
	 ModifiedBy BIGINT
	)
LANGUAGE plpgsql
AS $$
DECLARE
    InpCustomerId BIGINT;
BEGIN
	select c.customer_id into InpCustomerId
	from customers as c 
	where c.email = inpEmail;

	IF inpWaitingId = 0 THEN
		INSERT INTO public.waitinglist
		( customer_id, assigned_at, no_of_person,  created_by, section_id)
		VALUES (InpCustomerId, NOW(), inpNoOfPerson, ModifiedBy, inpSectionId);
	ELSE 
		 UPDATE public.waitinglist
        SET customer_id = InpCustomerId,
			no_of_person = inpNoOfPerson,
            section_id = inpSectionId,
            modified_at = NOW(),
            modified_by = ModifiedBy
        WHERE waiting_id = inpWaitingId AND isdelete = FALSE AND isassign = FALSE;
	END IF;
END;
$$;

CALL AddEditCustomerToWaitingList('vidhi123@gmail.com',0, 2,2, 36)



---------------------------------GetListOfCustomerWaiting----------------------------------------
create or replace function GetListOfCustomerWaiting(inpSectionId bigint )
returns JSON as $$
Declare customerWaiting JSON;
BEGIN
	select json_agg(row_to_json(list)) into customerWaiting
	from(
		select
		w.waiting_id as "ID",
		c.customer_name as "Name",
		w.no_of_person as "NoOfPerson",
		(
		select row_to_json(customerDetails)
		from (
			select 
			ci.email as "Email",
			ci.customer_name as "Name",
			ci.phoneno as "Mobileno",
			wi.no_of_person as "NoOfPerson",
			wi.section_id as "SectionID",
			si.section_name as "SectionName"
			from waitinglist as wi
			left join customers as ci on wi.customer_id = ci.customer_id
			left join sections as si on si.section_id = wi.section_id
			WHERE wi.waiting_id = w.waiting_id
			) customerDetails
		)as "customerDetails"
		from waitinglist as w 
		left join customers as c on w.customer_id = c.customer_id
		where  w.section_id = inpSectionId AND w.isdelete = FALSE AND w.isassign = FALSE
	)list;
	return customerWaiting;
END;
$$ LANGUAGE plpgsql;


select GetListOfCustomerWaiting(1)












