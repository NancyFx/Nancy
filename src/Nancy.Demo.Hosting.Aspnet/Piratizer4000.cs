namespace Yarrrr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Pirate dictionary be plundered from Davie Reed ( http://nifty.stanford.edu/2004/TalkLikeAPirate/pirate3.html )
    /// Yarrr!
    /// </summary>
    public static class HereBePiratesYarrr
    {
        public static string ToSentenceCase(this string input)
        {
            return String.Format("{0}{1}", input.Substring(0, 1).ToUpper(), input.Substring(1));
        }

        private static readonly Dictionary<string, string> lowerSubstitutions;
        private static readonly Dictionary<string, string> sentenceCaseSubstitutions;

        static HereBePiratesYarrr()
        {
            lowerSubstitutions =
                new Dictionary<string, string>
                {
                    { "hello", "ahoy" },
                    { "hi", "yo-ho-ho" },
                    { "pardon me", "avast" },
                    { "excuse me", "arrr" },
                    { "yes", "aye" },
                    { "my", "me" },
                    { "friend", "me bucko" },
                    { "sir", "matey" },
                    { "madam", "proud beauty" },
                    { "miss", "comely wench" },
                    { "stranger", "scurvy dog" },
                    { "officer", "foul blaggart" },
                    { "where", "whar" },
                    { "is", "be" },
                    { "are", "be" },
                    { "am", "be" },
                    { "the", "th'" },
                    { "you", "ye" },
                    { "your", "yer" },
                    { "tell", "be tellin'" },
                    { "know", "be knowin'" },
                    { "how far", "how many leagues" },
                    { "old", "barnacle-covered" },
                    { "attractive", "comely" },
                    { "happy", "grog-filled" },
                    { "quickly", "smartly" },
                    { "nearby", "broadside" },
                    { "restroom", "head" },
                    { "restaurant", "galley" },
                    { "hotel", "fleabag inn" },
                    { "pub", "Skull & Scuppers" },
                    { "mall", "market" },
                    { "bank", "buried treasure" },
                    { "die", "visit Davey Jones' Locker" },
                    { "died", "visited Davey Jones' Locker" },
                    { "kill", "keel-haul" },
                    { "killed", "keel-hauled" },
                    { "sleep", "take a caulk" },
                    { "stupid", "addled" },
                    { "after", "aft" },
                    { "stop", "belay" },
                    { "nonsense", "bilge" },
                    { "ocean", "briny deep" },
                    { "song", "shanty" },
                    { "money", "doubloons" },
                    { "food", "grub" },
                    { "nose", "prow" },
                    { "leave", "weigh anchor" },
                    { "cheat", "hornswaggle" },
                    { "forward", "fore" },
                    { "child", "sprog" },
                    { "children", "sprogs" },
                    { "sailor", "swab" },
                    { "lean", "careen" },
                    { "find", "come across" },
                    { "mother", "dear ol' mum, bless her black soul" },
                    { "drink", "barrel o' rum" },
                    { "of", "o'" },
                    { "!", "YARRR!" },
                };

            sentenceCaseSubstitutions = new Dictionary<string, string>(lowerSubstitutions.Count);
            foreach (var substitution in lowerSubstitutions)
            {
                sentenceCaseSubstitutions.Add(substitution.Key.ToSentenceCase(), substitution.Key.ToSentenceCase());
            }
        }

        public static string Piratize(this string boringEnglishString)
        {
            // TODO - turn this into a non-horrible regex ;-)
            return lowerSubstitutions.Aggregate(boringEnglishString, (current, substitution) => current.Replace(substitution.Key, substitution.Value));
        }
    }
}