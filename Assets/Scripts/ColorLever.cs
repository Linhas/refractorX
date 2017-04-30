using Interactive;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(LeverInteractive))]
public class ColorLever : MonoBehaviour
{

    public Color[] colors;

	// Use this for initialization
    [UsedImplicitly]
    private void Start ()
	{
	    LeverInteractive.OnStateChange +=ChangeColor;
	}
    
	
	/*// Update is called once per frame
    [UsedImplicitly]
    private void Update () {
		
	}*/

    private void ChangeColor(int state, string name)
    {
        if (name == gameObject.name && colors.Length>0)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", colors[state % colors.Length]);
        }

    }
}
