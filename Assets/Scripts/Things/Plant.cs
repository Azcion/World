﻿using Assets.Scripts.Defs;
using Assets.Scripts.Enums;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Things {

	[UsedImplicitly]
	public class Plant : Thing, IThing {

		public PlantDef Def;
		public PlantSize Size;

		private static readonly Vector3[] Nodes = {
			new Vector3(.30f, -.05f),
			new Vector3(.65f, .15f),
			new Vector3(.25f, .45f),
			new Vector3(.75f, 0),
			new Vector3(.40f, .20f),
			new Vector3(.70f, .40f)
		};

		private static GameObject _childPrefab;

		private float _growth;

		public GameObject Go => gameObject;
		public ThingType Type => ThingType.Plant;

        static Plant () {
            for (int i = 0; i < Nodes.Length; i++) {
                Vector3 v = Nodes[i];
                Nodes[i] = new Vector3(v.x, v.y, Map.SubY * v.y);
            }
        }

		public static Plant Create (Plant plant, PlantDef def) {
			plant.Def = def;

			return plant;
		}

		public void Initialize (float growth) {
			InitializeThing();

			Size = Def.PlantSize;
			_growth = growth;
			string suffix = "";

			if (Def.TexCount > 1) {
				suffix = ((char) ('A' + Random.Range(0, Def.TexCount))).ToString();
			}

			bool isSmall = Size == PlantSize.Small;
			bool flipX = Random.value < .5;
            IsSelectable = Def.Selectable;

			if (!isSmall) {
                SetSprite(Assets.GetSprite(Def.DefName + suffix), flipX);
				AdjustTransform(growth);
                return;
			}

			float nodeIndex = Random.Range(0, Nodes.Length);
			AdjustTransform(growth, (int) nodeIndex);

			if (Def.DefName == "Grass") {
                flipX = false;
				ChildRenderer.sharedMaterial = Assets.SwayMat;
			}

            SetSprite(Assets.GetSprite(Def.DefName + suffix), flipX);
			int cloneCount = Random.Range(0, 4);
			float nodeOrder = Random.value > .5f ? 1 : -1;
			nodeOrder *= Random.value > .8f ? 1.5f : 1;

			for (int i = 0; i < cloneCount; ++i) {
				nodeIndex += nodeOrder;
				int index = Mod((int) nodeIndex, Nodes.Length);
				CreateChildSprite(index);
			}
		}

		private static int Mod (int n, int m) {
			int r = n % m;
			return r < 0 ? r + m : r;
		}

		[UsedImplicitly]
		private void Awake () {
			if (_childPrefab != null) {
				return;
			}

			_childPrefab = new GameObject("Child Prefab", typeof(SpriteRenderer));
			_childPrefab.SetActive(false);
		}

		private void AdjustTransform (float growth, int nodeIndex = 0) {
			growth = Mathf.Clamp(growth, .15f, 1);
			float s;
			float y = 0;

			switch (Size) {
				case PlantSize.Small:
					s = Mathf.Lerp(1, 1.5f, growth);
					break;
				case PlantSize.Medium:
					s = Mathf.Lerp(1, 1.5f, growth);
					y = Mathf.Lerp(.2f, .04f, growth);
					break;
				default: // Large
					s = Mathf.Lerp(1, 1.95f, growth);
					y = .04f;
					break;
			}

			Child.localScale = new Vector3(s, s, 1);
			
			if (Size == PlantSize.Small) {
				Child.localPosition = Nodes[nodeIndex];
			} else {
				Child.localPosition = new Vector2(.5f, y);
			}
		}

		private void CreateChildSprite (int nodeIndex) {
			float s = Mathf.Lerp(1, 1.5f, _growth);
			GameObject go = Instantiate(_childPrefab, transform.position, Quaternion.identity, Tf);
			go.transform.localPosition = Nodes[nodeIndex];
			go.transform.localScale = new Vector3(s, s, 1);
			go.name = "Clone";
			SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
			sr.sprite = ChildRenderer.sprite;
			sr.sharedMaterial = ChildRenderer.sharedMaterial;
			go.SetActive(true);
		}

	}

}