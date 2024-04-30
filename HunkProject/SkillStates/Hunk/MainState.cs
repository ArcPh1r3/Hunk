using UnityEngine;
using RoR2;
using EntityStates;
using HunkMod.Modules;
using HunkMod.SkillStates.Emote;
using BepInEx.Configuration;
using HunkMod.Modules.Components;

namespace HunkMod.SkillStates.Hunk
{
    public class MainState : GenericCharacterMain
    {
		private HunkController hunk;
		private Animator animator;
		public LocalUser localUser;

		public override void OnEnter()
        {
            base.OnEnter();
			this.animator = this.modelAnimator;
			this.hunk = this.GetComponent<HunkController>();
			this.FindLocalUser();

			this.CheckForSuperSkin();
        }

		private void CheckForSuperSkin()
		{
			if (this.hunk && this.hunk.weaponTracker && Modules.Helpers.HunkHasWeapon(Modules.Weapons.ATM.instance.weaponDef, this.hunk.weaponTracker)) return;

			CharacterModel model = this.GetModelTransform().GetComponent<CharacterModel>();
			if (model && model.GetComponent<ModelSkinController>())
			{
				ModelSkinController msc = model.GetComponent<ModelSkinController>();
				if (msc.skins[msc.currentSkinIndex].nameToken.Contains("SUPER"))
				{
					if (Util.HasEffectiveAuthority(this.gameObject))
					{
						Chat.AddMessage("Thanks for supporting the mod :)");
					}

					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(Modules.Weapons.ATM.instance.itemDef.itemIndex), this.characterBody.corePosition, this.characterBody.inputBank.aimDirection * 30f);
				}
			}
		}

		public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.animator)
            {
                bool cock = false;
                if (!this.characterBody.outOfDanger || !this.characterBody.outOfCombat) cock = true;

                this.animator.SetBool("inCombat", cock);

				if (this.isGrounded) this.animator.SetFloat("airBlend", 0f);
				else this.animator.SetFloat("airBlend", 1f);

				this.animator.SetBool("isAiming", this.hunk.isAiming);
				if (this.hunk.isAiming) this.animator.SetFloat("aimBlend", 1f);
				else this.animator.SetFloat("aimBlend", 0f);
			}

			//emotes
			if (base.isAuthority && base.characterMotor.isGrounded)
			{
				this.CheckEmote<Rest>(Config.restKey);
				this.CheckEmote<Taunt>(Config.tauntKey);
				this.CheckEmote<Dance>(Config.danceKey);
			}
		}

		private void CheckEmote(KeyCode keybind, EntityState state)
		{
			if (Input.GetKeyDown(keybind))
			{
				if (!localUser.isUIFocused)
				{
					outer.SetInterruptState(state, InterruptPriority.Any);
				}
			}
		}

		private void CheckEmote<T>(ConfigEntry<KeyboardShortcut> keybind) where T : EntityState, new()
		{
			if (Modules.Config.GetKeyPressed(keybind))
			{
				FindLocalUser();

				if (localUser != null && !localUser.isUIFocused)
				{
					outer.SetInterruptState(new T(), InterruptPriority.Any);
				}
			}
		}

		private void FindLocalUser()
		{
			if (this.localUser == null)
			{
				if (base.characterBody)
				{
					foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList)
					{
						if (lu.cachedBody == base.characterBody)
						{
							this.localUser = lu;
							break;
						}
					}
				}
			}
		}

		public override void ProcessJump()
        {
			if (this.hasCharacterMotor)
			{
				bool hopooFeather = false;
				bool waxQuail = false;

				if (this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
				{
					int waxQuailCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
					float horizontalBonus = 1f;
					float verticalBonus = 1f;

					if (base.characterMotor.jumpCount >= base.characterBody.baseJumpCount)
					{
						hopooFeather = true;
						horizontalBonus = 1.5f;
						verticalBonus = 1.5f;
					}
					else if (waxQuailCount > 0 && base.characterBody.isSprinting)
					{
						float v = base.characterBody.acceleration * base.characterMotor.airControl;

						if (base.characterBody.moveSpeed > 0f && v > 0f)
						{
							waxQuail = true;
							float num2 = Mathf.Sqrt(10f * (float)waxQuailCount / v);
							float num3 = base.characterBody.moveSpeed / v;
							horizontalBonus = (num2 + num3) / num3;
						}
					}

					GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus, false);

					if (this.hasModelAnimator)
					{
						int layerIndex = base.modelAnimator.GetLayerIndex("Body");
						if (layerIndex >= 0)
						{
							if (this.characterBody.isSprinting)
                            {
								this.modelAnimator.CrossFadeInFixedTime("SprintJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
							}
							else
                            {
								if (hopooFeather)
                                {
									this.modelAnimator.CrossFadeInFixedTime("BonusJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
								}
								else
                                {
									this.modelAnimator.CrossFadeInFixedTime("Jump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
								}
                            }
						}
					}

					if (hopooFeather)
					{
						EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
						{
							origin = base.characterBody.footPosition
						}, true);
					}
					else if (base.characterMotor.jumpCount > 0)
					{
						EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
						{
							origin = base.characterBody.footPosition,
							scale = base.characterBody.radius
						}, true);
					}

					if (waxQuail)
					{
						EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
						{
							origin = base.characterBody.footPosition,
							rotation = Util.QuaternionSafeLookRotation(base.characterMotor.velocity)
						}, true);
					}

					base.characterMotor.jumpCount++;

					// set up double jump anim
					if (this.animator)
					{
						float x = this.animatorWalkParamCalculator.animatorWalkSpeed.y;
						float y = this.animatorWalkParamCalculator.animatorWalkSpeed.x;

						// neutral jump
						if (Mathf.Abs(x) <= 0.45f && Mathf.Abs(y) <= 0.45f || this.inputBank.moveVector == Vector3.zero)
						{
							x = 0f;
							y = 0f;
						}

						if (Mathf.Abs(x) > Mathf.Abs(y))
						{
							// side flip
							if (x > 0f) x = 1f;
							else x = -1f;
							y = 0f;
						}
						else if (Mathf.Abs(x) < Mathf.Abs(y))
						{
							// forward/backflips
							if (y > 0f) y = 1f;
							else y = -1f;
							x = 0f;
						}
						// eh this feels less dynamic. ignore the slight anim clipping issues ig and just blend them
						//  actualyl don't because the clipping issues are nightmarish

						// have to cache it at time of jump otherwise you can fuck up the jump anim in weird ways by turning during it
						this.animator.SetFloat("forwardSpeedCached", y);
						this.animator.SetFloat("rightSpeedCached", x);
						// turns out this wasn't even used in the end. the animation didn't break at all in practice, only in theory
						// Fuck You rob you fucking moron

						//  update: this was actually used. what the hell are you doing?
					}
				}
			}
		}
    }
}