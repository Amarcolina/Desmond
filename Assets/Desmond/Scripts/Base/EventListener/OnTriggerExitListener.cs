using UnityEngine;
using System.Collections;

public class OnTriggerExitListener : DesmondMessageListenerSingle<Collider> {
    public void OnTriggerEnter(Collider other) {
        dispatch(other);
    }
}