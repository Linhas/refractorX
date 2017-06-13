using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


	
 
 public class GUIActivate : MonoBehaviour
{
    void Start()
    {
        bool wasLoaded = SceneManager.SetActiveScene(gameObject.scene);
        Debug.Assert(wasLoaded);
    }
}

