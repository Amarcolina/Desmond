using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class CustomEvent : ScriptableObject {
    public string customEventName;
    public List<string> dataNames = new List<string>();
    public List<string> dataTypes = new List<string>();
    public List<object> dataDefaults = new List<object>(); //Object, AnimationCurve, or string for anything else
}

}