using CommandPattern;
using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputHandler))]
public class Timer : MonoBehaviour {

    public Transform TimerBar;
    public Transform RecordingBar;
    public Transform ModeIndicator;
    public Transform ModePlaying;
    public InputHandler InputHandler;
    public int TimeLimit;


    [SerializeField] private float currentAmount;
    [SerializeField] private float speed;
    // Use this for initialization
    //InputHandler.ReplayOnly  InputHandler.IsRecording
    // Update is called once per frame
  
    void Update () {
        if (currentAmount < TimeLimit)
        {
            currentAmount += speed * Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentAmount);
            string timeText = string.Format("{0:D2}:{1:D2}",  timeSpan.Minutes, timeSpan.Seconds);
            ModeIndicator.GetComponent<Text>().text = timeText;
            ModePlaying.gameObject.SetActive(true);
        }
        else
        {
            ModePlaying.gameObject.SetActive(false);
            ModeIndicator.GetComponent<Text>().text = "No Help!";
        }

        if (!InputHandler.ReplayOnly && InputHandler.IsRecording)
        {
            RecordingBar.GetComponent<Image>().fillAmount = currentAmount / TimeLimit;
        }
        //When replaying and not recording should be filling this one
        else if (InputHandler.ReplayOnly)
        {
            TimerBar.GetComponent<Image>().fillAmount = currentAmount / TimeLimit;
        }
        else
        {
            currentAmount = 0;
            TimerBar.GetComponent<Image>().fillAmount = currentAmount / TimeLimit;
            RecordingBar.GetComponent<Image>().fillAmount = currentAmount / TimeLimit;
        }
        }
    }
