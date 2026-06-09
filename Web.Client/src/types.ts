export interface Scripture {
  id: string;
  title: string;
  source: string; // e.g., 'Bhagavad Gita 2.47', 'Rigveda 1.1.1'
  category: 'Gita' | 'Vedas' | 'Upanishads' | 'Puranas' | 'Other';
  verse: string; // Original Sanskrit (usually Devanagari)
  transliterationDefault: string; // default transliteration (IAST or similar)
  translationDefaultEnglish: string;
  translationDefaultHindi: string;
}

export interface TranslationRequest {
  text: string;
  sourceLang: 'sanskrit' | 'hindi' | 'english' | 'auto';
  targetLang: 'sanskrit' | 'hindi' | 'english';
  scriptureContext?: string; // e.g., "Bhagavad Gita" to give better specialized meaning
}

export interface WordBreakdown {
  word: string;
  grammar?: string; // Sandhi split / Root-word / noun-declension / verb-conjugation
  meaningHindi: string;
  meaningEnglish: string;
}

export interface TranslationResponse {
  sourceLang: string;
  targetLang: string;
  translatedText: string;
  explanation?: string; // Philological or spiritual explanation representing Vedas/Gita context
  wordBreakdown?: WordBreakdown[]; // Detailed word-by-word parsing (excellent for Sanskrit)
  isFallback?: boolean; // Indicates if request was fulfilled via local rule-based fallback
}

export interface TransliterateRequest {
  text: string;
  sourceScript: 'devanagari' | 'iast' | 'itrans' | 'english_phonetic' | 'slp1';
  targetScript: 'devanagari' | 'iast' | 'itrans' | 'english_phonetic' | 'slp1';
}

export interface TransliterateResponse {
  sourceScript: string;
  targetScript: string;
  transliteratedText: string;
}

export interface ScriptureAnalyzeRequest {
  text: string;
  sourceContext?: string; // Optional context, e.g., "From Upanishads"
}

export interface ScriptureAnalyzeResponse {
  verse: string;
  identifiedSource?: string;
  transliterationIAST: string;
  transliterationPhonetic: string;
  translationEnglish: string;
  translationHindi: string;
  translationSanskrit?: string;
  spiritualSignificance: string;
  wordBreakdown: WordBreakdown[];
  poeticMeter?: string; // e.g., Anustubh, Gayatri, Tristubh meter
  isFallback?: boolean; // Indicates if request was fulfilled via local rule-based fallback
}
