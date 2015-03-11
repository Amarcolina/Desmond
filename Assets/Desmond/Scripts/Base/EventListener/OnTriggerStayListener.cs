using UnityEngine;
using System.Collections;

public class OnTriggerStayListener : DesmondMessageListenerSingle<Collider> {
    public void OnTriggerStay(Collider other) {
        dispatch(other);
    }
}