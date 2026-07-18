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


-- 5. EXECUTE: SearchVersesBySanskritFTS
-- Performs Full Text Search on Sanskrit shloka.
-- Parameters: query text, max rows (null for no limit), scripture_id (null or invalid for all scriptures)
SELECT * FROM scripture.SearchVersesBySanskritFTS('कर्मण्येवाधिकारस्ते', 5);
-- Limit search to Gita (scripture_id = 1)
SELECT * FROM scripture.SearchVersesBySanskritFTS('फलेषु', NULL, 1);
-- Limit search to Ramayana (scripture_id = 2)
SELECT * FROM scripture.SearchVersesBySanskritFTS('फलेषु', NULL, 2);
-- Invalid scripture ID (999) - falls back to all scriptures
SELECT * FROM scripture.SearchVersesBySanskritFTS('फलेषु', NULL, 999);


-- 6. EXECUTE: SearchVersesByTranslationFTS
-- Performs Full Text Search on translations (English, Hindi, or both).
-- Parameters: query text, max rows (null for no limit), translation language ('english', 'hindi', or 'both'), scripture_id
SELECT * FROM scripture.SearchVersesByTranslationFTS('duty', 10, 'english');
-- Limit search to Gita (scripture_id = 1)
SELECT * FROM scripture.SearchVersesByTranslationFTS('fruits', NULL, 'both', 1);
-- Limit search to Ramayana (scripture_id = 2)
SELECT * FROM scripture.SearchVersesByTranslationFTS('fruits', 10, 'both', 2);
-- Invalid scripture ID (999) - falls back to all scriptures
SELECT * FROM scripture.SearchVersesByTranslationFTS('fruits', 10, 'both', 999);


