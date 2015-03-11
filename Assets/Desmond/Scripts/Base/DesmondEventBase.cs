using UnityEngine;
using System.Collections;

public class DesmondEventBase : MonoBehaviour {
    private DesmondSceneBase sceneScript;

    protected void attatchMessageListener<T>(GameObject gameObject, object listener) where T : DesmondMessageListener{
        if (gameObject == null) {
            return;
        }

        T t = gameObject.GetComponent<T>();
        if (t == null) {
            t = gameObject.AddComponent<T>();
        }

        t.attatchListener(listener);
    }

    protected void detatchMessageListener<T>(GameObject gameObject, object listener) where T : DesmondMessageListener {
        if (gameObject == null) {
            return;
        }

        T t = gameObject.GetComponent<T>();
        if (t != null) {
            t.detatchListener(listener);
        }
    }

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
