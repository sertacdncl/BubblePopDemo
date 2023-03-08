using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class Extension
	{
		public static Color With(this Color origin, float? r = null, float? g = null, float? b = null, float? a = null)
		{
			return new Color(r ?? origin.r, g ?? origin.g, b ?? origin.b, a ?? origin.a);
		}

		public static float MinusChance(this float origin, float chance = .5f)
		{
			return Random.value > chance ? origin * -1 : origin;
		}

		public static float GetPercent(this float origin, float percent)
		{
			return origin * percent / 100;
		}


		public static float GetAnimationClipLength(this Animator origin, string clipName)
		{
			var clipLength = 0f;

			foreach (var clip in origin.runtimeAnimatorController.animationClips)
			{
				if (clip.name == clipName || clip.name.Contains(clipName))
				{
					clipLength = clip.length;
				}
			}

			return clipLength;
		}

		public static int ToInt(this bool origin)
		{
			return origin ? 1 : 0;
		}

		public static int ExceptRange(this int origin, int min, int max, List<int> listExcept)
		{
			int result;
			do
			{
				result = Random.Range(min, max);
			} while (listExcept.Contains(result));

			return result;
		}

		public static T GetValue<T>(Dictionary<T, float> list)
		{
			float max = 0;
			foreach (var item in list)
				max += item.Value;

			var randomValue = Random.Range(0f, max);

			float count = 0;
			foreach (var optionsData in list)
			{
				count += optionsData.Value;
				if (randomValue <= count)
					return optionsData.Key;
			}

			throw new System.Exception();
		}


		public static bool IsEmpty<T, TT>(this Dictionary<T, TT> origin)
		{
			return origin.Count == 0;
		}

		public static bool ToBool(this int origin)
		{
			return origin > 0;
		}
		
		public static Vector2 GetPoint(this float degree)
		{
			degree = 360 - degree;
			degree += 90;
			
			float radian = degree.ToRadian();

			float x = Mathf.Cos(radian);
			float y = Mathf.Sin(radian);

			return new Vector2(x, y);
		}

		public static float ToRadian(this float origin)
		{
			return origin / 57.2957795f;
		}

		public static float ToDegree(this float origin)
		{
			return origin * 57.2957795f;
		}

		public static float GetDegree(this Vector2 coordinate)
		{
			float x = coordinate.x;
			float y = coordinate.y;

			float radianX = Mathf.Acos(x);
			float degreeX = radianX.ToDegree();

			float radianY = Mathf.Asin(y);
			float degreeY = radianY.ToDegree();

			float degree = 0;

			//++
			degree = degreeX;

			//-+ 
			if (x < 0 && y >= 0)
				degree = degreeX;

			//+-
			if (x >= 0 && y < 0)
				degree = 360 + degreeY;

			//--
			if (x < 0 && y < 0)
				degree = (180 - degreeX) * 2 + degreeX;

			
			degree = 360 - degree;
			degree += 90;

			return degree;
		}
	}
}