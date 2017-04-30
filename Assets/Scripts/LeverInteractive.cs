using JetBrains.Annotations;
using UnityEngine;

namespace Interactive
{
    public class LeverInteractive : Interactive
    {
        public int TotalStates;
        public int InitialState;
        public bool ResetOnDeInteract;

        private int _currentState;

        public int CurrentState
        {
            get { return _currentState; }
        }

        public delegate void StateChange(int state, string name);
        public static event StateChange OnStateChange;

        // Use this for initialization
        [UsedImplicitly]
        protected override void Start()
        {
            base.Start();
            _currentState = Mathf.Abs(InitialState) % TotalStates;
        }

        /*// Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {

        }*/

        [UsedImplicitly]
        public override void Interact(GameObject actor)
        {
            base.Interact(actor);
            _currentState = _currentState+1 % TotalStates;
            if (OnStateChange != null)
                OnStateChange(_currentState, gameObject.name);
        }

        [UsedImplicitly]
        public override void DeInteract(GameObject actor)
        {
            base.DeInteract(actor);
            if (ResetOnDeInteract)
            {
                _currentState = InitialState;
                if (OnStateChange != null)
                    OnStateChange(_currentState, gameObject.name);
            }
        }
    }
}
