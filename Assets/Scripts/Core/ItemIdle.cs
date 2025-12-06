using DesignPattern.ObjectPool;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LongNC
{
    public class ItemIdle : ItemBase<ItemIdle.State, ColorId>
    {
        public enum State
        {
            Idle,
            AnimInX, InX,
            AnimDone, Done,
        }
        
        public override void ChangeState<T>(T t)
        {
            base.ChangeState(t);
            switch (_state)
            {
                case State.Idle:
                    break;
                case State.AnimInX:
                    AnimInX(_posInSlot, 1f, 0.5f, 48, State.InX, false, false, new Vector3(90f, 180f, 0f));
                    break;
                case State.InX:
                    break;
                case State.AnimDone:
                    TF.DOMoveY(TF.position.y + 2f, 0.5f);
                    _itemAlpha.DoAlpha(0f, 0.5f);
                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        ChangeState(State.Done);
                    });
                    break;
                case State.Done:
                    LevelControl.Instance.SetControl(true);
                    PoolingManager.Despawn(gameObject);
                    LevelControl.Instance.CheckLevel();
                    break;
            }
        }

        public override void OnClickDown()
        {
            // Debug.Log($"[Click Down] {name} - {_type} - {_state}");
            ChangeState(State.AnimInX);
        }
        
    }
}