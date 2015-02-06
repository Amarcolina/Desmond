using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class LoeadingBarUtil : MonoBehaviour
{
    private static List<int> chunks = new List<int>();
    private static List<int> progress = new List<int>();
    private static List<string> titleStrings = new List<string>();
    private static List<string> infoStrings = new List<string>();
    private static Stopwatch stopwatch = new Stopwatch();

    [ContextMenu("Do the test")]
    public void testIt()
    {
        beginChunk(100, "Testing chunk: ", "", () =>
        {
            for (int i = 0; i < 100; i++)
            {
                beginChunk(20, "" + i, "Sub Chunk: ", () =>
                {
                    for (int k = 0; k < 20; k++)
                    {
                        int limit = (int)(Mathf.Pow(10, Random.Range(1, 6)));
                        beginChunk(100000, "", k + " : ", () =>
                        {
                            for (int j = 0; j < limit; j++)
                            {
                                recordProgress("" + j);
                            }
                        });
                    }
                });
            }
        });
    }

    public delegate void Runnable();

    public static void beginChunk(int sections, string title, string info, Runnable runnable)
    {
        if (!stopwatch.IsRunning)
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        chunks.Add(sections);
        progress.Add(0);
        titleStrings.Add(title);
        infoStrings.Add(info);

        try
        {
            runnable();
        }
        finally
        {
            int lastIndex = chunks.Count - 1;
            chunks.RemoveAt(lastIndex);
            progress.RemoveAt(lastIndex);
            titleStrings.RemoveAt(lastIndex);
            infoStrings.RemoveAt(lastIndex);

            lastIndex--;
            if (lastIndex >= 0)
            {
                progress[lastIndex]++;
            }

            if (chunks.Count == 0)
            {
                EditorUtility.ClearProgressBar();
                stopwatch.Stop();
            }
        }
    }

    public static void recordProgress(string infoString = "")
    {
        progress[progress.Count - 1]++;
        if (stopwatch.ElapsedMilliseconds > 17)
        {
            displayBar(infoString);
            stopwatch.Reset();
            stopwatch.Start();
        }
    }

    private static void displayBar(string info = "")
    {
        float percent = 0.0f;
        float fraction = 1.0f;
        string titleString = "";
        string infoString = "";
        for (int i = 0; i < chunks.Count; i++)
        {
            float chunkSize = chunks[i];
            float chunkProgress = progress[i];

            percent += fraction * (chunkProgress / chunkSize);
            fraction /= chunkSize;

            titleString += titleStrings[i];
            infoString += infoStrings[i];
        }

        infoString += info;

        if (EditorUtility.DisplayCancelableProgressBar(titleString, infoString, percent))
        {
            throw new System.Exception("User canceled operation");
        }
    }
}
