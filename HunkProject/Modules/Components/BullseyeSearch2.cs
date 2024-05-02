using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HunkMod.Modules.Components
{
	public class BullseyeSearch2
	{
		private struct CandidateInfo
		{
			public HurtBox hurtBox;
			public Vector3 position;
			public float dot;
			public float distanceSqr;

			public struct EntityEqualityComparer : IEqualityComparer<BullseyeSearch2.CandidateInfo>
			{
				public bool Equals(BullseyeSearch2.CandidateInfo a, BullseyeSearch2.CandidateInfo b)
				{
					return a.hurtBox.healthComponent == b.hurtBox.healthComponent;
				}

				public int GetHashCode(BullseyeSearch2.CandidateInfo obj)
				{
					return obj.hurtBox.healthComponent.GetHashCode();
				}
			}
		}

		public enum SortMode
		{
			None,
			Distance,
			Angle,
			DistanceAndAngle
		}

		private delegate BullseyeSearch2.CandidateInfo Selector(HurtBox hurtBox);

		public CharacterBody viewer;
		public bool onlyBullseyes;
		public Vector3 searchOrigin;
		public Vector3 searchDirection;
		private float minThetaDot = -1f;
		private float maxThetaDot = 1f;
		public float minDistanceFilter;
		public float maxDistanceFilter = float.PositiveInfinity;
		public TeamMask teamMaskFilter = TeamMask.allButNeutral;
		public bool filterByLoS = true;
		public bool filterByDistinctEntity;
		public QueryTriggerInteraction queryTriggerInteraction;
		public BullseyeSearch2.SortMode sortMode = BullseyeSearch2.SortMode.Distance;
		private IEnumerable<BullseyeSearch2.CandidateInfo> candidatesEnumerable;

		public float minAngleFilter
		{
			set
			{
				this.maxThetaDot = Mathf.Cos(Mathf.Clamp(value, 0f, 180f) * 0.017453292f);
			}
		}

		public float maxAngleFilter
		{
			set
			{
				this.minThetaDot = Mathf.Cos(Mathf.Clamp(value, 0f, 180f) * 0.017453292f);
			}
		}

		private bool filterByDistance
		{
			get
			{
				return this.minDistanceFilter > 0f || this.maxDistanceFilter < float.PositiveInfinity || (this.viewer && this.viewer.visionDistance < float.PositiveInfinity);
			}
		}

		private bool filterByAngle
		{
			get
			{
				return this.minThetaDot > -1f || this.maxThetaDot < 1f;
			}
		}

		private Func<HurtBox, BullseyeSearch2.CandidateInfo> GetSelector()
		{
			bool getDot = this.filterByAngle;
			bool getDistanceSqr = this.filterByDistance;
			getDistanceSqr |= (this.sortMode == BullseyeSearch2.SortMode.Distance || this.sortMode == BullseyeSearch2.SortMode.DistanceAndAngle);
			getDot |= (this.sortMode == BullseyeSearch2.SortMode.Angle || this.sortMode == BullseyeSearch2.SortMode.DistanceAndAngle);
			bool getDifference = getDot | getDistanceSqr;
			bool getPosition = (getDot | getDistanceSqr) || this.filterByLoS;
			return delegate (HurtBox hurtBox)
			{
				BullseyeSearch2.CandidateInfo candidateInfo = new BullseyeSearch2.CandidateInfo
				{
					hurtBox = hurtBox
				};
				if (getPosition)
				{
					candidateInfo.position = hurtBox.transform.position;
				}
				Vector3 vector = default(Vector3);
				if (getDifference)
				{
					vector = candidateInfo.position - this.searchOrigin;
				}
				if (getDot)
				{
					candidateInfo.dot = Vector3.Dot(this.searchDirection, vector.normalized);
				}
				if (getDistanceSqr)
				{
					candidateInfo.distanceSqr = vector.sqrMagnitude;
				}
				return candidateInfo;
			};
		}

		public void RefreshCandidates()
		{
			Func<HurtBox, BullseyeSearch2.CandidateInfo> selector = this.GetSelector();

			if (this.onlyBullseyes)
			{
				this.candidatesEnumerable = (from hurtBox in HurtBox.readOnlyBullseyesList
											 where this.teamMaskFilter.HasTeam(hurtBox.teamIndex)
											 select hurtBox).Select(selector);
			}
			else
			{
				this.candidatesEnumerable = (from hurtBox in HunkMod.MainPlugin.hurtboxesList
											 where this.teamMaskFilter.HasTeam(hurtBox.teamIndex)
											 select hurtBox).Select(selector);
			}

			if (this.filterByAngle)
			{
				//this.candidatesEnumerable = this.candidatesEnumerable.Where(new Func<BullseyeSearch2.CandidateInfo, bool>(this.< RefreshCandidates > g__DotOkay | 25_1));
			}

			if (this.filterByDistance)
			{
				this.candidatesEnumerable = from v in this.candidatesEnumerable
											where Vector3.Distance(this.searchOrigin, v.hurtBox.transform.position) <= this.maxDistanceFilter
											select v;
			}

			if (this.filterByDistinctEntity)
			{
				this.candidatesEnumerable = this.candidatesEnumerable.Distinct(default(BullseyeSearch2.CandidateInfo.EntityEqualityComparer));
			}

			Func<BullseyeSearch2.CandidateInfo, float> sorter = this.GetSorter();
			if (sorter != null)
			{
				this.candidatesEnumerable = this.candidatesEnumerable.OrderBy(sorter);
			}
		}

		private Func<BullseyeSearch2.CandidateInfo, float> GetSorter()
		{
			switch (this.sortMode)
			{
				case BullseyeSearch2.SortMode.Distance:
					return (BullseyeSearch2.CandidateInfo candidateInfo) => candidateInfo.distanceSqr;
				case BullseyeSearch2.SortMode.Angle:
					return (BullseyeSearch2.CandidateInfo candidateInfo) => -candidateInfo.dot;
				case BullseyeSearch2.SortMode.DistanceAndAngle:
					return (BullseyeSearch2.CandidateInfo candidateInfo) => -candidateInfo.dot * candidateInfo.distanceSqr;
				default:
					return null;
			}
		}

		public void FilterCandidatesByHealthFraction(float minHealthFraction = 0f, float maxHealthFraction = 1f)
		{
			if (minHealthFraction > 0f)
			{
				if (maxHealthFraction < 1f)
				{
					this.candidatesEnumerable = this.candidatesEnumerable.Where(delegate (BullseyeSearch2.CandidateInfo v)
					{
						float combinedHealthFraction = v.hurtBox.healthComponent.combinedHealthFraction;
						return combinedHealthFraction >= minHealthFraction && combinedHealthFraction <= maxHealthFraction;
					});
					return;
				}
				this.candidatesEnumerable = from v in this.candidatesEnumerable
											where v.hurtBox.healthComponent.combinedHealthFraction >= minHealthFraction
											select v;
				return;
			}
			else
			{
				if (maxHealthFraction < 1f)
				{
					this.candidatesEnumerable = from v in this.candidatesEnumerable
												where v.hurtBox.healthComponent.combinedHealthFraction <= maxHealthFraction
												select v;
					return;
				}
				return;
			}
		}

		public void FilterOutGameObject(GameObject gameObject)
		{
			this.candidatesEnumerable = from v in this.candidatesEnumerable
										where v.hurtBox.healthComponent.gameObject != gameObject
										select v;
		}

		public IEnumerable<HurtBox> GetResults()
		{
			IEnumerable<BullseyeSearch2.CandidateInfo> source = this.candidatesEnumerable;
			if (this.filterByLoS)
			{
				source = from candidateInfo in source
						 where this.CheckLoS(candidateInfo.position)
						 select candidateInfo;
			}
			if (this.viewer)
			{
				source = from candidateInfo in source
						 where this.CheckVisible(candidateInfo.hurtBox.healthComponent.gameObject)
						 select candidateInfo;
			}
			return from candidateInfo in source
				   select candidateInfo.hurtBox;
		}

		private bool CheckLoS(Vector3 targetPosition)
		{
			Vector3 direction = targetPosition - this.searchOrigin;
			RaycastHit raycastHit;
			return !Physics.Raycast(this.searchOrigin, direction, out raycastHit, direction.magnitude, LayerIndex.world.mask, this.queryTriggerInteraction);
		}

		private bool CheckVisible(GameObject gameObject)
		{
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			return !component || component.GetVisibilityLevel(this.viewer) >= VisibilityLevel.Revealed;
		}
	}
}