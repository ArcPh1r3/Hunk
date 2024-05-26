using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class Decapitation : MonoBehaviour
    {
        public bool muted = false;

        private Transform headTransform;
        private Transform[] bastard = new Transform[0];
        private Transform[] fuckYou = new Transform[0];
        private bool imp;

        private void Awake()
        {
            CharacterBody characterBody = this.GetComponent<CharacterBody>();
            Transform modelTransform = this.transform;
            if (characterBody && characterBody.modelLocator && characterBody.modelLocator.modelTransform) modelTransform = characterBody.modelLocator.modelTransform;

            if (modelTransform)
            {
                ChildLocator childLocator = modelTransform.GetComponent<ChildLocator>();
                if (childLocator)
                {
                    Transform head = childLocator.FindChild("Head");
                    Transform head2 = childLocator.FindChild("HeadCenter");

                    if (head) this.headTransform = head;
                    if (!head && head2) this.headTransform = head2;

                    if (!this.headTransform)
                    {
                        if (modelTransform.gameObject.name == "mdlFlyingVermin")
                        {
                            this.IFUCKINGHATEYOUBASTARDFUCKYOUFUCKYOU(modelTransform);
                        }

                        if (modelTransform.gameObject.name == "mdlHermitCrab")
                        {
                            this.ThisIsALittleWorse(modelTransform);
                        }

                        if (modelTransform.gameObject.name == "mdlImp")
                        {
                            this.ThisSucks(modelTransform);
                            this.headTransform = childLocator.FindChild("Chest");
                        }

                        if (modelTransform.gameObject.name == "mdlImpBoss")
                        {
                            this.headTransform = childLocator.FindChild("Chest");
                        }

                        if (modelTransform.gameObject.name == "mdlNullifier")
                        {
                            this.headTransform = childLocator.FindChild("Muzzle");
                        }

                        if (modelTransform.gameObject.name == "mdlVoidMegaCrab")
                        {
                            this.headTransform = modelTransform.Find("VoidMegaCrabArmature/ROOT/base/body/eye");
                        }
                    }
                }
            }

            if (this.headTransform)
            {
                if (!this.muted)
                {
                    EffectManager.SpawnEffect(Modules.Assets.bloodExplosionEffect, new EffectData
                    {
                        origin = this.headTransform.position,
                        rotation = Quaternion.identity,
                        scale = 1f
                    }, false);
                    Util.PlaySound("sfx_hunk_blood_gurgle", this.headTransform.gameObject);
                }

                GameObject.Instantiate(Modules.Assets.bloodSpurtEffect, this.headTransform);
            }
        }

        private void IFUCKINGHATEYOUBASTARDFUCKYOUFUCKYOU(Transform modelTransform)
        {
            modelTransform.Find("mdlFlyingVerminMouth").gameObject.SetActive(false);

            this.headTransform = modelTransform.Find("FlyingVerminArmature/ROOT/Body");
            this.bastard = new Transform[]
            {
                modelTransform.Find("FlyingVerminArmature/ROOT/Body/Tail1"),
                modelTransform.Find("FlyingVerminArmature/ROOT/Body/Wing1.l"),
                modelTransform.Find("FlyingVerminArmature/ROOT/Body/Wing1.r")
            };
            this.fuckYou = new Transform[]
            {
                modelTransform.Find("FlyingVerminArmature/ROOT/Body/Foot.l"),
                modelTransform.Find("FlyingVerminArmature/ROOT/Body/Foot.r")
            };
        }

        private void ThisIsALittleWorse(Transform modelTransform)
        {
            this.headTransform = modelTransform.Find("HermitCrabArmature/ROOT/Base");
            this.bastard = new Transform[]
            {
                modelTransform.Find("HermitCrabArmature/ROOT/Base/leg1.thigh.l"),
                modelTransform.Find("HermitCrabArmature/ROOT/Base/leg1.thigh.r"),
                modelTransform.Find("HermitCrabArmature/ROOT/Base/leg2.thigh.l"),
                modelTransform.Find("HermitCrabArmature/ROOT/Base/leg2.thigh.r"),
                modelTransform.Find("HermitCrabArmature/ROOT/Base/leg3.thigh.l"),
                modelTransform.Find("HermitCrabArmature/ROOT/Base/leg3.thigh.r"),
            };
        }

        private void ThisSucks(Transform modelTransform)
        {
            this.imp = true;
            this.bastard = new Transform[]
            {
                modelTransform.Find("ImpArmature/ROOT/base/stomach/chest/Eyeball"),
                modelTransform.Find("ImpArmature/ROOT/base/stomach/chest/Eyelid.l"),
                modelTransform.Find("ImpArmature/ROOT/base/stomach/chest/Eyelid.u"),
            };
        }

        private void LateUpdate()
        {
            if (this.imp)
            {
                if (this.bastard.Length > 0)
                {
                    for (int i = 0; i < this.bastard.Length; i++)
                    {
                        this.bastard[i].localScale = Vector3.zero;
                    }
                }

                return;
            }

            if (this.headTransform) this.headTransform.localScale = Vector3.zero;
            if (this.bastard.Length > 0)
            {
                if (this.headTransform) this.headTransform.localScale = Vector3.one * 0.1f;
                for (int i = 0; i < this.bastard.Length; i++)
                {
                    this.bastard[i].localScale = Vector3.one * 10f;
                }
                if (this.fuckYou.Length > 0)
                {
                    for (int i = 0; i < this.fuckYou.Length; i++)
                    {
                        this.fuckYou[i].localScale = Vector3.one * 20f;
                    }
                }
            }
            else if (this.headTransform) this.headTransform.localScale = Vector3.zero;
        }
    }
}