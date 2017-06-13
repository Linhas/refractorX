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
        
        foreach (var lever in Levers)
        {
            if (lever == null) continue;
            if (name == lever.name)
            {
                myAnimator.SetBool("open", !myAnimator.GetBool("open"));

                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    GameObject go = gameObject.transform.GetChild(i).gameObject;
                    if (go.name == "tile_middle")
                    {
                        go.SetActive(myAnimator.GetBool("open"));
                        break;
                    }
                }

            }
        }
       
    }
}
