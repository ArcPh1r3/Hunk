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
            defaultCameraParams = NewCameraParams("ccpRobHunk", 70f, 0.6f, new Vector3(0.5f, 0f, -3f));
            aimCameraParams = NewCameraParams("ccpRobHunkAim", 70f, 0.65f, new Vector3(0.8f, 0f, -2f));
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