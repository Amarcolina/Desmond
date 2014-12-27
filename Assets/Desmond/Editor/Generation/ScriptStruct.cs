using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond{

public struct ScriptStructKey {
    public Node parentNode;
    public string elementId;

    public ScriptStructKey(Node parent, string id) {
        parentNode = parent;
        elementId = id;
    }

    public override string ToString() {
        return "[" + parentNode + "] : [" + elementId + "]";
    }
}


/* Represents a chunk of data that can be placed into a script
 * It belongs to a node, and belongs on a specific game object
 */
public abstract class ScriptStruct{
    public ScriptStructKey key;
    public GameObject parentGameObject;
    public int references = 0;
    public bool isPublic = false;

    public ScriptStruct(ScriptStructKey key) {
        this.key = key;
    }

    public virtual bool shouldPrint() {
        return isPublic || references != 0;
    }

    public abstract List<string> getLines();
}

public class GenericCodeStruct : ScriptStruct{
    public List<string> codeBlock = new List<string>();
    public string staticReference = "";

    public GenericCodeStruct(ScriptStructKey key) : base(key) { }

    public void addCode(string code){
        codeBlock.Add(code);
    }

    public void addCode(ICollection<string> code) {
        codeBlock.AddRange(code);
    }

    //Only inline if it is only referenced once and is private
    public bool shouldInline() {
        return references < 2 && !isPublic;
    }

    public bool isEmpty(){
        return codeBlock.Count == 0;
    }

    public override List<string> getLines(){
 	    return codeBlock;
    }

    public override bool shouldPrint() {
        return isPublic || references > 1;
    }
}

public class MethodStruct : GenericCodeStruct{
    public string methodName;

    public MethodStruct(ScriptStructKey key, string name) : base(key) {
        methodName = name;
    }

    public override List<string> getLines(){
 	    List<string> returnLines = new List<string>();
        returnLines.Add((isPublic ? "public " : "private ") + "void " + methodName + "(){");
        returnLines.AddRange(codeBlock);
        returnLines.Add("}");
        return returnLines;
    }
}

public class ExpressionMethodStruct : MethodStruct {
    public string returnType;

    public ExpressionMethodStruct(ScriptStructKey key, string name, string returnType) : base(key, name) {
        this.returnType = returnType;
    }

    public override List<string> getLines() {
        List<string> returnLines = new List<string>();
        returnLines.Add((isPublic ? "public " : "private ") + returnType + " " + methodName + "(){");
        returnLines.Add("return " + codeBlock[0] + ";");
        returnLines.Add("}");
        return returnLines;
    }
}

public class FieldStruct : ScriptStruct{
    public string type;
    public string name;
    public string defaultValue;

    public FieldStruct(ScriptStructKey key, string type, string name, string defaultValue) : base(key) {
        this.type = type;
        this.name = name;
        this.defaultValue = defaultValue;
    }

    public override List<string> getLines() {
        List<string> returnList = new List<string>();
        if (defaultValue == null || defaultValue == "") {
            returnList.Add((isPublic ? "public " : "private ") + type + " " + name + ";");
        } else {
            returnList.Add((isPublic ? "public " : "private ") + type + " " + name + " = " + defaultValue + ";");
        }
        return returnList;
    }
}

public class ObjectFieldStruct : FieldStruct {
    public Object value;

    public ObjectFieldStruct(ScriptStructKey key, string type, string name, Object value)
        : base(key, type, name, "") {
            this.value = value;
    }
}




}