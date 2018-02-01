using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

namespace Tapper
{

    public class tapper : MonoBehaviour
    {


        List<int> taps = new List<int>();
        List<Tap> previousTaps = new List<Tap>();
        // Use this for initialization
        int isPressing = 0;
        bool isRecording = true;
        int playingIndex = 0;
        Tap playingTap;


        int m_frameCounter = 0;
        float m_timeCounter = 0.0f;
        float m_lastFramerate = 0.0f;
        public float m_refreshTime = 0.5f;


        void Start()
        {

        }

        List<int> trimTaps(List<int> trimTheseTaps)
        {
            int firstOneIndex = -1;
            int lastOneIndex = -1;
            List<int> newTaps = new List<int>();
            for (int i = 0; i < trimTheseTaps.Count; i++)
            {
                if (firstOneIndex < 0 && trimTheseTaps[i] == 1)
                {
                    firstOneIndex = i;
                }
                if (firstOneIndex >= 0)
                {
                    newTaps.Add(trimTheseTaps[i]);
                }
                if (trimTheseTaps[i] == 1)
                {
                    lastOneIndex = newTaps.Count;
                }
            }
            Debug.Log(newTaps.Count);
            Debug.Log(lastOneIndex);
            if (lastOneIndex < newTaps.Count)
                newTaps.RemoveRange(lastOneIndex, newTaps.Count - lastOneIndex);
            return newTaps;


        }

        public void toggleRecording(){
            Material new_color = GetComponent<MeshRenderer>().material;
            if (isRecording)
            {
                isRecording = false;

                new_color.color = UnityEngine.Color.red;

                previousTaps.Add(new Tap(trimTaps( taps), m_lastFramerate));
                m_frameCounter = 0;
                m_timeCounter = 0.0f;
                m_lastFramerate = 0.0f;
                m_refreshTime = 0.5f;

                taps = new List<int>();
            }
            else{
                isRecording = true;

                new_color.color = UnityEngine.Color.white;
            }
            GetComponent<MeshRenderer>().material = new_color;
            
        }

        public void vibrateLastTap()
        {
            if (playingTap == null){
                if (playingTap != previousTaps[previousTaps.Count - 1])
                {

                    playingTap = previousTaps[previousTaps.Count - 1];
                }
            }
            playingIndex = 0;
            StartCoroutine(VibratePattern());
        }
        public void replayLastTap(){
            if (playingTap == null || playingTap != previousTaps[previousTaps.Count - 1])
            {
                playingTap = previousTaps[previousTaps.Count - 1];

            }
            playingIndex = 0;
        }

        IEnumerator VibratePattern()
        {
            bool wait = playingTap.FirstDigit == 1 ? false : true;
            foreach (long duration in playingTap.VibrationPattern)
            {
                
                if(!wait){
                    
                    Vibration.Vibrate(duration);
                }
                Debug.Log(wait);
                yield return new WaitForSeconds(duration / 1000f);
            }
        }




        // Update is called once per frame
        void Update()
        {
            Material new_color = GetComponent<MeshRenderer>().material;
            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    Debug.Log("Touch started");
                    // Construct a ray from the current touch coordinates
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    // Create a particle if hit
                    if (Physics.Raycast(ray))
                    {
                        isPressing = 1;

                        new_color.color = UnityEngine.Color.grey;
                        isRecording = true;
                    }
                }
                if( Input.GetTouch(i).phase == TouchPhase.Ended){
                    isPressing = 0;

                    new_color.color = UnityEngine.Color.white;
                }

            }

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Click");
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Create a particle if hit
                if (Physics.Raycast(ray))
                {
                    Debug.Log("Clicked");
                    isPressing = 1;
                    new_color.color = UnityEngine.Color.grey;
                    isRecording = true;
                }


            }
            if (Input.GetMouseButtonUp(0))
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Create a particle if hit
                if (Physics.Raycast(ray))
                {
                    isPressing = 0;
                    new_color.color = UnityEngine.Color.white;
                }
            }
            if (isRecording)
            {
                taps.Add(isPressing);
                if (m_timeCounter < m_refreshTime)
                {
                    m_timeCounter += Time.deltaTime;
                    m_frameCounter++;
                }
                else
                {
                    //This code will break if you set your m_refreshTime to 0, which makes no sense.
                    m_lastFramerate = (float)m_frameCounter / m_timeCounter;
                    m_frameCounter = 0;
                    m_timeCounter = 0.0f;
                }
            }
            /*if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                isRecording = true;
            }
            if (CrossPlatformInputManager.GetButtonDown("Horizontal"))
            {
                isRecording = false;
                Image background = GetComponent<Image>();
                background.color = UnityEngine.Color.red;

                previousTaps.Add(new Tap(taps, m_lastFramerate));
                m_frameCounter = 0;
                m_timeCounter = 0.0f;
                m_lastFramerate = 0.0f;
                m_refreshTime = 0.5f;

                taps = new List<int>();

            }
            if (CrossPlatformInputManager.GetButtonDown("Vertical"))
            {
                
                if (playingTap == null || playingTap != previousTaps[previousTaps.Count - 1])
                {
                    playingTap = previousTaps[previousTaps.Count - 1];
                   
                }
                playingIndex = 0;

            }
*/


            if(playingTap != null && playingIndex < playingTap.Pattern.Count && playingIndex >= 0){

                if (playingTap.Pattern[playingIndex++] == 0)
                {
                    new_color.color = Color.green;
                }
                else
                {
                    new_color.color = Color.blue;

                }
            }
            else if(!isRecording){
                playingIndex = -1;
                new_color.color = Color.red;
            }
            GetComponent<MeshRenderer>().material = new_color;
        }
    }
}