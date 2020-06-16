﻿using Assets.Scripts.Main;
using Assets.Scripts.Makers;
using Assets.Scripts.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Things {

	[RequireComponent(typeof(SpriteRenderer))]
	public class Selector : MonoBehaviour {

		public static GameObject Instance;
		public static Thing Thing;
		
		private static bool _didSelect;
		
		public static void Select (Transform target, Thing thing) {
			if (Architect.Planning) {
				return;
			}

			_didSelect = true;
			Instance.transform.SetParent(target);
			Vector2 v = target.position;
			Instance.transform.position = new Vector3(v.x + .5f, v.y, Order.SELECTOR);
			Instance.SetActive(true);
			Thing = thing;

			SelectedInfo.Set(thing);
			SelectedInfo.Show();
		}

		public static void Deselect (bool force = false) {
			if (!force && SelectedInfo.IsHoveringOver) {
				return;
			}

			Instance.SetActive(false);
			Instance.transform.SetParent(null);
			Thing.Deselect();
			SelectedInfo.Hide();
		}

		[UsedImplicitly]
		private void Start () {
			gameObject.SetActive(false);
			GetComponent<SpriteRenderer>().enabled = true;
			Instance = gameObject;
		}

		[UsedImplicitly]
		private void LateUpdate () {
			if (_didSelect) {
				_didSelect = false;
				return;
			}

			if (ApplicationController.Ready) {
				if (Input.GetMouseButtonDown(0)) {
					Deselect();
				}
			}
		}

	}

}