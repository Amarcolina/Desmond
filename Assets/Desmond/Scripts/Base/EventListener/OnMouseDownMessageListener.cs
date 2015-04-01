using UnityEngine;
using System.Collections;

public class OnMouseDownMessageListener : DesmondMessageListenerVoid {
    void OnMouseDown() {
        dispatch();
    }
}
