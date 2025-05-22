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
	(inpEmail text,
	inpCustomerName Text,
	inpCustomerNo bigint,
	 ModifiedBy bigint)
LANGUAGE plpgsql
AS $$
DECLARE
IsCustomerPresent BOOLEAN;
    row_count BIGINT;
BEGIN
    SELECT COUNT(*) INTO row_count 
    FROM customers c 
    WHERE c.email = 'v@vfgbf.com';
    
    IF row_count = 0 THEN
        IsCustomerPresent := FALSE;
    ELSE
        IsCustomerPresent := TRUE;
    END IF;


	case
		when IsCustomerPresent = true
		then Insert into  public.customers (customer_name, phoneno, email, created_by)
			values (inpCustomerName, inpCustomerNo, inpEmail, ModifiedBy)
		else UPDATE public.customers
			SET  customer_name=inpCustomerName, phoneno=inpCustomerNo, email=inpEmail,  modified_at=now(), modified_by=ModifiedBy
			WHERE email=inpEmail AND isdelete = false
		END;
		 
END;
$$

