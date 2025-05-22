----------------------------get kot details (all kot)--------------------------------------
create or replace function getKotDetails(inputStatus varchar(50))
returns JSON  AS $$
declare 
	kotlist JSON;
BEGIN
	Select Json_agg(row_to_json(t)) into kotlist
	from(
		select o.order_id as "OrderId",
				o.order_date as "orderDate",
				o."otherInstruction" as "OrderInstruction",
				(select json_agg(row_to_json(tablelist))
				from(
					select ts.table_id as "TableId",
							ts.table_name as "TableName"
					from kot as ks
					left join assigntable as ats on ks.order_id = ats.order_id
					left join tables as ts on ts.table_id = ats.table_id
					left join sections as ss on ts.section_id = ss.section_id
					where ats.order_id = o.order_id AND ats.isdelete = false
       				)as tablelist
				 ) as "tableList",
				 (select json_agg(row_to_json(itemlist))
				 from(
					select odi.item_id as "ItemId",
							ci.category_id as "CategoryId",
							odi.orderdetail_id as "OrderDetailId",
							ii.item_name as "ItemName",
							odi.extra_instruction as "ItemInstruction",
							odi.quantity - odi."readyQuantity" as "PendingItem",
							odi."readyQuantity" as "ReadyItem",
							case inputStatus
								when 'InProgress' then odi.quantity - odi."readyQuantity" 
								else odi."readyQuantity" 
							End as "Quantity",
							(select Json_agg(row_to_json(modlist))
							from(
								select mom.modifier_id as "ModifierId",
										mm.modifier_name as "ModifierName"
								from modifierorder as mom
								left join modifier as mm on mm.modifier_id = mom.modifier_id
								where mom.orderdetail_id = odi.orderdetail_id AND mom.isdelete = false
							)as modlist
							)as "ModifiersInItem"
					from orderdetails as odi
					left join items as ii on odi.item_id = ii.item_id
					left join category as ci on ci.category_id = ii.category_id
					where odi.order_id = o.order_id AND odi.isdelete = false
							
				 )as itemlist
				 )as "ItemsInOneCard"
				from kot as k
				left join orders as o on o.order_id = k.order_id
				where k.isdelete =false 
		)t;

	RETURN kotlist;
	END;
$$ LANGUAGE plpgsql;

DROP FUNCTION getkotdetails(character varying)

select getKotDetails('Rady')

select * from kot where order_id = 37



----------------------change item quantity and status ------------------------------------------------
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

