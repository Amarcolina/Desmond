using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

public class BoundsProperty : GenericPropertyValue<Bounds> {
    public override void assignToProperty(SerializedProperty property) {
        property.boundsValue = value;
    }
}

}
