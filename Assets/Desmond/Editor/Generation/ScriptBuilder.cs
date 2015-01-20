using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Desmond { 



public class ScriptBuilder {
    //Static variables shared by all script builders
    public static HashSet<GenericCodeStruct> nodesVisitedForReferenceCounting = new HashSet<GenericCodeStruct>();
    public static Dictionary<GameObject, ScriptBuilder> objectToBuilder = new Dictionary<GameObject, ScriptBuilder>();

    //Instance variables local to each builder
    public OrderedDictionary<ScriptStructKey, FieldStruct> defaultFields = new OrderedDictionary<ScriptStructKey, FieldStruct>();
    public OrderedDictionary<ScriptBuilder, FieldStruct> scriptFields = new OrderedDictionary<ScriptBuilder, FieldStruct>();
    public HashSet<ScriptBuilder> buildersReferenced = new HashSet<ScriptBuilder>();

    public OrderedDictionary<ScriptStructKey, GenericCodeStruct> codeStructs = new OrderedDictionary<ScriptStructKey, GenericCodeStruct>();
    public OrderedDictionary<ScriptStructKey, MethodStruct> methodStructs = new OrderedDictionary<ScriptStructKey, MethodStruct>();
    public OrderedDictionary<ScriptStructKey, ExpressionMethodStruct> expressionStructs = new OrderedDictionary<ScriptStructKey, ExpressionMethodStruct>();

    public HashSet<string> namespaceImports = new HashSet<string>();

    public Dictionary<string, StringNode> staticMethods = new Dictionary<string, StringNode>();

    public GameObject objectInstance;
    public string scriptName;

    public ScriptBuilder(string scriptName, GameObject instance = null) {
        this.scriptName = scriptName;
        objectInstance = instance;
        if (instance != null) {
            objectToBuilder[instance] = this;
        }
    }

    //##############################################
    //Public methods
    //Called from BoardBuilder during board building

    public static void clearStaticVariables() {
        nodesVisitedForReferenceCounting.Clear();
        objectToBuilder.Clear();
    }

    public void addNode(Node node) {
        foreach (FieldStruct s in node.getFieldStructs()) {
            defaultFields.tryAdd(s.key, s);
        }
        foreach (GenericCodeStruct s in node.getGenericCodeStructs()) {
            codeStructs.tryAdd(s.key, s);
        }
        foreach (MethodStruct s in node.getMethodStructs()) {
            methodStructs.tryAdd(s.key, s);
        }
        foreach (ExpressionMethodStruct s in node.getExpressionStructs()) {
            expressionStructs.tryAdd(s.key, s);
        }

        StringNode stringNode = node as StringNode;
        if (stringNode) {
            namespaceImports.UnionWith(stringNode.descriptor.namespaceImports);
        }
    }

    public HashSet<Node> buildStaticNodes() {
        HashSet<Node> newNodes = new HashSet<Node>();
        foreach (MethodStruct codeStruct in methodStructs) {
            Node createdNode = tryBuildStaticNode(codeStruct);
            if (createdNode != null) {
                newNodes.Add(createdNode);
            }
        }
        return newNodes;
    }

    public void countAllReferences() {
        foreach (GenericCodeStruct codeStruct in codeStructs) {
            countReferences(codeStruct);
        }
    }

    public void resolveAllExpressions() {
        foreach (ExpressionMethodStruct s in expressionStructs) {
            resolveGenericCode(s);
        }
    }

    public void resolveAllMethods() {
        foreach (MethodStruct s in methodStructs) {
            resolveGenericCode(s);
        }
    }

    public void resolveAllGenericCode() {
        foreach (GenericCodeStruct s in codeStructs) {
            resolveGenericCode(s);
        }
    }

    public List<PostCompilationJob> getPostCompilationJobs() {
        List<PostCompilationJob> jobs = new List<PostCompilationJob>();
        if (objectInstance != null) {
            AddScriptJob addScriptJob = ScriptableObject.CreateInstance<AddScriptJob>();
            addScriptJob.init(objectInstance, scriptName);
            jobs.Add(addScriptJob);

            foreach(ScriptBuilder referencedBuilder in buildersReferenced){
                LinkToScriptJob setPropertyJob = ScriptableObject.CreateInstance<LinkToScriptJob>();
                FieldStruct fieldStruct = scriptFields[referencedBuilder];

                setPropertyJob.init(objectInstance, scriptName, fieldStruct.name, referencedBuilder.objectInstance, referencedBuilder.scriptName);
                jobs.Add(setPropertyJob);
            }
        }

        foreach (FieldStruct field in defaultFields) {
            ObjectFieldStruct objectField = field as ObjectFieldStruct;
            if (objectField != null) {
                SetObjectJob setPropertyJob = ScriptableObject.CreateInstance<SetObjectJob>();
                setPropertyJob.init(objectInstance, scriptName, objectField.name, objectField.value);
                jobs.Add(setPropertyJob);
            }
            AnimationCurveFieldStruct animationCurveField = field as AnimationCurveFieldStruct;
            if (animationCurveField != null) {
                SetAnimationCurveJob setCurveJob = ScriptableObject.CreateInstance<SetAnimationCurveJob>();
                setCurveJob.init(objectInstance, scriptName, animationCurveField.name, animationCurveField.value);
                jobs.Add(setCurveJob);
            }
        }

        return jobs;
    }

    public void generateUniqueNames() {
        HashSet<string> methodNames = new HashSet<string>();

        foreach (MethodStruct s in methodStructs) {
            if (!s.shouldPrint()) {
                continue;
            }

            int suffix = 0;
            string prefix = s.methodName;
            while (methodNames.Contains(s.methodName)) {
                suffix++;
                s.methodName = prefix + suffix;
            }
            methodNames.Add(s.methodName);
        }

        foreach (ExpressionMethodStruct s in expressionStructs) {
            if (!s.shouldPrint()) {
                continue;
            }

            int suffix = 0;
            string prefix = s.methodName;
            while (methodNames.Contains(s.methodName)) {
                suffix++;
                s.methodName = prefix + suffix;
            }
            methodNames.Add(s.methodName);
        }

        //Build references for game objects we need to reference
        HashSet<string> fieldNames = new HashSet<string>();

        //Given the builders referenced generate the propper fields
        foreach (ScriptBuilder otherBuilder in buildersReferenced){
            FieldStruct scriptReferenceStruct = new FieldStruct(new ScriptStructKey(null, "void"), otherBuilder.scriptName, otherBuilder.scriptName + "Reference", "");
            scriptReferenceStruct.isPublic = true;
            scriptFields.tryAdd(otherBuilder, scriptReferenceStruct);
        }

        foreach (FieldStruct s in scriptFields) {
            if (!s.shouldPrint()) {
                continue;
            }

            int suffix = 0;
            string prefix = s.name;
            while (fieldNames.Contains(s.name)) {
                suffix++;
                s.name = prefix + suffix;
            }
            fieldNames.Add(s.name);
        }

        foreach (FieldStruct s in defaultFields) {
            if (!s.shouldPrint()) {
                continue;
            }

            int suffix = 0;
            string prefix = s.name;
            while (fieldNames.Contains(s.name)) {
                suffix++;
                s.name = prefix + suffix;
            }
            fieldNames.Add(s.name);
        }
    }

    //##############################################
    //Private methods
    //Called from ScriptBuilder (this) during board building

    private Node tryBuildStaticNode(MethodStruct codeStruct) {
        if (codeStruct.staticReference != null && codeStruct.staticReference != "") {
            
            StringNode node;
            if (!staticMethods.TryGetValue(codeStruct.staticReference, out node)) {
                node = ScriptableObject.CreateInstance<StringNode>();
                node.descriptor = NodeDescriptor.staticDescriptors[codeStruct.staticReference];
                node.name = node.descriptor.descriptorName;
                node.generateElements();

                staticMethods[codeStruct.staticReference] = node;

                List<GenericCodeStruct> list = node.getGenericCodeStructs();
                foreach(GenericCodeStruct generatedStruct in list){
                    codeStructs.tryAdd(generatedStruct.key, generatedStruct);
                }
            }
            ConnectableElement element = node.getElement("out") as ConnectableElement; 

            Node ourNode = codeStruct.key.parentNode;
            Element ourElement = ourNode.getElement(codeStruct.key.elementId);

            element.tryConnect(new ElementConnection(ourElement, node, ourNode));
            return node;
        }
        return null;
    }

    private void countReferences(GenericCodeStruct codeStruct) {
        if (nodesVisitedForReferenceCounting.Contains(codeStruct)) {
            return;
        }
        nodesVisitedForReferenceCounting.Add(codeStruct);

        Node node = codeStruct.key.parentNode;

        for (int i = 0; i < codeStruct.codeBlock.Count; i++) {
            string line = codeStruct.codeBlock[i].Trim();

            countBracedReferences(codeStruct, node, line);
            countMethodReferences(node, line);
        }
    }

    private void countBracedReferences(GenericCodeStruct codeStruct, Node node, string line) {
        string match;

        while (StringHelper.getBraced(line, out match)) {
            string[] inside = match.Split(' ');
            
            //If brace matches <id> pattern
            if (inside.Length == 1) {
                string id = inside[0];
                
                ScriptStructKey key = new ScriptStructKey(codeStruct.key.parentNode, id);
                foreach(FieldStruct s in defaultFields){
                    if(s.key.Equals(key)){
                        s.references++;
                        break;
                    }
                }
            } else if (inside.Length == 2) {
                string id = inside[1];

                //Get the element that represents the data input we just detected
                ConnectableElement element = node.getElement(id) as ConnectableElement;

                //Get each connection that element is connected to
                ElementConnection connection = element.connections[0];
                Element connectedElement = connection.connectedElement;
                Node connectedNode = connection.connectedNode;

                ScriptBuilder connectedBuilder;
                if (connectedNode.gameObjectInstance == null) {
                    connectedBuilder = this;
                } else {
                    connectedBuilder = objectToBuilder[connectedNode.gameObjectInstance];
                }

                ScriptStructKey connectedKey = new ScriptStructKey(connectedNode, connectedElement.id);
                ExpressionMethodStruct s = connectedBuilder.expressionStructs[connectedKey];

                //If we are connecting to a different builder
                if (connectedBuilder != this) {
                    s.isPublic = true;
                    buildersReferenced.Add(connectedBuilder);
                }
                s.references++;

                connectedBuilder.countReferences(s);
            } else {
                Debug.LogError(match);
            }

            //remove brace so that while loop can continue
            line = StringHelper.replaceBrace(line, "");
        }
    }

    private void countMethodReferences(Node node, string line) {
        if(line.StartsWith("->")){
            string id = line.Substring(2);

            //Get the element that represents the executing output we just detected

            ConnectableElement element = node.getElement(id) as ConnectableElement;

            //Get each connection that element is connected to
            foreach(ElementConnection connection in element.connections){
                Element connectedElement = connection.connectedElement;
                Node connectedNode = connection.connectedNode;

                ScriptBuilder connectedBuilder;
                if (connectedNode.gameObjectInstance == null) {
                    connectedBuilder = this;
                } else {
                    connectedBuilder = objectToBuilder[connectedNode.gameObjectInstance];
                }

                ScriptStructKey key = new ScriptStructKey(connectedNode, connectedElement.id);
                MethodStruct s = connectedBuilder.methodStructs[key];

                if (connectedBuilder != this) {
                    s.isPublic = true;
                    buildersReferenced.Add(connectedBuilder);
                }
                s.references++;

                connectedBuilder.countReferences(s);
            }
        }
    }

    private void resolveGenericCode(GenericCodeStruct codeStruct) {
        Node node = codeStruct.key.parentNode;

        for (int i = codeStruct.codeBlock.Count - 1; i >= 0;  i--) {
            string line = codeStruct.codeBlock[i].Trim();

            line = resolveBracedReferences(codeStruct, node, line);

            List<string> lines = resolveMethodReferences(node, line);
            codeStruct.codeBlock.RemoveAt(i);
            codeStruct.codeBlock.InsertRange(i, lines);
        }
    }

    private string resolveBracedReferences(GenericCodeStruct codeStruct, Node node, string line) {
        string match;

        while (StringHelper.getBraced(line, out match)) {
            string[] inside = match.Split(' ');
            //If brace matches <id> pattern
            if (inside.Length == 1) {
                string id = inside[0];

                FieldStruct s = defaultFields[new ScriptStructKey(node, id)];
                line = StringHelper.replaceBrace(line, s.name);
            } else if (inside.Length == 2) {
                string id = inside[1];

                ConnectableElement element = node.getElement(id) as ConnectableElement;
                ElementConnection connection = element.connections[0];
                Element connectedElement = connection.connectedElement;
                Node connectedNode = connection.connectedNode;

                ScriptBuilder connectedBuilder;
                if (connectedNode.gameObjectInstance == null) {
                    connectedBuilder = this;
                } else {
                    connectedBuilder = objectToBuilder[connectedNode.gameObjectInstance];
                }

                ScriptStructKey key = new ScriptStructKey(connectedNode, connectedElement.id);
                ExpressionMethodStruct s = connectedBuilder.expressionStructs[key];

                string expression;
                if (s.shouldInline()) {
                    expression = connectedBuilder.resolveBracedReferences(s, connectedNode, s.codeBlock[0]);                
                } else {
                    if (connectedBuilder != this) {
                        FieldStruct fieldStruct = scriptFields[connectedBuilder];
                        expression = fieldStruct.name + "." + s.methodName + "()";
                    } else {
                        expression = s.methodName + "()";
                    }
                }

                string toType = element.type;
                string fromType = connectedElement.type;

                string castExpression;
                if (!TypeConversion.tryCastExpression(fromType, expression, toType, out castExpression)) {
                    Debug.LogWarning("Could not auto-cast expression [" + expression + "] from type [" + fromType + "] to type [" + toType + "]");
                }

                line = StringHelper.replaceBrace(line, castExpression);
            } else {
                Debug.LogError("woah");
            }
        }

        return line;
    }

    private List<string> resolveMethodReferences(Node node, string line) {
        List<string> list = new List<string>();

        if (line.StartsWith("->")) {
            string id = line.Substring(2);

            //Get the element that represents the executing output we just detected

            ConnectableElement element = node.getElement(id) as ConnectableElement;

            //Get each connection that element is connected to
            foreach (ElementConnection connection in element.connections) {
                Element connectedElement = connection.connectedElement;
                Node connectedNode = connection.connectedNode;

                ScriptBuilder connectedBuilder;
                if (connectedNode.gameObjectInstance == null) {
                    connectedBuilder = this;
                } else {
                    connectedBuilder = objectToBuilder[connectedNode.gameObjectInstance];
                }

                ScriptStructKey key = new ScriptStructKey(connectedNode, connectedElement.id);
                MethodStruct s = connectedBuilder.methodStructs[key];

                if (s.shouldInline()) {
                    connectedBuilder.resolveGenericCode(s);
                    foreach (string linkedLine in s.codeBlock) {
                        list.Add(linkedLine);
                    }
                } else {
                    if (connectedBuilder != this) {
                        FieldStruct fieldStruct = scriptFields[connectedBuilder];
                        list.Add(fieldStruct.name + "." + s.methodName + "();");
                    } else {
                        list.Add(s.methodName + "();");
                    }
                }
            }
        } else {
            list.Add(line);
        }
        return list;
    }
}

}