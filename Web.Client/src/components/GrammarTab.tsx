import React from 'react';
import { motion } from 'motion/react';
import { ScriptureAnalyzeResponse } from '../types.ts';

interface GrammarTabProps {
  analysisResult: ScriptureAnalyzeResponse | null;
  isDark?: boolean;
}

export default function GrammarTab({ analysisResult, isDark = false }: GrammarTabProps) {
  return (
    <motion.div
      key="tab-grammar"
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -10 }}
      className="space-y-4 flex-1 flex flex-col"
    >
      <div className={`p-4 rounded-xl border transition-colors ${
        isDark ? 'bg-[#1E1D23] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
      }`}>
        <span className="text-[10px] uppercase font-mono tracking-wider text-[#78716C] font-bold">Deconstructive Study</span>
        <h4 className={`font-serif text-base font-bold transition-colors ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>Padapatha (Sadhana Word Breaks)</h4>
        <p className={`text-xs mt-1 text-justify leading-relaxed transition-colors ${isDark ? 'text-[#9B98A3]' : 'text-[#57534E]'}`}>
          Sanskrit compounds multiple words together through complex phonetic shift rules called <strong>Sandhi</strong>. The grid below splits the compounds and details grammatical cases.
        </p>
      </div>

      <div className={`border rounded-xl overflow-hidden overflow-x-auto flex-1 transition-colors ${
        isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-white border-[#E6E2D3]'
      }`}>
        <table className="w-full text-left border-collapse text-xs">
          <thead>
            <tr className={`border-b font-mono text-[10px] font-bold transition-colors ${
              isDark ? 'bg-[#1E1D23] border-[#2C2932] text-[#9B98A3]' : 'bg-[#F9F7F2] border-[#E6E2D3] text-[#44403C]'
            }`}>
              <th className="p-3">Sanskrit Word / Root</th>
              <th className="p-3">Grammar & Case Conjugation</th>
              <th className="p-3">English Meanings</th>
              <th className="p-3">हिन्दी अनुवाद (Hindi)</th>
            </tr>
          </thead>
          <tbody className={`divide-y font-sans transition-colors ${isDark ? 'divide-[#2C2932]' : 'divide-[#E6E2D3]'}`}>
            {analysisResult?.wordBreakdown && analysisResult.wordBreakdown.length > 0 ? (
              analysisResult.wordBreakdown.map((row, i) => (
                <tr key={`word-breakdown-${row.word}-${row.grammar || ''}-${i}`} className={`transition-colors ${isDark ? 'hover:bg-[#1E1D23]' : 'hover:bg-[#FDFBF7]'}`}>
                  <td className="p-3 font-serif font-bold text-[#D97706]">{row.word}</td>
                  <td className={`p-3 font-mono text-[11px] ${isDark ? 'text-[#9B98A3]' : 'text-[#57534E]'}`}>{row.grammar || 'Noun / indeclinable'}</td>
                  <td className={`p-3 ${isDark ? 'text-[#DFDCE6]' : 'text-[#2D241E]'}`}>{row.meaningEnglish}</td>
                  <td className={`p-3 font-serif ${isDark ? 'text-[#DFDCE6]' : 'text-[#44403C]'}`}>{row.meaningHindi}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={4} className={`p-12 text-center font-serif ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
                  Word-by-word grammatical splits are available when translating. Press "Translate & Analyze" to generate database metrics.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </motion.div>
  );
}
