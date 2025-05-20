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