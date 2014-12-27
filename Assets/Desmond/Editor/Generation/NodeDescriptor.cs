using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

public class FieldDescriptor{
    public string fieldType;
    public string id;
    public string defaultValue;

    public FieldDescriptor(string t, string i, string d) {
        fieldType = t;
        id = i;
        defaultValue = d;
    }
}

public class DataInDescriptor {
    public string dataType;
    public string id;
    public bool optional = false;

    public DataInDescriptor(string type, string i) {
        dataType = type;
        id = i;
    }
}

public class ExpressionDescriptor {
    public string returnType;
    public string id;
    public string expressionCode;

    public ExpressionDescriptor(string type, string i) {
        returnType = type;
        id = i;
    }
}

public class ExitPathDescriptor {
    public string id;
    public int pathsToExit = 1;

    public ExitPathDescriptor(string i) {
        id = i;
    }
}

public class MethodDescriptor {
    public string id;
    public string staticReference = null;
    public List<string> codeBlock = new List<string>();
    public HashSet<string> exitPaths = new HashSet<string>();
    public HashSet<string> inputVars = new HashSet<string>();

    public MethodDescriptor(string i) {
        id = i;
    }
}

public class FunctionDescriptor {
    public List<string> codeBlock = new List<string>();
}


public class NodeDescriptor : IPathable{
    private static Dictionary<string, NodeDescriptor> _descriptors = null;
    private static Dictionary<string, NodeDescriptor> _staticDescriptors = null;

    public string descriptorName;
    public bool isStatic = false;
    public bool isGameObject = false;

    public string getPath() {
        return descriptorName;
    }

    public Dictionary<string, FieldDescriptor> fields = new Dictionary<string, FieldDescriptor>();
    public Dictionary<string, DataInDescriptor> dataIns = new Dictionary<string, DataInDescriptor>();
    public Dictionary<string, ExpressionDescriptor> expressions = new Dictionary<string, ExpressionDescriptor>();
    public Dictionary<string, ExitPathDescriptor> exitPaths = new Dictionary<string, ExitPathDescriptor>();
    public Dictionary<string, List<MethodDescriptor>> methods = new Dictionary<string, List<MethodDescriptor>>();

    public HashSet<string> uniqueNames = new HashSet<string>();

    public HashSet<string> namespaceImports = new HashSet<string>();

    public List<FunctionDescriptor> functions = new List<FunctionDescriptor>();

    public static Dictionary<string, NodeDescriptor> descriptors {
        get {
            if (_descriptors == null) {
                loadAllDescriptors();
            }
            return _descriptors;
        }
    }

    public static Dictionary<string, NodeDescriptor> staticDescriptors {
        get {
            if (_staticDescriptors == null) {
                loadAllDescriptors();
            }
            return _staticDescriptors;
        }
    }

    public static void loadAllDescriptors() {
        _descriptors = new Dictionary<string, NodeDescriptor>();
        _staticDescriptors = new Dictionary<string, NodeDescriptor>();

        string path = "/Desmond/Editor/Generators/";
        descriptors.Clear();

        string[] files = Directory.GetFiles(Application.dataPath + path);
        foreach(string file in files){
            if (file.EndsWith(".dg")) {
                loadDescriptorsFromFile(file);
            }
        }
    }

    private static void loadDescriptorsFromFile(string filename) {
        MethodDescriptor method = null;
        FunctionDescriptor function = null;
        ExpressionDescriptor expression = null;
        
        NodeDescriptor current = null;

        string[] lines = System.IO.File.ReadAllLines(filename);
        foreach (string l in lines) {
            string lt = l.Trim();
            string[] s = lt.Split();

            //Match #name to create a new descriptor
            if (lt.StartsWith("#")) {
                current = new NodeDescriptor();
                s = lt.Substring(1).Split();
                current.descriptorName = s[0];
                for (int i = 1; i < s.Length; i++) {
                    if (s[i].ToLower() == "static") {
                        current.isStatic = true;
                    }
                    if (s[i].ToLower() == "gameobject") {
                        current.isGameObject = true;
                    }
                }

                if (current.isStatic) {
                    staticDescriptors[current.descriptorName] = current;
                } else {
                    descriptors[current.descriptorName] = current;
                }
                
                continue;
            }

            if (lt.StartsWith("!")) {
                current.namespaceImports.Add(lt.Substring(1).Trim());
                continue;
            }

            //Matching <type id> data in points
            string match;
            string copy = l;
            while (StringHelper.getBraced(copy, out match)) {
                s = match.Split();
                if (s.Length >= 2 && !current.dataIns.ContainsKey(s[1])) {
                    DataInDescriptor dataIn = new DataInDescriptor(s[0], s[1]);
                    if (s.Length >= 3 && s[2] == "optional") {
                        dataIn.optional = true;
                    }

                    if (method != null) {
                        method.inputVars.Add(s[1]);
                    }

                    current.dataIns[s[1]] = dataIn;
                }
                copy = StringHelper.replaceBrace(copy, "");
            }

            //Matching ->id data out points
            if (lt.StartsWith("->")) {
                string exitId = lt.Substring(2);
                if (current.exitPaths.ContainsKey(exitId)) {
                    current.exitPaths[exitId].pathsToExit++;
                } else {
                    current.exitPaths[exitId] = new ExitPathDescriptor(exitId);
                }
                if (method != null) {
                    method.exitPaths.Add(exitId);
                }
            }

            //Matching code and putting it into a defined method
            if (method != null) {
                //If we wind up on a line with no indenting whitespace, no more code
                if (l.TrimStart() == l) {
                    method = null;
                } else {
                    if (lt != "") {
                        method.codeBlock.Add(l);
                        continue;
                    }
                }
            }

            //Match the } that ends a function declaration
            if (function != null) {
                if (lt == "]") {
                    function = null;
                    continue;
                }
                function.codeBlock.Add(l);

                //Match the line after an expression definition
            } else if (expression != null) {
                expression.expressionCode = lt;
                expression = null;

            } else {
                if (lt == "") {
                    continue;
                }

                //match def type id default to create a field
                if (lt.StartsWith("def ")) {
                    string type = s[1];
                    string id = s[2];
                    string value = s.Length == 4 ? s[3] : "";
                    current.fields[id] = new FieldDescriptor(type, id, value);
                    continue;
                }

                //Match { the start of a litteral function block
                if (s[0] == "[") {
                    function = new FunctionDescriptor();
                    current.functions.Add(function);
                    continue;
                }

                //Match id the start of an execution block
                if (s.Length == 1) {
                    method = new MethodDescriptor(s[0]);
                    List<MethodDescriptor> descriptorList;
                    if (!current.methods.TryGetValue(method.id, out descriptorList)) {
                        descriptorList = new List<MethodDescriptor>();
                        current.methods[method.id] = descriptorList;
                    }
                    descriptorList.Add(method);
                    continue;
                }

                //Match type id the start of a data out expression
                if (s.Length == 2) {
                    if (s[0] == "void") {
                        method = new MethodDescriptor(s[1]);
                        method.staticReference = s[1];
                        List<MethodDescriptor> descriptorList;
                        if (!current.methods.TryGetValue(method.id, out descriptorList)) {
                            descriptorList = new List<MethodDescriptor>();
                            current.methods[method.id] = descriptorList;
                        }
                        descriptorList.Add(method);
                        continue;
                    } else if (s[0] == "unique"){
                        current.uniqueNames.Add(s[1]);
                        continue;
                    }else{
                        expression = new ExpressionDescriptor(s[0], s[1]);
                        current.expressions[expression.id] = expression;
                        continue;
                    }
                }
            }
        }
    }
}

}