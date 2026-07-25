using System.Collections.Generic;
using Riok.Mapperly.Abstractions;

using BusinessScripture = SanskritQuest.Business.Contracts.Scripture;
using ApiResponseScripture = SanskritQuest.Web.Api.Models.ScriptureResponse;
using BusinessScriptureDetails = SanskritQuest.Business.Contracts.ScriptureDetails;
using ApiResponseScriptureDetails = SanskritQuest.Web.Api.Models.ScriptureDetailsResponse;
using BusinessScriptureHierarchyNode = SanskritQuest.Business.Contracts.ScriptureHierarchyNode;
using ApiResponseScriptureHierarchyNode = SanskritQuest.Web.Api.Models.ScriptureHierarchyNodeResponse;
using BusinessVerseDetails = SanskritQuest.Business.Contracts.VerseDetails;
using ApiResponseVerseDetails = SanskritQuest.Web.Api.Models.VerseDetailsResponse;
using BusinessWordBreakdown = SanskritQuest.Business.Contracts.WordBreakdown;
using ApiResponseWordBreakdown = SanskritQuest.Web.Api.Models.WordBreakdown;
using BusinessTranslation = SanskritQuest.Business.Contracts.Translation;
using ApiResponseTranslation = SanskritQuest.Web.Api.Models.Translation;
using BusinessCommentary = SanskritQuest.Business.Contracts.Commentary;
using ApiResponseCommentary = SanskritQuest.Web.Api.Models.Commentary;
using BusinessHierarchyLevel = SanskritQuest.Business.Contracts.HierarchyLevel;
using ApiResponseHierarchyLevel = SanskritQuest.Web.Api.Models.HierarchyLevel;

namespace SanskritQuest.Web.Api.Mapping;

[Mapper]
public partial class ApiTranslator
{
	[MapProperty(nameof(BusinessScripture.ScriptureId), nameof(ApiResponseScripture.ScriptureId))]
	public partial ApiResponseScripture ScriptureBusinessToApi(BusinessScripture model);

	public partial ApiResponseScriptureDetails ScriptureDetailsBusinessToApi(BusinessScriptureDetails model);

	public partial ApiResponseScriptureHierarchyNode ScriptureHierarchyNodeBusinessToApi(BusinessScriptureHierarchyNode model);

	public partial ApiResponseVerseDetails VerseDetailsBusinessToApi(BusinessVerseDetails model);

	[MapProperty(nameof(BusinessWordBreakdown.TranslationEn), nameof(ApiResponseWordBreakdown.TranslationEn))]
	[MapProperty(nameof(BusinessWordBreakdown.TranslationHi), nameof(ApiResponseWordBreakdown.TranslationHi))]
	public partial ApiResponseWordBreakdown WordBreakdownBusinessToApi(BusinessWordBreakdown model);

	[MapProperty(nameof(BusinessTranslation.TranslationEn), nameof(ApiResponseTranslation.TranslationEn))]
	[MapProperty(nameof(BusinessTranslation.TranslationHi), nameof(ApiResponseTranslation.TranslationHi))]
	public partial ApiResponseTranslation TranslationBusinessToApi(BusinessTranslation model);

	public partial ApiResponseCommentary CommentaryBusinessToApi(BusinessCommentary model);

	public partial ApiResponseHierarchyLevel HierarchyLevelBusinessToApi(BusinessHierarchyLevel model);
}
