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