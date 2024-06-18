using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public class MouthGrenade : MonoBehaviour
    {
        public CharacterBody attackerBody;

        private GameObject grenadeInstance;
        private Transform jawTransform;

        private void Awake()
        {
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            if (modelLocator && modelLocator.modelTransform)
            {
                ChildLocator childLocator = modelLocator.modelTransform.GetComponent<ChildLocator>();
                if (childLocator)
                {
                    this.jawTransform = childLocator.FindChild("Head").Find("jaw");
                    this.grenadeInstance = GameObject.Instantiate(Modules.Assets.fragGrenade);
                    this.grenadeInstance.transform.parent = childLocator.FindChild("Head");
                    this.grenadeInstance.transform.localPosition = new Vector3(0f, 3f, -1f);
                    this.grenadeInstance.transform.localRotation = Quaternion.Euler(new Vector3(315f, 0f, 90f));
                    this.grenadeInstance.transform.localScale = Vector3.one * 2f;
                }
            }

            if (NetworkServer.active)
            {
                CharacterBody characterBody = this.GetComponent<CharacterBody>();
                if (characterBody) characterBody.AddBuff(Modules.Survivors.Hunk.grenadeInMouthBuff);
            }
        }

        private void Update()
        {
            if (!this.attackerBody)
            {
                Destroy(this);
            }
        }

        private void LateUpdate()
        {
            if (this.jawTransform)
            {
                this.jawTransform.localRotation = Quaternion.Euler(new Vector3(20f, 180f, 0f));
            }
        }

        public void Detonate()
        {
            if (this.attackerBody)
            {
                Vector3 pos = this.transform.position;
                if (this.jawTransform) pos = this.jawTransform.position;
                if (this.grenadeInstance) pos = this.grenadeInstance.transform.position;

                EffectManager.SpawnEffect(Modules.Assets.explosionEffect, new EffectData
                {
                    origin = pos,
                    rotation = Quaternion.identity,
                    scale = 1
                }, false);

                this.gameObject.AddComponent<Modules.Components.HunkHeadshotTracker>();

                if (Util.HasEffectiveAuthority(this.gameObject))
                {
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.radius = 5f;
                    blastAttack.procCoefficient = 1f;
                    blastAttack.position = pos;
                    blastAttack.attacker = this.attackerBody.gameObject;
                    blastAttack.crit = Util.CheckRoll(this.attackerBody.crit, this.attackerBody.master);
                    blastAttack.baseDamage = this.attackerBody.damage * 80f;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                    blastAttack.baseForce = 2000f;
                    blastAttack.bonusForce = Vector3.up * 600f;
                    blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                    blastAttack.damageType = DamageType.Stun1s | DamageType.ClayGoo;
                    blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                    blastAttack.Fire();
                }
            }

            if (this.grenadeInstance) Destroy(this.grenadeInstance);
            Destroy(this);
        }

        private void OnDestroy()
        {
            if (this.grenadeInstance)
            {
                Destroy(this.grenadeInstance);
            }
        }
    }
}