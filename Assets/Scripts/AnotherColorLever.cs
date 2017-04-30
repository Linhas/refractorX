using Interactive;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(LeverInteractive))]
public class AnotherColorLever : MonoBehaviour
{

    public Color Color;

    private readonly Color[] _colors = new Color[2];

    // Use this for initialization
    [UsedImplicitly]
    private void Start()
    {
        _colors[0] = gameObject.GetComponent<Renderer>().material.GetColor("_Color");
        _colors[1] = Color;
        LeverInteractive.OnStateChange += ChangeColor;
    }


    /*// Update is called once per frame
    [UsedImplicitly]
    private void Update () {
		
	}*/

    private void ChangeColor(int state, string name)
    {
        if (name == gameObject.name && _colors.Length > 0)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", _colors[state % _colors.Length]);
        }

    }
}
