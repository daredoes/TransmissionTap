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


}
