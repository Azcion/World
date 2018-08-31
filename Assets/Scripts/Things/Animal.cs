﻿using Assets.Scripts.Enums;
using Assets.Scripts.Graphics;
using Assets.Scripts.Utils;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Things {

	public class Animal : Pathfinding, ICreature {

		[UsedImplicitly]
		public Sprite[] Sprites;

		public AnimalType Type;

		private bool _didInitialize;

		public void Initialize (AnimalType type) {
			InitializePathfinding(AdjustSpeed(type));

			Type = type;
			Sprite.localPosition = new Vector2(.5f, .5f);
			Sprite.localScale = AdjustScale(type);
			SetSprite(AssetLoader.Get(type, Direction.Up), false);
			SetTint(AdjustTint(type));
			_didInitialize = true;
		}

		public ThingType ThingType () {
			return Enums.ThingType.Creature;
		}

		private static float AdjustSpeed (AnimalType type) {
			switch (type) {
				case AnimalType.Elephant:
					return 1;
				case AnimalType.Gazelle:
				case AnimalType.Iguana:
					return 2;
				case AnimalType.Tortoise:
					return .35f;
				default:
					return 2;
			}
		}

		private static Vector3 AdjustScale (AnimalType type) {
			switch (type) {
				case AnimalType.Elephant:
					return new Vector3(3, 3, 1);
				case AnimalType.Gazelle:
				case AnimalType.Iguana:
				case AnimalType.Tortoise:
					return new Vector3(1, 1, 1);
				default:
					return new Vector3(1, 1, 1);
			}
		}

		private static Color AdjustTint (AnimalType type) {
			switch (type) {
				case AnimalType.Elephant:
					return Color.gray;
				default:
					return Color.white;
			}
		}

		[UsedImplicitly]
		//todo move to manual update
		private void Update () {
			if (_didInitialize == false) {
				return;
			}

			if (DirectionChanged) {
				SetSprite(AssetLoader.Get(Type, Facing), Facing == Direction.Left);
				DirectionChanged = false;
			}

			if (Moving || Random.value < .995) {
				return;
			}

			//todo implement smarter targeting
			Vector3 v = Tf.localPosition;
			v += new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
			v = Calc.Clamp(v);

			FindPath(v);
		}

	}

}