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

