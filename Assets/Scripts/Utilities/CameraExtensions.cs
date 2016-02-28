using UnityEngine;
using System.Collections;

public static class CameraExtensions
{
    public static float DEFAULT_ZOOM_DURATION = 1f;
    

    public static float ZoomOn(this Camera camera, Vector2 target)
    {
        return camera.ZoomOn(target, DEFAULT_ZOOM_DURATION);
    }

    public static float ZoomOn(this Camera camera, Vector2 target, float duration)
    {
        Vector3 end = new Vector3(target.x, target.y, camera.transform.position.z);
        return camera.ZoomOn(end, duration);
    }

    public static float ZoomOn(this Camera camera, Vector3 target)
    {
        return camera.ZoomOn(target, DEFAULT_ZOOM_DURATION);
    }

    public static float ZoomOn(this Camera camera, Vector3 target, float duration)
    {
        Vector3 start = camera.transform.position;
        float distance = Vector3.Distance(start, target);

        Animation.Animate(delegate (float p) {
            camera.transform.position = Vector3.MoveTowards(start, target, p * distance);
        }, duration);

        return duration;
    }
}
