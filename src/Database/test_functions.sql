-- ========================================================================
-- Sanskrit Quest: Database Function Execution Examples
-- ========================================================================

-- 1. EXECUTE: GetAllScripture
-- Retrieves all scriptures with their categories and classes directly.
SELECT * FROM scripture.GetAllScripture();


-- 2. EXECUTE: GetScriptureDetails
-- Traverses hierarchy nodes and gets verse counts.
-- Takes scripture_id as input and returns a structured table.
SELECT * FROM scripture.GetScriptureDetails(1);


-- 3. EXECUTE: GetVersesDetails
-- Retrieves details of a single verse directly as a table row.
-- Takes verse_id as input.
SELECT * FROM scripture.GetVersesDetails(1);


-- 4. EXECUTE: GetAllVersesIdForHierarchy
-- Retrieves all verses recursively under a hierarchy node.
-- Takes hierarchy_id as input.
SELECT * FROM scripture.GetAllVersesIdForHierarchy(1);
