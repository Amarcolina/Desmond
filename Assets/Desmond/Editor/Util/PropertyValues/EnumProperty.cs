using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond {

[System.Serializable]
public class EnumProperty : GenericPropertyValue<int> {
    private static GUIStyle labelStyle = null;
    private static GUIStyle textFieldStyle = null;

    private string[] enumNames = null;
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
            inputString = enumNames[value];

            if (enumNames.Length == 0) {
                return;
            }
        }

        Rect popupRect = rect;
        popupRect.width = Node.LINE;
        popupRect.x += Node.SIDE - Node.LINE;

        int newEnumValue = EditorGUI.Popup(popupRect, value, enumNames);
        if (newEnumValue != value) {
            inputString = enumNames[newEnumValue];
            value = newEnumValue;
        }

        string calculatedName = enumNames[value];
        GUI.Label(rect, calculatedName, labelStyle);

        inputString = EditorGUI.TextField(rect, inputString, textFieldStyle);
        GUI.contentColor = Color.white;

        string chosenName = null;
        int chosenValue = 99;
        for (int i = 0; i < enumNames.Length; i++) {
            string enumName = enumNames[i];
            if (enumName.ToLower().Contains(inputString.ToLower())) {
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

    public override bool tryGetStringRepresentation(out string representation) {
        representation = fullTypeName + "." + enumNames[value];
        return true;
    }
}

}
