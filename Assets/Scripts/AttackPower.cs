using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPower : MonoBehaviour
{
	[SerializeField] float m_attackPower;

	public float Power
	{
		get { return m_attackPower; }
		set { m_attackPower = value; }
	}
}
