using UnityEngine;
using System.Collections;

public abstract class DesmondMessageListener : MonoBehaviour {
    public abstract void attatchListener(object listener);
    public abstract void detatchListener(object listener);
}

public abstract class DesmondMessageListenerSingle<T> : DesmondMessageListener {
    private event System.Action<T> e;

    public override void attatchListener(object listener) {
        e += (listener as System.Action<T>);
    }

    public override void detatchListener(object listener) {
        e -= (listener as System.Action<T>);
    }

    protected void dispatch(T t) {
        if (e != null) {
            e(t);
        }
    }
}

public abstract class DesmondMessageListenerVoid : DesmondMessageListener {
    private event System.Action e;

    public override void attatchListener(object listener) {
        e += (listener as System.Action);
    }

    public override void detatchListener(object listener) {
        e -= (listener as System.Action);
    }

    protected void dispatch() {
        if (e != null) {
            e();
        }
    }
}
