using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace HunkMod.Modules.Components
{
    public class HunkRunController : MonoBehaviour
    {
        public static HunkRunController instance { get; private set; }
        public Run run;

        public static GameObject heartChestPrefab;

        private void Awake()
        {
            run = GetComponentInParent<Run>();
        }

        private void Start()
        {
            instance = this;
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);

            var currStage = SceneManager.GetActiveScene().name;

            var position = Vector3.zero;
            var rotation = Quaternion.Euler(-90, 0, 0);

            switch (currStage)
            {
                //currently literally just locations stolen from ss2 ethereal tp for rn lol
                case "blackbeach":
                    position = new Vector3(-60, -51.2f, -231);
                    rotation = Quaternion.Euler(0, 0, 0);
                    //high cliff in the middle of the map
                    break;
                case "blackbeach2":
                    position = new Vector3(-101.4f, 1.5f, 20.1f);
                    rotation = Quaternion.Euler(0, 292.8f, 0);
                    //between two knocked-over pillars near the gate
                    break;
                case "golemplains":
                    position = new Vector3(283.7f, -50.0f, -154.7f);
                    rotation = Quaternion.Euler(0, 321, 0);
                    //top of the cliff, near debugging plains area
                    //home.
                    break;
                case "golemplains2":
                    position = new Vector3(9.8f, 127.5f, -251.8f);
                    rotation = Quaternion.Euler(0, 5, 0);
                    //on the cliff where the middle giant ring meets the ground
                    break;
                case "goolake":
                    position = new Vector3(53.9f, -45.9f, -219.6f);
                    rotation = Quaternion.Euler(0, 190, 0);
                    //on the clifftop near the ancient gate
                    break;
                case "foggyswamp":
                    position = new Vector3(-83.74f, -83.35f, 39.09f);
                    rotation = Quaternion.Euler(0, 104.27f, 0);
                    //on the wall / dam across from where two newt altars spawn
                    break;
                case "frozenwall":
                    position = new Vector3(-230.7f, 132, 239.4f);
                    rotation = Quaternion.Euler(0, 167, 0);
                    //on cliff near water, next to the lone tree
                    break;
                case "wispgraveyard":
                    position = new Vector3(-341.5f, 79, 0.5f);
                    rotation = Quaternion.Euler(0, 145, 0);
                    //small cliff outcrop above playable area, same large island with artifact code
                    break;
                case "dampcavesimple":
                    position = new Vector3(157.5f, -43.1f, -188.9f);
                    rotation = Quaternion.Euler(0, 318.4f, 0);
                    //on the overhang above rex w/ 3 big rocks
                    break;
                case "shipgraveyard":
                    position = new Vector3(20.5f, -23.7f, 185.1f);
                    rotation = Quaternion.Euler(0, 173.6f, 0);
                    //in the cave entrance nearest to the cliff, on the spire below the land bridge
                    break;
                case "rootjungle":
                    position = new Vector3(-196.6f, 190.1f, -204.5f);
                    rotation = Quaternion.Euler(0, 80, 0);
                    //top of the highest root in the upper / back area
                    break;
                /*case "skymeadow":
                    position = new Vector3(65.9f, 127.4f, -293.9f);
                    rotation = Quaternion.Euler(0, 194.8f, 0);
                    //on top of the tallest rock spire, opposite side of map from the moon
                    break;*/
                case "snowyforest":
                    position = new Vector3(-38.7f, 112.7f, 153.1f);
                    rotation = Quaternion.Euler(0, 54.1f, 0);
                    //on top of a lone elevated platform on a tree
                    break;
                case "ancientloft":
                    position = new Vector3(-133.4f, 33.5f, -280f);
                    rotation = Quaternion.Euler(0, 354.5f, 0);
                    //on a branch under the main platform in the back corner of the map
                    break;
                case "sulfurpools":
                    position = new Vector3(-33.6f, 36.8f, 164.1f);
                    rotation = Quaternion.Euler(0, 187f, 0);
                    //in the corner, atop of one of the columns
                    break;
                case "FBLScene":
                    position = new Vector3(58.3f, 372f, -88.8f);
                    rotation = Quaternion.Euler(0, 0, 0);
                    //overlooking the shore
                    break;
                case "drybasin":
                    position = new Vector3(149.4f, 65.7f, -212.7f);
                    rotation = Quaternion.Euler(0, 0, 0);
                    //in a cranny near collapsed aqueducts
                    break;
            }

            if (NetworkServer.active)
            {
                GameObject heartChest = Instantiate(heartChestPrefab, position, rotation);
                NetworkServer.Spawn(heartChest);
            }
        }

        private void OnDestroy()
        {
            On.RoR2.SceneDirector.Start -= SceneDirector_Start;
        }
    }
}