using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITextTriggerScript : MonoBehaviour {

    //public Font fonty;
    //GUIStyle style1;
    bool toggleGUI;
    public string text;



    // Use this for initialization
    void Start () {
        toggleGUI = false;
        //style1.font = fonty;
        //GUI.skin.font = fonty;
        
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
        if (toggleGUI == true)
        {
            GUI.Label(new Rect(30, 600, 500, 100), text /*, style1*/  );
            //Debug.Log("Font name: " + GUI.skin.font.name);
        }
        //Debug.Log("TRIGGERED");
    }
}
