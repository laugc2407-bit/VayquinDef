using UnityEngine;

public class AvatarIKFollowVR : MonoBehaviour
{
    [Header("VR Targets")]
    public Transform headTarget;
    public Transform leftController;
    public Transform rightController;

    [Header("Avatar IK Targets")]
    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;

    [Header("Avatar Bones")]
    public Transform headBone;

    [Header("Body")]
    public Transform bodyRoot;

    [Header("Offsets")]
    public Vector3 bodyOffset = new Vector3(0, -1.6f, 0);

    public Vector3 leftHandPositionOffset;
    public Vector3 rightHandPositionOffset;

    public Vector3 leftHandRotationOffset;
    public Vector3 rightHandRotationOffset;

    [Header("Smoothing")]
    public float smoothSpeed = 15f;

    void LateUpdate()
    {
        // =========================
        // BODY FOLLOW
        // =========================

        Vector3 targetBodyPosition = headTarget.position + bodyOffset;

        bodyRoot.position = Vector3.Lerp(
            bodyRoot.position,
            targetBodyPosition,
            Time.deltaTime * smoothSpeed
        );

        Vector3 forward = headTarget.forward;
        forward.y = 0;

        if (forward != Vector3.zero)
        {
            Quaternion bodyRotation = Quaternion.LookRotation(forward);

            bodyRoot.rotation = Quaternion.Slerp(
                bodyRoot.rotation,
                bodyRotation,
                Time.deltaTime * smoothSpeed
            );
        }

        // =========================
        // HEAD
        // =========================

        headBone.rotation = Quaternion.Slerp(
            headBone.rotation,
            headTarget.rotation,
            Time.deltaTime * smoothSpeed
        );

        // =========================
        // LEFT HAND IK
        // =========================

        leftHandIKTarget.position = Vector3.Lerp(
            leftHandIKTarget.position,
            leftController.position + leftHandPositionOffset,
            Time.deltaTime * smoothSpeed
        );

        leftHandIKTarget.rotation = Quaternion.Slerp(
            leftHandIKTarget.rotation,
            leftController.rotation * Quaternion.Euler(leftHandRotationOffset),
            Time.deltaTime * smoothSpeed
        );

        // =========================
        // RIGHT HAND IK
        // =========================

        rightHandIKTarget.position = Vector3.Lerp(
            rightHandIKTarget.position,
            rightController.position + rightHandPositionOffset,
            Time.deltaTime * smoothSpeed
        );

        rightHandIKTarget.rotation = Quaternion.Slerp(
            rightHandIKTarget.rotation,
            rightController.rotation * Quaternion.Euler(rightHandRotationOffset),
            Time.deltaTime * smoothSpeed
        );
    }
}