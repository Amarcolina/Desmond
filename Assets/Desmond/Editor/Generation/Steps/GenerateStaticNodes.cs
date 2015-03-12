using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Desmond {

public class GenerateMessageMethods : GenerationStep {

    public override void doStep() {
        Dictionary<string, GenericMethodStruct> messageStructs = new Dictionary<string, GenericMethodStruct>();

        LoadingBarUtil.beginChunk(nodes.Count, "", "Initializing Methods : ", () => {
            foreach (Node node in nodes) {
                List<MessageMethodDescriptor> messageMethods = node.getMessageMethods();
                foreach (MessageMethodDescriptor messageMethod in messageMethods) {
                    
                    GenericMethodStruct messageMethodStruct;
                    if (!messageStructs.TryGetValue(messageMethod.messageName, out messageMethodStruct)) {
                        ScriptElementKey key = new ScriptElementKey(node, "INTERNAL_MESSAGE_METHOD_KEY_" + messageMethod.messageName);
                        messageMethodStruct = new MessageMethodStruct(key, messageMethod.messageName);
                        script.methods[key] = messageMethodStruct;
                        messageStructs[messageMethod.messageName] = messageMethodStruct;
                    }
                    messageMethodStruct.addCode(messageMethod.codeBlock);
                }
            }
        });
    }
}

}