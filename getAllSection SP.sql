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