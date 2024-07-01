using System;
using EntityStates;
using EntityStates.Bison;
using RoR2;
using UnityEngine;

namespace HunkMod.SkillStates.Hunk.Counter
{
	public class BisonCharge : BaseState
	{
		public CharacterBody ownerBody;

		private float stopwatch;
		private float overlapResetStopwatch;
		private Animator animator;
		private Vector3 targetMoveVector;
		private OverlapAttack attack;
		private HitBoxGroup hitboxGroup;
		private ChildLocator childLocator;
		private Transform sphereCheckTransform;
		private string baseFootstepString;

		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.childLocator = this.animator.GetComponent<ChildLocator>();
			FootstepHandler component = this.animator.GetComponent<FootstepHandler>();
			if (component)
			{
				this.baseFootstepString = component.baseFootstepString;
				component.baseFootstepString = Charge.footstepOverrideSoundString;
			}
			Util.PlaySound(Charge.startSoundString, base.gameObject);
			base.PlayCrossfade("Body", "ChargeForward", 0.2f);
			this.ResetOverlapAttack();
			this.SetSprintEffectActive(true);
			if (this.childLocator)
			{
				this.sphereCheckTransform = this.childLocator.FindChild("SphereCheckTransform");
			}
			if (!this.sphereCheckTransform && base.characterBody)
			{
				this.sphereCheckTransform = base.characterBody.coreTransform;
			}
			if (!this.sphereCheckTransform)
			{
				this.sphereCheckTransform = base.transform;
			}
		}

		private void SetSprintEffectActive(bool active)
		{
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild("SprintEffect");
				if (transform == null)
				{
					return;
				}
				transform.gameObject.SetActive(active);
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			base.characterMotor.moveDirection = Vector3.zero;
			Util.PlaySound(Charge.endSoundString, base.gameObject);
			Util.PlaySound("stop_bison_charge_attack_loop", base.gameObject);
			this.SetSprintEffectActive(false);
			FootstepHandler component = this.animator.GetComponent<FootstepHandler>();
			if (component)
			{
				component.baseFootstepString = this.baseFootstepString;
			}
		}

		public override void FixedUpdate()
		{
			// no turning!
			//this.targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(this.targetMoveVector, base.inputBank.aimDirection, ref this.targetMoveVectorVelocity, Charge.turnSmoothTime, Charge.turnSpeed), Vector3.up).normalized;
			base.characterDirection.moveVector = this.targetMoveVector;
			Vector3 forward = base.characterDirection.forward;
			float value = this.moveSpeedStat * Charge.chargeMovementSpeedCoefficient;
			base.characterMotor.moveDirection = forward * Charge.chargeMovementSpeedCoefficient;
			this.animator.SetFloat("forwardSpeed", value);
			if (base.isAuthority && this.attack.Fire(null))
			{
				Util.PlaySound(Charge.headbuttImpactSound, base.gameObject);
			}
			if (this.overlapResetStopwatch >= 1f / Charge.overlapResetFrequency)
			{
				this.overlapResetStopwatch -= 1f / Charge.overlapResetFrequency;
			}

			if (base.fixedAge >= 0.1f)
            {
				if (base.isAuthority && Physics.OverlapSphere(this.sphereCheckTransform.position, Charge.overlapSphereRadius, LayerIndex.world.mask).Length != 0)
				{
					Util.PlaySound("sfx_hunk_kick_impact", this.gameObject);
					Util.PlaySound(Charge.headbuttImpactSound, base.gameObject);
					EffectManager.SimpleMuzzleFlash(Charge.hitEffectPrefab, base.gameObject, "SphereCheckTransform", true);
					base.healthComponent.TakeDamageForce(forward * Charge.selfStunForce, true, false);

					if (this.ownerBody)
					{
						BlastAttack blastAttack = new BlastAttack();
						blastAttack.radius = 2f;
						blastAttack.procCoefficient = 1f;
						blastAttack.position = this.characterBody.corePosition;
						blastAttack.attacker = this.ownerBody.gameObject;
						blastAttack.crit = Util.CheckRoll(this.ownerBody.crit, this.ownerBody.master);
						blastAttack.baseDamage = this.ownerBody.damage * 80f;
						blastAttack.falloffModel = BlastAttack.FalloffModel.None;
						blastAttack.baseForce = 0f;
						blastAttack.bonusForce = Vector3.zero;
						blastAttack.teamIndex = TeamComponent.GetObjectTeam(this.ownerBody.gameObject);
						blastAttack.damageType = DamageType.ClayGoo;
						blastAttack.attackerFiltering = AttackerFiltering.AlwaysHitSelf;
						blastAttack.Fire();

						EffectManager.SpawnEffect(Modules.Assets.kickImpactEffect, new EffectData
						{
							origin = this.characterBody.corePosition,
							scale = 2f
						}, true);
					}

					StunState stunState = new StunState();
					stunState.stunDuration = Charge.selfStunDuration;
					this.outer.SetNextState(stunState);
					return;
				}
			}

			this.stopwatch += Time.fixedDeltaTime;
			this.overlapResetStopwatch += Time.fixedDeltaTime;
			if (this.stopwatch > Charge.chargeDuration)
			{
				this.outer.SetNextStateToMain();
			}
			base.FixedUpdate();
		}

		private void ResetOverlapAttack()
		{
			if (!this.ownerBody) return;

			if (!this.hitboxGroup)
			{
				Transform modelTransform = base.GetModelTransform();
				if (modelTransform)
				{
					this.hitboxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Charge");
				}
			}

			this.attack = new OverlapAttack();
			this.attack.attacker = this.gameObject;
			this.attack.inflictor = this.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.ownerBody.gameObject);
			this.attack.damage = 40f * this.ownerBody.damage;
			this.attack.hitEffectPrefab = Modules.Assets.kickImpactEffect;
			this.attack.impactSound = Modules.Assets.punchImpactSoundDef.index;
			this.attack.forceVector = Vector3.up * Charge.upwardForceMagnitude;
			this.attack.isCrit = false;
			this.attack.pushAwayForce = Charge.awayForceMagnitude;
			this.attack.hitBoxGroup = this.hitboxGroup;
			this.attack.attackerFiltering = AttackerFiltering.NeverHitSelf;
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}