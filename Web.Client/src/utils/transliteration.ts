export function devanagariToSlp1(text: string): string {
  const vowels: { [key: string]: string } = {
    'अ': 'a', 'आ': 'A', 'इ': 'i', 'ई': 'I', 'उ': 'u', 'ऊ': 'U', 'ऋ': 'f', 'ॠ': 'F', 'ऌ': 'x', 'ॡ': 'X',
    'ए': 'e', 'ऐ': 'E', 'ओ': 'o', 'औ': 'O'
  };

  const matras: { [key: string]: string } = {
    'ा': 'A', 'ि': 'i', 'ी': 'I', 'ु': 'u', 'ू': 'U', 'ृ': 'f', 'ॄ': 'F', 'ॢ': 'x', 'ॣ': 'X',
    'े': 'e', 'ै': 'E', 'ो': 'o', 'ौ': 'O'
  };

  const consonants: { [key: string]: string } = {
    'क': 'k', 'ख': 'K', 'ग': 'g', 'घ': 'G', 'ङ': 'N',
    'च': 'c', 'छ': 'C', 'ज': 'j', 'झ': 'J', 'ञ': 'Y',
    'ट': 'w', 'ठ': 'W', 'ड': 'q', 'ढ': 'Q', 'ण': 'R',
    'त': 't', 'थ': 'T', 'द': 'd', 'ध': 'D', 'न': 'n',
    'प': 'p', 'फ': 'P', 'ब': 'b', 'भ': 'B', 'म': 'm',
    'य': 'y', 'र': 'r', 'ल': 'l', 'व': 'v',
    'श': 'S', 'ष': 'z', 'स': 's', 'ह': 'h',
    'ळ': 'L', 'क्ष': 'kz', 'त्र': 'tr', 'ज्ञ': 'jY'
  };

  const shunya: { [key: string]: string } = {
    'ं': 'M',
    'ः': 'H',
    'ँ': '~'
  };

  let result = '';
  for (let i = 0; i < text.length; i++) {
    const char = text[i];
    const nextChar = text[i + 1];

    if (char === 'ॐ') {
      result += 'oM';
      continue;
    }

    if (consonants[char] !== undefined) {
      let base = consonants[char];
      if (nextChar === '्') {
        result += base;
        i++; // skip halant
      } else if (matras[nextChar] !== undefined) {
        result += base + matras[nextChar];
        i++; // skip matra
      } else {
        result += base + 'a';
      }
    } else if (vowels[char] !== undefined) {
      result += vowels[char];
    } else if (shunya[char] !== undefined) {
      result += shunya[char];
    } else {
      result += char === '।' ? '|' : char === '॥' ? '||' : char === 'ऽ' ? "'" : char;
    }
  }
  return result;
}

export function devanagariToIast(text: string): string {
  const vowels: { [key: string]: string } = {
    'अ': 'a', 'आ': 'ā', 'इ': 'i', 'ई': 'ī', 'उ': 'u', 'ऊ': 'ū', 'ऋ': 'ṛ', 'ॠ': 'ṝ', 'ऌ': 'ḷ', 'ॡ': 'ḹ',
    'ए': 'e', 'ऐ': 'ai', 'ओ': 'o', 'औ': 'au'
  };

  const matras: { [key: string]: string } = {
    'ा': 'ā', 'ि': 'i', 'ी': 'ī', 'ु': 'u', 'ू': 'ū', 'ृ': 'ṛ', 'ॄ': 'ṝ', 'ॢ': 'ḷ', 'ॣ': 'ḹ',
    'े': 'e', 'ै': 'ai', 'ो': 'o', 'ौ': 'au'
  };

  const consonants: { [key: string]: string } = {
    'क': 'k', 'ख': 'kh', 'ग': 'g', 'घ': 'gh', 'ङ': 'ṅ',
    'च': 'c', 'छ': 'ch', 'ज': 'j', 'झ': 'jh', 'ञ': 'ñ',
    'ट': 'ṭ', 'ठ': 'ṭh', 'ड': 'ḍ', 'ढ': 'ḍh', 'ण': 'ṇ',
    'त': 't', 'थ': 'th', 'द': 'd', 'ध': 'dh', 'न': 'n',
    'प': 'p', 'फ': 'ph', 'ब': 'b', 'भ': 'bh', 'म': 'm',
    'य': 'y', 'र': 'r', 'ल': 'l', 'व': 'v',
    'श': 'ś', 'ष': 'ṣ', 'स': 's', 'ह': 'h',
    'ळ': 'ḷ', 'क्ष': 'kṣ', 'त्र': 'tr', 'ज्ञ': 'jñ'
  };

  const shunya: { [key: string]: string } = {
    'ं': 'ṁ',
    'ः': 'ḥ',
    'ँ': 'm̐'
  };

  let result = '';
  for (let i = 0; i < text.length; i++) {
    const char = text[i];
    const nextChar = text[i + 1];

    if (char === 'ॐ') {
      result += 'oṁ';
      continue;
    }

    if (consonants[char] !== undefined) {
      let base = consonants[char];
      if (nextChar === '्') {
        result += base;
        i++; // skip halant
      } else if (matras[nextChar] !== undefined) {
        result += base + matras[nextChar];
        i++; // skip matra
      } else {
        result += base + 'a';
      }
    } else if (vowels[char] !== undefined) {
      result += vowels[char];
    } else if (shunya[char] !== undefined) {
      result += shunya[char];
    } else {
      result += char === '।' ? '|' : char === '॥' ? '||' : char;
    }
  }
  return result;
}

export function iastToPhonetic(iast: string): string {
  return iast
    .replace(/ś/g, 'sh')
    .replace(/Ś/g, 'Sh')
    .replace(/ṣ/g, 'sh')
    .replace(/Ṣ/g, 'Sh')
    .replace(/ṛ/g, 'ri')
    .replace(/Ṛ/g, 'Ri')
    .replace(/ā/g, 'a')
    .replace(/Ā/g, 'A')
    .replace(/ī/g, 'i')
    .replace(/Ī/g, 'I')
    .replace(/ū/g, 'u')
    .replace(/Ū/g, 'U')
    .replace(/ṭ/g, 't')
    .replace(/Ṭ/g, 'T')
    .replace(/ḍ/g, 'd')
    .replace(/Ḍ/g, 'D')
    .replace(/ṇ/g, 'n')
    .replace(/Ṇ/g, 'N')
    .replace(/ṁ/g, 'm')
    .replace(/Ṁ/g, 'M')
    .replace(/ḥ/g, 'h')
    .replace(/Ḥ/g, 'H')
    .replace(/\|/g, '')
    .replace(/\s+/g, ' ')
    .trim();
}

/**
 * Backwards compatibility helper wrapper around devanagariToSlp1.
 */
export function webDevanagariToSlp1(text: string): string {
  return devanagariToSlp1(text);
}

/**
 * Basic offline converter mapping IAST diacritics to keyboard-friendly ITRANS markup.
 */
export function iastToItrans(iast: string): string {
  return iast
    .toLowerCase()
    .replace(/ā/g, 'aa')
    .replace(/ī/g, 'ii')
    .replace(/ū/g, 'uu')
    .replace(/ṛ/g, 'RRi')
    .replace(/ñ/g, '~n')
    .replace(/ṅ/g, 'N')
    .replace(/ś/g, 'sh_')
    .replace(/ṣ/g, 'Sh')
    .replace(/ṭ/g, 'T')
    .replace(/ḍ/g, 'D')
    .replace(/ṇ/g, 'N')
    .replace(/ḥ/g, 'H')
    .replace(/ṁ/g, 'M');
}

