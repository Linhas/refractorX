using JetBrains.Annotations;
using UnityEngine;

namespace Interactive
{
    public abstract class Interactive : MonoBehaviour
    {

        private bool _isInteracting = false;
        public bool IsInteracting
        {
            get { return _isInteracting; }
        }

        protected static Toolbox Toolbox;
        protected float Speed;

        // Use this for initialization
        [UsedImplicitly]
        protected virtual void Start()
        {
            Toolbox = Toolbox.Instance;
            Speed = Toolbox.Speed;
            gameObject.layer = LayerMask.NameToLayer("Interactive");
        }

        /*// Update is called once per frame
        [UsedImplicitly]
        private void Update() {

        }*/

        [UsedImplicitly]
        public virtual void Interact(GameObject actor)
        {
            _isInteracting = true;
        }

        [UsedImplicitly]
        public virtual void DeInteract(GameObject actor)
        {
            _isInteracting = false;
        }
    }
}
