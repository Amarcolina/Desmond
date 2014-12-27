using UnityEngine;


public class DesmondSceneScript : DesmondSceneBase{
    public UnityEngine.AudioClip enemySpiderDestroyedExplosion;
    public UnityEngine.AudioClip defaultAudioClip;
    
    public void Update(){
        if(Input.GetKeyDown(UnityEngine.KeyCode.None)){
            UnityEngine.AudioSource.PlayClipAtPoint(enemySpiderDestroyedExplosion,(new Vector3(0,0,0)),0);
            Debug.Log("clip played");
        }
    }
}
