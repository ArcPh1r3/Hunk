using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using RoR2.CharacterAI;
using System.Linq;

namespace HunkMod.SkillStates.Parasite
{
	public class Infest : BaseState
	{
		private Transform targetTransform;
		private GameObject infestVfxInstance;
		private OverlapAttack attack;
		private List<HurtBox> victimsStruck = new List<HurtBox>();

		public override void OnEnter()
		{
			base.OnEnter();
			this.PlayAnimation("Base", "Infest");
			Util.PlaySound(EntityStates.VoidInfestor.Infest.enterSoundString, base.gameObject);
			if (EntityStates.VoidInfestor.Infest.enterEffectPrefab) EffectManager.SimpleImpactEffect(EntityStates.VoidInfestor.Infest.enterEffectPrefab, base.characterBody.corePosition, Vector3.up, false);
			if (EntityStates.VoidInfestor.Infest.infestVfxPrefab)
			{
				this.infestVfxInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.VoidInfestor.Infest.infestVfxPrefab, base.transform.position, Quaternion.identity);
				this.infestVfxInstance.transform.parent = base.transform;
			}

			HitBoxGroup hitBoxGroup = null;
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform) hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Infest");
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = base.GetTeam();
			this.attack.damage = EntityStates.VoidInfestor.Infest.infestDamageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = null;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = base.RollCrit();
			this.attack.damageType = DamageType.Stun1s;
			this.attack.damageColorIndex = DamageColorIndex.Void;

			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.viewer = base.characterBody;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(base.characterBody.teamComponent.teamIndex);
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.minDistanceFilter = 0f;
			bullseyeSearch.maxDistanceFilter = EntityStates.VoidInfestor.Infest.searchMaxDistance;
			bullseyeSearch.searchOrigin = base.inputBank.aimOrigin;
			bullseyeSearch.searchDirection = base.inputBank.aimDirection;
			bullseyeSearch.maxAngleFilter = EntityStates.VoidInfestor.Infest.searchMaxAngle;
			bullseyeSearch.filterByLoS = true;
			bullseyeSearch.RefreshCandidates();

			HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
			if (hurtBox)
			{
				this.targetTransform = hurtBox.transform;
				if (base.characterMotor)
				{
					Vector3 position = this.targetTransform.position;
					float num = EntityStates.VoidInfestor.Infest.velocityInitialSpeed;
					Vector3 vector = position - base.transform.position;
					Vector2 vector2 = new Vector2(vector.x, vector.z);
					float magnitude = vector2.magnitude;
					float y = Trajectory.CalculateInitialYSpeed(magnitude / num, vector.y);
					Vector3 vector3 = new Vector3(vector2.x / magnitude * num, y, vector2.y / magnitude * num);
					base.characterMotor.velocity = vector3;
					base.characterMotor.disableAirControlUntilCollision = true;
					base.characterMotor.Motor.ForceUnground();
					base.characterDirection.forward = vector3;
				}
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.targetTransform && base.characterMotor)
			{
				Vector3 target = this.targetTransform.position - base.transform.position;
				Vector3 vector = base.characterMotor.velocity;
				vector = Vector3.RotateTowards(vector, target, EntityStates.VoidInfestor.Infest.velocityTurnRate * Time.fixedDeltaTime * 0.017453292f, 0f);
				base.characterMotor.velocity = vector;
				if (NetworkServer.active && this.attack.Fire(this.victimsStruck))
				{
					int i = 0;
					while (i < this.victimsStruck.Count)
					{
						HealthComponent healthComponent = this.victimsStruck[i].healthComponent;
						CharacterBody body = healthComponent.body;
						CharacterMaster master = body.master;
						if (healthComponent.alive && master != null && !body.isPlayerControlled && !body.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical))
						{
							/*master.teamIndex = TeamIndex.Void;
							body.teamComponent.teamIndex = TeamIndex.Void;
							body.inventory.SetEquipmentIndex(DLC1Content.Elites.Void.eliteEquipmentDef.equipmentIndex);
							// ^
							// todo: make our own elite type and replace this

							BaseAI component = master.GetComponent<BaseAI>();
							if (component)
							{
								component.enemyAttention = 0f;
								component.ForceAcquireNearestEnemyIfNoCurrentEnemy();
							}

							base.healthComponent.Suicide(null, null, DamageType.Generic);
							if (EntityStates.VoidInfestor.Infest.successfulInfestEffectPrefab)
							{
								EffectManager.SimpleImpactEffect(EntityStates.VoidInfestor.Infest.successfulInfestEffectPrefab, base.transform.position, Vector3.up, false);
								break;
							}
							break;*/
							// actually i'm pretty sure the void equipment just spawns a new infestor on death so i gotta take this off until we have a custom elite type
						}
						else
						{
							i++;
						}
					}
				}
			}

			if (base.characterDirection) base.characterDirection.moveVector = base.characterMotor.velocity;

			if (base.isAuthority && base.characterMotor && base.characterMotor.isGrounded && base.fixedAge > 0.1f)
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override void OnExit()
		{
			if (this.infestVfxInstance) EntityState.Destroy(this.infestVfxInstance);
			base.OnExit();
		}
	}
}