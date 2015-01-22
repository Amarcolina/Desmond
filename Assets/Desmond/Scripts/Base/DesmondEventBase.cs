using UnityEngine;
using System.Collections;

public class DesmondEventBase : MonoBehaviour {
    private DesmondSceneBase sceneScript;

    public virtual void recieveEvent(DesmondEvent desmondEvent) {

    }

    protected void throwEvent(GameObject targetObject, DesmondEvent desmondEvent){
        if (targetObject == null) {
            if (sceneScript == null) {
                sceneScript = FindObjectOfType<DesmondSceneBase>();
            }
            sceneScript.recieveEvent(desmondEvent);
        } else {
            DesmondEventBase[] eventBases = targetObject.GetComponents<DesmondEventBase>();
            foreach (DesmondEventBase eventBase in eventBases) {
                eventBase.recieveEvent(desmondEvent);
            }
        }
    }
}
