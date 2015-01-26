using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Desmond { 

/* Describes a member field defined by a Node
 */
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

/* Describes a data input defined by a node
 */
public class DataInDescriptor {
    public string dataType;
    public string id;

    public DataInDescriptor(string type, string i) {
        dataType = type;
        id = i;
    }
}

/* Describes an execution output defined by a node
 */
public class ExitPathDescriptor {
    public string id;

    public ExitPathDescriptor(string i) {
        id = i;
    }
}


/* Describes a data output defined by a node
 */
public class DataOutDescriptor {
    public string returnType;
    public string id;
    public string expressionCode;

    public DataOutDescriptor(string type, string i) {
        returnType = type;
        id = i;
    }
}

public class GenericMethodDescriptor {
    public List<string> codeBlock = new List<string>();
    public HashSet<string> methodLocalNames = new HashSet<string>();
}

/* Describes a method defined by a node.  Multiple MethodDescriptors
 * can be tied to the same execution input
 */
public class MethodDescriptor : GenericMethodDescriptor {
    public string id;
    public string staticReference = null;
    public HashSet<string> exitPaths = new HashSet<string>();
    public HashSet<string> inputVars = new HashSet<string>();

    public MethodDescriptor(string i) {
        id = i;
    }
}

/*
 * 
 * <classLocalName name>
 *      then <name>
 * 
 * <methodLocalName name>
 *      then <name>
 * 
 * $var varType varName (default)
 * 
 * $in inType inName
 * 
 * $method inputName (forceInline | preventInline | auto) (without arg arg arg)
 *     code
 *     code
 *     
 * $out outType outName (forceInline | preventInline | auto)
 *     code
 * 
 * $customCode
 *     code
 *     code
 * 
 * <varName>
 * 
 * <inName>
 * 
 * ->outName
 * 
*/
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
    public Dictionary<string, DataOutDescriptor> expressions = new Dictionary<string, DataOutDescriptor>();
    public Dictionary<string, ExitPathDescriptor> exitPaths = new Dictionary<string, ExitPathDescriptor>();
    public Dictionary<string, List<MethodDescriptor>> methods = new Dictionary<string, List<MethodDescriptor>>();

    public HashSet<string> uniqueNames = new HashSet<string>();

    public HashSet<string> namespaceImports = new HashSet<string>();

    public List<GenericMethodDescriptor> functions = new List<GenericMethodDescriptor>();

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
        GenericMethodDescriptor genericMethod = null;
        DataOutDescriptor expression = null;
        
        NodeDescriptor current = null;

        string[] lines = System.IO.File.ReadAllLines(filename);
        foreach (string line in lines) {
            string trimmedLine = line.Trim();
            string[] splitLine = trimmedLine.Split();

            //Match #name to create a new descriptor
            if (trimmedLine.StartsWith("#")) {
                current = new NodeDescriptor();
                splitLine = trimmedLine.Substring(1).Split();
                current.descriptorName = splitLine[0];
                for (int i = 1; i < splitLine.Length; i++) {
                    if (splitLine[i].ToLower() == "static") {
                        current.isStatic = true;
                    }
                    if (splitLine[i].ToLower() == "gameobject") {
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

            //Matching code and putting it into a defined method
            if (!trimmedLine.StartsWith("$")) {
                if (method != null) {
                    method.codeBlock.Add(line);
                    continue;
                }
                if (expression != null) {
                    expression.expressionCode = line;
                    expression = null;
                    continue;
                }
                if (genericMethod != null) {
                    genericMethod.codeBlock.Add(line);
                    continue;
                }
            }

            if (trimmedLine.StartsWith("!")) {
                current.namespaceImports.Add(trimmedLine.Substring(1).Trim());
                continue;
            }

            //matching $in type name
            if (splitLine[0] == "$in") {
                current.dataIns[splitLine[2]] = new DataInDescriptor(splitLine[1], splitLine[2]);
                continue;
            }

            //Matching ->id data out points
            if (trimmedLine.StartsWith("->")) {
                string exitId = trimmedLine.Substring(2);
                if (!current.exitPaths.ContainsKey(exitId)) {
                    current.exitPaths[exitId] = new ExitPathDescriptor(exitId);
                }
                if (method != null) {
                    method.exitPaths.Add(exitId);
                }
            }

            //$def type name default
            if(StringHelper.doesMatch(splitLine, s => s == "$def", s => s != null, s => s != null, s => true)){
                string defaultValue = splitLine.Length == 4 ? splitLine[3] : "";
                current.fields[splitLine[2]] = new FieldDescriptor(splitLine[1], splitLine[2], defaultValue);
                continue;
            }

            //$customCode
            if(StringHelper.doesMatch(splitLine, s => s == "$customCode")){
                genericMethod = new GenericMethodDescriptor();
                current.functions.Add(genericMethod);
                continue;
            }

            //$method name (inline) (without asd asd asd)

                //Match id the start of an execution block
                if (splitLine.Length == 1) {
                    method = new MethodDescriptor(splitLine[0]);
                    List<MethodDescriptor> descriptorList;
                    if (!current.methods.TryGetValue(method.id, out descriptorList)) {
                        descriptorList = new List<MethodDescriptor>();
                        current.methods[method.id] = descriptorList;
                    }
                    descriptorList.Add(method);
                    continue;
                }

                //Match type id the start of a data out expression
                if (splitLine.Length == 2) {
                    if (splitLine[0] == "void") {
                        method = new MethodDescriptor(splitLine[1]);
                        method.staticReference = splitLine[1];
                        List<MethodDescriptor> descriptorList;
                        if (!current.methods.TryGetValue(method.id, out descriptorList)) {
                            descriptorList = new List<MethodDescriptor>();
                            current.methods[method.id] = descriptorList;
                        }
                        descriptorList.Add(method);
                        continue;
                    } else if (splitLine[0] == "unique"){
                        current.uniqueNames.Add(splitLine[1]);
                        continue;
                    }else{
                        expression = new DataOutDescriptor(splitLine[0], splitLine[1]);
                        current.expressions[expression.id] = expression;
                        continue;
                    }
                }
            }
        }
    }
}

}