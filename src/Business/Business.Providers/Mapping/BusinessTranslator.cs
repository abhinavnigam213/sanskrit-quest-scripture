using System;
using System.Collections.Generic;
using System.Linq;
using Riok.Mapperly.Abstractions;
using SanskritQuest.Data.Contracts;
using SanskritQuest.Common.Utilities;
using SanskritQuest.Common.Contracts;

using BusinessScripture = SanskritQuest.Business.Contracts.Scripture;
using DataScripture = SanskritQuest.Data.Contracts.Scripture;
using DbScripture = SanskritQuest.Data.Contracts.DbScripture;
using ScriptureDetail = SanskritQuest.Data.Contracts.ScriptureDetail;
using BusinessScriptureHierarchyNode = SanskritQuest.Business.Contracts.ScriptureHierarchyNode;
using BusinessWordBreakdown = SanskritQuest.Business.Contracts.WordBreakdown;
using DbWordBreakdownItem = SanskritQuest.Data.Contracts.DbWordBreakdownItem;
using VerseDetail = SanskritQuest.Data.Contracts.VerseDetail;
using BusinessVerseDetails = SanskritQuest.Business.Contracts.VerseDetails;
using BusinessHierarchyLevel = SanskritQuest.Business.Contracts.HierarchyLevel;
using BusinessTranslation = SanskritQuest.Business.Contracts.Translation;

namespace SanskritQuest.Business.Providers.Mapping;

[Mapper]
public partial class BusinessTranslator
{
	public BusinessScripture DbScriptureToBusiness(DbScripture dbScripture)
	{
		if (dbScripture == null) return null!;
		var model = MapToBusinessModelInternal(dbScripture);
		var scriptureType = dbScripture.ScriptureId.GetEnumByDatabaseId<ScriptureType>();
		model.EnumName = scriptureType?.ToString() ?? string.Empty;
		return model;
	}

	[MapProperty(nameof(DbScripture.ScriptureId), nameof(BusinessScripture.ScriptureId))]
	private partial BusinessScripture MapToBusinessModelInternal(DbScripture dbScripture);

	public partial DbScripture ScriptureBusinessToData(BusinessScripture model);

	[MapProperty(nameof(ScriptureDetail.HierarchyTitles), nameof(BusinessScriptureHierarchyNode.Titles))]
	[MapProperty(nameof(ScriptureDetail.HierarchyDescription), nameof(BusinessScriptureHierarchyNode.Description))]
	public partial BusinessScriptureHierarchyNode ScriptureDetailToBusiness(ScriptureDetail detail);

	public partial BusinessWordBreakdown DbWordBreakdownToBusiness(DbWordBreakdownItem dbWord);

	public BusinessScripture LocalScriptureToBusiness(DataScripture localScripture)
	{
		if (localScripture == null) return null!;
		int.TryParse(localScripture.Id, out var id);
		var model = new BusinessScripture
		{
			ScriptureId = id,
			Code = localScripture.Source,
			Titles = new LocalizedTitles { En = localScripture.Title, Sa = localScripture.Title },
			Description = new LocalizedDescription { En = localScripture.Category },
			CategoryName = localScripture.Category,
			ClassName = localScripture.Source,
		};
		var scriptureType = id.GetEnumByDatabaseId<ScriptureType>();
		model.EnumName = scriptureType?.ToString() ?? string.Empty;
		return model;
	}

	[MapProperty(nameof(VerseDetail.ContentSanskrit), nameof(BusinessVerseDetails.SanskritShloka))]
	[MapProperty(nameof(VerseDetail.VerseData), nameof(BusinessVerseDetails.Translation))]
	public partial BusinessVerseDetails VerseDetailToBusiness(VerseDetail detail);
}
