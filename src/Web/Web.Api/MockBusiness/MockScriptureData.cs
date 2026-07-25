using System;
using System.Collections.Generic;
using System.Linq;
using SanskritQuest.Common.Contracts;
using SanskritQuest.Web.Api.Models;

namespace SanskritQuest.Web.Api.MockBusiness;

// TODO: Remove this mock implementation in the future once the actual business layer is implemented.
public static class MockScriptureData
{
    public static readonly List<ScriptureResponse> AllScriptures = new()
    {
        new ScriptureResponse
        {
            ScriptureId = 1,
            Code = "BG",
            Titles = new LocalizedTitles { En = "Bhagavad Gita", Hi = "भगवद्गीता", Sa = "भगवद्गीता" },
            Author = new LocalizedDescription { En = "Vyasa", Hi = "व्यास" },
            Description = new LocalizedDescription
            {
                En = "The Bhagavad Gita is a 700-verse Hindu scripture that is part of the epic Mahabharata.",
                Hi = "भगवद्गीता एक ७०० श्लोकों वाला हिंदू ग्रंथ है जो महाभारत महाकाव्य का हिस्सा है।"
            },
            CategoryId = 2,
            CategoryName = "Smriti",
            ClassId = 1,
            ClassName = "Itihasa",
            SourceId = 1,
            SearchWeight = 100,
            MetaTags = new List<string> { "Gita", "Karma Yoga", "Bhakti Yoga", "Jnana Yoga" },
            EnumName = "BhagavadGita"
        },
        new ScriptureResponse
        {
            ScriptureId = 2,
            Code = "VR",
            Titles = new LocalizedTitles { En = "Valmiki Ramayana", Hi = "वाल्मीकि रामायण", Sa = "वाल्मीकि रामायणम्" },
            Author = new LocalizedDescription { En = "Valmiki", Hi = "वाल्मीकि" },
            Description = new LocalizedDescription
            {
                En = "The Ramayana is an ancient Sanskrit epic, historically attributed to the sage Valmiki.",
                Hi = "रामायण एक प्राचीन संस्कृत महाकाव्य है, जो ऐतिहासिक रूप से ऋषि वाल्मीकि को समर्पित है।"
            },
            CategoryId = 2,
            CategoryName = "Smriti",
            ClassId = 1,
            ClassName = "Itihasa",
            SourceId = 2,
            SearchWeight = 90,
            MetaTags = new List<string> { "Ramayana", "Rama", "Dharma" },
            EnumName = "ValmikiRamayana"
        }
    };

    public static List<ScriptureHierarchyNodeResponse> GetMockHierarchy(int scriptureId, string enumName)
    {
        var hierarchy = new List<ScriptureHierarchyNodeResponse>();
        if (scriptureId == 1) // Bhagavad Gita
        {
            hierarchy.Add(new ScriptureHierarchyNodeResponse
            {
                HierarchyId = 10,
                ParentId = null,
                LocalLabel = "Chapter 1",
                Path = "1",
                NodeType = HierarchyNodeType.Adhyaya,
                Titles = new LocalizedTitles { En = "Arjuna Vishada Yoga", Hi = "अर्जुनविषादयोग", Sa = "अर्जुनविषादयोगः" },
                Description = new LocalizedDescription { En = "The Distress of Arjuna", Hi = "अर्जुन की विषाद अवस्था" },
                SequenceNumber = 1,
                DirectVerseCount = 0,
                RecursiveVerseCount = 47,
                FullHierarchyLabel = $"{enumName}.Chapter 1",
                Children = new List<ScriptureHierarchyNodeResponse>()
            });
        }
        else if (scriptureId == 2) // Valmiki Ramayana
        {
            var balaKanda = new ScriptureHierarchyNodeResponse
            {
                HierarchyId = 100,
                ParentId = null,
                LocalLabel = "Bala Kanda",
                Path = "1",
                NodeType = HierarchyNodeType.Kanda,
                Titles = new LocalizedTitles { En = "Book of Youth", Hi = "बालकाण्ड", Sa = "बालकाण्डम्" },
                Description = new LocalizedDescription { En = "The childhood and early life of Rama", Hi = "श्रीराम के बाल्यकाल की कथा" },
                SequenceNumber = 1,
                DirectVerseCount = 0,
                RecursiveVerseCount = 2234,
                FullHierarchyLabel = $"{enumName}.Bala Kanda"
            };

            balaKanda.Children.Add(new ScriptureHierarchyNodeResponse
            {
                HierarchyId = 101,
                ParentId = 100,
                LocalLabel = "Sarga 1",
                Path = "1/1",
                NodeType = HierarchyNodeType.Sarga,
                Titles = new LocalizedTitles { En = "Inquiry about the Ideal Man", Hi = "आदिकाव्य की भूमिका", Sa = "प्रथमः सर्गः" },
                Description = new LocalizedDescription { En = "Narada narrates the qualities of Rama to Valmiki", Hi = "नारद जी द्वारा वाल्मीकि जी को राम के गुणों का वर्णन" },
                SequenceNumber = 1,
                DirectVerseCount = 100,
                RecursiveVerseCount = 100,
                FullHierarchyLabel = $"{enumName}.Bala Kanda.Sarga 1"
            });

            balaKanda.Children.Add(new ScriptureHierarchyNodeResponse
            {
                HierarchyId = 102,
                ParentId = 100,
                LocalLabel = "Sarga 2",
                Path = "1/2",
                NodeType = HierarchyNodeType.Sarga,
                Titles = new LocalizedTitles { En = "The Birth of the Shloka Meter", Hi = "क्रौंचवध और श्लोक उत्पत्ति", Sa = "द्वितीयः सर्गः" },
                Description = new LocalizedDescription { En = "Valmiki witnesses the killing of a Krauncha bird and utters the first shloka", Hi = "क्रौंच पक्षी के वध पर वाल्मीकि जी के मुख से प्रथम श्लोक फूटना" },
                SequenceNumber = 2,
                DirectVerseCount = 43,
                RecursiveVerseCount = 43,
                FullHierarchyLabel = $"{enumName}.Bala Kanda.Sarga 2"
            });

            var ayodhyaKanda = new ScriptureHierarchyNodeResponse
            {
                HierarchyId = 200,
                ParentId = null,
                LocalLabel = "Ayodhya Kanda",
                Path = "2",
                NodeType = HierarchyNodeType.Kanda,
                Titles = new LocalizedTitles { En = "Book of Ayodhya", Hi = "अयोध्याकाण्ड", Sa = "अयोध्याकाण्डम्" },
                Description = new LocalizedDescription { En = "Preparations for Rama's coronation and exile", Hi = "राम के राज्याभिषेक की तैयारी और वनवास" },
                SequenceNumber = 2,
                DirectVerseCount = 0,
                RecursiveVerseCount = 4170,
                FullHierarchyLabel = $"{enumName}.Ayodhya Kanda"
            };

            ayodhyaKanda.Children.Add(new ScriptureHierarchyNodeResponse
            {
                HierarchyId = 201,
                ParentId = 200,
                LocalLabel = "Sarga 1",
                Path = "2/1",
                NodeType = HierarchyNodeType.Sarga,
                Titles = new LocalizedTitles { En = "Virtues of Prince Rama", Hi = "राम के गुणों का गान", Sa = "प्रथमः सर्गः" },
                Description = new LocalizedDescription { En = "Description of Rama's noble virtues and popularity", Hi = "दशरथ द्वारा राम के गुणों का दर्शन एवं युवराज बनाने का विचार" },
                SequenceNumber = 1,
                DirectVerseCount = 80,
                RecursiveVerseCount = 80,
                FullHierarchyLabel = $"{enumName}.Ayodhya Kanda.Sarga 1"
            });

            hierarchy.Add(balaKanda);
            hierarchy.Add(ayodhyaKanda);
        }
        return hierarchy;
    }

