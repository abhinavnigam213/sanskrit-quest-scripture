using System;
using System.Collections.Generic;
using SanskritQuest.Business.Contracts;
using SanskritQuest.Data.Contracts;
using SanskritQuest.Web.Api.Models;

namespace SanskritQuest.Web.Api.MockBusiness;

// TODO: Remove this mock implementation in the future once the actual business layer is implemented.
public class MockScriptureProvider : IMockScriptureProvider
{
    private readonly IScriptureProvider _realScriptureProvider;

    public MockScriptureProvider(IScriptureProvider realScriptureProvider)
    {
        _realScriptureProvider = realScriptureProvider;
    }

    public List<Scripture> GetPopularScriptures()
    {
        // Route to real scripture provider for popular scriptures (loads JSON files correctly)
        return _realScriptureProvider.GetPopularScriptures();
    }

    public IEnumerable<ScriptureResponse> GetAllScriptures()
    {
        return MockScriptureData.AllScriptures;
    }

    public ScriptureDetailsResponse? GetScriptureDetails(int scriptureId)
    {
        var header = MockScriptureData.AllScriptures.FirstOrDefault(s => s.ScriptureId == scriptureId);
        if (header == null)
        {
            return null;
        }

        return new ScriptureDetailsResponse
        {
            ScriptureId = header.ScriptureId,
            Code = header.Code,
            Titles = header.Titles,
            Author = header.Author,
            Description = header.Description,
            EnumName = header.EnumName,
            Hierarchy = MockScriptureData.GetMockHierarchy(header.ScriptureId, header.EnumName)
        };
    }

    public VerseDetailsResponse GetVerseDetails(string scriptureCode, int[] levelNumbers)
    {
        return MockScriptureData.GetMockVerseResponse(scriptureCode, levelNumbers);
    }
}
