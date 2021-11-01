
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MarketingBox.Integration.Service.Utils
{
    public static class LanguageUtils
    {
        public const string DefaultIso2Language = "EN";
        public const string DefaultIso3Language = "ENG";

        public static string GetIso3LanguageFirstOrDefault(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                return DefaultIso3Language;
            }

            language = language.ToLower();

            var languageCode = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => c.TwoLetterISOLanguageName == language ||
                c.ThreeLetterISOLanguageName == language)?.ThreeLetterISOLanguageName.ToUpper();

            if (languageCode == null)
            {
                languageCode = DefaultIso3Language;
            }

            return languageCode;
        }

        public static string GetIso2LanguageFromCountryFirstOrDefault(string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return DefaultIso2Language;
            }

            country = country.ToLower();

            var languageCode = GetLanguagesSpokenInCountry(country).FirstOrDefault();

            if (languageCode == null)
            {
                return DefaultIso2Language;
            }

            return languageCode;
        }

        private static IEnumerable<string> GetLanguagesSpokenInCountry(string country)
        {
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var languagesSpoken = allCultures.Where(c =>
            {
                if (c.IsNeutralCulture) return false;
                if (c.LCID == 0x7F) return false; // Ignore Invariant culture
                var region = new RegionInfo(c.LCID);
                return region.TwoLetterISORegionName == country;
            }).Select(c => c.TwoLetterISOLanguageName.ToUpper()).ToList();
            return languagesSpoken;
        }
    }
}
