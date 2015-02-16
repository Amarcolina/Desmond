using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class DefaultEnumNode : DefaultValueNode {
    public int enumValue = 0;

    private static GUIStyle labelStyle = null;
    private static GUIStyle textFieldStyle = null;

    private void initStyles() {
        labelStyle = new GUIStyle();
        labelStyle.alignment = TextAnchor.MiddleLeft;
        labelStyle.normal.textColor = Color.black;

        textFieldStyle = new GUIStyle();
        textFieldStyle.alignment = TextAnchor.MiddleLeft;
        textFieldStyle.normal.textColor = Color.green;
    }

    private string[] enumNames = null;
    private string inputString;

    public override List<ExpressionMethodStruct> getExpressionStructs() {
        List<ExpressionMethodStruct> list = new List<ExpressionMethodStruct>();

        ScriptElementKey key = new ScriptElementKey(this, "out");
        ExpressionMethodStruct s = new ExpressionMethodStruct(key, "default", type);
        s.addCode(getValueType().FullName + "." + enumNames[enumValue]);
        list.Add(s);

        return list;
    }

    public override void drawDefaultProperty(Rect r) {
        if (labelStyle == null) {
            initStyles();
        }

        if (enumNames == null || enumNames.Length == 0) {
            System.Type type = getValueType();
            enumNames = System.Enum.GetNames(type);
            inputString = enumNames[enumValue];

            if (enumNames.Length == 0) {
                return;
            }
        }

        Rect popupRect = r;
        popupRect.width = Node.LINE;
        popupRect.x += Node.SIDE - Node.LINE;

        int newEnumValue = EditorGUI.Popup(popupRect, enumValue, enumNames);
        if (newEnumValue != enumValue) {
            inputString = enumNames[newEnumValue];
            enumValue = newEnumValue;
        }

        string calculatedName = enumNames[enumValue];
        GUI.Label(r, calculatedName, labelStyle);

        inputString = EditorGUI.TextField(r, inputString, textFieldStyle);
        GUI.contentColor = Color.white;

        string chosenName = null;
        int chosenValue = 99;
        for(int i=0; i<enumNames.Length; i++) {
            string enumName = enumNames[i];
            if (enumName.ToLower().Contains(inputString.ToLower())) {
                int value = StringHelper.findSimilarity(enumName, inputString);
                if (chosenName == null || value < chosenValue) {
                    chosenName = enumName;
                    chosenValue = value;
                    enumValue = i;
                }
            }
        }

        if (chosenName == null) {
            inputString = calculatedName;
        }else{
            inputString = chosenName;
        }
    }
}

}