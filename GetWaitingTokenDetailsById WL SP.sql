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

