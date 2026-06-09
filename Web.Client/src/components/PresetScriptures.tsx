import React, { useState, useRef, useEffect } from 'react';
import { Compass, Sparkles, BookOpen, ChevronRight, LayoutGrid, List, ArrowRight } from 'lucide-react';
import { Scripture } from '../types.ts';

interface PresetScripturesProps {
  scriptureContext: string;
  onSelectScripture: (sc: Scripture) => void;
  isDark?: boolean;
  popularScriptures?: Scripture[];
}

type ViewMode = 'card' | 'list';

export default function PresetScriptures({ scriptureContext, onSelectScripture, isDark = false, popularScriptures = [] }: PresetScripturesProps) {
  const [selectedCategory, setSelectedCategory] = useState<string>('All');
  const [viewMode, setViewMode] = useState<ViewMode>('card');
  const [expandedCards, setExpandedCards] = useState<Record<string, boolean>>({});
  
  const categories = ['All', 'Gita', 'Vedas', 'Upanishads', 'Valmiki Ramayan', 'Stotram'];
  
  const filteredScriptures = selectedCategory === 'All' 
    ? popularScriptures 
    : popularScriptures.filter(sc => sc.category === selectedCategory);

  const toggleCardExpanded = (id: string, e: React.MouseEvent) => {
    e.stopPropagation();
    setExpandedCards(prev => ({
      ...prev,
      [id]: !prev[id]
    }));
  };

  if (popularScriptures.length === 0) {
    return (
      <div className={`text-center py-12 text-xs font-mono border border-dashed rounded-xl ${
        isDark ? 'text-[#5E5B66] border-[#2C2932]' : 'text-gray-400 border-[#E6E2D3]'
      }`}>
        <span className="inline-block w-4 h-4 border-2 border-[#D97706] border-t-transparent rounded-full animate-spin mr-2 align-middle" />
        Loading preset scriptures from the server...
      </div>
    );
  }

  return (
    <div className="space-y-6 pt-1">
      {/* Categories Filtering Block & View Mode Toggles */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between py-1 border-b border-[#E6E2D3]/40 dark:border-[#2C2932]/40 gap-4">
        <div className="flex flex-wrap items-center gap-3">
          <span className={`text-[10px] font-mono font-black uppercase tracking-widest ${isDark ? 'text-[#726E7A]' : 'text-[#78716C]'}`}>
            Category:
          </span>
          <div className="flex flex-wrap gap-1.5">
            {categories.map((cat) => (
              <button
                key={cat}
                onClick={() => setSelectedCategory(cat)}
                className={`px-3 py-1.5 text-xs font-semibold rounded-lg transition-all cursor-pointer ${
                  selectedCategory === cat
                    ? 'bg-[#D97706] text-white shadow-sm'
                    : isDark
                      ? 'bg-[#1E1D23] border border-[#2C2932] text-[#A19E95] hover:text-[#F1EFF7] hover:bg-[#2A2931]'
                      : 'bg-[#FFFDF9] border border-[#E6E2D3] text-[#57534E] hover:text-[#1C1917] hover:bg-[#F3EEE0]'
                }`}
              >
                {cat === 'All' ? 'All' : cat}
              </button>
            ))}
          </div>
        </div>

        {/* View Switcher toggle */}
        <div className="flex items-center gap-2 self-end md:self-auto">
          <span className={`text-[10px] font-mono font-black uppercase tracking-widest ${isDark ? 'text-[#726E7A]' : 'text-[#78716C]'}`}>
            View Mode:
          </span>
          <div className={`p-0.5 rounded-lg flex items-center border ${
            isDark ? 'bg-[#151419] border-[#2C2932]' : 'bg-[#F5F5F4] border-[#E6E2D3]'
          }`}>
            <button
              onClick={() => setViewMode('card')}
              className={`px-2.5 py-1 rounded-md text-xs font-semibold flex items-center gap-1 cursor-pointer transition-all ${
                viewMode === 'card'
                  ? 'bg-[#D97706] text-white shadow-sm'
                  : isDark
                    ? 'text-[#9B98A3] hover:text-[#F1EFF7]'
                    : 'text-[#57534E] hover:text-[#1C1917]'
              }`}
              title="Card View"
            >
              <LayoutGrid className="w-3.5 h-3.5" />
              <span>Card</span>
            </button>
            <button
              onClick={() => setViewMode('list')}
              className={`px-2.5 py-1 rounded-md text-xs font-semibold flex items-center gap-1 cursor-pointer transition-all ${
                viewMode === 'list'
                  ? 'bg-[#D97706] text-white shadow-sm'
                  : isDark
                    ? 'text-[#9B98A3] hover:text-[#F1EFF7]'
                    : 'text-[#57534E] hover:text-[#1C1917]'
              }`}
              title="List View"
            >
              <List className="w-3.5 h-3.5" />
              <span>List</span>
            </button>
          </div>
        </div>
      </div>

      {viewMode === 'list' ? (
        /* List View Layout: Beautiful Table Grid with Header Row */
        <div className="w-full overflow-hidden rounded-2xl border transition-all duration-300 shadow-sm border-[#E6E2D3] dark:border-[#2C2932]">
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse min-w-[700px]">
              <thead>
                <tr className={`border-b text-[10px] font-mono tracking-widest uppercase font-bold transition-colors ${
                  isDark 
                    ? 'bg-[#1E1D23] border-[#2C2932] text-[#9B98A3]' 
                    : 'bg-[#FFFDF9] border-[#E6E2D3] text-[#78716C]'
                }`}>
                  <th className="p-4 w-[24%] p-4 pl-6">Scripturial Reference</th>
                  <th className="p-4 w-[28%]">Vedic Sound (Sanskrit)</th>
                  <th className="p-4 w-[38%]">Bilingual Translations</th>
                  <th className="p-4 w-[10%] text-right pr-6">Action</th>
                </tr>
              </thead>
              <tbody className={`divide-y transition-colors divide-[#E6E2D3]/40 dark:divide-[#2C2932]/40 ${
                isDark ? 'bg-[#16151A]' : 'bg-white'
              }`}>
                {filteredScriptures.map((sc) => {
                  const isSelected = scriptureContext === sc.source;
                  return (
                    <tr
                      key={sc.id}
                      className={`group transition-colors duration-200 ${
                        isSelected
                          ? isDark
                            ? 'bg-[#1E1B24]'
                            : 'bg-[#FCF5E3]'
                          : isDark
                            ? 'hover:bg-[#1E1D23]/60'
                            : 'hover:bg-[#FFFDF9]'
                      }`}
                    >
                      <td className="p-4 pl-6 align-top">
                        <div className="space-y-2">
                          <span className="inline-block font-mono text-[9px] px-2.5 py-0.5 rounded bg-[#D97706]/10 border border-[#D97706]/20 text-[#D97706] font-extrabold uppercase">
                            {sc.category}
                          </span>
                          <h4 className={`font-serif text-sm font-bold transition-colors group-hover:text-[#D97706] ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>
                            {sc.title}
                          </h4>
                          <p className={`font-mono text-[10px] ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
                            {sc.source}
                          </p>
                        </div>
                      </td>

                      <td className="p-4 align-top">
                        <p className={`font-serif leading-relaxed text-xs sm:text-sm p-3 rounded-xl border text-justify transition-colors whitespace-pre-wrap font-bold ${
                          isDark 
                            ? 'bg-[#212027]/40 border-[#2D2934] text-amber-200 group-hover:bg-[#201F25]' 
                            : 'bg-[#FDFBF7] border-[#E2DDD0] text-[#1C1917]'
                        }`}>
                          {sc.verse}
                        </p>
                      </td>

                      <td className="p-4 align-top">
                        <div className="space-y-3 pl-1">
                          {/* English Translation */}
                          <div className="space-y-0.5">
                            <span className={`text-[9px] font-mono uppercase tracking-wider block font-bold ${isDark ? 'text-blue-400' : 'text-blue-600'}`}>
                              English (Translation)
                            </span>
                            <p className={`text-xs leading-relaxed text-justify ${isDark ? 'text-[#CBD5E1]' : 'text-[#57534E]'}`}>
                              {sc.translationDefaultEnglish}
                            </p>
                          </div>

                          {/* Hindi Translation */}
                          <div className="space-y-0.5 pt-1.5 border-t border-[#E6E2D3]/30 dark:border-[#2C2932]/30 border-dashed">
                            <span className={`text-[9px] font-mono uppercase tracking-wider block font-bold ${isDark ? 'text-emerald-400' : 'text-emerald-700'}`}>
                              Hindi / हिन्दी (अनुवाद)
                            </span>
                            <p className={`text-xs font-serif leading-relaxed text-justify ${isDark ? 'text-[#E1DCD3]' : 'text-[#2D241E]'}`}>
                              {sc.translationDefaultHindi}
                            </p>
                          </div>
                        </div>
                      </td>

                      <td className="p-4 align-top text-right pr-6">
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            onSelectScripture(sc);
                          }}
                          className="inline-flex items-center gap-1 px-3 py-1.5 text-xs font-bold text-[#D97706] rounded-lg transition-colors border border-[#D97706]/20 bg-[#D97706]/5 group-hover:bg-[#D97706] group-hover:text-white group-hover:border-[#D97706] shadow-sm ml-auto"
                        >
                          <span>Load</span>
                          <ArrowRight className="w-3.5 h-3.5 transition-transform group-hover:translate-x-0.5" />
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>
      ) : (
        /* Card View Layout with Flex wrapping alignment and stretch */
        <div className="flex flex-wrap justify-start gap-4 max-h-[550px] overflow-y-auto pr-1">
          {filteredScriptures.map((sc) => (
            <ScriptureCard
              key={sc.id}
              sc={sc}
              isSelected={scriptureContext === sc.source}
              isDark={isDark}
              onSelectScripture={onSelectScripture}
            />
          ))}
        </div>
      )}
    </div>
  );
}

/* ScriptureCard component with precise dynamic truncation detection and compact dictionary-like styling */
interface ScriptureCardProps {
  sc: Scripture;
  isSelected: boolean;
  isDark: boolean;
  onSelectScripture: (sc: Scripture) => void;
  key?: string;
}

function ScriptureCard({ sc, isSelected, isDark, onSelectScripture }: ScriptureCardProps) {
  const [isExpanded, setIsExpanded] = useState(false);
  const [englishTruncated, setEnglishTruncated] = useState(false);
  const [hindiTruncated, setHindiTruncated] = useState(false);

  const englishRef = useRef<HTMLParagraphElement | null>(null);
  const hindiRef = useRef<HTMLParagraphElement | null>(null);

  useEffect(() => {
    const handleResize = () => {
      if (isExpanded) return;
      
      const engEl = englishRef.current;
      if (engEl) {
        setEnglishTruncated(engEl.scrollHeight > engEl.clientHeight);
      }
      
      const hinEl = hindiRef.current;
      if (hinEl) {
        setHindiTruncated(hinEl.scrollHeight > hinEl.clientHeight);
      }
    };

    // Initial check
    handleResize();

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, [sc.translationDefaultEnglish, sc.translationDefaultHindi, isExpanded]);

  const toggleExpanded = (e: React.MouseEvent) => {
    e.stopPropagation();
    setIsExpanded(!isExpanded);
  };

  const hasTruncation = englishTruncated || hindiTruncated;

  return (
    <div
      className={`border rounded-xl p-4 flex flex-col justify-between transition-all flex-1 min-w-[325px] sm:min-w-[340px] md:min-w-[360px] select-none ${
        isSelected
          ? isDark
            ? 'bg-[#1E1B24] border-[#D97706] shadow-sm'
            : 'bg-[#FCF5E3] border-[#D97706] shadow-sm'
          : isDark
            ? 'bg-[#16151A] border-[#2C2932] hover:border-[#D97706]/60 hover:bg-[#201F25]'
            : 'bg-white border-[#E6E2D3] hover:border-[#D97706]/40 hover:bg-[#FAF9F5]/40'
      }`}
    >
      <div className="space-y-3">
        <div className="flex justify-between items-center text-xs">
          <span className="font-mono text-[9px] px-1.5 py-0.5 rounded bg-[#D97706]/10 border border-[#D97706]/20 text-[#D97706] font-extrabold uppercase">
            {sc.category}
          </span>
          <span className={`font-mono text-[9px] font-medium ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>{sc.source}</span>
        </div>

        <div className="space-y-1">
          <h4 className={`font-serif text-xs font-bold leading-tight transition-colors ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>
            {sc.title}
          </h4>
          {/* Sanskrit verse block */}
          <p className={`font-serif leading-relaxed text-xs p-1.5 rounded-lg border text-justify transition-colors whitespace-pre-wrap font-bold ${
            isDark 
              ? 'bg-[#212027]/40 border-[#2D2934] text-amber-200' 
              : 'bg-[#FDFBF7] border-[#E2DDD0] text-[#1C1917]'
          }`}>
            {sc.verse}
          </p>
        </div>

        {/* Dynamic translation view block containing both English and Hindi translations with line-clamping and toggle */}
        <div className={`space-y-1 pt-1.5 border-t border-dashed text-xs leading-normal ${isDark ? 'border-[#2C2932]' : 'border-[#E6E2D3]'}`}>
          <div>
            <p 
              ref={englishRef}
              className={`text-xs text-justify pl-0.5 ${isDark ? 'text-[#CBD5E1]' : 'text-[#57534E]'} ${
                isExpanded ? '' : 'line-clamp-3'
              }`}
            >
              <strong className="font-semibold text-blue-600 dark:text-blue-400">English: </strong>
              {sc.translationDefaultEnglish}
            </p>
          </div>
          <div className={`pt-1 border-t border-dashed ${isDark ? 'border-[#2C2932]' : 'border-[#E6E2D3]/60'}`}>
            <p 
              ref={hindiRef}
              className={`text-xs font-serif text-justify pl-0.5 ${isDark ? 'text-[#E1DCD3]' : 'text-[#2D241E]'} ${
                isExpanded ? '' : 'line-clamp-3'
              }`}
            >
              <strong className="font-semibold text-emerald-700 dark:text-emerald-400">Hindi: </strong>
              {sc.translationDefaultHindi}
            </p>
          </div>

          {/* Expand / Collapse Button only if content is actually truncated in browser */}
          {hasTruncation && (
            <button
              onClick={toggleExpanded}
              className={`text-[9px] font-black mt-0.5 underline underline-offset-2 decoration-amber-500/50 hover:text-[#D97706] pl-0.5 transition-colors block cursor-pointer text-left ${
                isDark ? 'text-amber-300 hover:text-amber-400' : 'text-[#57534E] hover:text-[#1C1917]'
              }`}
            >
              {isExpanded ? 'Show Less ↑' : 'Read More ↓'}
            </button>
          )}
        </div>
      </div>

      {/* Action hover footer - now handles the Load interaction exclusively */}
      <div className={`pt-1.5 border-t border-dashed mt-auto flex justify-end shrink-0 ${isDark ? 'border-[#2C2932]' : 'border-[#E6E2D3]'}`}>
        <button 
          onClick={() => onSelectScripture(sc)}
          className="inline-flex items-center gap-1 text-[10px] font-bold text-[#D97706] hover:text-amber-600 dark:hover:text-amber-400 cursor-pointer transition-colors"
        >
          <Compass className="w-3 h-3 shrink-0" />
          <span>Load in Active Translator</span>
          <ChevronRight className="w-2.5 h-2.5 transition-transform hover:translate-x-0.5" />
        </button>
      </div>
    </div>
  );
}
