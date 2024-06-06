using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;

namespace HunkMod.SkillStates.Virus
{
	public class Death : GenericCharacterDeath
	{
		private bool hasDied;

		public override void FixedUpdate()
		{
			if (base.isAuthority)
            {
				BlastAttack blastAttack = new BlastAttack();
				blastAttack.radius = 8f;
				blastAttack.procCoefficient = 0.01f;
				blastAttack.position = this.characterBody.corePosition;
				blastAttack.attacker = this.gameObject;
				blastAttack.crit = false;
				blastAttack.baseDamage = this.damageStat * 2f;
				blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
				blastAttack.baseForce = 4000f;
				blastAttack.bonusForce = Vector3.up * 600f;
				blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
				blastAttack.damageType = DamageType.AOE;
				blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

				blastAttack.Fire();
			}

			Util.PlaySound("sfx_hunk_grenade_explosion", this.gameObject);

			base.FixedUpdate();
			if (NetworkServer.active && !this.hasDied)
			{
				this.hasDied = true;

				EffectManager.SimpleImpactEffect(Modules.Assets.bloodExplosionEffect, base.characterBody.corePosition, Vector3.up, true);
				EffectManager.SimpleImpactEffect(Modules.Assets.cVirusExplosion, base.characterBody.corePosition, Vector3.up, true);

				base.DestroyBodyAsapServer();
				EntityState.Destroy(base.gameObject);
			}
		}

		public override void OnExit()
		{
			base.DestroyModel();
			base.OnExit();
		}
	}
}