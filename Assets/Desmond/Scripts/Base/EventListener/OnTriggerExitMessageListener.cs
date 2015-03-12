using UnityEngine;
using System.Collections;

public class OnTriggerExitMessageListener : DesmondMessageListenerSingle<Collider> {
    public void OnTriggerEnter(Collider other) {
        dispatch(other);
    }
}