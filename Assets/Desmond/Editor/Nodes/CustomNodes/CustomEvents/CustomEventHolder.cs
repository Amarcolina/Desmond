using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class CustomEventHolder : ScriptableObject {
    public List<CustomEvent> customEvents = new List<CustomEvent>();
}

}