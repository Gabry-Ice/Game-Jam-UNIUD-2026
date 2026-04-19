using UnityEngine;

public class ShowCameraBounds : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Camera cam = GetComponent<Camera>();
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        if (cam.orthographic)
        {
            // Per telecamere 2D/Ortografiche
            float spread = cam.farClipPlane - cam.nearClipPlane;
            float center = (cam.farClipPlane + cam.nearClipPlane) * 0.5f;
            Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(cam.orthographicSize * 2 * cam.aspect, cam.orthographicSize * 2, spread));
        }
        else
        {
            // Per telecamere Prospettiche
            Gizmos.DrawFrustum(Vector3.zero, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
        }
    }
}