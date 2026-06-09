import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { 
  Sparkles, 
  Book, 
  Globe, 
  BookOpen, 
  Terminal, 
  Scroll, 
  Languages, 
  Compass
} from 'lucide-react';

import { Scripture, ScriptureAnalyzeResponse, TransliterateResponse } from './types.ts';
import { devanagariToSlp1 } from './utils/transliteration.ts';
import apiClient from './utils/apiClient.ts';

const DEFAULT_PRELOAD_SLOKA = {
  verse: "कर्मण्येवाधिकारस्ते मा फलेषु कदाचन।\nमा कर्मफलहेतुर्भूर्मा ते सङ्गोऽस्त्वकर्मणि॥",
  source: "Bhagavad Gita 2.47",
  transliterationDefault: "karmaṇy-evādhikāras te mā phaleṣu kadācana |\nmā karma-phala-hetur bhūr mā te saṅgo 'stv akarmaṇi ||",
  translationDefaultEnglish: "You have a right to perform your prescribed duty, but you are not entitled to the fruits of action. Never consider yourself the cause of the results of your activities, and never be attached to not doing your duty.",
  translationDefaultHindi: "कर्म करने में ही तुम्हारा अधिकार है, उसके फलों में कभी नहीं। तुम स्वयं को अपने कर्मों के फलों का कारण मत मानो, और न ही अकर्मण्यता (काम न करने) में तुम्हारी आसक्ति हो।"
};

// Child components
import PresetScriptures from './components/PresetScriptures.tsx';
import TranslationWorkbench from './components/TranslationWorkbench.tsx';
import TranslationTab from './components/TranslationTab.tsx';
import GrammarTab from './components/GrammarTab.tsx';
import TransliterationTab from './components/TransliterationTab.tsx';
import DictionaryTab from './components/DictionaryTab.tsx';
import ApiPlayground from './components/ApiPlayground.tsx';
import Footer from './components/Footer.tsx';
import AboutView from './components/AboutView.tsx';

// Refactored child components
import TopBar from './components/TopBar.tsx';
import SidebarNavigation from './components/SidebarNavigation.tsx';
import VedicAlerts from './components/VedicAlerts.tsx';

