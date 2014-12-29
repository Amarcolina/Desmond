using UnityEngine;


public class CubeDesmondScript : DesmondSceneBase{
    public DesmondSceneScript DesmondSceneScriptReference;
    
    public void OnMouseDown(){
        DesmondSceneScriptReference.Destroy();
    }
}
