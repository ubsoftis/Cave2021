#if UNITY_POST_PROCESSING_STACK_V2
using Cave.StereoRendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cave
{
    public static class CaveCameraUtils_PostProcessingV2
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void InjectPostProcessingLayerClone()
        {
            CaveCameraUtils.OnCopyCameraSettings -= CopyPostProcessingLayer;
            CaveCameraUtils.OnCopyCameraSettings += CopyPostProcessingLayer;
        }

        private static void CopyPostProcessingLayer(Camera from, Camera to)
        {
            var fromLayer = from.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
            if(fromLayer)
            {
                var toLayer = to.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
                if (!toLayer) toLayer = to.gameObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();

                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(fromLayer), toLayer);

                toLayer.volumeTrigger = toLayer.transform;
                toLayer.finalBlitToCameraTarget = false;

                toLayer.enabled = fromLayer.enabled;
            }
        }
    }
}
#endif