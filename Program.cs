using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lingvistika1
{
    class Program
    {
        static void Canonicalization(StringBuilder givenWord)
        {
            for (int i = 0; i < givenWord.Length; i++)
            {
                if (givenWord[i] == 'ё')
                    givenWord[i] = 'е';
            }
        }

        static int IndexOfTheFirstVowel(StringBuilder givenWord)
        {
            char[] vowels = { 'а', 'е', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };
            int indexOfTheFirstVowel = -1;
            for (int i = 0; i < givenWord.Length; i++)
            {
                for (int j = 0; j < vowels.Length; j++)
                    if (givenWord[i] == vowels[j])
                    {
                        indexOfTheFirstVowel = i;
                        i = givenWord.Length;
                        j = vowels.Length;
                    }
            }
            return indexOfTheFirstVowel;
        }

        static bool regexHandle(StringBuilder input, string[] gerundGroup, string template)
        {
            bool isRegexHandled = false;
            if (template.Length > 0)
                template = ".*" + template;
            else
                template = ".*";
            for (int i = 0; i < gerundGroup.Length && !isRegexHandled; i++)
            {
                Regex regex = new Regex(template + gerundGroup[i] + "$");
                Match match = regex.Match(input.ToString());
                if (match.Success && !isRegexHandled)
                {
                    isRegexHandled = true;
                    input = input.Remove(input.Length - gerundGroup[i].Length, gerundGroup[i].Length);
                }
            }
            return isRegexHandled;
        }

        static bool regexHandle(StringBuilder input, string[] gerundGroup)
        {
            bool isRegexHandled = false;
            for (int i = 0; i < gerundGroup.Length && !isRegexHandled; i++)
            {
                Regex regex = new Regex(".*" + gerundGroup[i] + "$");
                Match match = regex.Match(input.ToString());
                if (match.Success && !isRegexHandled)
                {
                    isRegexHandled = !isRegexHandled;
                    input = input.Remove(input.Length - gerundGroup[i].Length, gerundGroup[i].Length);
                }
            }
            return isRegexHandled;
        }

        static bool PerfectiveGerundHandle(StringBuilder RV)
        {
            string[] perfectivegroup1 = { "в", "вши", "вшись" };
            string[] perfectivegroup2 = { "ив", "ивши", "ившись", "ыв", "ывши", "ывшись" };
            return regexHandle(RV, perfectivegroup1, "[а|я]") ^ regexHandle(RV, perfectivegroup2);
        }

        static bool ReflexiveGerundHandle(StringBuilder RV)
        {
            string[] reflexive = { "ся", "сь" };
            return regexHandle(RV, reflexive);
        }

        static bool AdjectivalGerundHandle(StringBuilder RV, StringBuilder givenWord, int indexOfTheFirstVowel)
        {
            string[] adjective = { "ее", "ие", "ые", "ое", "ими", "ыми", "ей", "ий", "ый", "ой", "ем", "им", "ым", "ом", "его", "ого", "ему", "ому", "их", "ых", "ую", "юю", "ая", "яя", "ою", "ею" };
            string[] participlegroup1 = { "ем", "нн", "вш", "ющ", "щ" };
            string[] participlegroup2 = { "ивш", "ывш", "ующ", "" }; //добавил пустую строку, чтобы проверить только на adjective(обязательно нужно ее добавлять во вторую группу, т.к. в первой в проверке учитывается [а|я]
            string adjectival = "";
            int lengthOfAdjectival = 0;
            int matchIndex = 0;
            int matchLength = 0;
            for (int i = 0; i < adjective.Length; i++)
            {
                for (int j = 0; j < participlegroup1.Length; j++)
                {
                    Regex regex = new Regex(".*[а|я]" + participlegroup1[j] + adjective[i] + "$");
                    Match match = regex.Match(RV.ToString());

                    if (match.Success && participlegroup2[j].Length + adjective[i].Length > lengthOfAdjectival)
                    {
                        lengthOfAdjectival = participlegroup1[j].Length + adjective[i].Length;
                        adjectival = participlegroup1[j] + adjective[i];
                        matchIndex = match.Index;
                        matchLength = match.Length;
                    }
                }

                for (int j = 0; j < participlegroup2.Length; j++)
                {
                    Regex regex = new Regex(".*" + participlegroup2[j] + adjective[i] + "$");
                    Match match = regex.Match(RV.ToString());
                    if (match.Success && participlegroup2[j].Length + adjective[i].Length > lengthOfAdjectival)
                    {
                        lengthOfAdjectival = participlegroup2[j].Length + adjective[i].Length;
                        adjectival = participlegroup2[j] + adjective[i];
                        matchIndex = match.Index;
                        matchLength = match.Length;
                    }
                }
            }
            if (lengthOfAdjectival > 0)
            {
                RV = RV.Remove(RV.Length - lengthOfAdjectival, lengthOfAdjectival);
                return true;
            }
            else return false;
        }

        static bool VerbGerundHandle(StringBuilder RV)
        {
            string[] verbgroup1 = { "ла", "на", "ете", "йте", "ли", "й", "л", "ем", "н", "ло", "но", "ет", "ют", "ны", "ть", "ешь", "нно" };
            string[] verbgroup2 = { "ила", "ыла", "ена", "ейте", "уйте", "ите", "или", "ыли", "ей", "уй", "ил", "ыл", "им", "ым", "ен", "ило",
                "ыло", "ено", "ят", "ует", "уют", "ит", "ыт", "ены", "ить", "ыть", "ишь", "ую", "ю"};
            return regexHandle(RV, verbgroup1, "[а|я]") ^ regexHandle(RV, verbgroup2);
        }

        static bool NounGerundHandle(StringBuilder RV)
        {
            string[] noun = { "а", "ев", "ов", "ие", "ье", "е", "иями", "ями", "ами", "еи", "ии", "и", "ией", "ей", "ой", "ий", "й", "иям", "ям", "ием", "ем", "ам", "ом", "о",
                "у", "ах", "иях", "ях", "ы", "ь", "ию", "ью", "ю", "ия", "ья", "я" };
            return regexHandle(RV, noun);
        }

        static bool DerivationalGerundHandle(StringBuilder R2)
        {
            string[] derivational = { "ост", "ость" };
            return regexHandle(R2, derivational);
        }

        static bool SuperlativeGerundHandle(StringBuilder RV)
        {
            string[] superlative = { "ейш", "ейше" };
            return regexHandle(RV, superlative);
        }

        static bool DoubleNHandle(StringBuilder RV)
        {
            if (RV.Length > 2)
            {
                if (RV[RV.Length - 2] == 'н' && RV[RV.Length - 1] == 'н')
                {
                    RV.Remove(RV.Length - 1, 1);
                    return true;
                }
            }
            return false;
        }

        static bool SoftSignHandle(StringBuilder RV)
        {
            if (RV.Length > 0 && RV[RV.Length - 1] == 'ь')
                RV.Remove(RV.Length - 1, 1);
            else return false;
            return true;
        }

        static void stepOne(StringBuilder RV, StringBuilder givenWord, int indexOfTheFirstVowel)
        {
            if (!PerfectiveGerundHandle(RV))
            {
                ReflexiveGerundHandle(RV);
                if (!AdjectivalGerundHandle(RV, givenWord, indexOfTheFirstVowel))
                    if (!VerbGerundHandle(RV))
                        NounGerundHandle(RV);
            }
        }

        static void stepTwo(StringBuilder RV)
        {
            if (RV.Length > 0 && RV[RV.Length - 1] == 'и')
                RV.Remove(RV.Length - 1, 1);
        }

        static void stepThree(StringBuilder givenWord, StringBuilder RV)
        {
            char[] vowels = { 'а', 'е', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };
            Regex regex = new Regex(".*" + RV.ToString());
            StringBuilder R2 = new StringBuilder(regex.Match(givenWord.ToString()).Value);
            regex = new Regex("[аеиоуыэюя][^аеиоуыэюя].*");
            if (R2.Length >= 2)
            {
                R2.Remove(0, regex.Match(R2.ToString()).Index + 2);
                if (R2.Length >= 2)
                {
                    R2.Remove(0, regex.Match(R2.ToString()).Index + 2);
                    DerivationalGerundHandle(R2);
                    regex = new Regex("^.*" + R2.ToString());
                    RV.Replace(RV.ToString(), regex.Match(RV.ToString()).Value);
                }
            }
        }

        static void stepFour(StringBuilder givenWord, StringBuilder RV, int indexOfTheFirstVowel)
        {
            if (!DoubleNHandle(RV))
                if (!SuperlativeGerundHandle(RV) && !DoubleNHandle(RV))
                    SoftSignHandle(RV);
            /*
            Regex regex = new Regex("^.*" + RV.ToString());
            givenWord.Replace(givenWord.ToString(), regex.Match(givenWord.ToString()).Value); 
            
            не проходит тест (валил, валился)
            
            Console.WriteLine(new Regex("^.*?л").Match("валился"));
            */
            givenWord.Remove(indexOfTheFirstVowel + RV.Length + 1, givenWord.Length - indexOfTheFirstVowel - RV.Length - 1);
        }

        static string Stemming(string word)
        {
            StringBuilder givenWord = new StringBuilder(word, 100);
            Canonicalization(givenWord);
            int indexOfTheFirstVowel = IndexOfTheFirstVowel(givenWord);
            StringBuilder RV = new StringBuilder(givenWord.ToString(), 100);
            RV.Remove(0, indexOfTheFirstVowel + 1);
            stepOne(RV, givenWord, indexOfTheFirstVowel);
            stepTwo(RV);
            stepThree(givenWord, RV);
            stepFour(givenWord, RV, indexOfTheFirstVowel);
            return givenWord.ToString();
        }

        static void CorrectStemming()
        {
            string[] words = { "в","вавиловка","вагнера","вагон","вагона","вагоне","вагонов","вагоном","вагоны","важная","важнее","важнейшие","важнейшими",
            "важничал","важно","важного","важное","важной","важном","важному","важности","важностию","важность","важностью","важную","важны","важные","важный","важным",
            "важных","вазах","вазы","вакса","вакханка","вал","валандался","валентина","валериановых","валерию","валетами","вали","валил","валился","валится","валов",
            "вальдшнепа","вальс","вальса","вальсе","вальсишку","вальтера","валяется","валялась","валялись","валялось","валялся","валять","валяются","вам","вами",
            "п","па","пава","павел","павильон","павильонам","павла","павлиний","павлиньи","павлиньим","павлович","павловна","павловне","павловной","павловну","павловны",
            "павловцы","павлыч","павлыча","пагубная","падает","падай","падал","падала","падаль","падать","падаю","падают","падающего","падающие","падеж",
            "падение","падением","падении","падений","падения","паденье","паденья","падет","падут","падучая","падчерицей","падчерицы","падшая","падшей","падшему",
            "падший","падшим","падших","падшую","паек","пазухи","пазуху","пай","пакет","пакетом","пакеты","пакостей","пакостно","пал"
            };
            string[] stems =
            { "в","вавиловк","вагнер","вагон","вагон","вагон","вагон","вагон","вагон","важн","важн","важн","важн","важнича","важн","важн","важн","важн","важн","важн",
            "важност","важност","важност","важност","важн","важн","важн","важн","важн","важн","ваз","ваз","вакс","вакханк","вал","валанда","валентин","валерианов","валер",
            "валет","вал","вал","вал","вал","вал","вальдшнеп","вальс","вальс","вальс","вальсишк","вальтер","валя","валя","валя","валя","валя","валя","валя","вам","вам",
            "п","па","пав","павел","павильон","павильон","павл","павлин","павлин","павлин","павлович","павловн","павловн","павловн","павловн","павловн","павловц","павлыч",
            "павлыч","пагубн","пада","пада","пада","пада","падал","пада","пада","пада","пада","пада","падеж","паден","паден","паден","паден","паден","паден","паден","падет",
            "падут","падуч","падчериц","падчериц","падш","падш","падш","падш","падш","падш","падш","паек","пазух","пазух","па","пакет","пакет","пакет","пакост","пакостн","пал"
            };
            bool correct = true;
            for (int i = 0; i < words.Length && correct; i++)
            {
                Console.WriteLine("correct word:" + words[i] + "\t correct stem: " + stems[i]);
                Console.WriteLine("result: " + Stemming(words[i]));
                Console.WriteLine(String.Equals(Stemming(words[i]), stems[i]));
                Console.WriteLine("******************************************************************");
                if (!String.Equals(Stemming(words[i]), stems[i]) && correct)
                    correct = !correct;
            }
            if (correct)
                Console.WriteLine("\n\n\nALL TESTS CORRECT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n\n\n");
        }

        static void Main(string[] args)
        {
            CorrectStemming();
            Console.WriteLine("введите слово");
            Console.WriteLine(Stemming(Console.ReadLine()));
            Console.ReadKey();
        }
    }
}
