using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class GolemLaser : MonoBehaviour
    {
        public Vector3 endPoint { get; private set; }
        public CharacterBody characterBody { get; private set; }

        private LineRenderer lineRenderer;

        private void Awake()
        {
            this.lineRenderer = this.GetComponent<LineRenderer>();
        }

        private void Start()
        {
            CharacterModel characterModel = this.transform.root.GetComponent<CharacterModel>();
            if (characterModel && characterModel.body) this.characterBody = characterModel.body;
        }
        
        private void Update()
        {
            if (this.lineRenderer) this.endPoint = this.lineRenderer.GetPosition(1);
            else this.endPoint = this.transform.position;
        }

        private void OnEnable()
        {
            Modules.Survivors.Hunk.golemLasers.Add(this);
        }

        private void OnDisable()
        {
            Modules.Survivors.Hunk.golemLasers.Remove(this);
        }
    }
}