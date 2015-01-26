using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class StringNode : Node{
    public string savedDescriptorName;

    private NodeDescriptor _descriptor = null;

    public NodeDescriptor descriptor {
        get {
            if (_descriptor == null) {
                if (!NodeDescriptor.descriptors.TryGetValue(savedDescriptorName, out _descriptor)) {
                    if (!NodeDescriptor.staticDescriptors.TryGetValue(savedDescriptorName, out _descriptor)) {
                        Debug.LogError(name + " could not find NodeDescription [" + savedDescriptorName + "]");
                    }
                }
            }
            return _descriptor;
        }
        set {
            _descriptor = value;
        }
    }

    public override void generateElements() {
        base.generateElements();
        isGameObject = descriptor.isGameObject;

        HashSet<string> methodIDsSoFar = new HashSet<string>();
        foreach (List<MethodDescriptor> list in descriptor.methods.Values) {
            foreach (MethodDescriptor a in list) {
                if (methodIDsSoFar.Contains(a.id)) {
                    continue;
                }
                methodIDsSoFar.Add(a.id);

                ExecutionInputInfo element = ScriptableObject.CreateInstance<ExecutionInputInfo>();
                element.init(a.id, "void", this);
                element.visible = a.staticReference == null || a.staticReference == "";
                elements.Add(element);
            }
        }

        foreach (DataInDescriptor a in descriptor.dataIns.Values) {
            InputWithDefaultInfo element = ScriptableObject.CreateInstance<InputWithDefaultInfo>();
            element.init(a.id, a.dataType, this);
            elements.Add(element);
        }
        foreach (ExitPathDescriptor a in descriptor.exitPaths.Values) {
            ExecutionOutInfo element = ScriptableObject.CreateInstance<ExecutionOutInfo>();
            element.init(a.id, "void", this);
            elements.Add(element);
        }
        foreach (DataOutDescriptor a in descriptor.expressions.Values) {
            DataOutInfo element = ScriptableObject.CreateInstance<DataOutInfo>();
            element.init(a.id, a.returnType, this);
            elements.Add(element);
        }
    }

    public override List<FieldStruct> getFieldStructs() {
        List<FieldStruct> list = new List<FieldStruct>();
        
        foreach (FieldDescriptor d in descriptor.fields.Values) {
            ScriptStructKey key = new ScriptStructKey(this, d.id);
            list.Add(new FieldStruct(key, d.fieldType, d.id, d.defaultValue));
        }

        return list;
    }

    public override List<MethodStruct> getMethodStructs() {
        List<MethodStruct> list = new List<MethodStruct>();

        foreach (List<MethodDescriptor> methodList in descriptor.methods.Values) {

            //0 is worst, negative is better
            int bestMethodRank = -1;
            MethodDescriptor bestMethod = null;
            bestMethod = methodList[0];

            /*
            foreach (MethodDescriptor d in methodList) {
                //for every input required, there must be a connection
                //for every output connected, there must be an exit path
                //preffer methods with the fewest exit paths

                bool canChooseMethod = true;

                foreach (string inputVarId in d.inputVars) {
                    InputWithDefaultInfo inputVar = getElement(inputVarId) as InputWithDefaultInfo;
                    if (inputVar == null) {
                        Debug.LogError("ID's did not match up!");
                        return null;
                    }

                    //A required input is not connected, abort
                    if (!inputVar.isConnected()) {
                        Debug.Log(1);
                        canChooseMethod = false;
                        break;
                    }
                }

                foreach(ExitPathDescriptor exitPath in descriptor.exitPaths.Values){
                    ExecutionOutInfo exitElement = getElement(exitPath.id) as ExecutionOutInfo;
                    if (exitElement == null) {
                        Debug.LogError("ID's did not match up!");
                        return null;
                    }

                    if (exitElement.isConnected()) {
                        if (!d.exitPaths.Contains(exitElement.id)) {
                            canChooseMethod = false;
                            break;
                        }
                    }
                }

                if(!canChooseMethod){
                    continue;
                }

                if (bestMethod == null || d.exitPaths.Count < bestMethodRank) {
                    bestMethod = d;
                    bestMethodRank = d.exitPaths.Count;
                }
            }

            if (bestMethod == null) {
                Debug.LogError(savedDescriptorName + ": Could not find suitable method for given connection states!");
                return new List<MethodStruct>();
            }
             * */

            ScriptStructKey key = new ScriptStructKey(this, bestMethod.id);
            MethodStruct s = new MethodStruct(key, name + StringHelper.capitalize(bestMethod.id));
            s.addCode(bestMethod.codeBlock);
            s.staticReference = bestMethod.staticReference;
            list.Add(s);
        }

        return list;
    }

    public override List<GenericCodeStruct> getGenericCodeStructs() {
        List<GenericCodeStruct> list = new List<GenericCodeStruct>();

        foreach (GenericMethodDescriptor d in descriptor.functions) {
            ScriptStructKey key = new ScriptStructKey(this, d.GetHashCode() + "");
            GenericCodeStruct s = new GenericCodeStruct(key);
            s.addCode(d.codeBlock);

            list.Add(s);
        }

        return list;
    }

    public override List<ExpressionMethodStruct> getExpressionStructs() {
        List<ExpressionMethodStruct> list = new List<ExpressionMethodStruct>();

        foreach (DataOutDescriptor d in descriptor.expressions.Values) {
            ScriptStructKey key = new ScriptStructKey(this, d.id);
            ExpressionMethodStruct s = new ExpressionMethodStruct(key, name + "Get" + StringHelper.capitalize(d.id), d.returnType);
            s.addCode(d.expressionCode);
            list.Add(s);
        }

        return list;
    }

    public override void OnBeforeSerialize() {
        base.OnBeforeSerialize();
        if (descriptor != null) {
            savedDescriptorName = _descriptor.descriptorName;
        }
    }
}

}