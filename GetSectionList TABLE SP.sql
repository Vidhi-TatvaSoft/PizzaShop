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
			left join tables as t on s.section_id = t.section_id
			where s.isdelete = false
			order By s.section_id
	)list;
	RETURN sectionList;
	END;
$$ LANGUAGE plpgsql;


select GetSectionList()