using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.UI
{
    public abstract class UIContainer : MonoBehaviour, IContainer
    {
        public List<IItem> items { get; }
        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}