using System;
using ScriptableObjects.Gameplay.Skill;
using UnityEngine;

public class TestSkillContainer : MonoBehaviour
{
    [SerializeField] private SO_SkillContainer skillContainer;

    private void Start()
    {
       var s = skillContainer.GetSkillConfig<SO_SkillSpawnPortal>();
       Debug.Log(s);
    }
}
