using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay.Skill
{
    [CreateAssetMenu(fileName = "SO_SkillContainer", menuName = "SO/Gameplay/Skill/Container", order = 51)]
    public class SO_SkillContainer : ScriptableObject
    {
        [SerializeField] private AssetReferenceT<ScriptableObject>[] skills;

        public T GetSkillConfig<T>()
        {
            foreach (var skill in skills)
            {
                if (!skill.RuntimeKeyIsValid()) // Проверяем, что ключ существует
                {
                    Debug.LogError($"Invalid Addressable key: {skill.RuntimeKey}");
                    continue;
                }
                var prefabHandle = Addressables.LoadAssetAsync<ScriptableObject>(skill).WaitForCompletion();
                if (prefabHandle is T result)
                {
                    return result;
                }
                Addressables.Release(prefabHandle);
            }

            throw new NullReferenceException();
        }
    }
}