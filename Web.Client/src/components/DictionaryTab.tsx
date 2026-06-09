import React, { useState, useEffect } from 'react';
import { motion } from 'motion/react';
import { BookOpen, ArrowRight, LayoutGrid, List } from 'lucide-react';
import apiClient from '../utils/apiClient.ts';

interface DictionaryTabProps {
  onTranslateWord: (word: string, category: string) => void;
  isDark?: boolean;
}

type CategoryType = 'All' | 'Vedas' | 'Upanishads' | 'Gita' | 'Ramayana' | 'Puranas';
type ViewMode = 'card' | 'list';

export default function DictionaryTab({ onTranslateWord, isDark = false }: DictionaryTabProps) {
  const [dictSearchQuery, setDictSearchQuery] = useState<string>('');
  const [dictCategory, setDictCategory] = useState<CategoryType>('All');
  const [viewMode, setViewMode] = useState<ViewMode>('card');
  const [specializedDict, setSpecializedDict] = useState<Record<string, any>>({});
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    let active = true;
    apiClient.get('/api/dictionary')
      .then(res => {
        if (active && res.data?.all) {
          setSpecializedDict(res.data.all);
          setIsLoading(false);
        }
      })
      .catch(err => {
        console.error("Failed to load dictionary from API:", err);
        if (active) setIsLoading(false);
      });
    return () => { active = false; };
  }, []);

  const categoryColors: Record<string, string> = isDark 
    ? {
        Vedas: 'bg-amber-500/10 text-amber-300 border-amber-500/30',
        Upanishads: 'bg-violet-500/10 text-violet-300 border-violet-500/30',
        Gita: 'bg-emerald-500/10 text-emerald-300 border-emerald-500/30',
        Ramayana: 'bg-blue-500/10 text-blue-300 border-blue-500/30',
        Puranas: 'bg-rose-500/10 text-rose-300 border-rose-500/30'
      }
    : {
        Vedas: 'bg-amber-100 text-amber-900 border-amber-200',
        Upanishads: 'bg-violet-100 text-violet-900 border-violet-200',
        Gita: 'bg-emerald-100 text-emerald-950 border-emerald-200',
        Ramayana: 'bg-blue-100 text-blue-900 border-blue-200',
        Puranas: 'bg-rose-100 text-rose-900 border-rose-200'
      };

  const filteredEntries = Object.entries(specializedDict)
    .filter(([word, entry]) => {
      const matchesCat = dictCategory === 'All' || entry.category === dictCategory;
      const normSearch = dictSearchQuery.toLowerCase();
      const matchesSearch =
        word.toLowerCase().includes(normSearch) ||
        entry.eng.toLowerCase().includes(normSearch) ||
        entry.hin.toLowerCase().includes(normSearch) ||
        entry.grammar.toLowerCase().includes(normSearch);
      return matchesCat && matchesSearch;
    });

  return (
    <motion.div
      key="tab-dictionary"
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -10 }}
      className="flex-1 flex flex-col space-y-4"
    >
      <div className={`border rounded-xl p-4 space-y-3 transition-colors ${
        isDark ? 'bg-[#1E1D23] border-[#2C2932]' : 'bg-[#FFFDF9] border-[#E6E2D3]'
      }`}>
        <div className="flex flex-col sm:flex-row gap-3 justify-between items-start sm:items-center">
          <div>
            <h4 className={`text-sm font-serif font-bold flex items-center gap-1.5 transition-colors ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>
              <BookOpen className="w-4 h-4 text-[#D97706]" />
              <span>Specialized Scripture Dictionaries</span>
            </h4>
            <p className={`text-[11px] transition-colors ${isDark ? 'text-[#9B98A3]' : 'text-[#57534E]'}`}>
              Browse accurate philosophical and theological terms across classical scriptures.
            </p>
          </div>
          <div className={`text-[10px] font-mono px-2 py-1 rounded border transition-colors ${
            isDark ? 'bg-[#16151A] text-[#9B98A3] border-[#2C2932]' : 'bg-[#F5F5F4] text-[#78716C] border-[#E6E2D3]'
          }`}>
            Total Terms: {filteredEntries.length} entries shown
          </div>
        </div>

        {/* Filter Search controls & Layout Mode Toggles */}
        <div className="grid grid-cols-1 md:grid-cols-12 gap-3 pt-1 items-center">
          <div className="relative md:col-span-4">
            <input
              type="text"
              placeholder="Search Devanagari, English or Hindi..."
              value={dictSearchQuery}
              onChange={(e) => setDictSearchQuery(e.target.value)}
              className={`w-full text-xs rounded-lg py-1.5 pl-3 pr-8 focus:outline-none focus:ring-1 focus:ring-[#D97706] focus:border-[#D97706] transition-colors ${
                isDark 
                  ? 'bg-[#151419] border-[#2C2932] text-[#F1EFF7] placeholder:text-[#5E5B66]' 
                  : 'bg-white border-[#E6E2D3] text-[#1C1917] placeholder:text-[#A8A29E]'
              }`}
            />
            {dictSearchQuery && (
              <button
                onClick={() => setDictSearchQuery('')}
                className="absolute right-2.5 top-1/2 -translate-y-1/2 text-[10px] bg-red-500/10 hover:bg-red-500/20 px-1.5 py-0.5 rounded text-red-500 cursor-pointer font-semibold transition-colors"
              >
                Clear
              </button>
            )}
          </div>

          {/* Category selector */}
          <div className="flex flex-wrap gap-1 items-center md:col-span-5">
            {(['All', 'Vedas', 'Upanishads', 'Gita', 'Ramayana', 'Puranas'] as CategoryType[]).map((cat) => (
              <button
                key={cat}
                onClick={() => setDictCategory(cat)}
                className={`px-2 py-1 rounded text-[10px] font-semibold transition-all cursor-pointer ${
                  dictCategory === cat
                    ? 'bg-[#D97706] text-white shadow-sm'
                    : isDark
                      ? 'bg-[#16151A] text-[#9B98A3] hover:bg-[#25242D] border border-[#2C2932]'
                      : 'bg-[#F5F5F4] text-[#57534E] hover:bg-gray-200 border border-[#E6E2D3]'
                }`}
              >
                {cat}
              </button>
            ))}
          </div>

          {/* View switches */}
          <div className="flex items-center gap-1.5 justify-end md:col-span-3">
            <span className={`text-[9px] font-mono font-black uppercase tracking-widest ${isDark ? 'text-[#726E7A]' : 'text-[#78716C]'}`}>
              View:
            </span>
            <div className={`p-0.5 rounded-lg flex items-center border ${
              isDark ? 'bg-[#151419] border-[#2C2932]' : 'bg-[#F5F5F4] border-[#E6E2D3]'
            }`}>
              <button
                onClick={() => setViewMode('card')}
                className={`px-2.5 py-1 rounded-md text-[10px] font-semibold flex items-center gap-1 cursor-pointer transition-all ${
                  viewMode === 'card'
                    ? 'bg-[#D97706] text-white shadow-sm'
                    : isDark
                      ? 'text-[#9B98A3] hover:text-[#F1EFF7]'
                      : 'text-[#57534E] hover:text-[#1C1917]'
                }`}
                title="Card View"
              >
                <LayoutGrid className="w-3 h-3" />
                <span>Card</span>
              </button>
              <button
                onClick={() => setViewMode('list')}
                className={`px-2.5 py-1 rounded-md text-[10px] font-semibold flex items-center gap-1 cursor-pointer transition-all ${
                  viewMode === 'list'
                    ? 'bg-[#D97706] text-white shadow-sm'
                    : isDark
                      ? 'text-[#9B98A3] hover:text-[#F1EFF7]'
                      : 'text-[#57534E] hover:text-[#1C1917]'
                }`}
                title="List View"
              >
                <List className="w-3 h-3" />
                <span>List</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      {isLoading ? (
        <div className={`text-center py-12 text-xs font-mono border border-dashed rounded-xl ${
          isDark ? 'text-[#5E5B66] border-[#2C2932]' : 'text-gray-400 border-[#E6E2D3]'
        }`}>
          <span className="inline-block w-4 h-4 border-2 border-[#D97706] border-t-transparent rounded-full animate-spin mr-2 align-middle" />
          Loading lexicon words from the server...
        </div>
      ) : viewMode === 'list' ? (
        /* List View Layout: Beautiful Table Grid with Header Row */
        <div className="w-full overflow-hidden rounded-xl border transition-all duration-300 shadow-sm border-[#E6E2D3] dark:border-[#2C2932] max-h-[420px] overflow-y-auto">
          <table className="w-full text-left border-collapse min-w-[650px]">
            <thead>
              <tr className={`border-b text-[10px] font-mono tracking-widest uppercase font-bold sticky top-0 z-10 transition-colors ${
                isDark 
                  ? 'bg-[#1E1D23] border-[#2C2932] text-[#9B98A3]' 
                  : 'bg-[#FFFDF9] border-[#E6E2D3] text-[#78716C]'
              }`}>
                <th className="p-3 pl-5 w-[18%]">Sanskrit Term</th>
                <th className="p-3 w-[15%]">Grammar</th>
                <th className="p-3 w-[15%]">Scripture</th>
                <th className="p-3 w-[24%]">English Translation</th>
                <th className="p-3 w-[22%]">Hindi / हिन्दी (अनुवाद)</th>
                <th className="p-3 text-right pr-5 w-[10%]">Action</th>
              </tr>
            </thead>
            <tbody className={`divide-y divide-[#E6E2D3]/40 dark:divide-[#2C2932]/40 ${
              isDark ? 'bg-[#16151A]' : 'bg-white'
            }`}>
              {filteredEntries.map(([word, entry]) => (
                <tr
                  key={word}
                  className={`group transition-colors duration-150 ${
                    isDark ? 'hover:bg-[#1E1D23]/60' : 'hover:bg-[#FFFDF9]'
                  }`}
                >
                  {/* Sanskrit Term */}
                  <td className="p-3 pl-5 align-middle">
                    <span className="font-serif text-base font-bold text-[#D97706] tracking-wider block">
                      {word}
                    </span>
                  </td>

                  {/* Grammar */}
                  <td className="p-3 align-middle text-xs font-mono">
                    <span className={isDark ? 'text-[#9B98A3]' : 'text-[#57534E]'}>
                      {entry.grammar}
                    </span>
                  </td>

                  {/* Scripture category badge */}
                  <td className="p-3 align-middle">
                    <span className={`text-[9px] font-semibold px-2 py-0.5 rounded-full border ${categoryColors[entry.category] || 'bg-gray-100 text-gray-800'}`}>
                      {entry.category}
                    </span>
                  </td>

                  {/* English meaning definition */}
                  <td className="p-3 align-middle text-xs leading-relaxed">
                    <span className={isDark ? 'text-[#DFDCE6]' : 'text-[#2D241E]'}>
                      {entry.eng}
                    </span>
                  </td>

                  {/* Hindi meaning translation */}
                  <td className="p-3 align-middle text-xs font-serif leading-relaxed">
                    <span className={isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}>
                      {entry.hin}
                    </span>
                  </td>

                  {/* Analyze detail handle button */}
                  <td className="p-3 align-middle text-right pr-5">
                    <button
                      onClick={() => onTranslateWord(word, entry.category)}
                      className="text-[9px] font-bold text-[#D97706] hover:bg-[#D97706] hover:text-white px-2.5 py-1 rounded transition-colors border border-[#D97706]/35 bg-[#D97706]/5 cursor-pointer inline-flex items-center gap-1 shadow-sm"
                    >
                      <span>Analyze</span>
                      <ArrowRight className="w-3 h-3 group-hover:translate-x-0.5 transition-transform" />
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        /* Word results flex layout card style with optimal stretch */
        <div className="flex flex-wrap justify-start gap-4 max-h-[480px] overflow-y-auto pr-1 items-start">
          {filteredEntries.map(([word, entry]) => (
            <div
              key={word}
              className={`border rounded-xl p-4 flex flex-col justify-between transition-all flex-1 min-w-[325px] sm:min-w-[340px] md:min-w-[360px] select-none ${
                isDark 
                  ? 'bg-[#1E1D23] border-[#2C2932] hover:border-[#D97706]/60 hover:bg-[#25242D]' 
                  : 'bg-white border-[#E6E2D3] hover:border-[#D97706]/40 hover:bg-[#FAF9F5]/40'
              }`}
            >
              <div className="space-y-3">
                <div className="space-y-1">
                  <div className="flex items-center justify-between">
                    <span className="font-serif text-lg font-bold text-[#D97706] tracking-wider">
                      {word}
                    </span>
                    <span className={`text-[9px] font-semibold px-2 py-0.5 rounded-full border ${categoryColors[entry.category] || 'bg-gray-100 text-gray-800'}`}>
                      {entry.category}
                    </span>
                  </div>
                  <div className={`text-[10px] font-mono leading-none ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
                    {entry.grammar}
                  </div>
                </div>
                
                <div className={`text-xs space-y-2.5 pt-1.5 border-t border-dashed ${isDark ? 'border-[#2C2932]' : 'border-[#E6E2D3]'}`}>
                  <p className={`leading-normal ${isDark ? 'text-[#DFDCE6]' : 'text-[#2D241E]'}`}>
                    <strong className={`font-mono text-[9px] uppercase tracking-wider block mb-0.5 ${isDark ? 'text-[#726E7A]' : 'text-gray-400'}`}>English</strong> 
                    {entry.eng}
                  </p>
                  <p className="leading-normal font-serif">
                    <strong className={`font-mono text-[9px] uppercase tracking-wider block mb-0.5 ${isDark ? 'text-[#726E7A]' : 'text-gray-400'}`}>Hindi / हिन्दी</strong> 
                    <span className={isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}>{entry.hin}</span>
                  </p>
                </div>
              </div>

              <div className="pt-1.5 flex justify-end shrink-0 border-t border-dashed mt-auto border-transparent">
                <button
                  onClick={() => onTranslateWord(word, entry.category)}
                  className="text-[10px] font-bold text-[#D97706] hover:bg-[#D97706]/10 px-2.5 py-1 rounded transition-colors border border-[#D97706]/20 bg-[#D97706]/5 cursor-pointer flex items-center gap-1"
                >
                  <span>Analyze Deeper</span>
                  <ArrowRight className="w-3 h-3" />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {!isLoading && filteredEntries.length === 0 && (
        <div className={`text-center py-12 text-xs font-mono border rounded-xl border-dashed ${isDark ? 'text-[#5E5B66] border-[#2C2932]' : 'text-gray-400 border-[#E6E2D3]'}`}>
          No matching scriptural terms found for "{dictSearchQuery}". Try another keyword!
        </div>
      )}
    </motion.div>
  );
}
