using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatToTime
{
	const float MaxTime = 3600.00f;

	public static string ChangeFloatToTime(float value)
	{
		// 一時間以上にならないようにカンストさせる
		if (MaxTime < value)
		{
			return "59:59.99";
		}

		string m, s, ms;
		string time = "";

		// xx:xx.xxをstring形式で表示
		m = GetMtoString(value);
		s = GetStoString(value);
		ms = GetMStoString(value);

		time += m + ":" + s + "." + ms;

		return time;
	}

	public static string ChangeFloatToM_S(float value)
	{
		if (MaxTime < value)
		{
			return "59:99";
		}

		string m, s;
		string time = "";

		m = GetMtoString(value);
		s = GetStoString(value);

		time += m + ":" + s;

		return time;
	}

	public static string ChangeFloatToS_MS(float value)
	{
		if (MaxTime < value)
		{
			return "59.99";
		}

		string s, ms;
		string time = "";

		s = GetStoString(value);
		ms = GetMStoString(value);

		time += s + "." + ms;

		return time;
	}


	static string GetMtoString(float value)
	{
		string m = "";

		m = ((int)(value / 60.0f)).ToString();

		if (m.ToString().Length == 1)
		{
			m = "0" + m;
		}

		return m;
	}

	static string GetStoString(float value)
	{
		string s = "";

		s = ((int)(value % 60.0f)).ToString();

		if (s.ToString().Length == 1)
		{
			s = "0" + s;
		}

		return s;
	}

	static string GetMStoString(float value)
	{
		string ms = "";

		ms = ((int)((value * 100) % 100.0f)).ToString();

		if (ms.ToString().Length == 1)
		{
			ms = "0" + ms;
		}

		return ms;
	}


}
