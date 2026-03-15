using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardConfig", menuName = "Card Config")]
public class CardConfig : ScriptableObject
{
    public int typeId;
    public Sprite icon;
    public string displayName;
}
