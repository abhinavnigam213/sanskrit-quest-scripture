import React from 'react';

interface SanskritQuestLogoProps {
  className?: string;
  isDark?: boolean;
}

export default function SanskritQuestLogo({ className = "w-12 h-12", isDark = false }: SanskritQuestLogoProps) {
  return (
    <img 
      src="/sq_logo_en_borderless.png" 
      alt="Sanskrit Quest Logo" 
      className={`${className} object-contain transition-all duration-300 hover:scale-105 shrink-0`}
      style={{ display: "inline-block" }}
      referrerPolicy="no-referrer"
    />
  );
}
