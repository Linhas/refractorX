using Interactive;
using JetBrains.Annotations;
using UnityEngine;

public class DoorInteractive : MonoBehaviour
{

    public GameObject Lever;

    public Animator myAnimator;

    // Use this for initialization
    [UsedImplicitly]
    private void Start()
    {
        LeverInteractive.OnStateChange += ChangeState;
    }

    private void ChangeState(int state, string name)
    {
        if (name == Lever.name)
        {
            myAnimator.SetBool("open", state==1);
        }
    }
}
