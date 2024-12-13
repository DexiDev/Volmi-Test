using Cinemachine;

namespace Game.Cameras.Cinemachine
{
    public class CinemachineIgnoreLookAtExtension : CinemachineExtension
    {
        /// <summary>Standard CinemachineExtension callback</summary>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Finalize)
                state.BlendHint |= CameraState.BlendHintValue.IgnoreLookAtTarget;
        }
    }
}