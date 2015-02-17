using UnityEngine;
using System.Collections;

namespace Desmond { 

public class Assert {

    public class AssertionFailed : System.Exception {
        public AssertionFailed(string message) : base(message) { }
    }

    public static void that(bool condition, string message = "") {
        if (!condition) {
            throw new AssertionFailed(message);//
        }
    }

    public static void equals(object a, object b, string message = "") {
        Debug.Log(a + " : " + a.GetHashCode());
        Debug.Log(b + " : " + b.GetHashCode());
        if (!a.Equals(b)) {
            throw new AssertionFailed(a + " did not equal " + b);
        }
    }
}

}