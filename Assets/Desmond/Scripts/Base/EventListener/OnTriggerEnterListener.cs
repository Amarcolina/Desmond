using UnityEngine;
using System.Collections;

public class OnTriggerEnterListener : DesmondMessageListenerSingle<Collider> {
    public void OnTriggerEnter(Collider other){
        dispatch(other);
    }
}
