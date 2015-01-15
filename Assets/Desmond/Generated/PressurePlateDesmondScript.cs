using UnityEngine;


public class PressurePlateDesmondScript : DesmondSceneBase{
    public DesmondSceneScript DesmondSceneScriptReference;
    
    public void OnTriggerEnter(Collider other){
        Debug.Log("asdasd");
        DesmondSceneScriptReference.TimelinePlay();
    }
    
    public void OnTriggerExit(Collider other){
        DesmondSceneScriptReference.TimelinePlayReverse();
    }
}
