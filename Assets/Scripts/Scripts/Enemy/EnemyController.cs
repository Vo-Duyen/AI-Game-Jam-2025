using Sirenix.Serialization;

public class EnemyController : EnemyBase<EnemyController.State, EnemyController.EnemyType>
{
    public enum EnemyType
    {
        A,
        B
    }
    public enum State
    {
        Idle,
        Patrol,
        
    }

    public override void ChangeState<T>(T state)
    {
        base.ChangeState(state);
        switch (_state)
        {
            case State.Idle:
                break;
            case State.Patrol:
                break;
        }
    }
}
