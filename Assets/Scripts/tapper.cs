using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
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

        ArrayList trimTaps(ArrayList trimTheseTaps)
        {
            int firstOneIndex = -1;
            int lastOneIndex = -1;
            ArrayList newTaps = new ArrayList();
            for (int i = 0; i < trimTheseTaps.Count; i++)
            {
                if (firstOneIndex < 0 && (int)trimTheseTaps[i] == 1)
                {
                    firstOneIndex = i;
                }
                if (firstOneIndex >= 0)
                {
                    newTaps.Add(trimTheseTaps[i]);
                }
                if ((int)trimTheseTaps[i] == 1)
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
            if (isRecording)
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
            else{
                isRecording = true;
            }
            
        }

        public void vibrateLastTap()
        {
            if (playingTap == null || playingTap != previousTaps[previousTaps.Count - 1])
            {
                playingTap = previousTaps[previousTaps.Count - 1];

            }
            playingIndex = 0;
            StartCoroutine(VibratePattern());
        }

        IEnumerator VibratePattern()
        {
#if UNITY_ANDROID
            bool wait = playingTap.FirstDigit == 1 ? false : true;
            foreach (long duration in playingTap.VibrationPattern)
            {
                if(!wait){
                    Tap.Vibrate(duration);
                }
                yield return new WaitForSeconds(duration / 1000f);
            }
#else
            yield return new WaitForEndOfFrame();
#endif
        }




        // Update is called once per frame
        void Update()
        {
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
                        Image background = GetComponent<Image>();
                        background.color = UnityEngine.Color.grey;
                        isRecording = true;
                    }
                }
                if( Input.GetTouch(i).phase == TouchPhase.Ended){
                    isPressing = 0;
                    Image background = GetComponent<Image>();
                    background.color = UnityEngine.Color.white;
                }

            }

            if (Input.GetMouseButtonDown(0))
            {
                if(EventSystem.current.IsPointerOverGameObject())
                {
                    isPressing = 1;
                    Image background = GetComponent<Image>();
                    background.color = UnityEngine.Color.grey;
                    isRecording = true;
                }


            }
            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    isPressing = 0;
                    Image background = GetComponent<Image>();
                    background.color = UnityEngine.Color.white;
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
            if (CrossPlatformInputManager.GetButtonDown("Jump"))
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
            Image panelBg = GetComponent<Image>();
            if(playingTap != null && playingIndex < playingTap.Pattern.Count && playingIndex >= 0){

                if (playingTap.Pattern[playingIndex++] == 0)
                {
                    panelBg.color = UnityEngine.Color.green;
                }
                else
                {
                    panelBg.color = UnityEngine.Color.blue;

                }
            }
            else if(!isRecording){
                playingIndex = -1;
                panelBg.color = UnityEngine.Color.red;
            }
            panelBg.transform.SetAsLastSibling();
        }
    }
}