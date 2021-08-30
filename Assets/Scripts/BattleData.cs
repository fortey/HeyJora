using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BattleData", menuName = "New BattleData", order = 52)]
public class BattleData : ScriptableObject
{
    [SerializeField]
    public Row[] rows;

    public Bot bot;


    [Serializable]
    public struct Row
    {
        public string[] units;
    }
}