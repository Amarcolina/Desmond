using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

  [System.Serializable]
  public class BoolProperty : GenericPropertyValue<bool> {
    public override void applyTo(SerializedProperty property) {
      property.boolValue = value;
    }

    public override bool tryGetStringRepresentation(out string representation) {
      representation = value ? "true" : "false";
      return true;
    }
  }

}