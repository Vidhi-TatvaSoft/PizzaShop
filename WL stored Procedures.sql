------------------------get all sectuions-------------------------------------
CREATE OR REPLACE FUNCTION GetAllSection()
returns json as $$
declare sectionList json;
BEGIN
	select Json_agg(row_to_json(list)) into sectionList
	from(
		select 
			s.section_id as "SectionId",
			s.section_name as "SectionName",
			(
			select Count(wi.waiting_id)  
				from waitinglist as wi
				where wi.section_id = s.section_id AND wi.isassign = false AND wi.isdelete = false	
			)as "WaitingCount"
			from sections as s
			where s.isdelete = false
			order By s.section_id
	)list;
	RETURN sectionList;
	END;
$$ LANGUAGE plpgsql;


select GetAllSection()


----------------------------get waiting list by section id -------------------------------------------
create or replace function GetWaitingListBySection()
returns json as $$
declare Waitinglist json;
BEGIN
	select Json_agg(row_to_json(list)) into Waitinglist
	from(
		select 
			w.waiting_id as "waitingId",
			w.created_at as "CreatedAt",
			c.customer_name as "Name",
			w.no_of_person as "NoOfPerson",
			c.phoneno as "Mobileno",
			c.email as "Email",
			w.section_id as "SectionID",
			s.section_name as "SectionName"
			from waitinglist as w
			left join customers as c on w.customer_id = c.customer_id
			left join sections as s on w.section_id = s.section_id
			where w.isdelete = false AND w.isassign = false 
			-- case 
			-- 	when inputSectionId = 0 then AND w.isdelete = false
			-- 	else AND w.section_id = inputSectionId
			-- end
			order By w.waiting_id
	)list;
	RETURN Waitinglist;
	END;
$$ LANGUAGE plpgsql;



---------------------------------get waiting token details (for update)----------------------------------
CREATE OR REPLACE FUNCTION GetWaitingTokenDetailsById(inputWaitingId bigint)
returns json as $$
declare waitingList json;
BEGIN
	select row_to_json(list) into waitingList
	from(
		select 
			w.waiting_id as "waitingId",
			c.email as "Email",
			c.customer_name as "Name",
			c.phoneno as "Mobileno",
			w.no_of_person as "NoOfPerson",
			w.section_id as "SectionID",
			s.section_name as "SectionName"
			from waitinglist as w
			left join customers as c on w.customer_id = c.customer_id
			left join sections as s on w.section_id = s.section_id
			where w.isdelete = false AND w.isassign =  false AND w.waiting_id = inputWaitingId
			order By w.waiting_id
	)list;
	RETURN waitingList;
	END;
$$ LANGUAGE plpgsql;


select GetWaitingTokenDetailsById(53)




-----------------------------delete waiting token-----------------------------------
create or replace function DeleteWaitingToken
		(inputWaitingId bigint,
		ModifiedBy bigint )
returns boolean  as $$
BEGIN
	update public.waitinglist 
		set modified_at = now(), 
			modified_by = ModifiedBy, 
			isdelete= 'true'
		where waiting_id = inputWaitingId AND isdelete = false AND isassign = false;
	RETURN  TRUE;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE DeleteWaitingTokenSP
	(inputWaitingId bigint,
	 ModifiedBy bigint)
LANGUAGE plpgsql
AS $$
BEGIN
	UPDATE public.waitinglist 
	SET modified_at = now(), 
		modified_by = ModifiedBy, 
		isdelete = 'true'
	WHERE waiting_id = inputWaitingId 
		AND isdelete = false 
		AND isassign = false;
END;
$$


-------------------------------get table by section id -------------------------------------
CREATE OR REPLACE FUNCTION GetTableBySection(InputSectionID bigint)
returns json as $$
declare sectionList json;
BEGIN
	select Json_agg(row_to_json(list)) into sectionList
	from(
		select 
			t.table_id as "TableId",
			t.table_name as "TableName",
			t.section_id as "SectionId",
			t.capacity as "Capacity"
			from tables as t
			left join sections as s on t.section_id = s.section_id
			where t.isdelete = false AND t.section_id = InputSectionID AND t.status = 'Available'
			order By t.table_id
	)list;
	RETURN sectionList;
	END;
$$ LANGUAGE plpgsql;

select GetTableBySection(2)



----------------------------assign table -------------------------------------
CREATE OR REPLACE PROCEDURE AssignTable
	(inputTableIds bigint[],
	inputWaitingId bigint,
	inputSectionId bigint,
	ModifiedBy bigint)
LANGUAGE plpgsql
AS $$
declare custid bigint;
		noofperson int;
BEGIN
-- update waiting list
	UPDATE public.waitinglist 
	SET modified_at = now(), 
		modified_by = ModifiedBy, 
		isassign = 'true',
		assigned_at = now(),
		section_id = inputSectionId
	WHERE waiting_id = inputWaitingId 
		AND isdelete = false 
		AND isassign = false;

		
	select w.customer_id  into custid
	from waitinglist as w
	where w.waiting_id = inputWaitingId and w.isdelete = false;
	select w.no_of_person into noofperson
	from waitinglist as w
	where w.waiting_id = inputWaitingId and w.isdelete = false;
	
	
	FOR i in 1..array_length(inputTableIds, 1)
	LOOP
	-- inset into assigntable 
		INSERT INTO public.assigntable(
		 customer_id, created_by, table_id, no_of_person)
		VALUES (custid, ModifiedBy,inputTableIds[i], noofperson );
		
		-- update tables
		UPDATE public.tables 
		SET modified_at = now(), 
			modified_by = ModifiedBy, 
			status = 'Assigned'
		WHERE table_id = inputTableIds[i]
			AND isdelete = false ;
	END LOOP;
END;
$$

