﻿using System.Collections.Generic;
using Assets.Scripts.Main;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Graphics {

	public class DayNightCycle : MonoBehaviour {

		public static float Progress = .25f;
		public static int LightLevel = 60;

		private const int L_MORNING = 60;
		private const int L_MIDDAY = 240;
		private const int L_EVENING = 60;
		private const int L_MIDNIGHT = -180;

		private static readonly int[] C_MORNING = { 255, 220, 0 };
		private static readonly int[] C_MIDDAY = { 255 * 3, 255 * 3, 255 * 3 };
		private static readonly int[] C_EVENING = { 154, 89, 54 };
		private static readonly int[] C_MIDNIGHT = { -255, -255, 20 };

		private Light _sun;

		[UsedImplicitly]
		private void OnEnable () {
			transform.position = new Vector3(transform.position.x, transform.position.y, Order.SUN);
		}

		[UsedImplicitly]
		private void Start () {
			_sun = GetComponent<Light>();
		}

		[UsedImplicitly]
		private void Update () {
			DoCycle();
		}

		private void DoCycle () {
			if (!Toggles.DoCycle) {
				return;
			}

			float t = Progress % .25f * 4;

			if (Progress > .75f) {
				LerpColor(C_EVENING, C_MIDNIGHT, t);
				LerpLight(L_EVENING, L_MIDNIGHT, t);
			} else if (Progress > .5f) {
				LerpColor(C_MIDDAY, C_EVENING, t);
				LerpLight(L_MIDDAY, L_EVENING, t);
			} else if (Progress > .25f) {
				LerpColor(C_MORNING, C_MIDDAY, t);
				LerpLight(L_MORNING, L_MIDDAY, t);
			} else {
				LerpColor(C_MIDNIGHT, C_MORNING, t);
				LerpLight(L_MIDNIGHT, L_MORNING, t);
			}
		}

		private void LerpColor (IList<int> from, IList<int> to, float time) {
			const float speed = 1f / 60;

			int g = (int) Mathf.Lerp(from[1], to[1], time);
			int b = (int) Mathf.Lerp(from[2], to[2], time);
			int r = (int) Mathf.Lerp(from[0], to[0], time);
			byte rb = (byte) Mathf.Clamp(r, 0, 255);
			byte gb = (byte) Mathf.Clamp(g, 0, 255);
			byte bb = (byte) Mathf.Clamp(b, 0, 255);

			_sun.color = new Color32(rb, gb, bb, 255);
			Progress += speed * Time.deltaTime;

			if (Progress >= 1) {
				Progress = 0;
			}
		}

		private void LerpLight (int from, int to, float time) {
			int l = (int) Mathf.Lerp(from, to, time);
			LightLevel = Mathf.Clamp(l, 0, 100);
		}

	}

}