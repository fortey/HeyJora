using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bot", menuName = "New Bot", order = 53)]
public class Bot : ScriptableObject
{
    [TextArea(60,1000)]
    public string code;
}