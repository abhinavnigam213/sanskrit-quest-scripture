using System.Collections.Generic;
using SanskritQuest.Data.Contracts;
using SanskritQuest.Web.Api.Models;

namespace SanskritQuest.Web.Api.MockBusiness;

// TODO: Remove this mock implementation in the future once the actual business layer is implemented.
public interface IMockScriptureProvider
{
    List<Scripture> GetPopularScriptures();
    IEnumerable<ScriptureResponse> GetAllScriptures();
    ScriptureDetailsResponse? GetScriptureDetails(int scriptureId);
    VerseDetailsResponse GetVerseDetails(string scriptureCode, int[] levelNumbers);
}
