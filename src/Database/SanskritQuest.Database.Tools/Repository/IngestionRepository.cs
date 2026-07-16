using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Npgsql;

namespace SanskritQuest.Database.Tools.Repository
{
    public class IngestionRepository : IIngestionRepository
    {
        private readonly string _connectionString;

        public IngestionRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<int> EnsureHierarchyNodeAsync(HierarchyNode node)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // First check if it exists
            using var selectCmd = new NpgsqlCommand(@"
                SELECT hierarchy_id FROM scripture.hierarchy
                WHERE scripture_id = @scriptureId
                  AND parent_id IS NOT DISTINCT FROM @parentId
                  AND local_label = @localLabel;", conn);

            selectCmd.Parameters.AddWithValue("scriptureId", node.ScriptureId);
            selectCmd.Parameters.AddWithValue("parentId", (object?)node.ParentId ?? DBNull.Value);
            selectCmd.Parameters.AddWithValue("localLabel", node.LocalLabel);

            var existingId = await selectCmd.ExecuteScalarAsync();
            if (existingId != null)
            {
                return Convert.ToInt32(existingId);
            }

            // Insert new hierarchy node
            using var insertCmd = new NpgsqlCommand(@"
                INSERT INTO scripture.hierarchy (scripture_id, parent_id, local_label, node_type, titles, description, sequence_number)
                VALUES (@scriptureId, @parentId, @localLabel, @nodeType, @titles::jsonb, @description::jsonb, @sequenceNumber)
                RETURNING hierarchy_id;", conn);

            insertCmd.Parameters.AddWithValue("scriptureId", node.ScriptureId);
            insertCmd.Parameters.AddWithValue("parentId", (object?)node.ParentId ?? DBNull.Value);
            insertCmd.Parameters.AddWithValue("localLabel", node.LocalLabel);
            insertCmd.Parameters.AddWithValue("nodeType", node.NodeType);
            insertCmd.Parameters.AddWithValue("titles", node.TitlesJson);
            insertCmd.Parameters.AddWithValue("description", node.DescriptionJson);
            insertCmd.Parameters.AddWithValue("sequenceNumber", node.SequenceNumber);

            var newId = await insertCmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }

        public async Task IngestVersesBatchAsync(List<VerseEntry> verses)
        {
            if (verses == null || verses.Count == 0) return;

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                foreach (var verse in verses)
                {
                    // Check if verse already exists to ensure idempotency
                    using var selectCmd = new NpgsqlCommand(@"
                        SELECT verse_id FROM scripture.verses
                        WHERE hierarchy_id = @hierarchyId AND verse_number = @verseNumber;", conn, transaction);
                    selectCmd.Parameters.AddWithValue("hierarchyId", verse.HierarchyId);
                    selectCmd.Parameters.AddWithValue("verseNumber", verse.VerseNumber);
                    
                    var existingVerseId = await selectCmd.ExecuteScalarAsync();

                    if (existingVerseId != null)
                    {
                        // Update
                        using var updateCmd = new NpgsqlCommand(@"
                            UPDATE scripture.verses
                            SET content_sanskrit = @contentSanskrit,
                                verse_data = @verseData::jsonb,
                                source_id = @sourceId,
                                search_weight = @searchWeight,
                                meta_tags = @metaTags::jsonb
                            WHERE verse_id = @verseId;", conn, transaction);

                        updateCmd.Parameters.AddWithValue("verseId", Convert.ToInt32(existingVerseId));
                        updateCmd.Parameters.AddWithValue("contentSanskrit", verse.ContentSanskrit);
                        updateCmd.Parameters.AddWithValue("verseData", verse.VerseDataJson);
                        updateCmd.Parameters.AddWithValue("sourceId", verse.SourceId);
                        updateCmd.Parameters.AddWithValue("searchWeight", verse.SearchWeight);
                        updateCmd.Parameters.AddWithValue("metaTags", verse.MetaTagsJson);

                        await updateCmd.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        // Insert
                        using var insertCmd = new NpgsqlCommand(@"
                            INSERT INTO scripture.verses (
                                hierarchy_id, verse_number, verse_type, content_sanskrit,
                                verse_data, source_id, search_weight, meta_tags
                            ) VALUES (
                                @hierarchyId, @verseNumber, @verseType, @contentSanskrit,
                                @verseData::jsonb, @sourceId, @searchWeight, @metaTags::jsonb
                            );", conn, transaction);

                        insertCmd.Parameters.AddWithValue("hierarchyId", verse.HierarchyId);
                        insertCmd.Parameters.AddWithValue("verseNumber", verse.VerseNumber);
                        insertCmd.Parameters.AddWithValue("verseType", verse.VerseType);
                        insertCmd.Parameters.AddWithValue("contentSanskrit", verse.ContentSanskrit);
                        insertCmd.Parameters.AddWithValue("verseData", verse.VerseDataJson);
                        insertCmd.Parameters.AddWithValue("sourceId", verse.SourceId);
                        insertCmd.Parameters.AddWithValue("searchWeight", verse.SearchWeight);
                        insertCmd.Parameters.AddWithValue("metaTags", verse.MetaTagsJson);

                        await insertCmd.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
