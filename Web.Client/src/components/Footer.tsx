import React from 'react';
import { BookOpen } from 'lucide-react';

interface FooterProps {
  onTabChange: (tab: 'translation' | 'grammar' | 'transliteration' | 'api' | 'dictionary') => void;
  isDark?: boolean;
}

export default function Footer({ onTabChange, isDark = false }: FooterProps) {
  return (
    <footer className={`transition-all duration-300 py-10 px-4 border-t ${
      isDark 
        ? 'bg-[#16151A] border-[#2C2932]' 
        : 'bg-[#F9F7F2] border-[#E6E2D3]'
    }`}>
      <div className={`max-w-full lg:max-w-[96%] mx-auto grid grid-cols-1 md:grid-cols-3 gap-8 text-xs ${isDark ? 'text-[#9B98A3]' : 'text-[#57534E]'}`}>
        
        <div className="space-y-3">
          <div className="flex items-center gap-2 text-[#D97706]">
            <BookOpen className="w-4 h-4" />
            <span className={`font-serif font-bold tracking-wider uppercase h-4 ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>Digital Sanatan Archives</span>
          </div>
          <p className="leading-relaxed font-serif italic">
            "तस्मादसक्तः सततं कार्यं कर्म समाचर।" • Therefore, without attachment, constantly perform the action which is duty.
          </p>
          <p className="leading-relaxed">
            Vedic translators split classical Sandhi rules of Sanskrit grammar phonetically. This workspace is strictly aligned with classical commentaries from Shankaracharya, Ramanujacharya, and contemporary Indological research.
          </p>
        </div>

        <div className="space-y-2">
          <h5 className={`font-serif font-bold tracking-wider uppercase ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>Active REST Endpoints</h5>
          <ul className={`space-y-1.5 font-mono text-[11px] ${isDark ? 'text-[#DFDCE6]' : 'text-[#2D241E]'}`}>
            <li className="flex gap-2 items-center">
              <span className="px-1.5 py-0.5 bg-green-500/10 text-green-400 text-[9px] font-bold rounded border border-green-500/20">POST</span>
              <span>/api/translate</span>
            </li>
            <li className="flex gap-2 items-center">
              <span className="px-1.5 py-0.5 bg-green-500/10 text-green-400 text-[9px] font-bold rounded border border-green-500/20">POST</span>
              <span>/api/transliterate</span>
            </li>
            <li className="flex gap-2 items-center">
              <span className="px-1.5 py-0.5 bg-green-500/10 text-green-400 text-[9px] font-bold rounded border border-green-500/20">POST</span>
              <span>/api/analyze</span>
            </li>
            <li className="flex gap-2 items-center">
              <span className="px-1.5 py-0.5 bg-blue-500/10 text-blue-400 text-[9px] font-bold rounded border border-blue-500/20">GET</span>
              <span>/api/scriptures</span>
            </li>
            <li className="flex gap-2 items-center">
              <span className="px-1.5 py-0.5 bg-blue-500/10 text-blue-400 text-[9px] font-bold rounded border border-blue-500/20">GET</span>
              <span>/api/dictionary</span>
            </li>
          </ul>
        </div>

        <div className="space-y-2">
          <h5 className={`font-mono uppercase tracking-widest text-[10px] font-bold ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>API Specifications & Response</h5>
          <p className="leading-relaxed font-sans">
            Access scripture translation data in robust JSON formats. Build apps, Slackbots, or websites by utilizing the unified, cross-origin supported Express API system.
          </p>
          <div className="pt-2">
            <span className="text-[10px] bg-[#D97706]/10 border border-[#D97706]/20 text-[#D97706] px-2.5 py-1 rounded-md font-mono inline-block font-bold">
              Bearer Token Authorized
            </span>
          </div>
        </div>

      </div>

      <div className={`max-w-full lg:max-w-[96%] mx-auto border-t mt-8 pt-6 flex flex-col md:flex-row justify-between items-center text-[11px] text-[#78716C] gap-4 ${
        isDark ? 'border-[#2C2932]' : 'border-[#E6E2D3]'
      }`}>
        <p>© 2026 Scriptural Translators Hub. Protected under Apache-2.0 License.</p>
        <div className="flex gap-4">
          <a href="#dev-api-playground" onClick={() => onTabChange('api')} className="hover:text-[#D97706] transition-colors font-semibold">API References</a>
          <span>•</span>
          <a href="#popular-scriptures-bar" className="hover:text-[#D97706] transition-colors font-semibold">Sanctified Presets</a>
          <span>•</span>
          <a href="#app-header-section" className="hover:text-[#D97706] transition-colors font-semibold">Security Details</a>
        </div>
      </div>
    </footer>
  );
}
