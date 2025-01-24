using System;
using ScriptableObjects.Unit.Item;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit.Item
{
    public abstract class ItemController : UnitController, IItem
    {
        [SerializeField] protected SO_Item so_Item;
        
    }
}