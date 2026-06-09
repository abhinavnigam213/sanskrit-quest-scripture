import React from 'react';
import { Trash2, RefreshCw, Sparkles, Globe, FileCode } from 'lucide-react';

interface TranslationWorkbenchProps {
  text: string;
  setText: (text: string) => void;
  sourceLang: 'sanskrit' | 'hindi' | 'english' | 'auto';
  setSourceLang: (lang: 'sanskrit' | 'hindi' | 'english' | 'auto') => void;
  targetLang: 'sanskrit' | 'hindi' | 'english';
  setTargetLang: (lang: 'sanskrit' | 'hindi' | 'english') => void;
  scriptureContext: string;
  setScriptureContext: (ctx: string) => void;
  isLoading: boolean;
  onTriggerAnalysis: () => void;
  onTriggerSimpleTranslation: () => void;
  onTriggerPureTransliteration: (target: 'iast' | 'itrans' | 'english_phonetic' | 'devanagari' | 'slp1') => void;
  isDark?: boolean;
  workflowMode?: 'input' | 'readonly';
  setWorkflowMode?: (mode: 'input' | 'readonly') => void;
}

export default function TranslationWorkbench({
  text,
  setText,
  sourceLang,
  setSourceLang,
  targetLang,
  setTargetLang,
  scriptureContext,
  setScriptureContext,
  isLoading,
  onTriggerAnalysis,
  onTriggerSimpleTranslation,
  onTriggerPureTransliteration,
  isDark = false,
  workflowMode = 'input',
  setWorkflowMode,
}: TranslationWorkbenchProps) {
  
  if (workflowMode === 'readonly') {
    return (
      <div className={`p-4 rounded-xl border flex flex-col space-y-3 transition-all duration-300 ${
        isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
      }`}>
        
        <div className="flex justify-between items-center text-xs">
          <div className="flex items-center gap-1.5">
            <span className="h-2 w-2 rounded-full bg-[#D97706] animate-pulse" />
            <span className={`font-serif font-bold ${isDark ? 'text-amber-200' : 'text-[#1C1917]'}`}>Active Verse Workspace</span>
          </div>
          <button
            type="button"
            onClick={() => setWorkflowMode?.('input')}
            className="text-xs font-semibold text-[#D97706] hover:text-[#B45309] transition-all cursor-pointer flex items-center gap-1"
          >
            <span>Edit Input &amp; Re-run</span>
          </button>
        </div>

        {/* Badges for parameters */}
        <div className="flex flex-wrap gap-1.5 text-[10px] font-mono">
          <span className={`px-2 py-0.5 rounded border ${isDark ? 'bg-[#201F25] text-[#DFDCE6] border-[#2C2932]' : 'bg-white text-[#57534E] border-[#E6E2D3]'}`}>
            Source: <strong className="font-semibold text-amber-600">{sourceLang === 'auto' ? 'AUTO-DETECT' : sourceLang.toUpperCase()}</strong>
          </span>
          <span className={`px-2 py-0.5 rounded border ${isDark ? 'bg-[#201F25] text-[#DFDCE6] border-[#2C2932]' : 'bg-white text-[#57534E] border-[#E6E2D3]'}`}>
            Target: <strong className="font-semibold text-amber-600">{targetLang.toUpperCase()}</strong>
          </span>
          {scriptureContext.trim() && (
            <span className={`px-2 py-0.5 rounded border ${isDark ? 'bg-[#201F25] text-[#DFDCE6] border-[#2C2932]' : 'bg-white text-[#57534E] border-[#E6E2D3]'}`}>
              Context: <strong className="font-semibold text-amber-600">{scriptureContext}</strong>
            </span>
          )}
        </div>

        {/* Input text display panel */}
        <div className={`p-3 rounded-lg border text-sm md:text-base font-serif leading-relaxed font-semibold italic text-center whitespace-pre-wrap ${
          isDark ? 'bg-[#1F1D24] text-[#F1EFF7] border-[#2C2932]' : 'bg-[#FFFDF9] text-[#1C1917] border-[#E6E2D3]'
        }`}>
          {text || <span className="text-gray-400 font-normal">No text loaded. Please click editable view to insert a verse.</span>}
        </div>

      </div>
    );
  }

  return (
    <div className={`p-5 rounded-2xl border flex flex-col space-y-4 transition-all duration-300 ${isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'}`}>
      
      <div className={`flex justify-between items-center border-b pb-3 ${isDark ? 'border-[#2C2932]' : 'border-[#E6E2D3]'}`}>
        <div className="flex items-center gap-2">
          <span className="h-2 w-2 rounded-full bg-[#D97706]" />
          <h3 className={`font-serif font-bold ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>Translation Workbench</h3>
        </div>
        <span className={`text-[11px] font-mono px-2 py-1 rounded border transition-colors ${isDark ? 'bg-[#201F25] text-[#9B98A3] border-[#2C2932]' : 'bg-white text-[#57534E] border-[#E6E2D3]'}`}>
          {text.trim().length} characters
        </span>
      </div>

      {/* Language Routing & Context parameters (3:3:4 ratio in 1 row) */}
      <div className="grid grid-cols-1 md:grid-cols-10 gap-3">
        
        {/* Source Select (3) */}
        <div className="flex flex-col space-y-1 md:col-span-3">
          <label className={`text-[10px] font-mono uppercase tracking-wider font-semibold ${isDark ? 'text-[#A19E95]' : 'text-[#78716C]'}`}>Source Language</label>
          <select
            value={sourceLang}
            onChange={(e: any) => setSourceLang(e.target.value)}
            className={`border rounded-xl px-3 py-2 text-xs font-medium focus:outline-none focus:border-[#D97706] shadow-sm transition-colors w-full ${
              isDark ? 'bg-[#201F25] border-[#2C2932] text-[#DFDCE6] [&>option]:bg-[#16151A] [&>option]:text-[#DFDCE6]' : 'bg-white border-[#E6E2D3] text-[#2D241E]'
            }`}
          >
            <option value="sanskrit">Sanskrit (संस्कृतम्)</option>
            <option value="hindi">Hindi (हिन्दी)</option>
            <option value="english">English (Anglais)</option>
            <option value="auto">Auto-Detect Script</option>
          </select>
        </div>

        {/* Target Select (3) */}
        <div className="flex flex-col space-y-1 md:col-span-3">
          <label className={`text-[10px] font-mono uppercase tracking-wider font-semibold ${isDark ? 'text-[#A19E95]' : 'text-[#78716C]'}`}>Target Language</label>
          <select
            value={targetLang}
            onChange={(e: any) => setTargetLang(e.target.value)}
            className={`border rounded-xl px-3 py-2 text-xs font-medium focus:outline-none focus:border-[#D97706] shadow-sm transition-colors w-full ${
              isDark ? 'bg-[#201F25] border-[#2C2932] text-[#DFDCE6] [&>option]:bg-[#16151A] [&>option]:text-[#DFDCE6]' : 'bg-white border-[#E6E2D3] text-[#2D241E]'
            }`}
          >
            <option value="english">English (Prose & Verse)</option>
            <option value="hindi">Hindi (गद्य अनुवाद)</option>
            <option value="sanskrit">Sanskrit (Devanagari mapping)</option>
          </select>
        </div>

        {/* Context prompt (4) */}
        <div className="flex flex-col space-y-1 md:col-span-4">
          <div className="flex justify-between">
            <label className={`text-[10px] font-mono uppercase tracking-wider font-semibold ${isDark ? 'text-[#A19E95]' : 'text-[#78716C]'}`}>Scriptural Context (Optional)</label>
            <span className={`text-[9.5px] font-mono ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>Improves match</span>
          </div>
          <input
            type="text"
            placeholder="e.g. Bhagavad Gita, Yajurveda, Upanishads"
            value={scriptureContext}
            onChange={(e) => setScriptureContext(e.target.value)}
            className={`border rounded-xl px-3 py-[9px] text-xs font-serif placeholder:font-sans placeholder:text-[#A8A29E] shadow-sm focus:outline-none focus:border-[#D97706] transition-colors w-full ${
              isDark ? 'bg-[#201F25] border-[#2C2932] text-[#F1EFF7]' : 'bg-white border-[#E6E2D3] text-[#1C1917]'
            }`}
          />
        </div>

      </div>

      {/* Large text workspace */}
      <div className="flex flex-col space-y-1">
        <div className="flex justify-between items-center text-[10px]">
          <label className={`font-mono uppercase tracking-wider font-semibold ${isDark ? 'text-[#A19E95]' : 'text-[#78716C]'}`}>Verse Input (Devanagari or English Text)</label>
          <button 
            type="button"
            onClick={() => { setText(''); setScriptureContext(''); }}
            className="flex items-center gap-1 text-[10px] text-red-500 hover:text-red-400 transition-colors cursor-pointer font-semibold"
            title="Clear content"
          >
            <Trash2 className="w-3 h-3" />
            <span>Clear</span>
          </button>
        </div>
        
        <textarea
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="Paste Sanskrit verse, Sloka, Mantra or commentary here (e.g. ॐ भूर् भुवः स्वः...)"
          rows={5}
          className={`w-full h-[130px] border rounded-xl p-4 font-serif placeholder:font-sans placeholder:text-[#A8A29E] leading-relaxed text-sm md:text-base focus:outline-none focus:border-[#D97706] resize-none shadow-sm transition-colors ${
            isDark ? 'bg-[#201F25] border-[#2C2932] text-[#F1EFF7]' : 'bg-white border-[#E6E2D3] text-[#1C1917]'
          }`}
        />
      </div>

      {/* API and Processing triggers */}
      <div className="flex flex-col gap-2 pt-2">
        
        {/* Advanced Indology Analysis Button */}
        <button
          type="button"
          onClick={onTriggerAnalysis}
          disabled={isLoading || !text.trim()}
          className="w-full flex items-center justify-center gap-2 bg-[#1C1917] hover:bg-[#2D241E] text-white font-bold tracking-wider uppercase py-3 rounded-xl transition-all duration-200 shadow-sm disabled:opacity-50 cursor-pointer text-xs"
        >
          {isLoading ? (
            <RefreshCw className="w-4 h-4 animate-spin text-white" />
          ) : (
            <Sparkles className="w-4 h-4 text-white fill-current" />
          )}
          <span>Translate & Analyze Verse</span>
        </button>

        {/* Direct Simple Translation / Transliteration alternate options */}
        <div className="grid grid-cols-2 gap-2">
          
          <button
            type="button"
            onClick={onTriggerSimpleTranslation}
            disabled={isLoading || !text.trim()}
            className={`py-2.5 px-3 border text-xs font-semibold flex items-center justify-center gap-1 transition-all shadow-sm cursor-pointer ${
              isDark ? 'bg-[#212026] border-[#2C2932] text-[#CBD5E1] hover:text-white hover:bg-[#2F2D38]' : 'bg-white border-[#E6E2D3] text-[#57534E] hover:text-[#D97706] hover:bg-[#F9F7F2]'
            }`}
          >
            <Globe className="w-3.5 h-3.5 text-[#D97706]" />
            <span>Direct Translation</span>
          </button>

          <button
            type="button"
            onClick={() => onTriggerPureTransliteration('iast')}
            disabled={isLoading || !text.trim()}
            className={`py-2.5 px-3 border text-xs font-semibold flex items-center justify-center gap-1 transition-all shadow-sm cursor-pointer ${
              isDark ? 'bg-[#212026] border-[#2C2932] text-[#CBD5E1] hover:text-white hover:bg-[#2F2D38]' : 'bg-white border-[#E6E2D3] text-[#57534E] hover:text-[#D97706] hover:bg-[#F9F7F2]'
            }`}
          >
            <FileCode className="w-3.5 h-3.5 text-[#D97706]" />
            <span>Build diacritics (IAST)</span>
          </button>

        </div>

      </div>

    </div>
  );
}
