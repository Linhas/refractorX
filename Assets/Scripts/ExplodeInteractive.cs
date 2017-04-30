using UnityEngine;

namespace Interactive
{
    [RequireComponent(typeof(Rigidbody))]
    public class ExplodeInteractive : Interactive
    {
        public float Force;
        public float Radius;
        public float UpForce;

        public override void Interact(GameObject actor)
        {
            base.Interact(actor);
            GetComponent<Rigidbody>().AddExplosionForce(Force, actor.transform.position, Radius, UpForce, ForceMode.Impulse);
        }
    }
}
