using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class ColorProperty : GenericPropertyValue<Color> {
    public override void assignToProperty(SerializedProperty property) {
        property.colorValue = value;
    }
}

}
