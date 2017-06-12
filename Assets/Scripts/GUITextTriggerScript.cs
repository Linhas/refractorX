using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITextTriggerScript : MonoBehaviour {

    //public Font fonty;
    public GUIStyle style1 = new GUIStyle();
    public GUIContent content1 = new GUIContent();
    bool toggleGUI;
    //public string text;



    // Use this for initialization
    void Start () {
        toggleGUI = false;
        
        style1.font = (Font)Resources.Load("rajdhani/Rajdhani-Medium");
        style1.fontSize = 40;

        //content1.text = text;
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    


    void OnTriggerEnter(Collider other)
    {
        toggleGUI = true;
    }

    void OnTriggerExit(Collider other)
    {
        toggleGUI = false;
    }

    void OnGUI()
    {

        //Debug.Log(style1.font);

        if (toggleGUI == true)
        {
            //GUI.skin.fontSize = 20;
            //GUI.skin.font = fonty;
            GUI.Box(new Rect(30, 600, 500, 100), content1 , style1);
            //Debug.Log("Font name: " + GUI.skin.font.name);
        }
        //Debug.Log("TRIGGERED");
    }
}
