using UnityEngine;


public class DesmondSceneScript : DesmondSceneBase{
    public UnityEngine.GameObject defaultGameObject;
    public UnityEngine.Object Cube;
    
    public void Update(){
        if(Input.GetKeyDown(UnityEngine.KeyCode.Space)){
            Debug.Log("Hello World!");
        }
    }
    
    public void Destroy(){
        UnityEngine.Object.Destroy(Cube);
        Debug.Log("cube was destroyed!");
    }
}
