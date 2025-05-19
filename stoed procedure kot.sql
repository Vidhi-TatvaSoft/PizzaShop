create or replace function getKotDetails()
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
				 ) as "tableString",
				 (select json_agg(row_to_json(itemlist))
				 from(
					select odi.item_id as "ItemId",
							odi.orderdetail_id as "OrderDetailId",
							ii.item_name as "ItemName",
							odi.extra_instruction as "ItemInstruction",
							odi.quantity - odi."readyQuantity" as "PendingItem",
							odi."readyQuantity" as "ReadyItem"
					from orderdetails as odi
					left join items as ii on odi.item_id = ii.item_id
					where odi.order_id = o.order_id
							
				 )as itemlist
				 )as "ItemsInOneCard"
				from kot as k
				left join orders as o on o.order_id = k.order_id
				where k.isdelete =false 
		)t;

	RETURN kotlist;
	END;
$$ LANGUAGE plpgsql;

DROP FUNCTION getkotdetails()

select getKotDetails()

select * from kot where order_id = 37




