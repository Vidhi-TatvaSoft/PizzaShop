----------------------------------------GetItemByCategory-----------------------------------
CREATE OR REPLACE FUNCTION get_items_by_category(categoryId BIGINT, search_text TEXT DEFAULT '')
RETURNS JSON AS $$
DECLARE
    result JSON;
BEGIN
    WITH filtered_items AS (
        SELECT 
            i.item_id,
            i.item_name,
            i.category_id,
            i.item_type_id,
            CEIL(i.rate) AS rate,
            i.item_image,
            i."IsFavourite",
            i.isdelete
        FROM items i
        WHERE 
            i.isavailable = true 
            AND i.isdelete = false
            AND (
                (categoryId = -1 AND i."IsFavourite" = true) OR
                (categoryId = 0) OR
                (categoryId > 0 AND i.category_id = categoryId)
            )
            AND (search_text = '' OR LOWER(TRIM(i.item_name)) LIKE LOWER(TRIM(search_text || '%')))
    )
    SELECT json_agg(
        json_build_object(
            'ItemId', item_id,
            'ItemName', item_name,
            'CategoryId', categoryId,
            'ItemTypeId', item_type_id,
            'Rate', rate,
            'ItemImage', item_image,
            'IsFavourite', "IsFavourite",
            'Isdelete', isdelete
        )
    ) INTO result
    FROM filtered_items;

    RETURN COALESCE(result, '[]'::JSON);
END;
$$ LANGUAGE plpgsql;



select get_items_by_category(1,'')

-----------------------------------SaveCustomerDetails-------------------------------------------
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



------------------------------------ GetModifiersByItemId --------------------------------------------
-- CREATE OR REPLACE FUNCTION get_modifier_by_itemid(itemId BIGINT)
-- RETURNS JSON AS $$
-- DECLARE
--     result JSON;
-- 	typeId BIGINT;
-- BEGIN
--     WITH items AS (
--         SELECT *
--         FROM items i
-- 		left join itemmodifiergroupmapping as imm on imm.item_id = i.item_id
-- 		left join modifiergroup as mg on mg.modifier_grp_id = imm.modifier_grp_id
-- 		left join modifier as m on m.modifier_grp_id = mg.modifier_grp_id
--         WHERE 
--             i.item_id = itemId AND i.isdelete = false
-- 		LIMIT 1;
--     )
-- 	select item_type_id from items;
--     SELECT json_agg(
--         json_build_object(
--             'ModifierGrpId', item_id,
--             'ModifierGrpName', item_name,
--             'min', categoryId,
--             'max', item_type_id,
--             'TypeId', rate,
--             'modifierList', item_image
--         )
--     ) INTO result
--     FROM items;

--     RETURN COALESCE(result, '[]'::JSON);
-- END;
-- $$ LANGUAGE plpgsql;
drop FUNCTION get_modifiers_by_item_id(itemId BIGINT)

CREATE OR REPLACE FUNCTION get_modifiers_by_item_id(itemId BIGINT)
RETURNS JSON AS $$
DECLARE
    result_json JSON;
    item_typeId BIGINT;
    item_exists BOOLEAN;
BEGIN
    -- Check if item exists and get ItemTypeId
    SELECT EXISTS (
        SELECT 1 FROM Items 
        WHERE item_id =itemId AND isdelete = FALSE
    ) INTO item_exists;

    IF NOT item_exists THEN
        RETURN '[]'::JSON;
    END IF;

    -- Get ItemTypeId
    SELECT item_type_id INTO item_typeId
    FROM items
    WHERE item_id = itemId  AND isdelete = FALSE;

    -- Build JSON result
    SELECT json_agg(
        json_build_object(
            'ModifierGrpId', mg.modifier_grp_id,
            'ModifierGrpName', mg.modifier_grp_name,
            'min', img.minvalue,
            'max', img.maxvalue,
            'TypeId', item_typeId,
            'modifierList', COALESCE((
                SELECT json_agg(
                    json_build_object(
                        'ModifierId', m.modifier_id,
                        'ModifierName', m.modifier_name,
                        'Rate', m.rate
                    )
                )
                FROM Modifier m
                WHERE m.modifier_grp_id = mg.modifier_grp_id
                AND m.isdelete = FALSE
            ), '[]'::JSON)
        )
    ) INTO result_json
    FROM itemmodifiergroupmapping img
    JOIN modifiergroup mg ON img.modifier_grp_id = mg.modifier_grp_id
    WHERE img.item_id = itemId
    AND img.isdelete = FALSE
    AND mg.isdelete = FALSE;

    -- Return empty array if no results
    RETURN COALESCE(result_json, '[]'::JSON);
END;
$$ LANGUAGE plpgsql;

select get_modifiers_by_item_id(24)




-----------------------------SaveRatings-------------------------------------------------------
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


















