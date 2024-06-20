using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public class HunkCatacombChecker : MonoBehaviour
    {
        private void Awake()
        {
            this.Invoke("Check", 1f);
        }

        private void Check()
        {
            if (GameObject.Find("Holder: GAMEPLAY SPACE").transform.Find("Blockers/LowerCrypt").gameObject.activeSelf)
            {
                if (NetworkServer.active)
                {
                    NetworkServer.Spawn(GameObject.Instantiate(Modules.Survivors.Hunk.instance.c4Interactable,
                        new Vector3(86.01f, 178.754f, -315.9598f),
                        Quaternion.Euler(new Vector3(0f, 180f, 0f))));
                }
            }

            Destroy(this);
        }
    }
}