using System;
using ScriptableObjects.Unit.InteractableObject.Item;
using UnityEngine;

namespace Unit.Item
{
    public abstract class ItemController : UnitController, IItem
    {
        [SerializeField] protected SO_ItemObject so_ItemObject;
        
    }
}