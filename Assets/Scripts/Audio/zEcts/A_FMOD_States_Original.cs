
/*public class A_FMOD_States_Original : MonoBehaviour
{
    public abstract class State
    {
        public abstract void OnEntered();
        public abstract void OnUpdate(float deltaTime);
        public abstract void OnExited();
    }

    public sealed class StateMachine
    {
        public State CurrentState { get; private set }

        public void SwitchStates(in State newState)
        {
            CurrentState?.OnExited();
            CurrentState = newState;
            CurrentState?.OnEntered();
        }
    }

    public sealed class ExampleState : State
    {
        // can put a constructor in any of these state classes to pass it any info you need, including the 
statemachine itself to be able to switch states from within a state
        public override void OnEntered() => Debug.Log("Entered ExampleState");
        public override void OnUpdate() => Debug.Log("Updated ExampleState");
        public override void OnExited() => Debug.Log("Exited ExampleState");
    }

    public sealed class ExampleMonoBehaviour : MonoBehaviour
    {
        private StateMachine _stateMachine;

        private void Awake() => _stateMachine = new StateMachine();
        private void Start() => _stateMachine.SwitchStates(new ExampleState());
        private void Update() => _stateMachine.CurrentState?.OnUpdate(Time.deltaTime);
    }
}*/

/*private void Start() extra for one shots
{
    StartCoroutine(PlayerWithinDistanceOfObjectRoutine(yourObj, yourDistance));
}

private IEnumerator PlayerWithinDistanceOfObjectRoutine(Transform obj, float distance)
{
    while (Vector3.Distance(obj.position, player.position) > distance)
        yield return null; // will wait for player to be within distance
    Debug.Log("Player within distance!"); // whatever code you want to happen once
}*/
