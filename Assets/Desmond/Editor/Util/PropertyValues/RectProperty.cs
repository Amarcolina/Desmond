using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class RectProperty : GenericPropertyValue<Rect> {
    public override void applyTo(SerializedProperty property) {
        property.rectValue = value;
    }
}

}
