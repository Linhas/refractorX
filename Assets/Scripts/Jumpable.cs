using JetBrains.Annotations;
using UnityEngine;

public class Jumpable : MonoBehaviour {

    //Jump forcs
    public float Force;
    public bool IsGrounded;

    // Use this for initialization
    [UsedImplicitly]
    void Start () {
		FixedUpdate();
	}
	
	[UsedImplicitly]
	private void FixedUpdate () {
        if(Physics.Raycast(transform.position, -Vector3.up, GetComponent<Collider>().bounds.extents.y + 0.01f))
        {
            IsGrounded = true;
        }
    }
}