export default function App() {
  const [text, setText] = useState<string>('');
  const [sourceLang, setSourceLang] = useState<'sanskrit' | 'hindi' | 'english' | 'auto'>('sanskrit');
  const [targetLang, setTargetLang] = useState<'sanskrit' | 'hindi' | 'english'>('english');
  const [scriptureContext, setScriptureContext] = useState<string>('');
  
  // Tabs for the result workspace (when activeNav is 'translator')
  const [activeTab, setActiveTab ] = useState<'translation' | 'grammar' | 'transliteration' | 'api' | 'dictionary'>('translation');
  const [copiedText, setCopiedText] = useState<string | null>(null);

  // Loading and Error state
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [apiError, setApiError] = useState<string | null>(null);

  // Theme support: 'light' | 'dark' | 'system'
  const [themeMode, setThemeMode] = useState<'light' | 'dark' | 'system'>(() => {
    if (typeof window !== 'undefined') {
      const stored = localStorage.getItem('sanskrit-quest-theme');
      if (stored === 'light' || stored === 'dark' || stored === 'system') {
        return stored;
      }
    }
    return 'system';
  });
  const [isDark, setIsDark] = useState<boolean>(false);

  // Font Size support: 'small' | 'normal' | 'large' | 'xlarge'
  const [fontSize, setFontSize] = useState<'small' | 'normal' | 'large' | 'xlarge'>(() => {
    if (typeof window !== 'undefined') {
      const stored = localStorage.getItem('sanskrit-quest-fontsize-v3');
      if (stored === 'small' || stored === 'normal' || stored === 'large' || stored === 'xlarge') {
        return stored;
      }
    }
    return 'normal';
  });

  // Save changes to localStorage
  useEffect(() => {
    localStorage.setItem('sanskrit-quest-theme', themeMode);
  }, [themeMode]);

  useEffect(() => {
    localStorage.setItem('sanskrit-quest-fontsize-v3', fontSize);
  }, [fontSize]);

  const [showResults, setShowResults ] = useState<boolean>(false);
  const [workflowMode, setWorkflowMode ] = useState<'input' | 'readonly'>('input');

  // Left Menu navigation: 'home' | 'translator' | 'lexicon' | 'api' | 'about'
  const [activeNav, setActiveNav] = useState<'home' | 'translator' | 'lexicon' | 'api' | 'about'>('home');
  const [mobileMenuOpen, setMobileMenuOpen] = useState<boolean>(false);
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState<boolean>(false);

  const [popularScriptures, setPopularScriptures] = useState<Scripture[]>([]);

  // Fetch popular scriptures from API on startup
  useEffect(() => {
    let active = true;
    apiClient.get('/api/scriptures')
      .then(res => {
        if (active && Array.isArray(res.data)) {
          setPopularScriptures(res.data);
        }
      })
      .catch(err => {
        console.error("Failed to load preset scriptures from API:", err);
      });
    return () => { active = false; };
  }, []);

  // Hydrated full analysis/translation state (Preload Bhagavad Gita 2.47)
  const [analysisResult, setAnalysisResult] = useState<ScriptureAnalyzeResponse | null>({
    verse: DEFAULT_PRELOAD_SLOKA.verse,
    identifiedSource: DEFAULT_PRELOAD_SLOKA.source,
    transliterationIAST: DEFAULT_PRELOAD_SLOKA.transliterationDefault,
    transliterationPhonetic: "karma-ny-eva-dhi-ka-ras te ma pha-le-shu ka-da-cha-na | ma karma-pha-la-he-tur bhu-rma te san-go 'stv a-kar-ma-ni ||",
    translationEnglish: DEFAULT_PRELOAD_SLOKA.translationDefaultEnglish,
    translationHindi: DEFAULT_PRELOAD_SLOKA.translationDefaultHindi,
    poeticMeter: "Anustubh (8 syllables per quarter)",
    spiritualSignificance: "This is one of the most celebrated verses of the Bhagavad Gita expressing Lord Krishna's core teaching on Karma Yoga (the path of selfless work). It teaches four main tenets: (1) Do your duty without hesitation, (2) Detach yourself from the fruits/results of the activity, (3) Do not assume your ego is the primary creator of outputs, and (4) Avoid falling into nihilistic lethargy or choosing non-action. True action exists as worship itself.",
    wordBreakdown: [
      { word: "कर्मणि", grammar: "Locative Singular (in Karma/duty)", meaningHindi: "कर्म में", meaningEnglish: "in action / in duty" },
      { word: "एव", grammar: "Indeclinable (Only/Indeed)", meaningHindi: "ही", meaningEnglish: "only / indeed" },
      { word: "अधिकारः", grammar: "Nominative Singular (Right/Authority)", meaningHindi: "अधिकार", meaningEnglish: "right / authority" },
      { word: "ते", grammar: "Genitive Singular (Thy/Your)", meaningHindi: "तुम्हारा", meaningEnglish: "your" },
      { word: "मा", grammar: "Negative particle (Let not/Never)", meaningHindi: "मत", meaningEnglish: "let not" },
      { word: "फलेषु", grammar: "Locative Plural (in results/fruits)", meaningHindi: "फलों में", meaningEnglish: "in the fruits / results" },
      { word: "कदाचन", grammar: "Adverb particle (at any time/ever)", meaningHindi: "कभी भी", meaningEnglish: "at any time / ever" },
      { word: "मा", grammar: "Negative particle", meaningHindi: "मत", meaningEnglish: "not" },
      { word: "कर्मफलहेतुः", grammar: "Compounded Nominative (source of results)", meaningHindi: "कर्मफल का कारण", meaningEnglish: "cause of action fruits" },
      { word: "भूः", grammar: "Aorist Imperative (be)", meaningHindi: "बनो / बनिए", meaningEnglish: "become / be" },
      { word: "ते", grammar: "Genitive Singular (your)", meaningHindi: "तुम्हारी", meaningEnglish: "your" },
      { word: "सङ्गः", grammar: "Nominative Singular (attachment)", meaningHindi: "आसक्ति", meaningEnglish: "attachment" },
      { word: "अस्तु", grammar: "Imperative Singular (let there be)", meaningHindi: "होवे", meaningEnglish: "let there be" },
      { word: "अकर्मणि", grammar: "Locative Singular (in non-action)", meaningHindi: "अकर्म में", meaningEnglish: "in non-action" }
    ]
  });

  // Transliteration matrix state
  const [transliterations, setTransliterations] = useState<{
    devanagari: string;
    iast: string;
    itrans: string;
    phonetic: string;
    slp1: string;
  }>({
    devanagari: DEFAULT_PRELOAD_SLOKA.verse,
    iast: DEFAULT_PRELOAD_SLOKA.transliterationDefault,
    itrans: "karmanyevaadhikaaraste maa phaleshu kadaachana | maa karmaphalaheturbhuurmaa te sango'stvakarmani ||",
    phonetic: "karma-ny-eva-dhi-ka-ras te ma pha-le-shu ka-da-cha-na | ma karma-pha-la-he-tur bhu-rma te san-go 'stv a-kar-ma-ni ||",
    slp1: "karmaRyevADikAraste mA Palezu kadAcana |\nmA karmapalaheturbhUrmA te saNgo'stvakarmaRi ||"
  });

  // Theme auto-detection effect
  useEffect(() => {
    const handleThemeCalculation = () => {
      if (themeMode === 'dark') {
        setIsDark(true);
        document.documentElement.classList.add('dark');
      } else if (themeMode === 'light') {
        setIsDark(false);
        document.documentElement.classList.remove('dark');
      } else {
        const systemPreference = window.matchMedia('(prefers-color-scheme: dark)').matches;
        setIsDark(systemPreference);
        if (systemPreference) {
          document.documentElement.classList.add('dark');
        } else {
          document.documentElement.classList.remove('dark');
        }
      }
    };

    handleThemeCalculation();

    if (themeMode === 'system') {
      const matcher = window.matchMedia('(prefers-color-scheme: dark)');
      const listener = (e: MediaQueryListEvent) => {
        setIsDark(e.matches);
        if (e.matches) {
          document.documentElement.classList.add('dark');
        } else {
          document.documentElement.classList.remove('dark');
        }
      };
      matcher.addEventListener('change', listener);
      return () => matcher.removeEventListener('change', listener);
    }
  }, [themeMode]);

  // Select a preset scripture
  const selectScripture = (sc: Scripture) => {
    setText(sc.verse);
    setScriptureContext(sc.source);
    
    // Quick hydrate UI state with default values instantly, then trigger deep analyze
    setTransliterations({
      devanagari: sc.verse,
      iast: sc.transliterationDefault,
      itrans: "Loading scriptural variations...",
      phonetic: "Loading chant phonetics...",
      slp1: devanagariToSlp1(sc.verse)
    });

    setAnalysisResult({
      verse: sc.verse,
      identifiedSource: sc.source,
      transliterationIAST: sc.transliterationDefault,
      transliterationPhonetic: "Generating chant instructions...",
      translationEnglish: sc.translationDefaultEnglish,
      translationHindi: sc.translationDefaultHindi,
      spiritualSignificance: "Triggering Gemini AI to build deep structural, poetic, and spiritual translation panels...",
      wordBreakdown: []
    });

    // Auto trigger translation/analysis & route to translator page
    setActiveNav('translator');
    setWorkflowMode('readonly');
    triggerAnalysis(sc.verse, sc.source);
  };

  // Trigger Sanskrit deep analysis API
  const triggerAnalysis = async (inputText: string, ctx?: string) => {
    if (!inputText.trim()) return;
    setIsLoading(true);
    setApiError(null);
    setWorkflowMode('readonly');
    setShowResults(true);
    try {
      const response = await apiClient.post('/api/analyze', {
        text: inputText,
        sourceContext: ctx || scriptureContext
      });
      const data = response.data;

      setAnalysisResult(data);

      // Trigger standard transliterations in parallel or fetch from model results
      setTransliterations({
        devanagari: data.verse,
        iast: data.transliterationIAST,
        itrans: inputText.length < 50 ? `${inputText} (translating...)` : "Generating scriptural representation...",
        phonetic: data.transliterationPhonetic,
        slp1: devanagariToSlp1(data.verse || inputText)
      });

      // Also trigger a pure transliteration run for ITRANS specifically
      try {
        const translitRes = await apiClient.post('/api/transliterate', {
          text: data.verse || inputText,
          sourceScript: 'devanagari',
          targetScript: 'itrans'
        });
        const transData: TransliterateResponse = translitRes.data;
        setTransliterations(prev => ({
          ...prev,
          itrans: transData.transliteratedText
        }));
      } catch (translitErr) {
        console.warn("Parallel ITRANS transliteration failed:", translitErr);
      }

    } catch (err: any) {
      console.error(err);
      const errorMessage = err.response?.data?.error || err.response?.data?.message || err.message || "An unexpected network error occurred while communicating with the server.";
      setApiError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  // Quick translation (Sanskrit, Hindi, English direct mapping)
  const triggerSimpleTranslation = async () => {
    if (!text.trim()) return;
    setIsLoading(true);
    setApiError(null);
    setWorkflowMode('readonly');
    setShowResults(true);
    try {
      const response = await apiClient.post('/api/translate', {
        text: text,
        sourceLang: sourceLang,
        targetLang: targetLang,
        scriptureContext: scriptureContext
      });
      const data = response.data;

      // Convert standard TranslationResponse into ScriptureAnalyzeResponse structure, appending explanations
      setAnalysisResult({
        verse: text,
        identifiedSource: scriptureContext || "User Input Text",
        transliterationIAST: transliterations.iast || "Click 'Transliterate' tab to build phonetic representations",
        transliterationPhonetic: "Generated translation details below.",
        translationEnglish: targetLang === 'english' ? data.translatedText : (sourceLang === 'english' ? text : "Use English target selector"),
        translationHindi: targetLang === 'hindi' ? data.translatedText : (sourceLang === 'hindi' ? text : "Use Hindi target selector"),
        translationSanskrit: targetLang === 'sanskrit' ? data.translatedText : undefined,
        spiritualSignificance: data.explanation || "No additional theological significance supplied.",
        wordBreakdown: data.wordBreakdown || []
      });

    } catch (err: any) {
      console.error(err);
      const errorMessage = err.response?.data?.error || err.response?.data?.message || err.message || "An unexpected error occurred during direct translation.";
      setApiError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  // Pure transliteration run
  const triggerPureTransliteration = async (target: 'iast' | 'itrans' | 'english_phonetic' | 'devanagari' | 'slp1') => {
    if (!text.trim()) return;
    setIsLoading(true);
    setApiError(null);
    setWorkflowMode('readonly');
    setShowResults(true);
    try {
      if (target === 'slp1') {
        const localSlp = devanagariToSlp1(text);
        setTransliterations(prev => ({
          ...prev,
          slp1: localSlp
        }));
        setActiveTab('transliteration');
        setIsLoading(false);
        return;
      }

      const response = await apiClient.post('/api/transliterate', {
        text: text,
        sourceScript: sourceLang === 'sanskrit' ? 'devanagari' : 'devanagari',
        targetScript: target
      });
      const data: TransliterateResponse = response.data;

      setTransliterations(prev => ({
        ...prev,
        [target === 'english_phonetic' ? 'phonetic' : target]: data.transliteratedText
      }));

    } catch (err: any) {
      console.error(err);
      const errorMessage = err.response?.data?.error || err.response?.data?.message || err.message || "An error occurred during transliteration mapping.";
      setApiError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCopyText = (content: string, key: string) => {
    navigator.clipboard.writeText(content);
    copiedTextAnim(key);
  };

  const copiedTextAnim = (key: string) => {
    setCopiedText(key);
    setTimeout(() => setCopiedText(null), 2000);
  };

  const handleTranslateWord = (word: string, category: string) => {
    setText(word);
    setSourceLang('sanskrit');
    setScriptureContext(category);
    setActiveNav('translator');
    setActiveTab('translation');
    triggerAnalysis(word, category);
  };

  // Dynamically compute translation card content
  const isSanskritTarget = targetLang === 'sanskrit' || !!analysisResult?.translationSanskrit;

  let leftCardTitle = "English Prose Translation";
  let leftCardContent = analysisResult?.translationEnglish || "";
  let leftCardCopyKey = "eng";

  let rightCardTitle = "हिन्दी गद्यानुवाद (Hindi Translation)";
  let rightCardContent = analysisResult?.translationHindi || "";
  let rightCardCopyKey = "hin";

  if (isSanskritTarget) {
    if (sourceLang === 'english') {
      leftCardTitle = "English Source Input";
    } else if (sourceLang === 'hindi') {
      leftCardTitle = "Hindi Source Input";
    } else {
      leftCardTitle = "Original Scripture Source";
    }
    leftCardContent = text || analysisResult?.verse || "";
    leftCardCopyKey = "src";

    rightCardTitle = "Sanskrit Translation (संस्कृतम्)";
    rightCardContent = analysisResult?.translationSanskrit || "";
    rightCardCopyKey = "san";
  } else {
    // If either has a "Use target selector" instruction
    if (leftCardContent === "Use English target selector") {
      if (sourceLang === 'hindi') {
        leftCardTitle = "Hindi Source Input";
      } else if (sourceLang === 'sanskrit') {
        leftCardTitle = "Sanskrit Source Input";
      } else {
        leftCardTitle = "Original Verse Input";
      }
      leftCardContent = text || analysisResult?.verse || "";
      leftCardCopyKey = "src";
    }
    if (rightCardContent === "Use Hindi target selector") {
      if (sourceLang === 'english') {
        rightCardTitle = "English Source Input";
      } else if (sourceLang === 'sanskrit') {
        rightCardTitle = "Sanskrit Source Input";
      } else {
        rightCardTitle = "Original Verse Input";
      }
      rightCardContent = text || analysisResult?.verse || "";
      rightCardCopyKey = "src";
    }
  }

  return (
    <div className={`h-screen flex flex-col antialiased transition-colors duration-350 overflow-hidden selection:bg-[#D97706] selection:text-white f-size-${fontSize} ${
      isDark ? 'bg-[#0F0E11] text-[#DFDCE6]' : 'bg-[#FDFBF7] text-[#2D241E]'
    }`}>
      
      {/* 1. GLOBAL WEBPAGE HEADER */}
      <TopBar
        isDark={isDark}
        activeNav={activeNav}
        setActiveNav={setActiveNav}
        setMobileMenuOpen={setMobileMenuOpen}
        setIsSidebarCollapsed={setIsSidebarCollapsed}
        themeMode={themeMode}
        setThemeMode={setThemeMode}
        fontSize={fontSize}
        setFontSize={setFontSize}
      />

      {/* 2. BODY ROW LAYOUT CONTAINER */}
      <div className="flex-1 flex flex-col md:flex-row min-h-0 overflow-hidden relative">
        
        {/* SIDEBAR NAVIGATION (DESKTOP + MOBILE) */}
        <SidebarNavigation
          isSidebarCollapsed={isSidebarCollapsed}
          setIsSidebarCollapsed={setIsSidebarCollapsed}
          activeNav={activeNav}
          setActiveNav={setActiveNav}
          mobileMenuOpen={mobileMenuOpen}
          setMobileMenuOpen={setMobileMenuOpen}
          isDark={isDark}
          themeMode={themeMode}
          setThemeMode={setThemeMode}
          fontSize={fontSize}
          setFontSize={setFontSize}
          onTranslatorClick={() => {
            setWorkflowMode('input');
            setShowResults(false);
            setText('');
            setScriptureContext('');
            setSourceLang('sanskrit');
            setTargetLang('english');
          }}
        />

        {/* 3. MAIN WORKSPACE CONTENT WINDOW */}
        <main className="flex-1 flex flex-col min-h-0 overflow-y-auto transition-colors duration-350">
          
          {/* Decorative Golden Top boundary strip */}
          <div className="h-1 w-full bg-[#D97706] shrink-0 shadow-sm" />

          <div className="flex-1 w-full max-w-full lg:max-w-[96%] mx-auto px-4 py-6 md:px-8 md:py-8 flex flex-col space-y-6">
            
            {/* Active Navigation Workspace router */}
            <AnimatePresence mode="wait">
              
              {/* Nav View 0: Home Curated Presets */}
              {activeNav === 'home' && (
                <motion.div
                  key="workspace-home"
                  initial={{ opacity: 0, y: 10 }}
                  animate={{ opacity: 1, y: 0 }}
                  exit={{ opacity: 0, y: -10 }}
                  className="space-y-4"
                >
                  <div className="flex items-center gap-2 border-b pb-4 mb-4 border-[#E6E2D3] dark:border-[#2C2932]">
                    <Scroll className="w-5 h-5 text-[#D97706]" />
                    <h3 className="font-serif text-lg font-bold">Sacred Verses & Presets</h3>
                  </div>
                  <PresetScriptures 
                    scriptureContext={scriptureContext} 
                    onSelectScripture={selectScripture} 
                    isDark={isDark}
                    popularScriptures={popularScriptures}
                  />
                </motion.div>
              )}

            {/* Nav View 1: Main Scripture Translation Workbench */}
            {activeNav === 'translator' && (
              <motion.div
                key="workspace-translator"
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -10 }}
                className="space-y-6"
              >
                <div className="flex items-center gap-2 border-b pb-4 mb-4 border-[#E6E2D3] dark:border-[#2C2932]">
                  <Languages className="w-5 h-5 text-[#D97706]" />
                  <h3 className="font-serif text-lg font-bold">Scripture Translator</h3>
                </div>

                {/* Local & Offline Vedic Scholar Engine dynamic alerts */}
                <VedicAlerts
                  showResults={showResults}
                  analysisResult={analysisResult}
                  apiError={apiError}
                  isDark={isDark}
                />

                {/* Input Workbench and Result Workspace - Rendered as a stacked full-width layout */}
                <div className="flex flex-col space-y-8">
                  
                  {/* Full Width Translation workbench */}
                  <div id="workspace-triggers" className="w-full">
                    <TranslationWorkbench
                      text={text}
                      setText={setText}
                      sourceLang={sourceLang}
                      setSourceLang={setSourceLang}
                      targetLang={targetLang}
                      setTargetLang={setTargetLang}
                      scriptureContext={scriptureContext}
                      setScriptureContext={setScriptureContext}
                      isLoading={isLoading}
                      onTriggerAnalysis={() => triggerAnalysis(text)}
                      onTriggerSimpleTranslation={triggerSimpleTranslation}
                      onTriggerPureTransliteration={triggerPureTransliteration}
                      isDark={isDark}
                      workflowMode={workflowMode}
                      setWorkflowMode={(mode) => {
                        setWorkflowMode(mode);
                        if (mode === 'input') {
                          setShowResults(false);
                        } else {
                          setShowResults(true);
                        }
                      }}
                    />
                  </div>

                  {/* Complete Results Console - Rendered below occupying full width, shown only after button triggers */}
                  <AnimatePresence>
                    {showResults && (
                      <motion.div
                        id="results-panel"
                        initial={{ opacity: 0, y: 30 }}
                        animate={{ opacity: 1, y: 0 }}
                        exit={{ opacity: 0, y: -20 }}
                        transition={{ duration: 0.4, ease: "easeOut" }}
                        className="w-full flex flex-col space-y-4 scroll-mt-6"
                      >
                        {/* Console Tab selectors */}
                        <div className={`flex p-1 rounded-xl w-full select-none overflow-x-auto border transition-colors ${
                          isDark ? 'bg-[#151419] border-[#2C2932]' : 'bg-[#F5F5F4] border-[#E6E2D3]'
                        }`}>
                          <button
                            onClick={() => setActiveTab('translation')}
                            className={`flex-1 min-w-[90px] py-2 text-xs font-semibold rounded-lg flex items-center justify-center gap-1.5 cursor-pointer transition-all ${
                              activeTab === 'translation' 
                                ? isDark
                                  ? 'bg-[#25242D] shadow-sm text-amber-500 border border-[#2C2932]'
                                  : 'bg-white shadow-sm text-[#1C1917] font-bold border border-[#E6E2D3]' 
                                : 'text-[#78716C] hover:text-[#D97706]'
                            }`}
                          >
                            <Sparkles className="w-3.5 h-3.5" />
                            <span>Translation</span>
                          </button>

                          <button
                            onClick={() => setActiveTab('grammar')}
                            className={`flex-1 min-w-[90px] py-2 text-xs font-semibold rounded-lg flex items-center justify-center gap-1.5 cursor-pointer transition-all ${
                              activeTab === 'grammar' 
                                ? isDark
                                  ? 'bg-[#25242D] shadow-sm text-amber-500 border border-[#2C2932]'
                                  : 'bg-white shadow-sm text-[#1C1917] font-bold border border-[#E6E2D3]' 
                                : 'text-[#78716C] hover:text-[#D97706]'
                            }`}
                          >
                            <Book className="w-3.5 h-3.5" />
                            <span>Padapatha</span>
                          </button>

                          <button
                            onClick={() => setActiveTab('transliteration')}
                            className={`flex-1 min-w-[90px] py-2 text-xs font-semibold rounded-lg flex items-center justify-center gap-1.5 cursor-pointer transition-all ${
                              activeTab === 'transliteration' 
                                ? isDark
                                  ? 'bg-[#25242D] shadow-sm text-amber-500 border border-[#2C2932]'
                                  : 'bg-white shadow-sm text-[#1C1917] font-bold border border-[#E6E2D3]' 
                                : 'text-[#78716C] hover:text-[#D97706]'
                            }`}
                          >
                            <Globe className="w-3.5 h-3.5" />
                            <span>Transliteration</span>
                          </button>
                        </div>

                        {/* Results Outer Content Box */}
                        <div className={`border rounded-2xl p-5 min-h-[380px] flex flex-col relative overflow-hidden shadow-sm transition-colors ${
                          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-white border-[#E6E2D3]'
                        }`}>
                          <AnimatePresence mode="wait">
                            {isLoading && (
                              <div 
                                key="translation-loading-overlay"
                                className={`absolute inset-0 z-25 flex flex-col items-center justify-center space-y-3 transition-all ${
                                  isDark ? 'bg-[#0F0E11]/90' : 'bg-white/80'
                                }`}
                              >
                                <span className="w-9 h-9 border-2 border-[#D97706] border-t-transparent rounded-full animate-spin" />
                                <p className="text-sm font-serif text-[#D97706] font-bold">Deconstructing Sandhi Rules & Vedic commentaries...</p>
                                <p className="text-[10px] font-mono text-[#78716C] tracking-widest">Model parameters synced</p>
                              </div>
                            )}

                            {activeTab === 'translation' && (
                              <TranslationTab
                                analysisResult={analysisResult}
                                leftCardTitle={leftCardTitle}
                                leftCardContent={leftCardContent}
                                leftCardCopyKey={leftCardCopyKey}
                                rightCardTitle={rightCardTitle}
                                rightCardContent={rightCardContent}
                                rightCardCopyKey={rightCardCopyKey}
                                copiedText={copiedText}
                                onCopy={handleCopyText}
                                isDark={isDark}
                              />
                            )}

                            {activeTab === 'grammar' && (
                              <GrammarTab analysisResult={analysisResult} isDark={isDark} />
                            )}

                            {activeTab === 'transliteration' && (
                              <TransliterationTab
                                transliterations={transliterations}
                                copiedText={copiedText}
                                onCopy={handleCopyText}
                                onRegenerateMatrix={() => {
                                  triggerPureTransliteration('english_phonetic');
                                  triggerPureTransliteration('slp1');
                                }}
                                isDark={isDark}
                              />
                            )}
                          </AnimatePresence>
                        </div>
                      </motion.div>
                    )}
                  </AnimatePresence>

                </div>
              </motion.div>
            )}

            {/* Nav View 2: Sacred Scripture Lexicon Search Panel */}
            {activeNav === 'lexicon' && (
              <motion.div
                key="workspace-lexicon"
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -10 }}
                className="space-y-4"
              >
                <div className="flex items-center gap-2 border-b pb-4 mb-4 border-[#E6E2D3] dark:border-[#2C2932]">
                  <BookOpen className="w-5 h-5 text-[#D97706]" />
                  <h3 className="font-serif text-lg font-bold">Sacred Lexicon (Dict)</h3>
                </div>
                <DictionaryTab onTranslateWord={handleTranslateWord} isDark={isDark} />
              </motion.div>
            )}

            {/* Nav View 3: Developer specs and Sandbox console */}
            {activeNav === 'api' && (
              <motion.div
                key="workspace-api"
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -10 }}
                className="space-y-4"
              >
                <div className="flex items-center gap-2 border-b pb-4 mb-4 border-[#E6E2D3] dark:border-[#2C2932]">
                  <Terminal className="w-5 h-5 text-[#D97706]" />
                  <h3 className="font-serif text-lg font-bold">Developer Console</h3>
                </div>
                <ApiPlayground currentText={text} isDark={isDark} />
              </motion.div>
            )}

            {/* Nav View 4: About information portal */}
            {activeNav === 'about' && (
              <motion.div
                key="workspace-about"
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -10 }}
                className="space-y-4"
              >
                <div className="flex items-center gap-2 border-b pb-4 mb-4 border-[#E6E2D3] dark:border-[#2C2932]">
                  <Compass className="w-5 h-5 text-[#D97706]" />
                  <h3 className="font-serif text-lg font-bold">About Sanskrit Quest</h3>
                </div>
                <AboutView isDark={isDark} />
              </motion.div>
            )}

          </AnimatePresence>

        </div>

        {/* Global Themed Footer */}
        <Footer 
          onTabChange={(tab) => {
            setActiveNav('translator');
            setActiveTab(tab);
            setTimeout(() => {
              document.getElementById('results-panel')?.scrollIntoView({ behavior: 'smooth' });
            }, 100);
          }} 
          isDark={isDark}
        />

      </main>
      </div>

    </div>
  );
}
