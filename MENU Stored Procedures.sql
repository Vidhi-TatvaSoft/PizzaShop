create or replace procedure SaveCustomerDetails(
	customerId BIGINT, 
	name TEXT, 
	mobileNo BIGINT, 
	NoofPersons INT, 
	userId BIGINT)
language plpgsql as $$
BEGIN
	update customers 
	set customer_name = name,
		phoneno = mobileNo,
		modified_at =now(),
		modified_by = userId
	where customer_id = customerId AND isdelete = FALSE;

	update waitinglist
	set no_of_person = NoofPersons,
		modified_at = now(),
		modified_by = userId
	where customer_id = customerId AND isassign = TRUE;

	UPDATE Assigntable
        SET 
            no_of_person = NoofPersons,
            modified_at = now(),
            modified_by = userId
        WHERE customer_id = customerId AND Isdelete = FALSE;
END;
$$

-----------------------------FavouriteItemManage--------------------------------------
CREATE OR REPLACE PROCEDURE FavouriteItemManage(
    itemId BIGINT,
    IsFavourite BOOLEAN,
    userId BIGINT
) language plpgsql as $$
BEGIN
    UPDATE items
    SET "IsFavourite" = IsFavourite,
        modified_at = NOW(),
        modified_by = userId
    WHERE item_id = itemId AND isdelete = FALSE;

END;
$$



DROP FUNCTION FavouriteItemManage( itemId BIGINT,
    IsFavourite BOOLEAN,
    userId BIGINT)
















