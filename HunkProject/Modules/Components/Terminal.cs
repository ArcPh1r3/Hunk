using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    internal class Terminal : NetworkBehaviour
    {
        public ItemDef itemDef;

        public ChestBehavior chestBehavior;
        public PurchaseInteraction purchaseInteraction;
        public GenericDisplayNameProvider genericDisplayNameProvider;

        private ParticleSystem beam;

        private void Awake()
        {
            this.beam = this.GetComponentInChildren<ParticleSystem>();
            purchaseInteraction = this.GetComponent<PurchaseInteraction>();
            genericDisplayNameProvider = this.GetComponent<GenericDisplayNameProvider>();

            if (purchaseInteraction != null)
            {
                purchaseInteraction.costType = (CostTypeIndex)Survivors.Hunk.sampleCostTypeIndex;
                purchaseInteraction.cost = 1;
                purchaseInteraction.displayNameToken = MainPlugin.developerPrefix + "_HUNK_TERMINAL_NAME";
                purchaseInteraction.contextToken = MainPlugin.developerPrefix + "_HUNK_TERMINAL_CONTEXT";
                if (genericDisplayNameProvider != null)
                {
                    genericDisplayNameProvider.displayToken = MainPlugin.developerPrefix + "_HUNK_TERMINAL_NAME";
                }
            }

            this.InvokeRepeating("Check", 0.5f, 0.5f);
        }

        private void Check()
        {
            if (this.hunkHasSample)
            {
                if (!this.beam.isPlaying) this.beam.Play();
            }
            else
            {
                if (this.beam.isPlaying) this.beam.Stop();
            }
        }

        private bool hunkHasSample
        {
            get
            {
                bool anyRealers = false;
                foreach (CharacterMaster master in CharacterMaster.instancesList)
                {
                    if (master.hasAuthority)
                    {
                        if (master.inventory)
                        {
                            if (master.inventory.GetItemCount(Modules.Survivors.Hunk.gVirusSample) > 0 || master.inventory.GetItemCount(Modules.Survivors.Hunk.tVirusSample) > 0 || master.inventory.GetItemCount(Modules.Survivors.Hunk.cVirusSample) > 0)
                            {
                                anyRealers = true;
                                break;
                            }
                        }
                    }
                }
                return anyRealers;
            }
        }
    }
}