using UnityEngine;
using UnityEditor; 
using System.Collections;

namespace Desmond {

public class AnimationCurveProperty : GenericPropertyValue<AnimationCurve> {
    public override void assignToProperty(SerializedProperty property) {
        property.animationCurveValue = value;
    }
}

}