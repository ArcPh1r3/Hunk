using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class TemplarAirDodge : AirDodge
    {
        private HealthComponent target;
        public GameObject targetObject;
        private bool boomboom;

        protected override bool forcePerfect => true;

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.targetObject) this.target = this.targetObject.GetComponent<HealthComponent>();
        }

        public override void OnExit()
        {
            base.OnExit();

            this.Kaboom();
        }

        protected override void SetNextState()
        {
            this.outer.SetNextState(new SlowRoll());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= 0.6f) this.Kaboom();
        }

        private void Kaboom()
        {
            if (this.boomboom) return;
            this.boomboom = true;

            this.hunk.TriggerCounter();

            if (this.target)
            {
                EffectManager.SpawnEffect(Modules.Assets.explosionEffect, new EffectData
                {
                    origin = this.target.body.corePosition,
                    rotation = Quaternion.identity,
                    scale = 1
                }, false);

                this.target.gameObject.AddComponent<Modules.Components.TemplarExplosionTracker>();

                if (base.isAuthority)
                {
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.radius = 5f;
                    blastAttack.procCoefficient = 1f;
                    blastAttack.position = this.target.body.corePosition;
                    blastAttack.attacker = this.gameObject;
                    blastAttack.crit = this.RollCrit();
                    blastAttack.baseDamage = this.damageStat * 80f;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                    blastAttack.baseForce = 2000f;
                    blastAttack.bonusForce = Vector3.up * 600f;
                    blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                    blastAttack.damageType = DamageType.Stun1s | DamageType.ClayGoo;
                    blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                    blastAttack.Fire();
                }
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(this.targetObject);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            this.targetObject = reader.ReadGameObject();
        }
    }
}