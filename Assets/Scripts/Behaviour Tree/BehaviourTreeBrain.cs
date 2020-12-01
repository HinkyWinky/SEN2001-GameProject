using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    public class BehaviourTreeBrain : MonoBehaviour
    {
        private Player Player => GameManager.Cur.Player;

        private Enemy enemy;
        private NavMeshAgent agent;
        private NavMeshPath path;

        public CheckIsHealthFullLeaf checkIsHealthFullLeaf;
        public CheckGreaterDistanceLeaf checkGreaterDistanceLeaf;
        public ActionMoveLeaf actionMoveLeaf;

        public Sequence rootNode;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();
            agent = GetComponent<NavMeshAgent>();

            path = new NavMeshPath();
        }

        private void Start()
        {
            checkIsHealthFullLeaf.SetFieldsOnUpdate(enemy.health, enemy.maxHealth);
            checkGreaterDistanceLeaf.SetFieldsOnStart(agent);
            checkGreaterDistanceLeaf.SetFieldsOnUpdate(Player.transform.position);
            actionMoveLeaf.SetFieldsOnStart(agent, ref path);
            actionMoveLeaf.SetFieldsOnUpdate(Player.transform.position);

            rootNode = new Sequence(new List<Node>
            {
                checkIsHealthFullLeaf,
                checkGreaterDistanceLeaf,
                actionMoveLeaf
            });
        }

        private void Update()
        {
            checkIsHealthFullLeaf.SetFieldsOnUpdate(enemy.health, enemy.maxHealth);
            checkGreaterDistanceLeaf.SetFieldsOnUpdate(Player.transform.position);
            actionMoveLeaf.SetFieldsOnUpdate(Player.transform.position);

            Evaluate();
        }

        private void Evaluate()
        {
            rootNode.Evaluate();
        }
    }
}
