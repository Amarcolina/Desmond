using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

  [System.Serializable]
  public class EnumProperty : GenericPropertyValue<int> {
    private static GUIStyle labelStyle = null;
    private static GUIStyle textFieldStyle = null;

    [System.NonSerialized]
    private string[] enumNames = null;
    [System.NonSerialized]
    private GUIContent[] enumContent = null;
    [System.NonSerialized]
    private string inputString;

    public override void applyTo(SerializedProperty property) {
      property.enumValueIndex = value;
    }

    private void initStyles() {
      labelStyle = new GUIStyle();
      labelStyle.alignment = TextAnchor.MiddleLeft;
      labelStyle.normal.textColor = Color.black;

      textFieldStyle = new GUIStyle();
      textFieldStyle.alignment = TextAnchor.MiddleLeft;
      textFieldStyle.normal.textColor = Color.green;
    }

    public override void drawCustomPropertyEditor(Rect rect) {
      if (labelStyle == null) {
        initStyles();
      }

      if (enumNames == null || enumNames.Length == 0) {
        System.Type type = TypeUtil.searchForType(fullTypeName);
        enumNames = System.Enum.GetNames(type);
        enumContent = new GUIContent[enumNames.Length];
        for (int i = 0; i < enumNames.Length; i++) {
          enumContent[i] = new GUIContent(enumNames[i]);
          enumNames[i] = enumNames[i].ToLower();
        }

        inputString = enumNames[value];

        if (enumNames.Length == 0) {
          return;
        }
      }

      Rect popupRect = rect;
      popupRect.width = Node.LINE;
      popupRect.x += Node.SIDE - Node.LINE;

      int newEnumValue = EditorGUI.Popup(popupRect, value, enumContent);
      if (newEnumValue != value) {
        inputString = enumNames[newEnumValue];
        value = newEnumValue;
      }

      string calculatedName = enumNames[value];
      GUI.Label(rect, calculatedName, labelStyle);

      EditorGUI.BeginChangeCheck();
      inputString = EditorGUI.TextField(rect, inputString, textFieldStyle);
      if (EditorGUI.EndChangeCheck()) {
        GUI.contentColor = Color.white;

        string chosenName = null;
        int chosenValue = 99;
        string inputLower = inputString.ToLower();
        for (int i = 0; i < enumNames.Length; i++) {
          string enumName = enumNames[i];
          if (enumName.Contains(inputLower)) {
            int similarity = StringHelper.findSimilarity(enumName, inputString);
            if (chosenName == null || similarity < chosenValue) {
              chosenName = enumName;
              chosenValue = similarity;
              value = i;
            }
          }
        }

        if (chosenName == null) {
          inputString = calculatedName;
        } else {
          inputString = chosenName;
        }
      }
    }

    public override bool tryGetStringRepresentation(out string representation) {
      representation = fullTypeName + "." + enumNames[value];
      return true;
    }
  }

}
