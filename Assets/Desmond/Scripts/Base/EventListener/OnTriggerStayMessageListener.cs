using UnityEngine;
using System.Collections;

public class OnTriggerStayMessageListener : DesmondMessageListenerSingle<Collider> {
    public void OnTriggerStay(Collider other) {
        dispatch(other);
    }
}