using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[SerializeField] NavMeshAgent m_agent;
	[SerializeField] Transform m_target;

	private void Update()
	{
		m_agent.SetDestination(m_target.position);
	}
}
