using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class ColorProperty : GenericPropertyValue<Color> {
    public override void applyTo(SerializedProperty property) {
        property.colorValue = value;
    }
}

}
