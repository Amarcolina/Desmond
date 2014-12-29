using UnityEngine;


public class DesmondSceneScript : DesmondSceneBase{
    public UnityEngine.GameObject Cube;
    public UnityEngine.GameObject Cube1;
    public UnityEngine.GameObject MainCamera;
    
    public void Update(){
        Debug.Log(Cube.transform.position.magnitude);
        if(Input.GetKeyDown(UnityEngine.KeyCode.Space)){
            Cube1.active = MainCamera.active;
        }
    }
}
