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


    [SerializeField] private float currentAmount;
    [SerializeField] private float speed;
    // Use this for initialization
    //InputHandler.ReplayOnly  InputHandler.IsRecording
    // Update is called once per frame
  
    void Update () {
        currentAmount += speed * Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentAmount);
        double value = currentAmount / 60;
        double percentage = value - Math.Floor(value);
        string timeText = string.Format("{0:D2}:{1:D2}",  timeSpan.Minutes, timeSpan.Seconds);
        ModeIndicator.GetComponent<Text>().text = timeText;
        ModePlaying.gameObject.SetActive(true);

            //ModePlaying.gameObject.SetActive(false);
            //ModeIndicator.GetComponent<Text>().text = "No Help!";

        if (!InputHandler.ReplayOnly && InputHandler.IsRecording)
        {
            TimerBar.GetComponent<Image>().fillAmount = 0;
            RecordingBar.GetComponent<Image>().fillAmount = (float)percentage;
        }
        //When replaying and not recording should be filling this one
        else if (!InputHandler.ReplayOnly && InputHandler.IsReplaying())
        {
            RecordingBar.GetComponent<Image>().fillAmount = 0;
            TimerBar.GetComponent<Image>().fillAmount = (float)percentage;
        }
        else if (!InputHandler.ReplayOnly)
        {
            currentAmount = 0;
            TimerBar.GetComponent<Image>().fillAmount =0;
            RecordingBar.GetComponent<Image>().fillAmount = 0;
        }
    }
}
