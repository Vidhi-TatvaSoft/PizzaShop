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