using UnityEngine;
using System.Collections;

namespace Desmond { 

[System.Serializable]
public class PostCompilationJob : ScriptableObject , System.IComparable<PostCompilationJob>{
    public virtual void doJob() {
    }

    public virtual int getPriority() {
        return 0;
    }

    //Larger numbers have higher priority
    public int CompareTo(PostCompilationJob job) {
        return job.getPriority() - getPriority();
    }
}

}