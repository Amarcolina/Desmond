using UnityEngine;
using UnityEditor; 
using System.Collections;

namespace Desmond {

public class AnimationCurveProperty : GenericPropertyValue<AnimationCurve> {
    public override void applyTo(SerializedProperty property) {
        property.animationCurveValue = value;
    }
}

}