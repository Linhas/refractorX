using System.Collections;
using UnityEngine;

namespace Interactive
{
    [RequireComponent(typeof(Rigidbody))]
    public class PushInteractive : Interactive
    {
        private bool _isMoving;

        protected Vector3 StartPosition;
        protected Vector3 EndPosition;

        public override void Interact(GameObject actor)
        {
            base.Interact(actor);
            if (!_isMoving)
            {
                StartPosition = gameObject.transform.position;
                EndPosition = gameObject.transform.position + (actor.transform.rotation * Vector3.forward);
                RaycastHit? hit = Utils.CheckTile(EndPosition + (Vector3.down * gameObject.transform.position.y) + (Vector3.down * 0.05f), 0.2f);
                if (hit != null)
                {
                    new Task(Move());
                }
            }
        }

        public IEnumerator Move()
        {
            _isMoving = true;
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * Speed;
                gameObject.transform.position = Vector3.Lerp(StartPosition, EndPosition, t);
                yield return null;
            }
            _isMoving = false;
            yield return 0;
        }
    }
}
