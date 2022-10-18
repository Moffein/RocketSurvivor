using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Linq;

namespace RocketSurvivor.Components.Projectile
{
	[RequireComponent(typeof(TeamFilter))]
	[RequireComponent(typeof(ProjectileTargetComponent))]
	public class ProjectileDirectionalTargetFinder2 : MonoBehaviour
	{
		// Token: 0x0600434C RID: 17228 RVA: 0x00124F80 File Offset: 0x00123180
		private void Start()
		{
			if (!NetworkServer.active)
			{
				base.enabled = false;
				return;
			}
			this.bullseyeSearch = new BullseyeSearch();
			this.teamFilter = base.GetComponent<TeamFilter>();
			this.targetComponent = base.GetComponent<ProjectileTargetComponent>();
			this.transform = base.transform;
			this.searchTimer = 0f;
		}

		// Token: 0x0600434D RID: 17229 RVA: 0x00124FD8 File Offset: 0x001231D8
		private void FixedUpdate()
		{
			this.searchTimer -= Time.fixedDeltaTime;
			if (this.searchTimer <= 0f)
			{
				this.searchTimer += this.targetSearchInterval;
				if (this.allowTargetLoss && this.targetComponent.target != null && this.lastFoundTransform == this.targetComponent.target && !this.PassesFilters(this.lastFoundHurtBox))
				{
					this.SetTarget(null);
				}
				if (!this.onlySearchIfNoTarget || this.targetComponent.target == null)
				{
					this.SearchForTarget();
				}
				this.hasTarget = (this.targetComponent.target != null);
				if (this.hadTargetLastUpdate != this.hasTarget)
				{
					if (this.hasTarget)
					{
						UnityEvent unityEvent = this.onNewTargetFound;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
					}
					else
					{
						UnityEvent unityEvent2 = this.onTargetLost;
						if (unityEvent2 != null)
						{
							unityEvent2.Invoke();
						}
					}
				}
				this.hadTargetLastUpdate = this.hasTarget;
			}
		}

		// Token: 0x0600434E RID: 17230 RVA: 0x001250D8 File Offset: 0x001232D8
		private bool PassesFilters(HurtBox result)
		{
			CharacterBody body = result.healthComponent.body;
			return body && (!this.ignoreAir || !body.isFlying) && (!body.isFlying || float.IsInfinity(this.flierAltitudeTolerance) || this.flierAltitudeTolerance >= Mathf.Abs(result.transform.position.y - this.transform.position.y));
		}

		// Token: 0x0600434F RID: 17231 RVA: 0x00125154 File Offset: 0x00123354
		private void SearchForTarget()
		{
			this.bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			this.bullseyeSearch.teamMaskFilter.RemoveTeam(this.teamFilter.teamIndex);
			this.bullseyeSearch.filterByLoS = this.testLoS;
			this.bullseyeSearch.searchOrigin = this.transform.position;
			this.bullseyeSearch.searchDirection = this.transform.forward;
			this.bullseyeSearch.maxDistanceFilter = this.lookRange;
			this.bullseyeSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
			this.bullseyeSearch.maxAngleFilter = this.lookCone;
			this.bullseyeSearch.RefreshCandidates();
			IEnumerable<HurtBox> source = this.bullseyeSearch.GetResults().Where(new Func<HurtBox, bool>(this.PassesFilters));
			this.SetTarget(source.FirstOrDefault<HurtBox>());
		}

		// Token: 0x06004350 RID: 17232 RVA: 0x0002EB15 File Offset: 0x0002CD15
		private void SetTarget(HurtBox hurtBox)
		{
			this.lastFoundHurtBox = hurtBox;
			this.lastFoundTransform = ((hurtBox != null) ? hurtBox.transform : null);
			this.targetComponent.target = this.lastFoundTransform;
		}

		// Token: 0x06004351 RID: 17233 RVA: 0x0012522C File Offset: 0x0012342C
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Gizmos.DrawWireSphere(position, this.lookRange);
			Gizmos.DrawRay(position, transform.forward * this.lookRange);
			Gizmos.DrawFrustum(position, this.lookCone * 2f, this.lookRange, 0f, 1f);
			if (!float.IsInfinity(this.flierAltitudeTolerance))
			{
				Gizmos.DrawWireCube(position, new Vector3(this.lookRange * 2f, this.flierAltitudeTolerance * 2f, this.lookRange * 2f));
			}
		}

		// Token: 0x04004168 RID: 16744
		[Tooltip("How far ahead the projectile should look to find a target.")]
		public float lookRange;

		// Token: 0x04004169 RID: 16745
		[Tooltip("How wide the cone of vision for this projectile is in degrees. Limit is 180.")]
		[Range(0f, 180f)]
		public float lookCone;

		// Token: 0x0400416A RID: 16746
		[Tooltip("How long before searching for a target.")]
		public float targetSearchInterval = 0.5f;

		// Token: 0x0400416B RID: 16747
		[Tooltip("Will not search for new targets once it has one.")]
		public bool onlySearchIfNoTarget;

		// Token: 0x0400416C RID: 16748
		[Tooltip("Allows the target to be lost if it's outside the acceptable range.")]
		public bool allowTargetLoss;

		// Token: 0x0400416D RID: 16749
		[Tooltip("If set, targets can only be found when there is a free line of sight.")]
		public bool testLoS;

		// Token: 0x0400416E RID: 16750
		[Tooltip("Whether or not airborne characters should be ignored.")]
		public bool ignoreAir;

		// Token: 0x0400416F RID: 16751
		[Tooltip("The difference in altitude at which a result will be ignored.")]
		public float flierAltitudeTolerance = float.PositiveInfinity;

		// Token: 0x04004170 RID: 16752
		public UnityEvent onNewTargetFound;

		// Token: 0x04004171 RID: 16753
		public UnityEvent onTargetLost;

		// Token: 0x04004172 RID: 16754
		private new Transform transform;

		// Token: 0x04004173 RID: 16755
		private TeamFilter teamFilter;

		// Token: 0x04004174 RID: 16756
		private ProjectileTargetComponent targetComponent;

		// Token: 0x04004175 RID: 16757
		private float searchTimer;

		// Token: 0x04004176 RID: 16758
		private bool hasTarget;

		// Token: 0x04004177 RID: 16759
		private bool hadTargetLastUpdate;

		// Token: 0x04004178 RID: 16760
		private BullseyeSearch bullseyeSearch;

		// Token: 0x04004179 RID: 16761
		private HurtBox lastFoundHurtBox;

		// Token: 0x0400417A RID: 16762
		private Transform lastFoundTransform;

		public BullseyeSearch.SortMode SearchMode = BullseyeSearch.SortMode.Angle;
	}
}
