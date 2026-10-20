using UnityEngine;

public class MeleeAttackState : State
{
    public override void Enter()
    {
        Debug.Log("IdleState Enter");
    }

    public override void Update()
    {
        // Vacio porque nunca llegan a este estado >:C
       // Asi que menoje
    }

    public override void Exit()
    {
        Debug.Log("IdleState Exit");
    }
}
