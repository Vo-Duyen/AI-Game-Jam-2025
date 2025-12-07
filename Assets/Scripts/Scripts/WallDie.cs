
using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class WallDie : SerializedMonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ICharacter>(out var character))
        {
            if (character is EnemyController enemyController)
            {
                enemyController.ChangeState(EnemyController.State.Die);
            }

            if (character is PlayerController playerController)
            {
                playerController.SetDie();
            }
        }
    }
}
