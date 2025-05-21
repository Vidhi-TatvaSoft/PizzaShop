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
				from sections as si
				left join tables as ti on si.section_id = ti.section_id
				where  ti.isdelete = false AND ti.status = 'Available'
				group by(si.section_id)
			)as  "AvailableCount",
			(
			select  Count(ti.table_id)  
				from sections as si
				left join tables as ti on si.section_id = ti.section_id
				where  ti.isdelete = false AND ti.status = 'Running'
				group by(si.section_id)
			)as  "RunningCount",
			(
			select  Count(ti.table_id)  
				from sections as si
				left join tables as ti on si.section_id = ti.section_id
				where  ti.isdelete = false AND ti.status = 'Assigned'
				group by(si.section_id)
			)as  "AssignedCount",
			from sections as s
			left join tables as t on s.section_id = t.secton_id
			where s.isdelete = false
			order By s.section_id
	)list;
	RETURN sectionList;
	END;
$$ LANGUAGE plpgsql;


select GetAllSection()