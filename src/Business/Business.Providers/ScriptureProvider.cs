using SanskritQuest.Business.Contracts;
using SanskritQuest.Business.Providers.Mapping;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Common.Utilities;
using SanskritQuest.Data.Contracts;

namespace SanskritQuest.Business.Providers;

public class ScriptureProvider : IScriptureProvider
{
	private readonly ILocalDataSetsProvider _localDataSetsProvider;
	private readonly IScripturesDataProvider _scripturesDataProvider;

	public ScriptureProvider(
		ILocalDataSetsProvider localDataSetsProvider,
		IScripturesDataProvider scripturesDataProvider)
	{
		_localDataSetsProvider = localDataSetsProvider;
		_scripturesDataProvider = scripturesDataProvider;
	}

	public List<Contracts.Scripture> GetPopularScriptures()
	{
		var translator = new BusinessTranslator();
		return _localDataSetsProvider.PopularScriptures
			.Select(s => translator.LocalScriptureToBusiness(s))
			.ToList();
	}

	public async Task<IEnumerable<Contracts.Scripture>> GetAllScripturesAsync(CancellationToken cancellationToken = default)
	{
		var translator = new BusinessTranslator();
		var dbScriptures = await _scripturesDataProvider.GetAllScripturesAsync(cancellationToken);
		return dbScriptures.Select(s => translator.DbScriptureToBusiness(s));
	}

	public async Task<ScriptureDetails?> GetScriptureDetailsAsync(int scriptureId, CancellationToken cancellationToken = default)
	{
		var dbDetails = await _scripturesDataProvider.GetScriptureDetailsAsync(scriptureId, cancellationToken);
		var detailsList = dbDetails.ToList();
		if (detailsList.Count == 0)
		{
			return null;
		}

		var first = detailsList.First();
		var translator = new BusinessTranslator();

		// Map each detail item to node business model
		var allNodes = detailsList
			.Select(d => translator.ScriptureDetailToBusiness(d))
			.ToList();

		var nodeLookup = allNodes.ToDictionary(n => n.HierarchyId);
		var rootNodes = new List<ScriptureHierarchyNode>();

		// Build hierarchy tree
		foreach (var node in allNodes)
		{
			if (node.ParentId.HasValue && nodeLookup.TryGetValue(node.ParentId.Value, out var parentNode))
			{
				parentNode.Children.Add(node);
			}
			else
			{
				rootNodes.Add(node);
			}
		}

		// Resolve EnumName
		var scriptureType = scriptureId.GetEnumByDatabaseId<ScriptureType>();
		var enumName = scriptureType?.ToString() ?? string.Empty;

		// Compute FullHierarchyLabel recursively
		void ComputeLabels(ScriptureHierarchyNode node, string parentPath)
		{
			node.FullHierarchyLabel = string.IsNullOrEmpty(parentPath)
				? $"{enumName}.{node.LocalLabel}"
				: $"{parentPath}.{node.LocalLabel}";

			foreach (var child in node.Children)
			{
				ComputeLabels(child, node.FullHierarchyLabel);
			}
		}

		foreach (var rootNode in rootNodes)
		{
			ComputeLabels(rootNode, string.Empty);
		}

		return new ScriptureDetails
		{
			ScriptureId = first.ScriptureId,
			Code = first.ScriptureCode,
			Titles = first.ScriptureTitles,
			Author = first.ScriptureAuthor,
			Description = first.ScriptureDescription,
			EnumName = enumName,
			Hierarchy = rootNodes
		};
	}