    public static VerseDetailsResponse GetMockVerseResponse(string scriptureCode, int[] levelNumbers)
    {
        var hierarchy = new List<HierarchyLevel>();
        
        if (scriptureCode.Equals("BG", StringComparison.OrdinalIgnoreCase))
        {
            hierarchy.Add(new HierarchyLevel { LevelType = "Adhyaya", LevelNumber = levelNumbers.ElementAtOrDefault(0), LevelNameSanskrit = "अर्जुनविषादयोगः" });
            hierarchy.Add(new HierarchyLevel { LevelType = "Shloka", LevelNumber = levelNumbers.ElementAtOrDefault(1), LevelNameSanskrit = "श्लोक " + levelNumbers.ElementAtOrDefault(1) });
        }
        else
        {
            hierarchy.Add(new HierarchyLevel { LevelType = "Kanda", LevelNumber = levelNumbers.ElementAtOrDefault(0), LevelNameSanskrit = "बालकाण्डम्" });
            hierarchy.Add(new HierarchyLevel { LevelType = "Sarga", LevelNumber = levelNumbers.ElementAtOrDefault(1) });
            hierarchy.Add(new HierarchyLevel { LevelType = "Shloka", LevelNumber = levelNumbers.ElementAtOrDefault(2) });
        }

        return new VerseDetailsResponse
        {
            ScriptureName = scriptureCode.Equals("BG", StringComparison.OrdinalIgnoreCase) ? "Bhagavad Gita" : "Valmiki Ramayana",
            Hierarchy = hierarchy,
            SanskritShloka = "धर्मक्षेत्रे कुरुक्षेत्रे समवेता युयुत्सवः।\nमामकाः पाण्डवाश्चैव किमकुर्वत सञ्जय॥",
            Transliteration = "dharmakṣetre kurukṣetre samavetā yuyutsavaḥ |\nmāmakāḥ pāṇāvaścai'va kimakurvata sañjaya ||",
            Translation = new Translation
            {
                TranslationEn = "Dhritarashtra said: O Sanjay, after gathering on the holy field of Kurukshetra, and desiring to fight, what did my sons and the sons of Pandu do?",
                TranslationHi = "धृतराष्ट्र ने कहा: हे संजय! धर्मभूमि कुरुक्षेत्र में युद्ध की इच्छा से एकत्र हुए मेरे और पाण्डु के पुत्रों ने क्या किया?"
            },
            WordByWordBreakdown = new List<WordBreakdown>()
            {
                new() { SanskritWord = "धर्मक्षेत्रे", Transliteration = "dharmakṣetre", TranslationEn = "on the field of righteousness", TranslationHi = "धर्मभूमि में" },
                new() { SanskritWord = "कुरुक्षेत्रे", Transliteration = "kurukṣetre", TranslationEn = "on the field of Kuru", TranslationHi = "कुरुक्षेत्र में" }
            },
            Commentaries = new List<Commentary>()
            {
                new() { Author = "Adi Shankaracharya", EnglishCommentary = "Commentary in English...", HindiCommentary = "हिन्दी में व्याख्या..." }
            }
        };
    }
}
