using Cyber;
using UnityEngine;

public class EnemyAttackEventTrigger : MonoBehaviour
{
    private BTForEnemy bt;

    private void Awake()
    {
        bt = transform.parent.GetComponent<BTForEnemy>();
    }

    public void ShowAttackCollider(int isShow)
    {
        bt.ShowAttackCollider(isShow == 1);
    }

    public void ShowAttackArea(int isShow)
    {
        bt.ShowAttackArea(isShow == 1);
    }
}

