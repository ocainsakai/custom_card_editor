using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StandardCards", menuName = "Scriptable Objects/StandardCards")]
public class StandardCards : ScriptableObject
{
    public List<CardData> cards;
}
