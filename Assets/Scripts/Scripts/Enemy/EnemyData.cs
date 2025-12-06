using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/New Enemy Data", fileName = "EnemyData")]
public class EnemyData : SerializedScriptableObject
{
    public EnemyController.EnemyType typeEnemy;
    public float maxHealth;
    public float distanceCheckGround;
    public float moveSpeed;
    public float jumpForce;
    public float patrolRange;
    public float attackFollow;
    public float attackRange;
}
