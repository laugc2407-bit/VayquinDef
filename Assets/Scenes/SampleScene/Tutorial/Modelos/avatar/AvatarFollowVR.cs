using UnityEngine;

public class AvatarFollowVR : MonoBehaviour
{
    public Transform headTarget;
    public Transform leftHandTarget;
    public Transform rightHandTarget;

    public Transform headBone;
    public Transform leftHandBone;
    public Transform rightHandBone;

    [Header("Body Follow")]
    public Transform bodyRoot; // normalmente el mismo avatar
    public Vector3 bodyOffset;

    void LateUpdate()
    {
        // 🔹 1. Mover TODO el cuerpo (posición global)
        Vector3 targetPosition = headTarget.position + bodyOffset;
        bodyRoot.position = targetPosition;

        // 🔹 2. Rotación del cuerpo (opcional pero recomendado)
        Vector3 forward = headTarget.forward;
        forward.y = 0; // evita que se incline

        if (forward != Vector3.zero)
        {
            bodyRoot.rotation = Quaternion.LookRotation(forward);
        }

        // 🔹 3. Cabeza
        headBone.rotation = headTarget.rotation;

        // 🔹 4. Manos SOLO rotación (para evitar deformaciones)
        leftHandBone.rotation = leftHandTarget.rotation;
        rightHandBone.rotation = rightHandTarget.rotation;
    }
}