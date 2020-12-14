using System;
using Game.AI;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class CheckIsWayOpen : CheckLeaf
{
    private StateMachine Machine => btState.machine;

    [SerializeField, Range(-50f, 50f)] private float stopDistance = 1f;
    private Vector3 targetPos;

    protected override NodeStates Check()
    {
        Vector3 startPos = Machine.rig.position;
        Vector3 destination = targetPos + (startPos - targetPos).normalized * stopDistance;

        NavMesh.SamplePosition(destination, out NavMeshHit hit, 10f, 1);
        if (hit.hit)
            destination = hit.position;
        else
            return NodeStates.FAILURE;

        NavMesh.CalculatePath(startPos, destination, NavMesh.AllAreas, Machine.checkPath);
        if (Machine.checkPath.status == NavMeshPathStatus.PathComplete)
        {
            if (Machine.checkPath.corners.Length <= 2)
            {
                Machine.checkPath.ClearCorners();
                return NodeStates.SUCCESS;
            }
        }
        return NodeStates.FAILURE;
    }

    public void UpdateLeaf(Vector3 targetPosition)
    {
        targetPos = targetPosition;
    }
}
