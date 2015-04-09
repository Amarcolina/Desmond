using UnityEngine;
using System.Collections;

public class OnTriggerEnterMessageListener : DesmondMessageListenerSingle<Collider> {
    public void OnTriggerEnter(Collider other){
        dispatch(other);
    }
}
