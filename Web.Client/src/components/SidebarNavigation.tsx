import React, { useState, useEffect, useRef } from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { 
  Scroll, 
  Languages, 
  BookOpen, 
  Terminal, 
  Compass, 
  Settings, 
  Sun, 
  Moon, 
  Laptop, 
  ChevronDown, 
  ChevronRight
} from 'lucide-react';
import SanskritQuestLogo from './SanskritQuestLogo.tsx';

interface SidebarNavigationProps {
  isSidebarCollapsed: boolean;
  setIsSidebarCollapsed: React.Dispatch<React.SetStateAction<boolean>>;
  activeNav: 'home' | 'translator' | 'lexicon' | 'api' | 'about';
  setActiveNav: React.Dispatch<React.SetStateAction<'home' | 'translator' | 'lexicon' | 'api' | 'about'>>;
  mobileMenuOpen: boolean;
  setMobileMenuOpen: React.Dispatch<React.SetStateAction<boolean>>;
  isDark: boolean;
  themeMode: 'light' | 'dark' | 'system';
  setThemeMode: (mode: 'light' | 'dark' | 'system') => void;
  fontSize: 'small' | 'normal' | 'large' | 'xlarge';
  setFontSize: (size: 'small' | 'normal' | 'large' | 'xlarge') => void;
  onTranslatorClick: () => void;
}