	public async Task<VerseDetails?> GetVerseDetailsAsync(int verseId, CancellationToken cancellationToken = default)
	{
		var detail = await _scripturesDataProvider.GetVerseDetailsAsync(verseId, cancellationToken);
		if (detail == null)
		{
			return null;
		}

		var translator = new BusinessTranslator();
		var model = translator.VerseDetailToBusiness(detail);

		// Resolve transliteration in provider only
		model.ScriptureName = detail.ScriptureTitles?.En ?? string.Empty;
		model.Transliteration = Transliterator.DevanagariToIast(model.SanskritShloka);
		if (model.WordByWordBreakdown != null)
		{
			foreach (var breakdown in model.WordByWordBreakdown)
			{
				breakdown.Transliteration = Transliterator.DevanagariToIast(breakdown.SanskritWord);
			}
		}

		// Build the hierarchy path
		var scriptureDetails = await GetScriptureDetailsAsync(detail.ScriptureId, cancellationToken);
		if (scriptureDetails != null)
		{
			// Flat list lookup to find the target node
			var flatNodes = new List<ScriptureHierarchyNode>();
			void Flatten(IEnumerable<ScriptureHierarchyNode> nodes)
			{
				foreach (var node in nodes)
				{
					flatNodes.Add(node);
					Flatten(node.Children);
				}
			}
			Flatten(scriptureDetails.Hierarchy);

			var node = flatNodes.FirstOrDefault(n => n.HierarchyId == detail.HierarchyId);
			if (node != null)
			{
				var levelChain = new List<Contracts.HierarchyLevel>();
				var current = node;
				while (current != null)
				{
					levelChain.Add(new Contracts.HierarchyLevel
					{
						LevelType = current.NodeType.ToString(),
						LevelNumber = current.SequenceNumber,
						// Use Sanskrit title, fallback to English then Hindi
						LevelNameSanskrit = current.Titles?.Sa ?? current.Titles?.En ?? current.Titles?.Hi
					});

					current = current.ParentId.HasValue
						? flatNodes.FirstOrDefault(n => n.HierarchyId == current.ParentId.Value)
						: null;
				}

				levelChain.Reverse();

				// Add leaf level representing the verse itself
				levelChain.Add(new Contracts.HierarchyLevel
				{
					LevelType = detail.VerseType.ToString(),
					LevelNumber = int.TryParse(detail.VerseNumber, out var num) ? num : 0,
					LevelNameSanskrit = "श्लोक " + detail.VerseNumber
				});

				model.Hierarchy = levelChain;
			}
		}

		return model;
	}

	public async Task<VerseDetails?> GetVerseDetailsByHierarchyAsync(string scriptureCode, int[] levelNumbers, CancellationToken cancellationToken = default)
	{
		if (levelNumbers == null || levelNumbers.Length == 0)
		{
			return null;
		}

		var scriptures = await GetAllScripturesAsync(cancellationToken);
		var scripture = scriptures.FirstOrDefault(s => s.Code.Equals(scriptureCode, StringComparison.OrdinalIgnoreCase));
		if (scripture == null)
		{
			return null;
		}

		var details = await GetScriptureDetailsAsync(scripture.ScriptureId, cancellationToken);
		if (details == null)
		{
			return null;
		}

		// Parse path and verse number
		var targetPath = string.Join("/", levelNumbers.SkipLast(1));
		var verseNumberStr = levelNumbers.Last().ToString();

		// Find hierarchy node matching the path
		var flatNodes = new List<ScriptureHierarchyNode>();
		void Flatten(IEnumerable<ScriptureHierarchyNode> nodes)
		{
			foreach (var node in nodes)
			{
				flatNodes.Add(node);
				Flatten(node.Children);
			}
		}
		Flatten(details.Hierarchy);

		var matchedNode = flatNodes.FirstOrDefault(n => n.Path == targetPath);
		if (matchedNode == null)
		{
			return null;
		}

		var verses = await _scripturesDataProvider.GetAllVersesIdForHierarchyAsync(matchedNode.HierarchyId, cancellationToken);
		foreach (var verse in verses)
		{
			var vDetail = await GetVerseDetailsAsync(verse.VerseId, cancellationToken);
			if (vDetail != null && vDetail.Hierarchy.LastOrDefault()?.LevelNumber.ToString() == verseNumberStr)
			{
				return vDetail;
			}
		}

		return null;
	}
}
