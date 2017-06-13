using Interactive;
using JetBrains.Annotations;
using UnityEngine;

public class DoorInteractiveMulti : MonoBehaviour
{

    public GameObject[] Levers;

    public Animator myAnimator;

    // Use this for initialization
    [UsedImplicitly]
    private void Start()
    {
        LeverInteractive.OnStateChange += ChangeState;
    }

    private void ChangeState(int state, string name)
    {
        Debug.Log(Levers[0].name +" "+ Levers[1].name);
        foreach (var lever in Levers)
        {
            if (name == lever.name)
            {
                myAnimator.SetBool("open", !myAnimator.GetBool("open"));
            }
        }
       
    }
}