export default function SidebarNavigation({
  isSidebarCollapsed,
  setIsSidebarCollapsed,
  activeNav,
  setActiveNav,
  mobileMenuOpen,
  setMobileMenuOpen,
  isDark,
  themeMode,
  setThemeMode,
  fontSize,
  setFontSize,
  onTranslatorClick
}: SidebarNavigationProps) {
  
  const [isSidebarSettingsExpanded, setIsSidebarSettingsExpanded] = useState<boolean>(() => {
    if (typeof window !== 'undefined') {
      const stored = localStorage.getItem('sanskrit-quest-sidebar-settings-expanded');
      if (stored === 'false') return false;
    }
    return true;
  });

  const sidebarSettingsContainerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    localStorage.setItem('sanskrit-quest-sidebar-settings-expanded', String(isSidebarSettingsExpanded));
  }, [isSidebarSettingsExpanded]);

  // Click outside / Focus shift outside handler for sidebar settings block
  useEffect(() => {
    function handleOutside(event: Event) {
      const target = event.target as HTMLElement;
      if (!isSidebarSettingsExpanded) return;

      const insideContainer = sidebarSettingsContainerRef.current?.contains(target);

      if (!insideContainer) {
        setIsSidebarSettingsExpanded(false);
      }
    }

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === 'Escape' && isSidebarSettingsExpanded) {
        setIsSidebarSettingsExpanded(false);
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
  }, [isSidebarSettingsExpanded]);

  const navItems = [
    { id: 'home', label: 'Sacred Verses & Presets', shortLabel: 'Verses & Presets', icon: Scroll, description: 'Browse popular verses & presets' },
    { id: 'translator', label: 'Scripture Translator', shortLabel: 'Translator', icon: Languages, description: 'Workspace & sandhi breakdown' },
    { id: 'lexicon', label: 'Sacred Lexicon (Dict)', shortLabel: 'Lexicon', icon: BookOpen, description: 'Theological dictionary query' },
    { id: 'api', label: 'Developer Console', shortLabel: 'Console', icon: Terminal, description: 'REST APIs & sandboxes' },
    { id: 'about', label: 'About Sanskrit Quest', shortLabel: 'About', icon: Compass, description: 'Sanatana Dharma & portal details' }
  ] as const;

  const fontSizes = [
    { id: 'small', label: 'Small', desc: 'Compact layout' },
    { id: 'normal', label: 'Normal', desc: 'Standard Vedic reading' },
    { id: 'large', label: 'Large', desc: 'Comfortable focus' },
    { id: 'xlarge', label: 'Vedic XL', desc: 'Bold word breakdown' }
  ] as const;

  const handleNavClick = (id: 'home' | 'translator' | 'lexicon' | 'api' | 'about') => {
    setActiveNav(id);
    if (id === 'translator') {
      onTranslatorClick();
    }
  };

  return (
    <>
      {/* DESKTOP PERSISTENT SIDEBAR */}
      <aside className={`hidden md:flex flex-col ${isSidebarCollapsed ? 'w-20 overflow-visible' : 'w-72 overflow-y-auto'} border-r h-full shrink-0 transition-all duration-300 shadow-sm ${
        isDark ? 'bg-[#16151A] border-[#2C2932]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
      }`}>
        <div className={`${isSidebarCollapsed ? 'pt-0 px-3 pb-3' : 'pt-0 px-6 pb-6'} flex flex-col justify-between min-h-full`}>
          <div>
            {/* Branding Header */}
            <div className="flex flex-col items-center justify-center text-center pt-[10px] pb-[10px] mb-1.5 transition-all duration-300">
              {!isSidebarCollapsed ? (
                <div className="flex items-center justify-center shrink-0">
                  <SanskritQuestLogo className="w-36 h-36" isDark={isDark} />
                </div>
              ) : (
                <div className="flex items-center justify-center shrink-0">
                  <SanskritQuestLogo className="w-10 h-10" isDark={isDark} />
                </div>
              )}
            </div>

            {/* Menu Items */}
            <div className="space-y-2">
              {!isSidebarCollapsed && (
                <span className={`text-[9px] font-mono block tracking-widest uppercase font-black px-2.5 mb-2 ${isDark ? 'text-[#726E7A]' : 'text-[#78716C]'}`}>
                  Core Navigation
                </span>
              )}
              {navItems.map((item) => {
                const isActive = activeNav === item.id;
                const Icon = item.icon;
                return (
                  <button
                    key={`desktop-nav-${item.id}`}
                    id={`desktop-nav-btn-${item.id}`}
                    onClick={() => handleNavClick(item.id)}
                    className={`relative group w-full transition-all cursor-pointer rounded-xl flex ${
                      isSidebarCollapsed 
                        ? 'flex-col items-center justify-center p-2.5 gap-1.5 text-center' 
                        : 'items-start gap-4 p-3 text-left'
                    } ${
                      isActive 
                        ? 'bg-[#D97706] text-white font-semibold shadow-sm' 
                        : isDark
                          ? 'text-[#A19E95] hover:bg-[#1E1D23] hover:text-[#F1EFF7]'
                          : 'text-[#57534E] hover:bg-[#E6E2D3]/30 hover:text-[#1C1917]'
                    }`}
                  >
                    <Icon 
                      className={`shrink-0 transition-transform duration-300 group-hover:scale-105 ${
                        isSidebarCollapsed ? 'w-5 h-5' : 'w-4 h-4 mt-0.5'
                      } ${isActive ? 'text-white' : 'text-[#D97706]'}`}
                      fill={isActive ? 'currentColor' : 'none'}
                    />
                    
                    {isSidebarCollapsed ? (
                      <span className="text-[9px] font-serif tracking-tight leading-none block whitespace-nowrap overflow-hidden text-ellipsis w-full">
                        {item.shortLabel}
                      </span>
                    ) : (
                      <div className="min-w-0 flex-1">
                        <span className="text-xs leading-none block">{item.label}</span>
                        <span className={`text-[10px] block mt-1 font-normal leading-tight opacity-80 ${isActive ? 'text-orange-100' : 'text-[#78716C]'}`}>
                          {item.description}
                        </span>
                      </div>
                    )}

                    {/* Floating tooltip when collapsed */}
                    {isSidebarCollapsed && (
                      <div className={`pointer-events-none absolute left-[86px] top-1/2 -translate-y-1/2 z-50 rounded-lg px-3 py-1.5 text-[10px] font-sans text-left whitespace-nowrap opacity-0 group-hover:opacity-100 transition-opacity duration-200 shadow-md border ${
                        isDark ? 'bg-[#1C1B20] text-amber-100 border-[#2C2932]' : 'bg-white text-[#1C1917] border-[#E6E2D3]'
                      }`}>
                        <div className="font-bold font-serif">{item.label}</div>
                        <div className={`text-[9px] font-normal opacity-75 mt-0.5 ${isDark ? 'text-gray-400' : 'text-gray-500'}`}>{item.description}</div>
                      </div>
                    )}
                  </button>
                );
              })}
            </div>
          </div>

          {/* Sidebar Bottom Integrated Settings Block */}
          {!isSidebarCollapsed ? (
            <div ref={sidebarSettingsContainerRef} className="border-t pt-4 border-[#E6E2D3] dark:border-[#2C2932] mt-auto">
              <button
                id="sidebar-settings-toggle"
                onClick={() => setIsSidebarSettingsExpanded(prev => !prev)}
                className={`w-full flex items-center justify-between p-2 rounded-xl transition-all cursor-pointer text-left mb-2 outline-none group ${
                  isDark 
                    ? 'hover:bg-[#201F25]/85 text-amber-200' 
                    : 'hover:bg-[#FAF8F4]/80 text-[#1C1917]'
                }`}
                title={isSidebarSettingsExpanded ? "Click to collapse scholar settings" : "Click to expand scholar settings"}
              >
                <div className="flex items-center gap-2">
                  <Settings className="w-4 h-4 text-[#D97706]" />
                  <span className="text-[10.5px] font-mono tracking-widest uppercase font-black text-amber-600 dark:text-amber-500">Scholar Settings</span>
                </div>
                {isSidebarSettingsExpanded ? (
                  <ChevronDown className="w-3.5 h-3.5 text-[#D97706]" />
                ) : (
                  <ChevronRight className="w-3.5 h-3.5 text-gray-400 dark:text-gray-500 group-hover:text-[#D97706]" />
                )}
              </button>
              
              <AnimatePresence initial={false}>
                {isSidebarSettingsExpanded && (
                  <motion.div
                    initial={{ height: 0, opacity: 0 }}
                    animate={{ height: "auto", opacity: 1 }}
                    exit={{ height: 0, opacity: 0 }}
                    transition={{ duration: 0.2, ease: "easeInOut" }}
                    className="overflow-hidden"
                  >
                    {/* Theme Block */}
                    <div className="mb-3.5 px-1 pt-1">
                      <span className="text-[9.5px] font-mono uppercase tracking-widest block mb-1 opacity-70">Interface Theme</span>
                      <div className="grid grid-cols-3 gap-1 rounded-lg border border-[#E6E2D3] dark:border-[#2C2932] p-0.5 bg-[#FAF8F4] dark:bg-[#1C1A20]">
                        {(['light', 'dark', 'system'] as const).map((mode) => {
                          const isSelected = themeMode === mode;
                          let ModeIcon = Sun;
                          if (mode === 'dark') ModeIcon = Moon;
                          if (mode === 'system') ModeIcon = Laptop;
                          return (
                            <button
                              key={`sidebar-theme-${mode}`}
                              id={`sidebar-theme-btn-${mode}`}
                              onClick={() => setThemeMode(mode)}
                              className={`py-1 text-[11px] font-semibold rounded flex flex-col items-center gap-0.5 justify-center transition-all cursor-pointer ${
                                isSelected
                                  ? 'bg-[#D97706] text-white shadow-sm font-bold'
                                  : isDark
                                    ? 'text-[#A19E95] hover:bg-[#25242D]'
                                    : 'text-[#57534E] hover:bg-[#E6E2D3]/40'
                              }`}
                            >
                              <ModeIcon className="w-3 h-3 shrink-0" />
                              <span className="capitalize text-[9px]">{mode}</span>
                            </button>
                          );
                        })}
                      </div>
                    </div>

                    {/* Font Size Block */}
                    <div className="px-1 pb-1">
                      <div className="flex justify-between items-center mb-1">
                        <span className="text-[9.5px] font-mono uppercase tracking-widest block opacity-70">Font Size</span>
                        <span className="text-[9px] font-serif font-black text-[#D97706] bg-amber-500/10 px-1 py-0.25 rounded">
                          {fontSize === 'small' ? 'Compact' : fontSize === 'normal' ? 'Scholar' : fontSize === 'large' ? 'Sadhana' : 'Deva XL'}
                        </span>
                      </div>
                      <div className="grid grid-cols-4 gap-1 rounded-lg border border-[#E6E2D3] dark:border-[#2C2932] p-0.5 bg-[#FAF8F4] dark:bg-[#1C1A20]">
                        {fontSizes.map((opt) => {
                          const isSelected = fontSize === opt.id;
                          return (
                            <button
                              key={`sidebar-font-${opt.id}`}
                              id={`sidebar-font-btn-${opt.id}`}
                              onClick={() => setFontSize(opt.id)}
                              className={`py-1.5 text-[11px] font-serif font-black rounded flex flex-col items-center justify-center transition-all cursor-pointer ${
                                isSelected
                                  ? 'bg-[#D97706] text-white shadow-sm'
                                  : isDark
                                    ? 'text-[#A19E95] hover:bg-[#25242D]'
                                    : 'text-[#57534E] hover:bg-[#E6E2D3]/40'
                              }`}
                              title={opt.desc}
                            >
                              <span className={opt.id === 'small' ? 'text-[9px]' : opt.id === 'normal' ? 'text-xs' : opt.id === 'large' ? 'text-sm' : 'text-base'}>A</span>
                            </button>
                          );
                        })}
                      </div>
                    </div>
                  </motion.div>
                )}
              </AnimatePresence>
            </div>
          ) : (
            /* Collapsed Sidebar: quiet settings button placeholder */
            <div className="flex flex-col items-center justify-center border-t pt-4 border-[#E6E2D3] dark:border-[#2C2932] mt-auto">
              <button
                onClick={() => setIsSidebarCollapsed(false)}
                className="p-2.5 rounded-xl text-amber-600 dark:text-amber-500 hover:bg-[#E6E2D3]/30 dark:hover:bg-[#1E1D23] transition-colors cursor-pointer"
                title="Expand Workspace Settings"
              >
                <Settings className="w-5 h-5 animate-spin-slow" />
              </button>
            </div>
          )}
        </div>
      </aside>

      {/* MOBILE OVERLAY NAVIGATION DRAWER */}
      <AnimatePresence>
        {mobileMenuOpen && (
          <motion.div 
            key="mobile-drawer-overlay"
            id="mobile-drawer-overlay"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="fixed inset-0 bg-black/40 z-30 md:hidden animate-fade-in" 
            onClick={() => setMobileMenuOpen(false)} 
          />
        )}
        {mobileMenuOpen && (
          <motion.div
            key="mobile-drawer-content"
            id="mobile-drawer-content"
            initial={{ opacity: 0, x: -100 }}
            animate={{ opacity: 1, x: 0 }}
            exit={{ opacity: 0, x: -100 }}
            transition={{ type: 'spring', damping: 20 }}
            className={`fixed inset-y-0 left-0 w-64 pt-20 pb-6 px-4 z-30 border-r flex flex-col justify-between md:hidden transition-colors ${
              isDark ? 'bg-[#16151A] border-[#2B2831]' : 'bg-[#F9F7F2] border-[#E6E2D3]'
            }`}
          >
              <div className="space-y-6">
                <div className="space-y-1.5">
                  <span className={`text-[9px] font-mono block tracking-widest uppercase font-black px-2 ${isDark ? 'text-[#726E7A]' : 'text-gray-400'}`}>
                    Navigation Menu
                  </span>
                  {navItems.map((item) => {
                    const isActive = activeNav === item.id;
                    const Icon = item.icon;
                    return (
                      <button
                        key={`mobile-nav-${item.id}`}
                        id={`mobile-nav-btn-${item.id}`}
                        onClick={() => {
                          handleNavClick(item.id);
                          setMobileMenuOpen(false);
                        }}
                        className={`w-full text-left p-3 rounded-lg flex items-center gap-3 transition-colors cursor-pointer ${
                          isActive 
                            ? 'bg-[#D97706] text-white font-bold shadow-sm' 
                            : isDark
                              ? 'text-[#A19E95] hover:bg-[#1E1D23] hover:text-[#F1EFF7]'
                              : 'text-[#57534E] hover:bg-[#E6E2D3]/30 hover:text-[#1C1917]'
                        }`}
                      >
                        <Icon className={`w-4 h-4 shrink-0 ${isActive ? 'text-white' : 'text-[#D97706]'}`} />
                        <div>
                          <span className="text-xs font-semibold block">{item.label}</span>
                        </div>
                      </button>
                    );
                  })}
                </div>
              </div>

              {/* Mobile settings integrated */}
              <div className="border-t pt-4 border-[#E6E2D3]/60 dark:border-[#2C2932] mt-auto">
                <div className="flex items-center gap-2 mb-3">
                  <Settings className="w-4 h-4 text-[#D97706]" />
                  <span className="text-[10px] font-mono tracking-widest uppercase font-black text-amber-600 dark:text-amber-500">Scholar Settings</span>
                </div>
                
                {/* Theme select (Mobile) */}
                <div className="mb-3.5">
                  <span className="text-[10px] font-mono uppercase tracking-widest block mb-1 opacity-70">Interface Theme</span>
                  <div className="grid grid-cols-3 gap-1 rounded-lg border border-[#E6E2D3] dark:border-[#2C2932] p-0.5 bg-[#FAF8F4] dark:bg-[#100F12]">
                    {(['light', 'dark', 'system'] as const).map((mode) => {
                      const isSelected = themeMode === mode;
                      let ModeIcon = Sun;
                      if (mode === 'dark') ModeIcon = Moon;
                      if (mode === 'system') ModeIcon = Laptop;
                      return (
                        <button
                          key={`mobile-theme-${mode}`}
                          id={`mobile-theme-btn-${mode}`}
                          onClick={() => setThemeMode(mode)}
                          className={`py-1 text-[10px] font-semibold rounded flex flex-col items-center justify-center transition-all cursor-pointer ${
                            isSelected
                              ? 'bg-[#D97706] text-white shadow-sm font-bold'
                              : isDark
                                ? 'text-[#A19E95] hover:bg-[#25242D]'
                                : 'text-[#57534E] hover:bg-[#E6E2D3]/40'
                          }`}
                        >
                          <ModeIcon className="w-3.5 h-3.5 mb-0.5" />
                          <span className="capitalize">{mode}</span>
                        </button>
                      );
                    })}
                  </div>
                </div>

                {/* Font Size select (Mobile) */}
                <div>
                  <div className="flex justify-between items-center mb-1">
                    <span className="text-[10px] font-mono uppercase tracking-widest block opacity-70">Font Size</span>
                    <span className="text-[9px] font-serif font-black text-[#D97706] bg-amber-500/10 px-1 py-0.25 rounded">
                      {fontSize === 'small' ? 'Compact' : fontSize === 'normal' ? 'Scholar' : fontSize === 'large' ? 'Sadhana' : 'Deva XL'}
                    </span>
                  </div>
                  <div className="grid grid-cols-4 gap-1 rounded-lg border border-[#E6E2D3] dark:border-[#2C2932] p-0.5 bg-[#FAF8F4] dark:bg-[#100F12]">
                    {fontSizes.map((opt) => {
                      const isSelected = fontSize === opt.id;
                      return (
                        <button
                          key={`mobile-font-${opt.id}`}
                          id={`mobile-font-btn-${opt.id}`}
                          onClick={() => setFontSize(opt.id)}
                          className={`py-1.5 text-[11px] font-serif font-black rounded flex flex-col items-center justify-center transition-all cursor-pointer ${
                            isSelected
                              ? 'bg-[#D97706] text-white shadow-sm'
                              : isDark
                                ? 'text-[#A19E95] hover:bg-[#25242D]'
                                : 'text-[#57534E] hover:bg-[#E6E2D3]/40'
                          }`}
                        >
                          <span className={opt.id === 'small' ? 'text-[9px]' : opt.id === 'normal' ? 'text-xs' : opt.id === 'large' ? 'text-sm' : 'text-base'}>A</span>
                        </button>
                      );
                    })}
                  </div>
                </div>
              </div>
            </motion.div>
        )}
      </AnimatePresence>
    </>
  );
}
