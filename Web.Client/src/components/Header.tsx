import React from 'react';
import { Compass } from 'lucide-react';
import SanskritQuestLogo from './SanskritQuestLogo';

interface HeaderProps {
  isDark?: boolean;
}

export default function Header({ isDark = false }: HeaderProps) {
  return (
    <header 
      id="app-header-section" 
      className={`relative p-5 rounded-2xl border flex flex-col md:flex-row justify-between items-start md:items-center gap-4 overflow-hidden shadow-sm transition-all duration-300 ${
        isDark 
          ? 'bg-[#16151A] border-[#2C2932]' 
          : 'bg-[#F9F7F2] border-[#E6E2D3]'
      }`}
    >
      
      <div className="flex items-center gap-4">
        <div className="flex items-center justify-center">
          <SanskritQuestLogo className="w-24 h-24" isDark={isDark} />
        </div>
        <div>
          <div className="flex items-center gap-2">
            <span className="font-mono text-[#D97706] tracking-widest text-[10px] font-bold uppercase bg-[#D97706]/10 px-2 py-0.5 rounded border border-[#D97706]/20 shadow-sm">
              Sanskrit Quest
            </span>
            <span className={`text-[10px] font-mono px-1.5 py-0.5 rounded border ${
              isDark 
                ? 'bg-[#212026] text-[#A19E95] border-[#2C2932]' 
                : 'bg-[#E6E2D3] text-[#57534E] border-[#E6E2D3]'
            }`}>
              v1.2 Full-Stack
            </span>
          </div>
          <h1 className={`font-serif text-xl md:text-2xl font-bold tracking-tight mt-1 transition-colors ${
            isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'
          }`}>
            Scripture <span className="font-sans font-normal text-[#D97706]">Translator</span> & Transliteration
          </h1>
          <p className={`font-serif text-xs sm:text-sm mt-1.5 font-semibold italic tracking-wide transition-colors leading-relaxed ${
            isDark ? 'text-[#F5E6C4]' : 'text-[#452711]'
          }`}>
            "ॐ असतो मा सद्गमय। तमसो मा ज्योतिर्गमय।" — Lead us from darkness to light.
          </p>
        </div>
      </div>

      <div className={`flex flex-col items-end gap-1 text-right self-stretch md:self-auto p-3 rounded-xl border transition-all duration-300 ${
        isDark 
          ? 'bg-[#1D1C22] border-[#2C2932]' 
          : 'bg-[#F9F7F2] border-[#E6E2D3]'
      }`}>
        <div className="flex items-center gap-1 text-xs">
          <Compass className="w-4 h-4 text-[#D97706] animate-pulse" />
          <span className={`font-semibold ${isDark ? 'text-[#E5E3DB]' : 'text-[#1C1917]'}`}>Sanskrit • Hindi • English</span>
        </div>
        <p className={`text-[10px] mt-1 leading-relaxed ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
          Vedic parsing engine powered by Gemini AI model with word-by-word sandhi logic.
        </p>
      </div>
    </header>
  );
}
