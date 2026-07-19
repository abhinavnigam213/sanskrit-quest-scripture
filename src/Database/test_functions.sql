-- ========================================================================
-- Sanskrit Quest: Database Function Execution Examples
-- ========================================================================

-- 1. EXECUTE: GetAllScripture
-- Retrieves all scriptures. Shows extraction of localized JSONB fields.
SELECT 
    scripture_id, 
    code, 
    titles->>'en' AS title_en, 
    titles->>'hi' AS title_hi,
    description->>'en' AS desc_en,
    author->>'en' AS author_en,
    author->>'hi' AS author_hi
FROM scripture.GetAllScripture();


-- 2. EXECUTE: GetScriptureDetails
-- Traverses hierarchy nodes and gets verse counts.
-- Shows extracting localized JSONB author and title keys.
SELECT 
    scripture_id, 
    scripture_code, 
    scripture_author->>'en' AS scripture_author_en,
    scripture_titles->>'en' AS scripture_title_en,
    hierarchy_id, 
    local_label,
    hierarchy_titles->>'en' AS hierarchy_title_en
FROM scripture.GetScriptureDetails(1);


-- 3. EXECUTE: GetVersesDetails
-- Retrieves details of a single verse directly as a table row.
-- Extracts scripture author JSONB and word_by_word_breakdown JSONB.
SELECT 
    verse_id, 
    verse_number, 
    content_sanskrit,
    scripture_author->>'en' AS scripture_author_en,
    word_by_word_breakdown
FROM scripture.GetVersesDetails(1);


-- 4. EXECUTE: GetAllVersesIdForHierarchy
-- Retrieves all verses recursively under a hierarchy node.
SELECT * FROM scripture.GetAllVersesIdForHierarchy(1);


-- 5. EXECUTE: SearchVersesBySanskritFTS
-- Performs Full Text Search on Sanskrit shloka.
-- Displays word_by_word_breakdown JSONB and scripture_author JSONB.
SELECT 
    rank, 
    verse_id, 
    verse_number, 
    content_sanskrit,
    scripture_author->>'en' AS author_en,
    word_by_word_breakdown
FROM scripture.SearchVersesBySanskritFTS('कर्मण्येवाधिकारस्ते', 5);


-- 6. EXECUTE: SearchVersesByTranslationFTS
-- Performs Full Text Search on translations.
-- Displays word_by_word_breakdown JSONB and scripture_author JSONB.
SELECT 
    rank, 
    verse_id, 
    verse_number, 
    translation_en,
    scripture_author->>'en' AS author_en,
    word_by_word_breakdown
FROM scripture.SearchVersesByTranslationFTS('duty', 10, 'english');
