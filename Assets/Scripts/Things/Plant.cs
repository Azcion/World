﻿using Assets.Scripts.Defs;
using Assets.Scripts.Enums;
using Assets.Scripts.Graphics;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Things {

	[UsedImplicitly]
	public class Plant : Thing, IThing {

		public PlantDef Def;
		public PlantSize Size;

		private const ThingType TYPE = Enums.ThingType.Plant;

		private static GameObject _childPrefab;

		private float _growth;

		public static Plant Create (Plant plant, PlantDef def) {
			plant.Def = def;

			return plant;
		}

		public void Initialize (float growth) {
			InitializeThing();

			Size = Def.PlantSize;
			_growth = growth;
			AdjustTransform(growth);
			SetSprite(AssetLoader.Get(TYPE, Def.DefName), Random.value < .5);
			IsSelectable = Def.DefName != "Plant_Grass";

			if (Size == PlantSize.Small) {
				//todo generate better random points
				for (int i = 0; i < Random.Range(0, 4); ++i) {
					CreateChildSprite();
				}
			}
		}

		public GameObject GameObject () {
			return gameObject;
		}

		public ThingType ThingType () {
			return Enums.ThingType.Plant;
		}

		[UsedImplicitly]
		private void Awake () {
			if (_childPrefab != null) {
				return;
			}

			_childPrefab = new GameObject("Child Prefab", typeof(SpriteRenderer));
			_childPrefab.SetActive(false);
		}

		private void AdjustTransform (float growth) {
			growth = Mathf.Clamp(growth, .15f, 1);
			float s;
			float x;
			float y;
			
			switch (Size) {
				case PlantSize.Small:
					s = Mathf.Lerp(.65f, .85f, growth);
					x = Random.Range(.1f, .9f);
					y = Random.Range(.1f, .9f);
					break;
				case PlantSize.Medium:
					s = Mathf.Lerp(1, 1.5f, growth);
					x = .5f;
					y = Mathf.Lerp(.2f, .04f, growth);
					break;
				case PlantSize.Large:
				default:
					s = Mathf.Lerp(1.28f, 1.95f, growth);
					x = .5f;
					y = .04f;
					break;
			}

			Child.localScale = new Vector3(s, s, 1);
			Child.localPosition = new Vector2(x, y);
		}

		private void CreateChildSprite () {
			float x = Random.Range(.1f, .9f);
			float y = Random.Range(.1f, .9f);
			float s = Mathf.Lerp(.65f, .85f, _growth);
			Vector2 pos = new Vector3(x, y);
			GameObject go = Instantiate(_childPrefab, pos, Quaternion.identity, Tf);
			go.transform.localScale = new Vector3(s, s, 1);
			SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
			sr.sprite = ChildRenderer.sprite;
			sr.sharedMaterial = ChildRenderer.sharedMaterial;
		}

	}

}