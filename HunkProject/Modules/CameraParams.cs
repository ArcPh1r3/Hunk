using RoR2;
using UnityEngine;

internal enum HunkCameraParams
{
    DEFAULT,
    AIM,
    AIM_SNIPER,
    EMOTE
}

namespace HunkMod.Modules
{
    internal static class CameraParams
    {
        internal static CharacterCameraParamsData defaultCameraParams;
        internal static CharacterCameraParamsData aimCameraParams;
        internal static CharacterCameraParamsData sniperAimCameraParams;
        internal static CharacterCameraParamsData emoteCameraParams;

        internal static void InitializeParams()
        {
            if (Modules.Config.overTheShoulderCamera.Value)
            {
                //defaultCameraParams = NewCameraParams("ccpRobHunk", 70f, 0.15f, new Vector3(0.6f, 0.75f, -3.5f));
                defaultCameraParams = NewCameraParams("ccpRobHunk", 70f, 0.15f, new Vector3(2f, 0.08f, -3.2f));
                //aimCameraParams = NewCameraParams("ccpRobHunkAim", 70f, 0.1f, new Vector3(1.3f, 0.8f, -3f));
                aimCameraParams = NewCameraParams("ccpRobHunkAim", 70f, 0.1f, new Vector3(2.2f, 0.1f, -2f));
            }
            else
            {
                defaultCameraParams = NewCameraParams("ccpRobHunk", 70f, 1.37f, new Vector3(0f, 0f, -8.1f));
                aimCameraParams = NewCameraParams("ccpRobHunkAim", 70f, 0.8f, new Vector3(1f, 0f, -5f));
            }

            sniperAimCameraParams = NewCameraParams("ccpRobHunkSniperAim", 70f, 0.8f, new Vector3(0f, 0f, 0.75f));
            emoteCameraParams = NewCameraParams("ccpRobHunkEmote", 70f, 0.4f, new Vector3(0f, 0f, -6f));
        }

        private static CharacterCameraParamsData NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition)
        {
            return NewCameraParams(name, pitch, pivotVerticalOffset, standardPosition, 0.1f);
        }

        private static CharacterCameraParamsData NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 idealPosition, float wallCushion)
        {
            CharacterCameraParamsData newParams = new CharacterCameraParamsData();

            newParams.maxPitch = pitch;
            newParams.minPitch = -pitch;
            newParams.pivotVerticalOffset = pivotVerticalOffset;
            newParams.idealLocalCameraPos = idealPosition;
            newParams.wallCushion = wallCushion;

            return newParams;
        }

        internal static CameraTargetParams.CameraParamsOverrideHandle OverrideCameraParams(CameraTargetParams camParams, HunkCameraParams camera, float transitionDuration = 0.5f)
        {
            CharacterCameraParamsData paramsData = GetNewParams(camera);

            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = paramsData,
                priority = 0,
            };

            return camParams.AddParamsOverride(request, transitionDuration);
        }

        internal static CharacterCameraParams CreateCameraParamsWithData(HunkCameraParams camera)
        {

            CharacterCameraParams newPaladinCameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();

            newPaladinCameraParams.name = camera.ToString().ToLower() + "Params";

            newPaladinCameraParams.data = GetNewParams(camera);

            return newPaladinCameraParams;
        }

        internal static CharacterCameraParamsData GetNewParams(HunkCameraParams camera)
        {
            CharacterCameraParamsData paramsData = defaultCameraParams;

            switch (camera)
            {

                default:
                case HunkCameraParams.DEFAULT:
                    paramsData = defaultCameraParams;
                    break;
                case HunkCameraParams.AIM:
                    paramsData = aimCameraParams;
                    break;
                case HunkCameraParams.AIM_SNIPER:
                    paramsData = sniperAimCameraParams;
                    break;
                case HunkCameraParams.EMOTE:
                    paramsData = emoteCameraParams;
                    break;
            }

            return paramsData;
        }
    }
}