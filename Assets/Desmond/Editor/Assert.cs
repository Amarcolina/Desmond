using UnityEngine;
using System.Collections;

namespace Desmond { 

public class Assert {

    public class AssertionFailed : System.Exception {
        public AssertionFailed(string message) : base(message) { }
    }

    public static void that(bool condition, string message = "") {
        if (!condition) {
            throw new AssertionFailed(message);
        }
    }

    public static void equals(object a, object b, string message = "") {
        if (a != b) {
            throw new AssertionFailed(message);
        }
    }
}

}