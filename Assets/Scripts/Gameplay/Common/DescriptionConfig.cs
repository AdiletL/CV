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

        /// <summary>
        /// Форматирует шаблон с подстановкой значений + базовая разметка (bold).
        /// </summary>
        public string Format(Dictionary<string, string> values)
        {
            if (templateFile == null)
                return "";

            string result = templateFile.text;

            // BOLD: {bold:name} → <b>{name}</b>
            result = Regex.Replace(result, @"\{bold:([\w\d_]+)\}", match =>
            {
                string key = match.Groups[1].Value;
                if (values.TryGetValue(key, out string val))
                    return $"<b>{val}</b>";
                return $"<b>{{{key}}}</b>"; // fallback
            });

            // REGULAR KEYS: {key}
            result = Regex.Replace(result, @"\{([\w\d_]+)\}", match =>
            {
                string key = match.Groups[1].Value;
                if (values.TryGetValue(key, out string val))
                    return val;
                return $"{{{key}}}"; // если не найдено — оставить как есть
            });

            return result;
        }

        /// <summary>
        /// Предпросмотр с мок-данными.
        /// </summary>
        protected virtual Dictionary<string, string> GetPreviewValues()
        {
            return new Dictionary<string, string>
            {
                { "name", "Молния" },
                { "damage", "120" },
                { "cooldown", "4.5" },
                { "count", "2" },
                { "rarity", "Epic" }
            };
        }

        private string FormatWithPreviewData()
        {
            return Format(GetPreviewValues());
        }

        /// <summary>
        /// Проверка на отсутствующие переменные.
        /// </summary>
        [Button("Проверить плейсхолдеры")]
        private void ValidatePlaceholders()
        {
            if (templateFile == null)
            {
                Debug.LogWarning($"[{"name"}] Не выбран .txt файл.");
                return;
            }

            string template = templateFile.text;

            var foundKeys = new HashSet<string>();
            var regex = new Regex(@"\{(?:bold:)?([\w\d_]+)\}");
            foreach (Match match in regex.Matches(template))
            {
                foundKeys.Add(match.Groups[1].Value);
            }

            var previewKeys = GetPreviewValues();

            foreach (var key in foundKeys)
            {
                if (!previewKeys.ContainsKey(key))
                {
                    Debug.LogWarning($"[{"name"}] В шаблоне используется плейсхолдер {{{key}}}, но он не указан в словаре.");
                }
            }

            Debug.Log($"[{"name"}] Проверка шаблона завершена. Всего плейсхолдеров: {foundKeys.Count}.");
        }
    }
}