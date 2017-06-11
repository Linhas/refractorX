using JetBrains.Annotations;
using UnityEngine;
using CommandPattern;

namespace Interactive
{
    
    public class LeverInteractive : Interactive
    {
        public int TotalStates;
        public int InitialState;
        public bool ResetOnDeInteract;


        /*
        public AnimationClip fullAnimation;
        public Animation myAnimation;
        */
        public InputHandler InputHandler;

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
            /*
            myAnimation = GetComponent<Animation> ();
            myAnimation.AddClip(fullAnimation, "open", 0, 19);
            myAnimation.AddClip(fullAnimation, "close", 19, 35);
            */
        }

        /*// Update is called once per frame
        [UsedImplicitly]
        private void Update()
        {

        }*/

        [UsedImplicitly]
        public override void Interact(GameObject actor)
        {
            Debug.Log("Interact!!!");
            base.Interact(actor);
            _currentState = _currentState+1 % TotalStates;
            if (OnStateChange != null)
                OnStateChange(_currentState, gameObject.name);
            
            if (_currentState == 0)
                InputHandler.myAnimator.SetFloat("fechado", 0.0f);
            else
                InputHandler.myAnimator.SetFloat("fechado", 1.0f);

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
