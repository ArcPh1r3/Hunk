using UnityEngine;
using RoR2;
using System.Collections.Generic;

namespace HunkMod.Modules.Components
{
    public class HunkCameraAdjuster : MonoBehaviour
    {
        public float smoothSpeed = 16f;

        private float checkStopwatch;
        private CharacterBody body;
        private HunkController hunk;
        private Transform baseTransform;
        private Transform fakeBaseTransform;
        private Transform cameraTracker;
        private ChildLocator childLocator;
        private SphereSearch search;
        private List<HurtBox> hits = new List<HurtBox>();
        private float offset;
        private float desiredOffset;
        private float offsetMult;
        private bool isRor2Cam;

        private void Awake()
        {
            this.body = this.GetComponent<CharacterBody>();
            this.hunk = this.GetComponent<HunkController>();
            this.childLocator = this.GetComponent<ModelLocator>().modelTransform.GetComponent<ChildLocator>();
            this.baseTransform = this.childLocator.FindChild("Base");
            this.fakeBaseTransform = this.childLocator.FindChild("FakeBase");
            this.cameraTracker = this.childLocator.FindChild("CameraTracker");
            this.cameraTracker.transform.parent = null;
            this.desiredOffset = 0f;
            this.offset = 0f;
            this.isRor2Cam = !Modules.Config.overTheShoulderCamera.Value;

            this.search = new SphereSearch
            {
                mask = LayerIndex.entityPrecise.mask,
                radius = 60f
            };

            this.offsetMult = Modules.Config.cameraZoomInfluence.Value;
            this.smoothSpeed = Modules.Config.cameraSmoothSpeed.Value;
            this.checkStopwatch = 0.5f;

            if (this.isRor2Cam) this.smoothSpeed *= 1.5f;
        }

        private void Update()
        {
            this.checkStopwatch -= Time.deltaTime;

            if (this.checkStopwatch <= 0f)
            {
                this.CheckForEnemies();
            }
        }

        private void CheckForEnemies()
        {
            if (!this.body) return;

            this.hits.Clear();

            this.search.ClearCandidates();
            this.search.origin = this.body.corePosition;
            this.search.RefreshCandidates();
            this.search.FilterCandidatesByDistinctHurtBoxEntities();
            this.search.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(this.body.teamComponent.teamIndex));
            this.search.GetHurtBoxes(this.hits);

            float maxOffset = 0f;
            foreach (HurtBox h in this.hits)
            {
                HealthComponent hp = h.healthComponent;
                if (hp)
                {
                    if (hp.body.hullClassification == HullClassification.Golem)
                    {
                        maxOffset = 1.5f;
                    }

                    if (hp.body.hullClassification == HullClassification.BeetleQueen)
                    {
                        maxOffset = 2.5f;
                    }
                }
            }

            if (this.hunk.counterTimer > 0f) maxOffset = 0f;

            if (this.offset < maxOffset) this.checkStopwatch = 1.5f;
            else this.checkStopwatch = 0.5f;

            this.desiredOffset = maxOffset;
        }

        private void LateUpdate()
        {
            if (!this.cameraTracker) return;
            if (!this.baseTransform) return;

            if (this.smoothSpeed <= 0f)
            {
                this.cameraTracker.position = this.transform.position + new Vector3(0f, 0.6f, 0f);
                return;
            }

            float d = this.desiredOffset * this.offsetMult;
            if (this.hunk.isAiming) d = 0f;
            this.offset = Mathf.Lerp(this.offset, d, 0.8f * Time.deltaTime);

            if (this.isRor2Cam) this.offset = 0f;

            float bias = 0.75f;
            if (this.hunk.isRolling) bias = 1f;
            Vector3 desiredPosition = Vector3.Lerp(this.fakeBaseTransform.position, this.baseTransform.position, bias);

            float speedMult = (this.body.moveSpeed / this.body.baseMoveSpeed);
            if (speedMult <= 0f) speedMult = 1f;
            float speed = this.smoothSpeed * speedMult;
            if (this.hunk.isRolling || this.hunk.immobilized) speed = 80f;
            if (this.cameraTracker && this.baseTransform)
            {
                this.cameraTracker.position = Vector3.Lerp(this.cameraTracker.position, desiredPosition - (this.body.inputBank.aimDirection * this.offset), speed * Time.deltaTime);
            }
        }
    }
}