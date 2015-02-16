using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class DefaultValueNode : Node {
    public string type;

    protected SerializedProperty defaultProperty = null;
    protected SerializedObject defaultObject = null;

    private static Dictionary<string, System.Type> _typeMap = new Dictionary<string, System.Type>();
    private static Dictionary<string, System.Type> typeToNodeType = new Dictionary<string,System.Type>();

    static DefaultValueNode(){
        typeToNodeType["System.Int32"] = typeof(DefaultIntNode);
        typeToNodeType["System.Single"] = typeof(DefaultFloatNode);
        typeToNodeType["System.Boolean"] = typeof(DefaultBoolNode);
        typeToNodeType["System.String"] = typeof(DefaultStringNode);
        typeToNodeType["UnityEngine.AnimationCurve"] = typeof(DefaultAnimationCurveNode);
        typeToNodeType["UnityEngine.Vector2"] = typeof(DefaultVector2Node);
        typeToNodeType["UnityEngine.Vector3"] = typeof(DefaultVector3Node);
        typeToNodeType["UnityEngine.Color"] = typeof(DefaultColorNode);
        typeToNodeType["UnityEngine.Quaternion"] = typeof(DefaultQuaternionNode);
    }

    public static DefaultValueNode getDefaultValueNode(string typeName) {
        DefaultValueNode node = null;

        System.Type primitiveNodeType;
        if(typeToNodeType.TryGetValue(typeName, out primitiveNodeType)){
            node = (DefaultValueNode) ScriptableObject.CreateInstance(primitiveNodeType);
        }
        
        if (node == null) {
            System.Type type = searchForType(typeName);
            if (type == null) {
                return null;
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(type)) {
                node = ScriptableObject.CreateInstance<DefaultObjectNode>();
            } else if(type.IsEnum){
                node = ScriptableObject.CreateInstance<DefaultEnumNode>();
            }else {
                return null;
            }
        }

        node.type = typeName;
        node.isVisible = false;
        node.hideFlags = HideFlags.HideInHierarchy;
        node.generateElements();
        return node;
    }

    public override void generateElements() {
        DataOutInfo dataOut = ScriptableObject.CreateInstance<DataOutInfo>();
        dataOut.init("out", type, this);
        elements.Add(dataOut);
    }

    public System.Type getValueType() {
        return searchForType(type);
    }

    public static System.Type searchForType(string typeName) {
        System.Type type;
        if(_typeMap.TryGetValue(typeName, out type)){
            return type;
        }

        System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var A in assemblies) {
            type = System.Type.GetType(typeName + "," + A.FullName);
            if (type != null) {
                _typeMap[typeName] = type;
                return type;
            }
        }

        return null;
    }

    public ScriptElementKey getKey() {
        return new ScriptElementKey(this, "out");
    }

    public virtual void drawDefaultProperty(Rect r) {
        if (defaultProperty == null) {
            defaultObject = new SerializedObject(this);
            defaultProperty = defaultObject.FindProperty("value");
        }
        defaultObject.Update();
        EditorGUI.PropertyField(r, defaultProperty, GUIContent.none);
        defaultObject.ApplyModifiedProperties();
    }
}

}