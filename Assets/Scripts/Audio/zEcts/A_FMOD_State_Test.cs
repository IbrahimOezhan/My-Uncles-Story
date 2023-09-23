public abstract class A_FMOD_State_Test
{
    public abstract void OnEntered();
    public abstract void OnUpdate(float deltaTime);
    public abstract void OnExited();


    public sealed class StateMachine
    {
        public A_FMOD_State_Test CurrentState { get; private set; }

        public void SwitchStates(in A_FMOD_State_Test newState)
        {
            CurrentState?.OnExited();
            CurrentState = newState;
            CurrentState?.OnEntered();
        }
    }

    /*public sealed class A_FMOD_Sealed_Test : A_FMOD_State_Test
    {
        // can put a constructor in any of these state classes to pass it any info you need, including the statemachine
        // itself to be able to switch states from within a state
        public override void OnEntered() => Debug.Log("Entered ExampleState");
        public override void OnUpdate() => Debug.Log("Updated ExampleState");
        public override void OnExited() => Debug.Log("Exited ExampleState");
    }*/
}
