-- ========================================================================
-- Sanskrit Quest: Stored Procedure Execution Examples
-- ========================================================================

-- 1. EXECUTE: GetAllScripture
-- Since it returns multiple rows via a refcursor, we must run it in a 
-- transaction block.
BEGIN;

-- Declare/initialize cursor variable name 'scriptures_cursor'
CALL scripture.GetAllScripture('scriptures_cursor');

-- Fetch all rows from the returned cursor
FETCH ALL FROM scriptures_cursor;

-- End the transaction
COMMIT;


-- 2. EXECUTE: GetScriptureDetails
-- Traverses hierarchy nodes and gets verse counts.
-- Takes scripture_id as input and refcursor as INOUT.
BEGIN;

-- Retrieve details for scripture_id = 1 (Bhagavad Gita)
CALL scripture.GetScriptureDetails(1, 'gita_details_cursor');

-- Fetch all rows
FETCH ALL FROM gita_details_cursor;

COMMIT;


-- 3. EXECUTE: GetVersesDetails
-- Uses standard output parameters to retrieve details of a single verse.
-- In clients like pgAdmin/DBeaver, you can simply call it passing placeholders,
-- and it will return a single row containing the output parameter values:
CALL scripture.GetVersesDetails(
    1,     -- Input verse_id
    NULL,  -- o_verse_number (OUT)
    NULL,  -- o_verse_type (OUT)
    NULL,  -- o_content_sanskrit (OUT)
    NULL,  -- o_translation_en (OUT)
    NULL,  -- o_translation_hi (OUT)
    NULL,  -- o_word_breakdown (OUT)
    NULL,  -- o_source_id (OUT)
    NULL,  -- o_search_weight (OUT)
    NULL,  -- o_meta_tags (OUT)
    NULL,  -- o_hierarchy_id (OUT)
    NULL,  -- o_hierarchy_parent_id (OUT)
    NULL,  -- o_hierarchy_local_label (OUT)
    NULL,  -- o_hierarchy_path (OUT)
    NULL,  -- o_hierarchy_node_type (OUT)
    NULL,  -- o_hierarchy_title_en (OUT)
    NULL,  -- o_hierarchy_title_hi (OUT)
    NULL,  -- o_hierarchy_title_sa (OUT)
    NULL,  -- o_scripture_id (OUT)
    NULL,  -- o_scripture_code (OUT)
    NULL,  -- o_scripture_title_en (OUT)
    NULL,  -- o_scripture_title_hi (OUT)
    NULL   -- o_scripture_title_sa (OUT)
);


-- 4. EXECUTE: GetAllVersesIdForHierachy
-- Retrieves all verses recursively under a hierarchy node.
-- Takes hierarchy_id and refcursor as INOUT.
BEGIN;

-- Retrieve verses under hierarchy_id = 1 (Gita Root Node)
CALL scripture.GetAllVersesIdForHierachy(1, 'verses_cursor');

FETCH ALL FROM verses_cursor;

COMMIT;
