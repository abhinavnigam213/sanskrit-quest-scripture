import React, { useState, useEffect, useRef } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { 
  Menu, 
  X, 
  Compass, 
  Info, 
  Settings, 
  Sun, 
  Moon, 
  Laptop
} from 'lucide-react';
import SanskritQuestLogo from './SanskritQuestLogo.tsx';

interface TopBarProps {
  isDark: boolean;
  activeNav: 'home' | 'translator' | 'lexicon' | 'api' | 'about';
  setActiveNav: (nav: 'home' | 'translator' | 'lexicon' | 'api' | 'about') => void;
  setMobileMenuOpen: React.Dispatch<React.SetStateAction<boolean>>;
  setIsSidebarCollapsed: React.Dispatch<React.SetStateAction<boolean>>;
  themeMode: 'light' | 'dark' | 'system';
  setThemeMode: (mode: 'light' | 'dark' | 'system') => void;
  fontSize: 'small' | 'normal' | 'large' | 'xlarge';
  setFontSize: (size: 'small' | 'normal' | 'large' | 'xlarge') => void;
}

export default function TopBar({
  isDark,
  activeNav,
  setActiveNav,
  setMobileMenuOpen,
  setIsSidebarCollapsed,
  themeMode,
  setThemeMode,
  fontSize,
  setFontSize
}: TopBarProps) {
  const [isSettingsOpen, setIsSettingsOpen] = useState<boolean>(false);
  const settingsRef = useRef<HTMLDivElement>(null);
  const headerTriggerRef = useRef<HTMLButtonElement>(null);

  const fontSizes = [
    { id: 'small', label: 'Small', desc: 'Compact layout' },
    { id: 'normal', label: 'Normal', desc: 'Standard Vedic reading' },
    { id: 'large', label: 'Large', desc: 'Comfortable focus' },
    { id: 'xlarge', label: 'Vedic XL', desc: 'Bold word breakdown' }
  ] as const;

  useEffect(() => {
    function handleOutside(event: Event) {
      const target = event.target as HTMLElement;
      if (!isSettingsOpen) return;

      const insideDropdown = settingsRef.current?.contains(target);
      const insideHeaderBtn = headerTriggerRef.current?.contains(target);

      if (!insideDropdown && !insideHeaderBtn) {
        setIsSettingsOpen(false);
      }
    }

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === 'Escape' && isSettingsOpen) {
        setIsSettingsOpen(false);
      }
    }

    document.addEventListener('mousedown', handleOutside);
    document.addEventListener('touchstart', handleOutside);
    document.addEventListener('focusin', handleOutside);
    document.addEventListener('keydown', handleKeyDown);

    return () => {
      document.removeEventListener('mousedown', handleOutside);
      document.removeEventListener('touchstart', handleOutside);
      document.removeEventListener('focusin', handleOutside);
      document.removeEventListener('keydown', handleKeyDown);
    };
  }, [isSettingsOpen]);

  return (
    <header className={`w-full border-b z-40 sticky top-0 transition-all duration-300 shadow-sm shrink-0 ${
      isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
    }`}>
      <div className="w-full px-4 py-4 md:px-8 flex flex-col lg:flex-row items-start lg:items-center justify-between gap-4">
        
        <div className="flex items-center gap-4 w-full lg:w-auto">
          {/* Collapse/Expand toggle button */}
          <button 
            id="toggle-sidebar-trigger"
            onClick={() => {
              if (typeof window !== 'undefined' && window.innerWidth >= 768) {
                setIsSidebarCollapsed(prev => !prev);
              } else {
                setMobileMenuOpen(prev => !prev);
              }
            }}
            className={`p-2 rounded-lg transition-colors cursor-pointer ${
              isDark ? 'hover:bg-[#1E1D23] text-[#F1EFF7]' : 'hover:bg-[#E6E2D3]/30 text-[#1C1917]'
            }`}
            title="Toggle Sidebar Menu"
          >
            <Menu className="w-5 h-5" />
          </button>

          <div className="flex items-center justify-center shrink-0">
            <SanskritQuestLogo className="w-11 h-11 md:w-[50px] md:h-[50px]" isDark={isDark} />
          </div>
          <div>
            <h1 className="font-sans text-lg md:text-xl lg:text-2xl font-medium tracking-tight transition-colors flex items-center flex-wrap">
              <span className={isDark ? 'text-[#F1EFF7]' : 'text-[#1C1917]'}>SANSKRIT QUEST</span>
              <span className="text-[#D97706] pl-1.5 font-bold">(संस्कृत अन्वेषणम्)</span>
            </h1>
            <p className={`font-serif text-xs sm:text-sm mt-1.5 font-semibold italic tracking-wide transition-colors leading-relaxed ${
              isDark ? 'text-[#F5E6C4]' : 'text-[#452711]'
            }`}>
              "ॐ असतो मा सद्गमय। तमसो मा ज्योतिर्गमय।" — Lead us from darkness to light.
            </p>
          </div>
        </div>

        {/* Right Block: Scripture engine info + Theme Switcher Dropdown */}
        <div className="flex flex-row items-center gap-3 shrink-0 ml-auto lg:ml-0">
          
          {/* Engine Info Area */}
          <div className={`hidden sm:flex flex-col items-end gap-0.5 text-right p-2 px-3 rounded-xl border transition-all duration-300 ${
            isDark 
              ? 'bg-[#1D1C22] border-[#2C2932]' 
              : 'bg-white border-[#E6E2D3]'
          }`}>
            <div className="flex items-center gap-1 text-[11px]">
              <Compass className="w-3.5 h-3.5 text-[#D97706] animate-pulse" />
              <span className={`font-bold ${isDark ? 'text-[#E5E3DB]' : 'text-[#1C1917]'}`}>Sanskrit • Hindi • English</span>
            </div>
            <p className={`text-[9px] font-mono leading-tight ${isDark ? 'text-[#9B98A3]' : 'text-[#78716C]'}`}>
              Vedic parsing engine powered by Gemini AI
            </p>
          </div>

          {/* About Navigation Link */}
          <button
            id="nav-about-trigger"
            onClick={() => setActiveNav('about')}
            className={`p-2 rounded-xl border flex items-center justify-center transition-all cursor-pointer shadow-sm shrink-0 h-10 w-10 ${
              activeNav === 'about'
                ? 'bg-[#D97706] text-white border-[#D97706]'
                : isDark
                  ? 'bg-[#201F25] text-[#F1EFF7] border-[#2C2932] hover:bg-[#2C2B32]'
                  : 'bg-white text-[#1C1917] border-[#E6E2D3] hover:bg-[#F9F7F2]'
            }`}
            title="About Portal"
          >
            <Info className="w-4 h-4" />
          </button>

          {/* Unified Settings Trigger Dropdown */}
          <div className="relative shrink-0">
            <button
              id="top-settings-trigger"
              ref={headerTriggerRef}
              onClick={() => setIsSettingsOpen(prev => !prev)}
              className={`p-2 rounded-xl border flex items-center justify-center transition-all cursor-pointer shadow-sm h-10 w-10 ${
                isSettingsOpen
                  ? 'bg-[#D97706] text-white border-[#D97706]'
                  : isDark 
                    ? 'bg-[#201F25] text-amber-200 border-[#2C2932] hover:bg-[#2C2B32]' 
                    : 'bg-white text-[#1C1917] border-[#E6E2D3] hover:bg-[#F9F7F2]'
              }`}
              title="Scholar Settings"
            >
              <Settings className={`w-4 h-4 ${isSettingsOpen ? 'text-white' : 'text-[#D97706]'}`} />
            </button>

            <AnimatePresence>
              {isSettingsOpen && (
                <motion.div
                  ref={settingsRef}
                  initial={{ opacity: 0, y: 8 }}
                  animate={{ opacity: 1, y: 0 }}
                  exit={{ opacity: 0, y: 8 }}
                  className={`absolute right-0 mt-2 w-72 rounded-2xl border p-4 shadow-xl z-50 transition-all ${
                    isDark ? 'bg-[#1C1B20] border-[#2E2C33]' : 'bg-white border-[#E6E2D3]'
                  }`}
                >
                    {/* Header Title */}
                    <div className="flex items-center gap-2 mb-3 border-b pb-2 border-[#E6E2D3]/60 dark:border-[#2C2932]">
                      <Settings className="w-4 h-4 text-[#D97706]" />
                      <h4 className="font-serif font-bold text-xs text-amber-600 dark:text-amber-500">Vedic Workspace Settings</h4>
                    </div>

                    {/* Theme Settings */}
                    <div className="mb-4">
                      <span className={`text-[10px] font-mono uppercase tracking-widest font-bold opacity-70 block mb-1.5 ${isDark ? 'text-gray-300' : 'text-gray-600'}`}>
                        Interface Theme
                      </span>
                      <div className="grid grid-cols-3 gap-1 rounded-lg border border-[#E6E2D3] dark:border-[#2C2932] p-0.5 bg-[#FAF8F4] dark:bg-[#100F12]">
                        {(['light', 'dark', 'system'] as const).map((mode) => {
                          const isSelected = themeMode === mode;
                          let ModeIcon = Sun;
                          if (mode === 'dark') ModeIcon = Moon;
                          if (mode === 'system') ModeIcon = Laptop;
                          return (
                            <button
                              key={`settings-theme-${mode}`}
                              id={`theme-btn-${mode}`}
                              onClick={() => setThemeMode(mode)}
                              className={`py-1.5 px-2 text-[10px] font-semibold rounded flex flex-col items-center gap-1 transition-all cursor-pointer ${
                                isSelected
                                  ? 'bg-[#D97706] text-white shadow-sm font-bold'
                                  : isDark
                                    ? 'text-[#A19E95] hover:bg-[#25242D] hover:text-[#F1EFF7]'
                                    : 'text-[#57534E] hover:bg-[#E6E2D3]/40 hover:text-[#1C1917]'
                              }`}
                            >
                              <ModeIcon className="w-3.5 h-3.5" />
                              <span className="capitalize">{mode}</span>
                            </button>
                          );
                        })}
                      </div>
                    </div>

                    {/* Font-Size Settings */}
                    <div className="mb-1">
                      <div className="flex justify-between items-center mb-1.5">
                        <span className={`text-[10px] font-mono uppercase tracking-widest font-bold opacity-70 ${isDark ? 'text-gray-300' : 'text-gray-600'}`}>
                          Workspace Font Size
                        </span>
                        <span className="text-[9px] font-serif font-black text-[#D97706] bg-amber-500/10 px-1.5 py-0.5 rounded">
                          {fontSize === 'small' ? 'Compact' : fontSize === 'normal' ? 'Scholar' : fontSize === 'large' ? 'Sadhana' : 'Deva XL'}
                        </span>
                      </div>
                      
                      <div className="grid grid-cols-4 gap-1 rounded-lg border border-[#E6E2D3] dark:border-[#2C2932] p-0.5 bg-[#FAF8F4] dark:bg-[#100F12]">
                        {fontSizes.map((opt) => {
                          const isSelected = fontSize === opt.id;
                          return (
                            <button
                              key={`settings-font-${opt.id}`}
                              id={`font-btn-${opt.id}`}
                              onClick={() => setFontSize(opt.id)}
                              className={`py-1.5 text-[11px] font-serif font-black rounded flex flex-col items-center justify-center transition-all cursor-pointer ${
                                isSelected
                                  ? 'bg-[#D97706] text-white shadow-sm'
                                  : isDark
                                    ? 'text-[#A19E95] hover:bg-[#25242D] hover:text-[#F1EFF7]'
                                    : 'text-[#57534E] hover:bg-[#E6E2D3]/40 hover:text-[#1C1917]'
                              }`}
                              title={opt.desc}
                            >
                              <span className={opt.id === 'small' ? 'text-[9px]' : opt.id === 'normal' ? 'text-[11px]' : opt.id === 'large' ? 'text-xs' : 'text-sm'}>
                                A
                              </span>
                            </button>
                          );
                        })}
                      </div>
                    </div>
                  </motion.div>
              )}
            </AnimatePresence>
          </div>

        </div>

      </div>
    </header>
  );
}
