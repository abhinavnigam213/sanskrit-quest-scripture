import React from 'react';
import { motion } from 'motion/react';
import { Sparkles, Copy, Check } from 'lucide-react';
import { ScriptureAnalyzeResponse } from '../types.ts';

interface TranslationTabProps {
  analysisResult: ScriptureAnalyzeResponse | null;
  leftCardTitle: string;
  leftCardContent: string;
  leftCardCopyKey: string;
  rightCardTitle: string;
  rightCardContent: string;
  rightCardCopyKey: string;
  copiedText: string | null;
  onCopy: (content: string, key: string) => void;
  isDark?: boolean;
}

export default function TranslationTab({
  analysisResult,
  leftCardTitle,
  leftCardContent,
  leftCardCopyKey,
  rightCardTitle,
  rightCardContent,
  rightCardCopyKey,
  copiedText,
  onCopy,
  isDark = false,
}: TranslationTabProps) {
  return (
    <motion.div
      key="tab-translation"
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -10 }}
      className="space-y-5 flex-1 flex flex-col"
    >
      
      {/* Verse Identification header */}
      <div className={`flex justify-between items-center p-4 rounded-xl border transition-all duration-300 ${
        isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
      }`}>
        <div>
          <span className="text-[10px] uppercase font-mono tracking-wider text-[#D97706] font-bold animate-pulse">Identified Liturgy Resource</span>
          <h4 className={`font-serif text-base font-bold mt-1 ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>{analysisResult?.identifiedSource || "Custom Sanskrit Input"}</h4>
        </div>
        {analysisResult?.poeticMeter && (
          <div className="text-right">
            <span className={`text-[10px] uppercase font-mono tracking-wider font-bold block ${isDark ? 'text-[#A19E95]' : 'text-[#78716C]'}`}>Chhandas (Poetic Meter)</span>
            <span className={`font-serif text-sm mt-1 font-medium italic block ${isDark ? 'text-[#FA9524]' : 'text-[#2D241E]'}`}>{analysisResult?.poeticMeter}</span>
          </div>
        )}
      </div>

       {/* Translations stack */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        
        {/* Left Dynamic Translation/Source Card */}
        <div className={`p-6 border rounded-xl flex flex-col relative transition-all duration-300 ${
          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
        }`}>
          <div className={`flex justify-between items-center text-[10px] font-mono tracking-wider mb-2 uppercase border-b pb-1.5 font-bold ${
            isDark ? 'text-[#A19E95] border-[#2C2932]' : 'text-[#78716C] border-[#E6E2D3]'
          }`}>
            <span>{leftCardTitle}</span>
            <button 
              type="button"
              onClick={() => onCopy(leftCardContent, leftCardCopyKey)}
              className="text-[#D97706] hover:text-[#B45309] cursor-pointer"
            >
              {copiedText === leftCardCopyKey ? <Check className="w-3 h-3 text-green-600 font-bold" /> : <Copy className="w-3 h-3" />}
            </button>
          </div>
          <p className={`text-sm font-serif leading-relaxed italic mt-2 ${isDark ? 'text-[#CBD5E1]' : 'text-[#1C1917]'}`}>
            "{leftCardContent || "Translation not started. Run 'Translate' module to generate values."}"
          </p>
        </div>

        {/* Right Dynamic Translation Card */}
        <div className={`p-6 border rounded-xl flex flex-col relative font-serif transition-all duration-300 ${
          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
        }`}>
          <div className={`flex justify-between items-center text-[10px] font-mono tracking-wider mb-2 uppercase border-b pb-1.5 font-bold ${
            isDark ? 'text-[#A19E95] border-[#2C2932]' : 'text-[#78716C] border-[#E6E2D3]'
          }`}>
            <span>{rightCardTitle}</span>
            <button 
              type="button"
              onClick={() => onCopy(rightCardContent, rightCardCopyKey)}
              className="text-[#D97706] hover:text-[#B45309] cursor-pointer"
            >
              {copiedText === rightCardCopyKey ? <Check className="w-3 h-3 text-green-600 font-bold" /> : <Copy className="w-3 h-3" />}
            </button>
          </div>
          <p className={`text-sm leading-relaxed italic mt-2 ${isDark ? 'text-[#CBD5E1]' : 'text-[#1C1917]'}`}>
            "{rightCardContent || "Translation not started. Run 'Translate' module to generate values."}"
          </p>
        </div>

      </div>

      {/* Deep theological explanation */}
      <div className={`p-5 rounded-2xl border space-y-4 shadow-none transition-all duration-300 ${
        isDark ? 'bg-[#111015] border-[#2C2932]' : 'bg-[#FDFBF7] border-[#E6E2D3]'
      }`}>
        <div className="flex items-center gap-1.5 text-[#D97706]">
          <Sparkles className="w-4 h-4 fill-[#D97706]" />
          <h4 className="font-serif text-sm font-bold tracking-wider uppercase">Spiritual Significance & Scholarly Commentary</h4>
        </div>
        <div className={`font-sans text-xs leading-relaxed space-y-2 ${isDark ? 'text-[#CBD5E1]' : 'text-[#44403C]'}`}>
          {analysisResult?.spiritualSignificance ? (
             analysisResult.spiritualSignificance.split('\n').map((para, i) => (
               <p key={`signif-${i}`}>{para}</p>
             ))
          ) : (
            <p className={`font-serif ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>Deep commentarial insights will appear here once translated via backend services.</p>
          )}
        </div>
      </div>

    </motion.div>
  );
}
