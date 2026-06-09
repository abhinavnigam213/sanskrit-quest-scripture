import React from 'react';
import { motion } from 'motion/react';
import { Copy, Check, HelpCircle } from 'lucide-react';

interface Transliterations {
  devanagari: string;
  iast: string;
  itrans: string;
  phonetic: string;
  slp1: string;
}

interface TransliterationTabProps {
  transliterations: Transliterations;
  copiedText: string | null;
  onCopy: (content: string, key: string) => void;
  onRegenerateMatrix: () => void;
  isDark?: boolean;
}

export default function TransliterationTab({
  transliterations,
  copiedText,
  onCopy,
  onRegenerateMatrix,
  isDark = false,
}: TransliterationTabProps) {
  return (
    <motion.div
      key="tab-transliteration"
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -10 }}
      className="space-y-4 flex-1 flex flex-col"
    >
      <div className={`p-4 rounded-xl border transition-all duration-300 ${
        isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
      }`}>
        <div className="flex justify-between items-center">
          <div>
            <span className="text-[10px] uppercase font-mono tracking-wider text-[#D97706] font-bold">Script Converter Matrix</span>
            <h4 className={`font-serif text-base font-bold ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>Orthographic Character Conversions</h4>
          </div>
          <div className="flex gap-2">
            <button
              type="button"
              onClick={onRegenerateMatrix}
              className={`border hover:text-[#D97706] px-3 py-1.5 text-[10px] font-mono rounded-lg transition-colors cursor-pointer ${
                isDark ? 'bg-[#201F25] border-[#2C2932] text-[#DFDCE6] hover:bg-[#2C2932]' : 'bg-white border-[#E6E2D3] text-[#57534E] hover:bg-[#F9F7F2]'
              }`}
            >
              Regenerate Matrix
            </button>
          </div>
        </div>
        <p className={`text-xs mt-1 text-justify leading-relaxed ${isDark ? 'text-[#9B98A3]' : 'text-[#57534E]'}`}>
          Indic transliteration maps original characters verbatim into other script standards so researchers can pronounce mantras perfectly without reading Devanagari.
        </p>
      </div>

      {/* Standardized Conversions Panel */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 flex-1">
        
        {/* DEVANAGARI CARD */}
        <div className={`border rounded-xl p-4 flex flex-col transition-all duration-300 ${
          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
        }`}>
          <div className={`flex justify-between items-center text-[10px] font-mono tracking-wider uppercase border-b pb-1.5 mb-2 font-bold ${
            isDark ? 'text-[#D97706] border-[#2C2932]' : 'text-[#D97706] border-[#E6E2D3]'
          }`}>
            <span>Devanagari (Traditional Sanskrit)</span>
            <button 
              type="button"
              onClick={() => onCopy(transliterations.devanagari, 'dev')}
              className="text-[#D97706] hover:text-[#B45309] cursor-pointer"
            >
              {copiedText === 'dev' ? <Check className="w-3.5 h-3.5 text-green-600 font-bold" /> : <Copy className="w-3.5 h-3.5" />}
            </button>
          </div>
          <p className={`flex-1 font-serif whitespace-pre-wrap text-sm leading-relaxed antialiased ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>
            {transliterations.devanagari}
          </p>
        </div>

        {/* IAST CARD */}
        <div className={`border rounded-xl p-4 flex flex-col transition-all duration-300 ${
          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
        }`}>
          <div className={`flex justify-between items-center text-[10px] font-mono tracking-wider uppercase border-b pb-1.5 mb-2 font-bold ${
            isDark ? 'text-[#CBD5E1] border-[#2C2932]' : 'text-[#1C1917] border-[#E6E2D3]'
          }`}>
            <span>IAST (Academic Diacritics)</span>
            <button 
              type="button"
              onClick={() => onCopy(transliterations.iast, 'ias')}
              className="text-[#D97706] hover:text-[#B45309] cursor-pointer"
            >
              {copiedText === 'ias' ? <Check className="w-3.5 h-3.5 text-green-600 font-bold" /> : <Copy className="w-3.5 h-3.5" />}
            </button>
          </div>
          <p className={`flex-1 font-serif whitespace-pre-wrap text-sm leading-relaxed italic tracking-wider ${isDark ? 'text-[#FA9524]' : 'text-[#2D241E]'}`}>
            {transliterations.iast}
          </p>
        </div>

        {/* ITRANS CARD */}
        <div className={`border rounded-xl p-4 flex flex-col transition-all duration-300 ${
          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
        }`}>
          <div className={`flex justify-between items-center text-[10px] font-mono tracking-wider uppercase border-b pb-1.5 mb-2 font-bold ${
            isDark ? 'text-[#9B98A3] border-[#2C2932]' : 'text-[#44403C] border-[#E6E2D3]'
          }`}>
            <span>ITRANS (Keyboard ASCII Syntax)</span>
            <button 
              type="button"
              onClick={() => onCopy(transliterations.itrans, 'itr')}
              className="text-[#D97706] hover:text-[#B45309] cursor-pointer"
            >
              {copiedText === 'itr' ? <Check className="w-3.5 h-3.5 text-green-600 font-bold" /> : <Copy className="w-3.5 h-3.5" />}
            </button>
          </div>
          <p className={`flex-1 font-mono whitespace-pre-wrap text-xs leading-relaxed ${isDark ? 'text-[#CBD5E1]' : 'text-[#57534E]'}`}>
            {transliterations.itrans}
          </p>
        </div>

        {/* CHANTING FRIENDLY CARD */}
        <div className={`border rounded-xl p-4 flex flex-col transition-all duration-300 ${
          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
        }`}>
          <div className={`flex justify-between items-center text-[10px] font-mono tracking-wider uppercase border-b pb-1.5 mb-2 font-bold ${
            isDark ? 'text-[#9B98A3] border-[#2C2932]' : 'text-[#57534E] border-[#E6E2D3]'
          }`}>
            <span>English Phonetic (Easy Chanting)</span>
            <button 
              type="button"
              onClick={() => onCopy(transliterations.phonetic, 'pho')}
              className="text-[#D97706] hover:text-[#B45309] cursor-pointer"
            >
              {copiedText === 'pho' ? <Check className="w-3.5 h-3.5 text-green-600 font-bold" /> : <Copy className="w-3.5 h-3.5" />}
            </button>
          </div>
          <p className={`flex-1 font-serif whitespace-pre-wrap text-xs leading-relaxed ${isDark ? 'text-[# CBD5E1]' : 'text-[#1C1917]'}`}>
            {transliterations.phonetic}
          </p>
        </div>

        {/* SLP1 CARD */}
        <div className={`border rounded-xl p-4 flex flex-col transition-all duration-300 ${
          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
        }`}>
          <div className={`flex justify-between items-center text-[10px] font-mono tracking-wider uppercase border-b pb-1.5 mb-2 font-bold ${
            isDark ? 'text-[#0F766E] border-[#2C2932]' : 'text-[#0F766E] border-[#E6E2D3]'
          }`}>
            <span>Sanskrit Library Phonetic (SLP1)</span>
            <button 
              type="button"
              onClick={() => onCopy(transliterations.slp1, 'slp')}
              className="text-[#D97706] hover:text-[#B45309] cursor-pointer"
            >
              {copiedText === 'slp' ? <Check className="w-3.5 h-3.5 text-green-600 font-bold" /> : <Copy className="w-3.5 h-3.5" />}
            </button>
          </div>
          <p className={`flex-1 font-mono whitespace-pre-wrap text-xs leading-relaxed tracking-wide font-medium ${isDark ? 'text-[#5EEAD4]' : 'text-[#115E59]'}`}>
            {transliterations.slp1}
          </p>
        </div>

      </div>

      {/* Quick helper about Sanskrit / Transliteration formats */}
      <div className={`p-4 border rounded-xl flex items-start gap-3 transition-colors mt-2 ${
        isDark ? 'bg-[#1D1C22] border-[#2C2932]' : 'bg-[#FDFBF7] border-[#E6E2D3]'
      }`}>
        <HelpCircle className="w-4 h-4 text-[#D97706] shrink-0 mt-0.5" />
        <div className={`text-xs space-y-1.5 ${isDark ? 'text-[#CBD5E1]' : 'text-[#57534E]'}`}>
          <span className="font-bold text-[#D97706] font-serif text-xs block">Understanding Transliteration Schemes:</span>
          <p><strong>Devanagari</strong> is the traditional divine script of India.</p>
          <p><strong>IAST</strong> (International Alphabet of Sanskrit Transliteration) is the international scientific standard utilizing diacritic markers (likeī, ṣ, ṁ) for perfect pronouncing metrics.</p>
          <p><strong>ITRANS</strong> is a standard English-keyboard layout translation representing phonetic tones in simple characters.</p>
          <p><strong>SLP1</strong> (Sanskrit Library Phonetic Basic Scheme) is an ASCII-only encoding mapping each distinctive Sanskrit speech sound to a single keyboard character.</p>
        </div>
      </div>

    </motion.div>
  );
}
