using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/New Enemy Data", fileName = "EnemyData")]
public class EnemyData : SerializedScriptableObject
{
    public EnemyController.EnemyType typeEnemy;
    public float maxHealth;
    public float damage;
    public float distanceCheckGround;
    public float moveSpeed;
    public float runSpeeed;
    public float jumpForce;
    public float patrolRange;
    public float attackFollowRange;
    public float attackRange;
    public float getHitRange;
}
