import React from 'react';
import { AlertCircle } from 'lucide-react';
import { ScriptureAnalyzeResponse } from '../types.ts';

interface VedicAlertsProps {
  showResults: boolean;
  analysisResult: ScriptureAnalyzeResponse | null;
  apiError: string | null;
  isDark: boolean;
}

export default function VedicAlerts({
  showResults,
  analysisResult,
  apiError,
  isDark
}: VedicAlertsProps) {
  return (
    <>
      {/* Local & Offline Vedic Scholar Engine announcement */}
      {showResults && !analysisResult?.isFallback && (
        <div 
          id="alert-vedic-active"
          className={`p-4 rounded-xl border flex items-start gap-3 text-xs leading-relaxed transition-all duration-300 ${
            isDark 
              ? 'bg-[#1D1C22]/80 border-[#2D2A35]/60 text-[#DFDCE6]' 
              : 'bg-[#FAF8F4] border-[#E8E4D7] text-[#2D241E]'
          }`}
        >
          <AlertCircle className="w-5 h-5 text-[#D97706] shrink-0 mt-0.5 animate-pulse" />
          <div className="flex-1">
            <div className="flex items-center gap-2 mb-1 flex-wrap">
              <span className="font-bold font-serif text-sm text-[#D97706]">🕉️ Offline Vedic Scholar Engine Active & Ready</span>
              <span className={`text-[9px] font-mono uppercase font-bold px-1.5 py-0.5 rounded ${
                isDark ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20' : 'bg-emerald-50 text-emerald-700 border border-emerald-200'
              }`}>
                Status: Fully Operational
              </span>
            </div>
            <p className={`text-xs leading-normal ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
              Classical Sanskrit archives, rule-based sandhi parsing, and scriptural transliterators are fully available locally. Multi-lingual translation requests will automatically leverage the Gemini AI model if online mode is actively selected.
            </p>
          </div>
        </div>
      )}

      {/* Secret Key Validation warning */}
      {apiError && apiError.includes("GEMINI_API_KEY") && (
        <div 
          id="alert-secrets-required"
          className="p-4 bg-red-50 dark:bg-red-950/20 border border-red-200 dark:border-red-500/30 rounded-xl text-red-800 dark:text-red-300 flex items-start gap-3 text-xs leading-relaxed"
        >
          <AlertCircle className="w-5 h-5 text-red-600 shrink-0 mt-0.5" />
          <div>
            <span className="font-bold block mb-1">Secrets Configuration Required</span>
            Your Sanskrit translation services are currently running in secure API proxy mode. To perform live translations via Gemini in Cloud Run, go to **Settings &gt; Secrets** panel in the AI Studio UI and enter your <code className="bg-red-500/10 dark:bg-red-500/20 px-1 py-0.5 rounded font-mono border border-red-500/10">GEMINI_API_KEY</code>.
          </div>
        </div>
      )}

      {/* Fallback Engine alert */}
      {showResults && analysisResult?.isFallback && (
        <div 
          id="alert-fallback-active"
          className="p-4 bg-[#FFFBEB] dark:bg-amber-950/10 border border-[#FDE68A] dark:border-amber-500/20 rounded-xl text-[#78350F] dark:text-amber-300 flex items-start gap-3 text-xs leading-relaxed shadow-sm block"
        >
          <AlertCircle className="w-5 h-5 text-[#D97706] shrink-0 mt-0.5 animate-pulse" />
          <div>
            <span className="font-bold block mb-1 font-serif text-sm">🕉️ Offline Scholar Engine Fallback Active</span>
            Live Gemini AI translation quota is currently resting. The application has automatically engaged its offline transliterator, rule-based sandhi analyzer, and preloaded Classical scripture corpus. You can still transliterate any text and translate popular matches entirely offline!
          </div>
        </div>
      )}
    </>
  );
}
