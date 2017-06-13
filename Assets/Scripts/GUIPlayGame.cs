using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIPlayGame : MonoBehaviour {

    //Scene scene = SceneManager.GetActiveScene();

    public void onClick()
    {

        //Scene scene = SceneManager.GetActiveScene();
        //Debug.Log("Active scene is '" + scene.name + "'.");

        //scene = SceneManager.GetSceneByName("MainScene");

        StartCoroutine(MyCoroutine());
        

        //scene = SceneManager.GetActiveScene();
        //Debug.Log("Active scene is '" + scene.name + "'.");
        /*
        //SceneManager.UnloadSceneAsync("MenuScene");
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainScene"));
        SceneManager.LoadSceneAsync("MainScene");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainScene"));
        




        scene = SceneManager.GetActiveScene();
        Debug.Log("Active scene is '" + scene.name + "'.");
        */
    }


    
    IEnumerator MyCoroutine()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "MainScene");
        /*
        var loading = SceneManager.LoadSceneAsync("MainScene");
        yield return loading;
        var scene = SceneManager.GetSceneByName("MainScene");
        SceneManager.SetActiveScene(scene);
        */
    }
    
}
