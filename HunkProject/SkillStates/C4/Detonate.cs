using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.C4
{
    public class Detonate : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            EffectManager.SpawnEffect(Modules.Assets.explosionEffect, new EffectData
            {
                origin = this.transform.position,
                rotation = Quaternion.identity,
                scale = 1
            }, false);

            if (GameObject.Find("Holder: GAMEPLAY SPACE"))
            {
                if (GameObject.Find("Holder: GAMEPLAY SPACE").transform.Find("Blockers").gameObject.activeSelf)
                {
                    if (GameObject.Find("Holder: GAMEPLAY SPACE").transform.Find("Blockers/LowerCrypt").gameObject.activeSelf)
                    {
                        GameObject.Find("Holder: GAMEPLAY SPACE").transform.Find("Blockers/LowerCrypt").gameObject.SetActive(false);
                    }
                }
            }

            Destroy(this.gameObject);
        }
    }
}