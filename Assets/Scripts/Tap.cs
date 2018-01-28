using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tap
{
    List<long> v_pattern;
    List<int> m_pattern;
    float m_framerate;
    int startsWith;
    public List<int> Pattern
    {
        get
        {
            return m_pattern;
        }
    }
    public float FrameRate
    {
        get
        {
            return m_framerate;
        }
    }

    public List<long> VibrationPattern{
        get{
            return v_pattern;
        }
    }

    public int FirstDigit{
        get{
            return startsWith;
        }
    }



    public Tap(List<int> pattern, float framerate)
    {
        m_pattern = pattern;
        v_pattern = new List<long>();
        m_framerate = framerate;
        startsWith = m_pattern[0];
        int currentlyReading = startsWith;
        int last_index = 0;
        for (int i = 0; i < m_pattern.Count; i++)
        {
            if (m_pattern[i] != currentlyReading)
            {
                currentlyReading = m_pattern[i];
                v_pattern.Add((long)((i - last_index) / framerate) * 1000);
            }
        }



    }
    // Use this to rate a pattern against this one as the answer key
    float rateAgainst(List<int> pattern)
    {
        int points_made = 0;
        int points_possible = 0;
        int shortest_index = pattern.Count > m_pattern.Count ? m_pattern.Count : pattern.Count;
        for (int i = 0; i < shortest_index; i++)
        {
            points_possible++;
            if (pattern[i] == m_pattern[i])
            {
                points_made++;
            }
        }
        return (float)points_made / (float)points_possible * 100f;
    }

    // Use this for initialization
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {

    }


#if UNITY_ANDROID
    private static readonly AndroidJavaObject Vibrator =
        new AndroidJavaClass("com.unity3d.player.UnityPlayer")// Get the Unity Player.
        .GetStatic<AndroidJavaObject>("currentActivity")// Get the Current Activity from the Unity Player.
        .Call<AndroidJavaObject>("getSystemService", "vibrator");// Then get the Vibration Service from the Current Activity.

    static void KyVibrator()
    {
        // Trick Unity into giving the App vibration permission when it builds.
        // This check will always be false, but the compiler doesn't know that.
        if (Application.isEditor) Handheld.Vibrate();
    }

    public static void Vibrate(long milliseconds)
    {
        Vibrator.Call("vibrate", milliseconds);
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
        Vibrator.Call("vibrate", pattern, repeat);
    }
#endif

}
