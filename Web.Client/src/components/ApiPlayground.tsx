import React, { useState } from 'react';
import { Terminal, Copy, Check, Play, Server } from 'lucide-react';
import apiClient from '../utils/apiClient.ts';

interface ApiPlaygroundProps {
  currentText: string;
  isDark?: boolean;
}

export default function ApiPlayground({ currentText, isDark = false }: ApiPlaygroundProps) {
  const [selectedEndpoint, setSelectedEndpoint] = useState<'/api/translate' | '/api/transliterate' | '/api/analyze' | '/api/scriptures' | '/api/dictionary'>('/api/translate');
  const [copiedSection, setCopiedSection] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'docs' | 'tester'>('docs');
  const [selectedLang, setSelectedLang] = useState<'curl' | 'js' | 'python'>('curl');
  
  // Interactive Tester State
  const [reqBody, setReqBody] = useState<string>('');
  const [apiResponse, setApiResponse] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [apiError, setApiError] = useState<string | null>(null);
  const [dictQuery, setDictQuery] = useState<string>('all');

  // Default Request Templates
  const defaultBodies = {
    '/api/translate': JSON.stringify({
      text: currentText || "कर्मण्येवाधिकारस्ते मा फलेषु कदाचन।",
      sourceLang: "sanskrit",
      targetLang: "english",
      scriptureContext: "Bhagavad Gita"
    }, null, 2),
    '/api/transliterate': JSON.stringify({
      text: currentText || "ॐ भूर् भुवः स्वः।",
      sourceScript: "devanagari",
      targetScript: "iast"
    }, null, 2),
    '/api/analyze': JSON.stringify({
      text: currentText || "त्र्यम्बकं यजामहे सुगन्धिं पुष्टिवर्धनम्।",
      sourceContext: "Rigveda"
    }, null, 2),
    '/api/scriptures': '',
    '/api/dictionary': ''
  };

  // Sync request body when endpoint changes
  React.useEffect(() => {
    setReqBody(defaultBodies[selectedEndpoint] || '');
  }, [selectedEndpoint, currentText]);

  const copyToClipboard = (text: string, id: string) => {
    navigator.clipboard.writeText(text);
    setCopiedSection(id);
    setTimeout(() => setCopiedSection(null), 2000);
  };

  const getCodeSnippet = (lang: 'curl' | 'js' | 'python', endpoint: string, bodyStr: string) => {
    const host = window.location.origin;
    const isGet = endpoint === '/api/scriptures' || endpoint === '/api/dictionary';
    
    // Support optional query parameter for dictionary lookup
    const finalEndpoint = (endpoint === '/api/dictionary' && dictQuery)
      ? `/api/dictionary?word=${encodeURIComponent(dictQuery)}`
      : endpoint;

    let bodyObj = null;
    if (!isGet) {
      try {
        bodyObj = JSON.parse(bodyStr || '{}');
      } catch (e) {
        bodyObj = {};
      }
    }

    if (lang === 'curl') {
      if (isGet) {
        return `curl -X GET "${host}${finalEndpoint}" \\
  -H "Authorization: Bearer <your_jwt_token>"`;
      }
      return `curl -X POST "${host}${finalEndpoint}" \\
  -H "Content-Type: application/json" \\
  -H "Authorization: Bearer <your_jwt_token>" \\
  -d '${JSON.stringify(bodyObj)}'`;
    } else if (lang === 'js') {
      if (isGet) {
        return `fetch("${host}${finalEndpoint}", {
  headers: {
    "Authorization": "Bearer <your_jwt_token>"
  }
})
  .then(res => res.json())
  .then(data => console.log(data))
  .catch(err => console.error(err));`;
      }
      return `fetch("${host}${finalEndpoint}", {
  method: "POST",
  headers: {
    "Content-Type": "application/json",
    "Authorization": "Bearer <your_jwt_token>"
  },
  body: JSON.stringify(${JSON.stringify(bodyObj, null, 2).replace(/\n/g, '\n  ')})
})
  .then(res => res.json())
  .then(data => console.log(data))
  .catch(err => console.error(err));`;
    } else if (lang === 'python') {
      if (isGet) {
        return `import requests

url = "${host}${finalEndpoint}"
headers = {
    "Authorization": "Bearer <your_jwt_token>"
}

response = requests.get(url, headers=headers)
data = response.json()
print(data)`;
      }
      return `import requests

url = "${host}${finalEndpoint}"
headers = {
    "Authorization": "Bearer <your_jwt_token>"
}
payload = ${JSON.stringify(bodyObj, null, 4)}

response = requests.post(url, json=payload, headers=headers)
data = response.json()
print(data)`;
    }
    return '';
  };

  const handleTestApi = async () => {
    setIsLoading(true);
    setApiError(null);
    setApiResponse('');
    try {
      const isGet = selectedEndpoint === '/api/scriptures' || selectedEndpoint === '/api/dictionary';
      
      const finalEndpoint = (selectedEndpoint === '/api/dictionary' && dictQuery)
        ? `/api/dictionary?word=${encodeURIComponent(dictQuery)}`
        : selectedEndpoint;

      let response;
      if (isGet) {
        response = await apiClient.get(finalEndpoint);
      } else {
        // Validate JSON input
        let parsed;
        try {
          parsed = JSON.parse(reqBody);
        } catch (jsonErr) {
          throw new Error("Invalid input format. Please check your JSON syntax.");
        }
        response = await apiClient.post(finalEndpoint, parsed);
      }

      setApiResponse(JSON.stringify(response.data, null, 2));
    } catch (err: any) {
      const errorMessage = err.response?.data?.error || err.response?.data?.message || err.message || 'Failed to complete API request.';
      setApiError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  const endpointBriefs = {
    '/api/scriptures': {
      desc: 'Retrieves popular, pre-configured classical scriptures, verse text, translations, and initial diacritic profiles.',
      method: 'GET'
    },
    '/api/dictionary': {
      desc: 'Retrieves categorized scripture dictionary entries. Supports dynamic "?word=..." parameters to query individual terms, plus "?word=all" to load all.',
      method: 'GET'
    },
    '/api/translate': {
      desc: 'Translates Sanskrit or Hindi sources into English prose and poetic forms utilizing advanced context inputs.',
      method: 'POST'
    },
    '/api/transliterate': {
      desc: 'Converts orthographic streams between Devanagari, IAST, ITRANS, SLP1, and chanting friendly equivalents.',
      method: 'POST'
    },
    '/api/analyze': {
      desc: 'Deconstructs compound words (Sandhi breaks), details Case conjugation parameters, and returns Meter metrics.',
      method: 'POST'
    }
  };

  return (
    <div id="dev-api-playground" className="p-1 max-w-full">
      {/* Endpoint Bar picker */}
      <div className={`flex flex-col lg:flex-row gap-4 justify-between items-start lg:items-center p-4 rounded-xl border mb-6 transition-all duration-300 ${
        isDark ? 'bg-[#1E1D23] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
      }`}>
        <div>
          <div className="flex items-center gap-2">
            <span className="p-1.5 bg-[#D97706]/10 border border-[#D97706]/20 text-[#D97706] rounded-md">
              <Server className="w-4 h-4" />
            </span>
            <h3 className={`font-serif text-lg font-bold transition-colors ${isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}`}>Scriptural API Specifications</h3>
          </div>
          <p className={`text-xs mt-1 max-w-xl transition-colors ${isDark ? 'text-[#9B98A3]' : 'text-[#57534E]'}`}>
            Integrate Vedic translations directly into your applications. All endpoints authenticate with your workspace token automatically.
          </p>
        </div>

        {/* Tab selector Docs vs Tester */}
        <div className={`flex p-0.5 rounded-lg border self-stretch lg:self-auto transition-colors ${
          isDark ? 'bg-[#151419] border-[#2C2932]' : 'bg-[#F5F5F4] border-[#E6E2D3]'
        }`}>
          <button
            onClick={() => setActiveTab('docs')}
            className={`px-4 py-2 text-xs font-semibold rounded-md transition-all duration-200 cursor-pointer ${
              activeTab === 'docs' 
                ? isDark
                  ? 'bg-[#25242D] text-[#F1EFF7] border border-[#2C2932] shadow-sm'
                  : 'bg-white text-[#1C1917] font-bold border border-[#E6E2D3] shadow-sm' 
                : 'text-[#9B98A3] hover:text-[#D97706]'
            }`}
          >
            Documentation & Examples
          </button>
          <button
            onClick={() => setActiveTab('tester')}
            className={`px-4 py-2 text-xs font-semibold rounded-md transition-all duration-200 cursor-pointer ${
              activeTab === 'tester' 
                ? isDark
                  ? 'bg-[#25242D] text-[#F1EFF7] border border-[#2C2932] shadow-sm'
                  : 'bg-white text-[#1C1917] font-bold border border-[#E6E2D3] shadow-sm' 
                : 'text-[#9B98A3] hover:text-[#D97706]'
            }`}
          >
            Interactive Sandbox
          </button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-12 gap-6">
        {/* Left endpoint selector Column */}
        <div className="md:col-span-4 space-y-3">
          <label className={`text-xs font-mono tracking-wider uppercase font-bold ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>Interactive APIs</label>
          {Object.entries(endpointBriefs).map(([path, details]) => {
            const isSelected = selectedEndpoint === path;
            return (
              <button
                key={path}
                onClick={() => setSelectedEndpoint(path as any)}
                className={`w-full text-left p-4 rounded-xl border transition-all duration-200 cursor-pointer ${
                  isSelected 
                    ? isDark
                      ? 'bg-[#1E1D23] border-[#D97706] shadow-sm text-[#F1EFF7]'
                      : 'bg-[#F9F7F2] border-[#D97706] shadow-sm text-[#1C1917]' 
                    : isDark
                      ? 'bg-[#16151A] border-[#2C2932] hover:border-[#D97706]/40 hover:bg-[#201F26]'
                      : 'bg-white border-[#E6E2D3] hover:border-[#D97706]/40 hover:bg-[#FDFBF7]'
                }`}
              >
                <div className="flex gap-2 items-center">
                  <span className={`px-1.5 py-0.5 border text-[10px] font-mono font-bold rounded ${details.method === 'GET' ? 'bg-blue-500/10 text-blue-400 border-blue-500/20' : 'bg-green-500/10 text-green-400 border-green-500/20'}`}>
                    {details.method}
                  </span>
                  <span className="font-mono text-xs text-[#D97706] font-bold">{path}</span>
                </div>
                <p className={`text-xs mt-2 line-clamp-2 leading-relaxed transition-colors ${isDark ? 'text-[#9B98A3]' : 'text-[#57534E]'}`}>
                  {details.desc}
                </p>
              </button>
            );
          })}

          <div className={`border rounded-xl p-4 text-xs mt-4 space-y-2 transition-colors ${
            isDark ? 'bg-[#1A191E] border-[#2C2932] text-[#9B98A3]' : 'bg-[#FDFBF7] border-[#E6E2D3] text-[#57534E]'
          }`}>
            <span className="font-bold text-[#D97706] font-serif">Integration Quick Tips:</span>
            <p>Ensure payload includes <code className="text-[#D97706] font-mono">"Content-Type: application/json"</code>.</p>
            <p>Sanskrit word breaks are computed recursively per line to preserve classical rhythms.</p>
          </div>
        </div>

        {/* Right Tab Content Column */}
        <div className={`md:col-span-8 p-5 rounded-2xl border flex flex-col min-h-[460px] shadow-sm transition-colors ${
          isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-white border-[#E6E2D3]'
        }`}>
          {activeTab === 'docs' ? (
            <div className="flex-1 flex flex-col">
              {/* Language Picker in Header */}
              <div className={`flex justify-between items-center px-4 py-2 rounded-lg border mb-4 transition-colors ${
                isDark ? 'bg-[#1E1D23] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
              }`}>
                <div className="flex gap-2 items-center text-xs text-[#57534E]">
                  <Terminal className="w-3.5 h-3.5 text-[#78716C]" />
                  <span className={`font-semibold ${isDark ? 'text-[#DFDCE6]' : 'text-[#57534E]'}`}>Interactive Code Blocks</span>
                </div>
                <div className="flex gap-1.5">
                  {(['curl', 'js', 'python'] as const).map((lang) => (
                    <button
                      key={lang}
                      onClick={() => setSelectedLang(lang)}
                      className={`px-2.5 py-1 text-[10px] font-mono rounded transition-colors cursor-pointer ${
                        selectedLang === lang 
                          ? 'bg-[#D97706]/10 text-[#D97706] border border-[#D97706]/20 font-bold' 
                          : 'text-[#78716C] hover:text-[#D97706]'
                      }`}
                    >
                      {lang === 'curl' ? 'cURL' : lang === 'js' ? 'Fetch API' : 'Python'}
                    </button>
                  ))}
                </div>
              </div>

              {/* Snippet display */}
              <div className={`relative flex-1 rounded-xl border p-4 font-mono text-xs overflow-x-auto leading-relaxed min-h-[280px] transition-colors ${
                isDark ? 'bg-[#111014] border-[#2C2932] text-amber-100' : 'bg-[#FDFBF7] border-[#E6E2D3] text-[#1C1917]'
              }`}>
                <button
                  onClick={() => copyToClipboard(getCodeSnippet(selectedLang, selectedEndpoint, reqBody), 'snippet')}
                  className={`absolute top-3 right-3 p-1.5 border rounded transition-all cursor-pointer shadow-sm ${
                    isDark 
                      ? 'bg-[#1E1D23] hover:bg-[#25242D] border-[#2C2932] text-[#9B98A3]' 
                      : 'bg-white hover:bg-[#F9F7F2] border-[#E6E2D3] text-[#78716C]'
                  }`}
                  title="Copy Code Snippet"
                >
                  {copiedSection === 'snippet' ? <Check className="w-3.5 h-3.5 text-green-500 font-bold" /> : <Copy className="w-3.5 h-3.5" />}
                </button>
                <div className="whitespace-pre pr-8">
                  {getCodeSnippet(selectedLang, selectedEndpoint, reqBody)}
                </div>
              </div>
            </div>
          ) : (
            <div className="flex-1 flex flex-col space-y-4">
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 flex-1">
                {/* JSON Input Sandbox */}
                <div className="flex flex-col space-y-2">
                  <div className="flex justify-between items-center">
                    <label className={`text-[10px] font-mono uppercase tracking-wider font-bold ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
                      {(selectedEndpoint === '/api/scriptures' || selectedEndpoint === '/api/dictionary') ? 'Request Parameters' : 'Request Body (JSON)'}
                    </label>
                    {(selectedEndpoint !== '/api/scriptures' && selectedEndpoint !== '/api/dictionary') && (
                      <button
                        onClick={() => setReqBody(defaultBodies[selectedEndpoint])}
                        className="text-[10px] text-[#D97706] hover:text-[#B45309] transition-colors font-bold cursor-pointer"
                      >
                        Reset Payload
                      </button>
                    )}
                  </div>
                  {selectedEndpoint === '/api/scriptures' ? (
                    <div className={`flex-1 w-full border rounded-xl p-4 font-mono text-xs flex items-center justify-center text-center min-h-[220px] transition-colors ${
                      isDark ? 'bg-[#1A191E] border-[#2C2932] text-[#5E5B66]' : 'bg-[#F5F5F4]/60 border-[#E6E2D3] text-[#78716C]'
                    }`}>
                      GET request. No request body or extra payload parameters are required for retrieving popular scriptures.
                    </div>
                  ) : selectedEndpoint === '/api/dictionary' ? (
                    <div className={`flex-1 w-full border rounded-xl p-4 flex flex-col justify-center min-h-[220px] transition-colors ${
                      isDark ? 'bg-[#1E1D23] border-[#2C2932]' : 'bg-[#FFFDF9] border-[#E6E2D3]'
                    }`}>
                      <div className="space-y-4">
                        <div>
                          <label className={`block text-xs font-serif font-bold mb-1 ${isDark ? 'text-amber-200' : 'text-[#1C1917]'}`}>
                            Dictionary Word Query (?word=)
                          </label>
                          <p className={`text-[10px] mb-2 leading-normal ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
                            Type a theological word (e.g. <strong>"karma"</strong>, <strong>"yoga"</strong>, <strong>"atman"</strong>, or <strong>"moksha"</strong>) to query specific entries. Input <strong>"all"</strong> or <strong>"al"</strong> to load all entries.
                          </p>
                          <input
                            type="text"
                            value={dictQuery}
                            onChange={(e) => setDictQuery(e.target.value)}
                            placeholder="e.g. all, al, karma, yoga..."
                            className={`w-full text-xs rounded-lg py-2 px-3 focus:outline-none focus:ring-1 focus:ring-[#D97706] focus:border-[#D97706] transition-all font-mono ${
                              isDark 
                                ? 'bg-[#151419] border-[#2C2932] text-[#F1EFF7] placeholder:text-[#5E5B66]' 
                                : 'bg-white border-[#E6E2D3] text-[#1C1917] placeholder:text-[#A8A29E]'
                            }`}
                          />
                        </div>
                        <div className={`text-[10px] font-mono p-2.5 rounded border leading-relaxed ${
                          isDark ? 'bg-[#16151A] text-[#9B98A3] border-[#2A2730]' : 'bg-[#F9F7F2] text-[#57534E] border-[#E6E2D3]'
                        }`}>
                          <span className="font-bold text-[#D97706]">Tip:</span> Individual queries yield fine-grained term definitions along with Sanskrit grammar rules, English prose meanings, and Hindi translations.
                        </div>
                      </div>
                    </div>
                  ) : (
                    <textarea
                      value={reqBody}
                      onChange={(e) => setReqBody(e.target.value)}
                      className={`flex-1 w-full border rounded-xl p-3 font-mono text-[11px] focus:outline-none focus:border-[#D97706] resize-none min-h-[220px] transition-colors ${
                        isDark 
                          ? 'bg-[#111014] border-[#2C2932] text-amber-200' 
                          : 'bg-[#FDFBF7] border-[#E6E2D3] text-[#1C1917]'
                      }`}
                    />
                  )}
                  <button
                    onClick={handleTestApi}
                    disabled={isLoading}
                    className={`w-full flex items-center justify-center gap-2 font-bold py-2.5 rounded-xl transition-all disabled:opacity-50 text-xs shadow-sm cursor-pointer uppercase tracking-wider ${
                      isDark 
                        ? 'bg-[#E5E3DB] hover:bg-[#F2F1ED] text-[#0F0E11]' 
                        : 'bg-[#1C1917] hover:bg-[#2D241E] text-white'
                    }`}
                  >
                    {isLoading ? (
                      <span className="w-4 h-4 border-2 border-current border-t-transparent rounded-full animate-spin" />
                    ) : (
                      <Play className="w-3.5 h-3.5 fill-current text-current" />
                    )}
                    <span>{isLoading ? 'Executing Request...' : 'Trigger API Request'}</span>
                  </button>
                </div>

                {/* Response Code Block */}
                <div className="flex flex-col space-y-2">
                  <div className="flex justify-between items-center h-5">
                    <span className={`text-[10px] font-mono uppercase tracking-wider font-bold ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>API RESPONSE</span>
                    {apiResponse && (
                      <button
                        onClick={() => copyToClipboard(apiResponse, 'response')}
                        className="flex items-center gap-1 text-[10px] text-[#D97706] hover:text-[#B45309] font-bold cursor-pointer"
                      >
                        {copiedSection === 'response' ? <Check className="w-3 text-green-500 font-bold" /> : <Copy className="w-3" />}
                        <span>Copy Output</span>
                      </button>
                    )}
                  </div>

                  <div className={`flex-1 border rounded-xl p-3 font-mono text-[11px] overflow-y-auto max-h-[300px] lg:max-h-none min-h-[260px] relative transition-colors ${
                    isDark 
                      ? 'bg-[#111014] border-[#2C2932] text-green-400' 
                      : 'bg-[#FDFBF7] border-[#E6E2D3] text-[#1C1917]'
                  }`}>
                    {isLoading && (
                      <div className={`absolute inset-0 flex flex-col gap-2 items-center justify-center text-xs font-bold transition-all ${
                        isDark ? 'bg-[#16151A]/90' : 'bg-white/80'
                      }`}>
                        <span className="w-6 h-6 border-2 border-[#D97706] border-t-transparent rounded-full animate-spin" />
                        <span className="text-[#D97706]">Fetching scriptural parameters...</span>
                      </div>
                    )}
                    {apiError ? (
                      <div className="text-red-400 bg-red-950/20 p-3 rounded-lg border border-red-500/30 whitespace-pre-wrap leading-relaxed font-semibold">
                        ⚠️ Error: {apiError}
                      </div>
                    ) : apiResponse ? (
                      <pre className="whitespace-pre-wrap">{apiResponse}</pre>
                    ) : (
                      <div className={`h-full flex items-center justify-center text-[11px] text-center px-4 leading-relaxed ${
                        isDark ? 'text-[#5E5B66]' : 'text-[#78716C]'
                      }`}>
                        Customize the JSON body on the left and click "Trigger API Request" to parse translation and metadata in real-time.
                      </div>
                    )}
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
