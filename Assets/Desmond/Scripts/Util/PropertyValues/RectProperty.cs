using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class RectProperty : GenericPropertyValue<Rect> {
    public override void assignToProperty(SerializedProperty property) {
        property.rectValue = value;
    }
}

}
