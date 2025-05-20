create or replace function ChangeItemQuantitiesAndStatus
		(orderdetailIds integer[],
		itemquantity integer[], 
		inputStatus varchar(50), 
		ModifiedBy bigint )
returns boolean  as $$
BEGIN
	For i in 1..array_length(orderdetailIds, 1)
	Loop
		update public.orderdetails 
		set modified_at = now(), 
			modified_by = ModifiedBy, 
			"readyQuantity" = "readyQuantity" + 
                CASE 
                    WHEN inputStatus = 'InProgress' THEN itemquantity[i]
                    ELSE -itemquantity[i]
                END
		where orderdetail_id = orderdetailIds[i] AND isdelete =false;
	END Loop;
	RETURN  TRUE;
END;
$$ LANGUAGE plpgsql;

