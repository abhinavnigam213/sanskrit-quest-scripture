import React from 'react';
import { Sparkles, BookOpen, Compass, ShieldCheck, Heart, Cpu, ArrowRight } from 'lucide-react';

interface AboutViewProps {
  isDark?: boolean;
}

export default function AboutView({ isDark = false }: AboutViewProps) {
  return (
    <div className="space-y-8 py-4">
      {/* 1. Hero / Portal Introduction Panel (Adapted from former Preset Scripture Page) */}
      <div className={`p-6 md:p-8 rounded-3xl border relative overflow-hidden transition-all duration-300 shadow-sm ${
        isDark 
          ? 'bg-gradient-to-br from-[#1E1B24] to-[#121115] border-[#2C2932]' 
          : 'bg-gradient-to-br from-[#FCFAF5] to-[#F3EEE0] border-[#E6E2D3]'
      }`}>
        <div className="absolute right-0 top-0 -mr-6 -mt-6 w-36 h-36 bg-amber-500/10 rounded-full blur-3xl animate-pulse" />
        <div className="relative z-10 max-w-4xl space-y-4">
          <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-[10px] font-mono font-bold tracking-wider uppercase bg-[#D97706]/10 text-[#D97706] border border-[#D97706]/20">
            <Sparkles className="w-3 h-3 fill-current animate-pulse" />
            Sanskrit Quest Divine Hub
          </span>
          <h1 className={`font-serif text-2xl md:text-3.5xl font-black tracking-tight leading-snug ${isDark ? 'text-amber-100' : 'text-[#1C1917]'}`}>
            Classical Sanatana Dharma &amp; Sanskrit Scriptures Portal
          </h1>
          <p className={`text-xs md:text-sm leading-relaxed max-w-3xl ${isDark ? 'text-[#CBD5E1]' : 'text-[#57534E]'}`}>
            Explore, transliterate, and perform advanced grammatical padapatha deconstructions of sacred Vedic formulations. Choose from the preset collection or use our modern Scripture Translator module for real-time sandhi break analysis.
          </p>
        </div>
      </div>

      {/* 2. Core Philosophy Section */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-stretch">
        <div className={`lg:col-span-7 p-6 md:p-8 rounded-2.5xl border flex flex-col justify-between space-y-6 ${
          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#FFFDF9] border-[#E6E2D3]'
        }`}>
          <div className="space-y-4">
            <div className="flex items-center gap-2">
              <span className="text-[#D97706] font-serif text-2xl">ॐ</span>
              <h3 className={`font-serif text-lg font-bold tracking-tight ${isDark ? 'text-amber-50' : 'text-[#2D241E]'}`}>
                The Divine Echo of Sanatana Dharma
              </h3>
            </div>
            <p className={`text-xs md:text-sm leading-relaxed text-justify ${isDark ? 'text-[#A19E95]' : 'text-[#57534E]'}`}>
              Sanatana Dharma refers to the eternal law, natural order, and cosmic truth that is structurally free of beginning or end. Sanskrit, often deemed <em>Devabhāṣā</em> (the tongue of the Gods), is much more than a human language. It is a highly mathematical, phonetically precise, and vibrationally vibrant audio system designed to capture the structural frequencies of the cosmos.
            </p>
            <p className={`text-xs md:text-sm leading-relaxed text-justify ${isDark ? 'text-[#A19E95]' : 'text-[#57534E]'}`}>
              When mantras are chanted with authentic pronunciation, they create precise sound wave configurations that harmonize both the mind of the practitioner and the surrounding ecology. Sanskrit Quest serves as a bridge, enabling modern scholars and curious seekers to decrypt the deep grammar (<em>Vyākaraṇa</em>) and spiritual translations of these scriptures.
            </p>
          </div>
          
          <div className={`p-4 rounded-xl border flex items-start gap-3.5 ${
            isDark ? 'bg-[#211F26] border-[#312E3B]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
          }`}>
            <Heart className="w-5 h-5 text-[#D97706] shrink-0 mt-0.5" />
            <p className={`text-[11px] md:text-xs leading-relaxed italic ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
              "The mantras of the Vedas and Bhagavad Gita are not historical opinions; they are eternal laws of consciousness recorded through purified hearing (Śruti)."
            </p>
          </div>
        </div>

        {/* Bento Card Highlight */}
        <div className={`lg:col-span-5 p-6 md:p-8 rounded-2.5xl border flex flex-col justify-between relative overflow-hidden ${
          isDark 
            ? 'bg-gradient-to-br from-[#1E1C22]/80 to-[#151418] border-[#2C2932]' 
            : 'bg-gradient-to-br from-[#FCFAF6] to-[#F1EDE2] border-[#E6E2D3]'
        }`}>
          <div className="space-y-4">
            <span className="text-[10px] font-mono tracking-widest font-black text-[#D97706] bg-[#D97706]/10 px-2 py-0.5 rounded border border-[#D97706]/20 inline-block uppercase">
              Sanskrit Quest Capabilities
            </span>
            <h3 className={`font-serif text-lg font-bold ${isDark ? 'text-amber-100' : 'text-[#1C1917]'}`}>
              Bridging Ancient Sounds & Modern AI
            </h3>
            <p className={`text-xs leading-relaxed ${isDark ? 'text-[#9B98A3]' : 'text-[#57534E]'}`}>
              Sanskrit Quest combines centuries-old rule engines with the latest semantic capabilities of Gemini models to support:
            </p>
            <ul className="space-y-2.5 text-xs">
              <li className="flex items-start gap-2">
                <span className="text-[#D97706]">•</span>
                <span className={`${isDark ? 'text-[#CBD5E1]' : 'text-[#2D241E]'}`}>
                  <strong>Advanced Sandhi Deconstruction (Padapatha):</strong> Break complex compiled Sanskrit phrases back into underlying components.
                </span>
              </li>
              <li className="flex items-start gap-2">
                <span className="text-[#D97706]">•</span>
                <span className={`${isDark ? 'text-[#CBD5E1]' : 'text-[#2D241E]'}`}>
                  <strong>Syllabic Poetic Metrics:</strong> Display identified poetic structures (like Anustubh, Gayatri, or Tristubh meters).
                </span>
              </li>
              <li className="flex items-start gap-2">
                <span className="text-[#D97706]">•</span>
                <span className={`${isDark ? 'text-[#CBD5E1]' : 'text-[#2D241E]'}`}>
                  <strong>Multi-Script Matrix Output:</strong> Read instantly in Devanagari, IAST, ITRANS, and phonetic styles simultaneously.
                </span>
              </li>
            </ul>
          </div>

          <div className="pt-4 border-t border-dashed border-[#E6E2D3] dark:border-[#2C2932] mt-4 flex items-center justify-between">
            <span className={`text-[10px] font-mono ${isDark ? 'text-gray-500' : 'text-gray-400'}`}>Est. 2026 Portal</span>
            <span className="flex items-center gap-1.5 text-xs text-[#D97706] font-bold">
              Explore Active Engine
              <ArrowRight className="w-3.5 h-3.5" />
            </span>
          </div>
        </div>
      </div>

      {/* 3. Deep Dive Technical Capability Cards */}
      <div className="space-y-4">
        <div>
          <span className="text-[10px] font-mono font-bold text-[#D97706] tracking-widest uppercase block mb-1">Architecture</span>
          <h3 className={`font-serif text-lg font-bold ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>Interactive Platform Workflows</h3>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {/* Card 1: Sacred Word Parsing */}
          <div className={`p-5 rounded-2xl border flex flex-col justify-between ${
            isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#FFFDF9] border-[#E6E2D3]'
          }`}>
            <div className="space-y-3">
              <div className="p-2.5 rounded-lg bg-orange-500/10 text-[#D97706] w-fit">
                <BookOpen className="w-5 h-5" />
              </div>
              <h4 className={`font-serif text-sm font-bold ${isDark ? 'text-amber-50' : 'text-[#1C1917]'}`}>
                Theological Lexicography
              </h4>
              <p className={`text-[11px] md:text-xs leading-relaxed ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
                Traditional translators often struggle to locate words because of Sandhi modifications. Our Sacred Lexicon translates core compound components and queries roots directly against theological dictionaries offline, providing prompt etymological clarity.
              </p>
            </div>
          </div>

          {/* Card 2: Offline Fallback Guard */}
          <div className={`p-5 rounded-2xl border flex flex-col justify-between ${
            isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#FFFDF9] border-[#E6E2D3]'
          }`}>
            <div className="space-y-3">
              <div className="p-2.5 rounded-lg bg-emerald-500/10 text-emerald-600 dark:text-emerald-500 w-fit">
                <ShieldCheck className="w-5 h-5" />
              </div>
              <h4 className={`font-serif text-sm font-bold ${isDark ? 'text-amber-50' : 'text-[#1C1917]'}`}>
                Scholarly Hybrid Engine
              </h4>
              <p className={`text-[11px] md:text-xs leading-relaxed ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
                The platform relies on a dual-engine protocol. If external API rates are exceeded, a prebuilt rule-compiler runs completely locally inside your browser. It instantly calculates exact sandhi splits and outputs diacritic arrays without a single server connection.
              </p>
            </div>
          </div>

          {/* Card 3: Deep Context Synthesis */}
          <div className={`p-5 rounded-2xl border flex flex-col justify-between ${
            isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#FFFDF9] border-[#E6E2D3]'
          }`}>
            <div className="space-y-3">
              <div className="p-2.5 rounded-lg bg-amber-500/10 text-amber-600 dark:text-amber-500 w-fit">
                <Cpu className="w-5 h-5" />
              </div>
              <h4 className={`font-serif text-sm font-bold ${isDark ? 'text-amber-50' : 'text-[#1C1917]'}`}>
                Gemini Vedic Contextualizer
              </h4>
              <p className={`text-[11px] md:text-xs leading-relaxed ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
                Traditional translation scripts convert Sanskrit letters literally but miss spiritual metaphors. By infusing the Gemini AI module with scriptures-specific context, our engine interprets verses according to classical school paradigms (such as Advaita, Visishtadvaita, or Vedic rituals).
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* 4. Sanskrit Learning Tips / Tutorial block */}
      <div className={`p-6 rounded-2.5xl border ${
        isDark ? 'bg-[#1D1C22]/60 border-[#2D2A35]' : 'bg-[#F9F8F5] border-[#E3DFD2]'
      }`}>
        <h4 className={`font-serif text-sm font-bold mb-2.5 flex items-center gap-1.5 ${isDark ? 'text-amber-100' : 'text-[#1C1917]'}`}>
          <Compass className="w-4 h-4 text-[#D97706]" />
          Seeking Wisdom: How to Get Started
        </h4>
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4 text-[11px] md:text-xs leading-relaxed">
          <div className="space-y-1">
            <span className="font-mono text-[#D97706] font-extrabold block">01 / CHOOSE PRESETS</span>
            <p className={`${isDark ? 'text-[#A19E95]' : 'text-[#57534E]'}`}>Start with <strong>Sanctified Presets</strong> to explore curated verses from Lord Krishna, the Upanishads, and original Vedas.</p>
          </div>
          <div className="space-y-1">
            <span className="font-mono text-[#D97706] font-extrabold block">02 / DECONSTRUCT WORDS</span>
            <p className={`${isDark ? 'text-[#A19E95]' : 'text-[#57534E]'}`}>Activate the <strong>Padapatha</strong> view inside results to read exact breakdowns of root nouns and ancient verb formats.</p>
          </div>
          <div className="space-y-1">
            <span className="font-mono text-[#D97706] font-extrabold block">03 / COMPARE SCRIPTS</span>
            <p className={`${isDark ? 'text-[#A19E95]' : 'text-[#57534E]'}`}>Switch between <strong>ITRANS, IAST, and Phonetics</strong> to practice proper Vedic chanting accent patterns easily.</p>
          </div>
          <div className="space-y-1">
            <span className="font-mono text-[#D97706] font-extrabold block">04 / DICTIONARY SEARCH</span>
            <p className={`${isDark ? 'text-[#A19E95]' : 'text-[#57534E]'}`}>Use our etymological lookup to see how roots like <em>vid-</em> (to know) branch out into words like <em>Veda</em>.</p>
          </div>
        </div>
      </div>
    </div>
  );
}
