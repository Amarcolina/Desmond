using UnityEngine;
using System.IO;
using System.Collections;

public class DesmondSceneScript : DesmondSceneBase{
    private System.Single timelineTime = 0.0f;
    private System.Single timelineDirection = 1.0f;
    private System.Single timelineDuration = 0.0f;
    public AnimationCurve curve;
    public UnityEngine.Transform Door;
    
    private IEnumerator timelineCoroutine(){
        bool finished = false;
        while(!finished){
            timelineTime += Time.deltaTime * timelineDirection;
            if(timelineDirection == 1.0f){
                if(timelineTime > timelineDuration){
                    timelineTime = timelineDuration;
                    finished = true;
                }
            }else{
                if(timelineTime <= 0.0f){
                    timelineTime = 0.0f;
                    finished = true;
                }
            }
            Door.localPosition = getCurve().Evaluate(timelineTime) * UnityEngine.Vector3.up;
            yield return null;
        }
    }
    
    public void Awake(){
        timelineDuration = getCurve()[getCurve().length - 1].time;
    }
    
    public void TimelinePlay(){
        Debug.Log("here?");
        timelineDirection = 1.0f;
        StopCoroutine("timelineCoroutine");
        StartCoroutine("timelineCoroutine");
    }
    
    public void TimelinePlayReverse(){
        timelineDirection = -1.0f;
        StopCoroutine("timelineCoroutine");
        StartCoroutine("timelineCoroutine");
    }
    
    private UnityEngine.AnimationCurve getCurve(){
        return curve;
    }
}
