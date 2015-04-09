using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class BoundsProperty : GenericPropertyValue<Bounds> {
    public override void applyTo(SerializedProperty property) {
        property.boundsValue = value;
    }
}

}
