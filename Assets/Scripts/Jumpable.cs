using JetBrains.Annotations;
using UnityEngine;
using CommandPattern;

[RequireComponent(typeof(InputHandler))]
public class Jumpable : MonoBehaviour {

    //Jump forcs
    public float Force;
    public bool IsGrounded;
    public InputHandler InputHandler = null;

    // Use this for initialization
    [UsedImplicitly]
    void Start () {
		
        InputHandler = gameObject.GetComponent<InputHandler>();
        FixedUpdate();
    }
	
	[UsedImplicitly]
	private void FixedUpdate () {
        if(Physics.Raycast(transform.position, -Vector3.up, GetComponent<Collider>().bounds.extents.y + 0.01f))
        {
            IsGrounded = true;
        }
        if(IsGrounded)
        {
            InputHandler.myAnimator.SetFloat("speed", 0.0f);
        }
    }
}
