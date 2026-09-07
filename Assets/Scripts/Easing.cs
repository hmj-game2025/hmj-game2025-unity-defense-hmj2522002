using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Easing
{
	static float Exec(float begin, float end, float t)
	{
		return (begin * (1 - t) + end * t);
	}

	static Vector2 Exec(Vector2 begin, Vector2 end, float t)
	{
		return (begin * (1 - t) + end * t);
	}

	static Vector3 Exec(Vector3 begin, Vector3 end, float t)
	{
		return (begin * (1 - t) + end * t);
	}

	public static float EaseInCubic(float begin, float end, float t)
	{
		t = Mathf.Clamp01(t);
		return Exec(begin, end, t * t * t);
	}

	public static Vector2 EaseInCubic(Vector2 begin, Vector2 end, float t)
	{
		t = Mathf.Clamp01(t);
		return Exec(begin, end, t * t * t);
	}

	public static Vector3 EaseInCubic(Vector3 begin, Vector3 end, float t)
	{
		t = Mathf.Clamp01(t);
		return Exec(begin, end, t * t * t);
	}

	public static float EaseOutCubic(float begin, float end, float t)
	{
		t = Mathf.Clamp01(t);
		return Exec(begin, end, 1 - Mathf.Pow(1 - t, 3));
	}

	public static Vector2 EaseOutCubic(Vector2 begin, Vector2 end, float t)
	{
		t = Mathf.Clamp01(t);
		return Exec(begin, end, 1 - Mathf.Pow(1 - t, 3));
	}

	public static Vector3 EaseOutCubic(Vector3 begin, Vector3 end, float t)
	{
		t = Mathf.Clamp01(t);
		return Exec(begin, end, 1 - Mathf.Pow(1 - t, 3));
	}
}
