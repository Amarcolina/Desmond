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
    public string staticReference;

    public DataInDescriptor(string dataType, string id, string staticReference) {
        this.dataType = dataType;
        this.id = id;
        this.staticReference = staticReference;
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
    public InlineBehavior inlineBehavior;

    public DataOutDescriptor(string returnType, string id, InlineBehavior inlineBehavior) {
        this.returnType = returnType;
        this.id = id;
        this.inlineBehavior = inlineBehavior;
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
    public HashSet<string> withouts = new HashSet<string>();
    public InlineBehavior inlineBehavior;

    public MethodDescriptor(string i, string staticReference, HashSet<string> withouts, InlineBehavior inlineBehavior) {
        id = i;
        this.staticReference = staticReference;
        this.withouts = withouts;
        this.inlineBehavior = inlineBehavior;
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
 * <varName>
 * 
 * <inName>
 * 
 * $var varType varName (default)
 * 
 * $in inType inName (static reference)
 * 
 * $method inputName (static reference) (forceInline | preventInline | auto) (without arg arg arg)
 *     code
 *     code
 *     
 * $out outType outName (forceInline | preventInline | auto)
 *     code
 * 
 * $customMethod
 *     code
 *     code
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

            if (trimmedLine.StartsWith("!")) {
                current.namespaceImports.Add(trimmedLine.Substring(1).Trim());
                continue;
            }

            string command = splitLine[0];
            if (command.StartsWith("$")) {
                method = null;
                expression = null;
                genericMethod = null;

                switch (command) {
                    case "$in": {
                        current.dataIns[splitLine[2]] = new DataInDescriptor(splitLine[1], splitLine[2], parseStaticOption(splitLine));
                        break;
                    }
                    case "$def": {
                        string defaultValue = splitLine.Length == 4 ? splitLine[3] : "";
                        current.fields[splitLine[2]] = new FieldDescriptor(splitLine[1], splitLine[2], defaultValue);
                        break;
                    }
                    case "$customMethod": {
                        genericMethod = new GenericMethodDescriptor();
                        current.functions.Add(genericMethod);
                        break;
                    }
                    case "$method": {
                        method = new MethodDescriptor(splitLine[1],
                                                parseStaticOption(splitLine),
                                                parseWithoutOption(splitLine),
                                                parseInlineOption(splitLine, InlineBehavior.AUTO));

                        List<MethodDescriptor> descriptorList;
                        if (!current.methods.TryGetValue(method.id, out descriptorList)) {
                            descriptorList = new List<MethodDescriptor>();
                            current.methods[method.id] = descriptorList;
                        }
                        descriptorList.Add(method);
                        break;
                    }
                    case "$out": {
                        expression = new DataOutDescriptor(splitLine[1],
                                                    splitLine[2],
                                                    parseInlineOption(splitLine, InlineBehavior.FORCE_INLINE));
                        current.expressions[expression.id] = expression;
                        break;
                    }
                    case "$classLocalName": {
                        current.uniqueNames.Add(splitLine[1]);
                        break;
                    }
                    default: {
                        Debug.LogWarning("Unexpected declaration " + command);
                        break;
                    }
                }
                continue;
            }

            foreach (string[] match in StringHelper.getMatchingBraces(line, s => s == "methodLocalName", s => s != null)) {
                if (method != null) {
                    method.methodLocalNames.Add(match[1]);
                }
                if (genericMethod != null) {
                    genericMethod.methodLocalNames.Add(match[1]);
                }
            }

            //Matching ->id data out points
            if (trimmedLine.StartsWith("->")) {
                string exitId = trimmedLine.Substring(2);
                if (!current.exitPaths.ContainsKey(exitId)) {
                    current.exitPaths[exitId] = new ExitPathDescriptor(exitId);
                }
            }

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
    }

    private static string parseStaticOption(string[] s) {
        for (int i = 0; i < s.Length; i++) {
            if (s[i] == "static") {
                return s[i + 1];
            }
        }
        return null;
    }

    private static InlineBehavior parseInlineOption(string[] s, InlineBehavior def) {
        for (int i = 0; i < s.Length; i++) {
            string word = s[i];
            if (word == "forceInline") {
                return InlineBehavior.FORCE_INLINE;
            } else if (word == "preventInline") {
                return InlineBehavior.PREVENT_INLINE;
            } else if (word == "auto") {
                return InlineBehavior.AUTO;
            }
        }
        return def;
    }

    private static HashSet<string> parseWithoutOption(string[] s){
        HashSet<string> withouts = new HashSet<string>();
        bool startWithout = false;
        for (int i = 0; i < s.Length; i++) {
            if (startWithout) {
                withouts.Add(s[i]);
            }
            if (s[i] == "without") {
                startWithout = true;
            }
        }
        return withouts;
    }
}

}