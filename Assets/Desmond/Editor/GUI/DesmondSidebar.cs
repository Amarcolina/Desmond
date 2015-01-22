using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Desmond { 

[System.Serializable]
public class DesmondSidebar {
    public const float width = 200;
    public BoardList boardList;
    public CustomEventEditor eventEditor;

    public delegate void WindowDelegate(Rect r);

    public DesmondSidebar() {
        boardList = new BoardList();
        eventEditor = new CustomEventEditor();
    }

    public void doSidebar(Rect rect) {
        float s = rect.height / 3.0f;
        float h = doWindow(s, boardList.doList);
        h = doWindow(s, eventEditor.drawEditor, h);
        //h = doWindow(s, boardList.doList, h);
    }

    private float doWindow(float height, WindowDelegate d, float currY = 0) {
        Rect r = new Rect(0, currY, width, height);
        GUI.BeginGroup(r);
        d(new Rect(0, 0, width, height));
        GUI.EndGroup();
        return currY + height;
    }
}

}