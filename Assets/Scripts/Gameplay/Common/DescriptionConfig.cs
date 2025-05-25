using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class DescriptionConfig
    {
        [LabelText("Файл шаблона (.txt)")]
        public TextAsset templateFile;

        [ShowInInspector]
        [MultiLineProperty(10)]
        [ReadOnly]
        [LabelText("Предпросмотр")]
        public string Preview => FormatWithPreviewData();
        
        private string ExtractSection(string tag)
        {
            if (templateFile == null || string.IsNullOrEmpty(templateFile.text))
                return null;

            string text = templateFile.text;
            string pattern = $@"#{tag}\s*(.*?)\s*(?=(#\w+|$))";

            var match = Regex.Match(text, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (!match.Success)
                return null;

            string section = match.Groups[1].Value.Trim();

            return section;
        }

        public string GetHeader() => ExtractSection("HEADER");

        public string GetDescription()
        {
            var result = ExtractSection("DESCRIPTION");
            var previewKeys = GetPreviewValues();
            result = ReplacePlaceholdersInText(result, previewKeys);
            return result;
        }
        
        /// <summary>
        /// Предпросмотр с мок-данными.
        /// </summary>
        protected virtual Dictionary<string, string> GetPreviewValues()
        {
            return new Dictionary<string, string>
            {
                { "name", ExtractSection("HEADER") },
                /*{ "damage", "120" },
                { "cooldown", "4.5" },
                { "count", "2" },
                { "rarity", "Epic" }*/
            };
        }

        private string FormatWithPreviewData()
        {
            if (templateFile == null || string.IsNullOrEmpty(templateFile.text)) return null;
            return templateFile.ToString();
        }

        /// <summary>
        /// Проверка на отсутствующие переменные.
        /// </summary>
        private string ReplacePlaceholdersInText(string text, Dictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return Regex.Replace(text, @"\{([\w\d_]+)\}", match =>
            {
                string key = match.Groups[1].Value;
                return values.TryGetValue(key, out var value) ? value : match.Value;
            });
        }
    }
}