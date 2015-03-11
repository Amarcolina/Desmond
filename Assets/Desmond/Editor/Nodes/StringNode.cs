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
            ScriptElementKey key = new ScriptElementKey(this, d.id);
            list.Add(new FieldStruct(key, d.fieldType, d.id, d.defaultValue));
        }

        return list;
    }

    public override List<MethodStruct> getMethodStructs() {
        List<MethodStruct> list = new List<MethodStruct>();

        foreach (List<MethodDescriptor> methodList in descriptor.methods.Values) {

            MethodDescriptor bestMethod = null;

            foreach (MethodDescriptor d in methodList) {

                bool canChooseMethod = true;
                foreach (string without in d.withouts) {
                    ConnectableElement element = getElement(without) as ConnectableElement;
                    Assert.that(element != null, "Element " + without + " must exist and be a connectable element");

                    if (element.isConnected()) {
                        canChooseMethod = false;
                        break;
                    }
                }

                if (!canChooseMethod) {
                    continue;
                }

                if (bestMethod == null || d.withouts.Count > bestMethod.withouts.Count) {
                    bestMethod = d;
                }
            }

            Assert.that(bestMethod != null, savedDescriptorName + ": Could not find suitable method for given connection states!");

            ScriptElementKey key = new ScriptElementKey(this, bestMethod.id);
            MethodStruct s = new MethodStruct(key, name + StringHelper.capitalize(bestMethod.id));
            s.addCode(bestMethod.codeBlock);
            s.staticReference = bestMethod.staticReference;
            list.Add(s);
        }

        return list;
    }

    public override List<CustomMethodStruct> getCustomMethodStructs() {
        List<CustomMethodStruct> list = new List<CustomMethodStruct>();

        foreach (GenericMethodDescriptor d in descriptor.functions) {
            ScriptElementKey key = new ScriptElementKey(this, d.GetHashCode() + "");
            CustomMethodStruct s = new CustomMethodStruct(key);
            s.addCode(d.codeBlock);

            list.Add(s);
        }

        return list;
    }

    public override List<ExpressionMethodStruct> getExpressionStructs() {
        List<ExpressionMethodStruct> list = new List<ExpressionMethodStruct>();

        foreach (DataOutDescriptor d in descriptor.expressions.Values) {
            ScriptElementKey key = new ScriptElementKey(this, d.id);
            ExpressionMethodStruct s = new ExpressionMethodStruct(key, name + "Get" + StringHelper.capitalize(d.id), d.returnType);
            s.addCode(d.expressionCode);
            list.Add(s);
        }

        return list;
    }

    public override HashSet<string> getNamespaceImports() {
        return descriptor.namespaceImports;
    }

    public override HashSet<string> getUniqueNames() {
        return descriptor.uniqueNames;
    }

    public override void OnBeforeSerialize() {
        base.OnBeforeSerialize();
        if (descriptor != null) {
            savedDescriptorName = _descriptor.descriptorName;
        }
    }
}

}