using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodTypeIconSet", menuName = "Game Data/Food Type Icon Set")]
public class FoodTypeIconSet : ScriptableObject
{
    [Serializable]
    public class FoodTypeIconEntry
    {
        public FoodType type;
        public Sprite icon;
    }

    public List<FoodTypeIconEntry> iconEntries;

    private Dictionary<FoodType, Sprite> iconDict;

    public Sprite GetIcon(FoodType type)
    {
        if (iconDict == null)
        {
            iconDict = new Dictionary<FoodType, Sprite>();
            foreach (var entry in iconEntries)
            {
                if (!iconDict.ContainsKey(entry.type))
                    iconDict.Add(entry.type, entry.icon);
            }
        }

        iconDict.TryGetValue(type, out Sprite icon);
        return icon;
    }
}