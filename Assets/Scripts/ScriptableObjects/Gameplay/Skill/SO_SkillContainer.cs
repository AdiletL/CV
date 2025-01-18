using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ScriptableObjects.Gameplay.Skill
{
    [CreateAssetMenu(fileName = "SO_SkillContainer", menuName = "SO/Gameplay/Skill/Container", order = 51)]
    public class SO_SkillContainer : ScriptableObject
    {
        [SerializeField] private AssetReference[] skills;

        public async UniTask<T> GetSkillConfig<T>()
        {
            foreach (var skill in skills)
            {
                var prefabHandle = Addressables.LoadAssetAsync<T>(skill);
                await prefabHandle.Task;

                if (prefabHandle.Status ==
                    UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    var result = prefabHandle.Result;
                    if (result.GetType() == typeof(T))
                    {
                        return result;
                    }
                }
            }

            throw new NullReferenceException();
        }
    }
}