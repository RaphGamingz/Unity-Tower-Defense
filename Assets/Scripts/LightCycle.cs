using UnityEngine;
public class LightCycle : MonoBehaviour
{
    public Light lighting;
    public AnimationCurve lightCurve;
    void Start()
    {
        InvokeRepeating("updateLighting", 0f, 120f); //Update lighting every 2 minutes
    }
    void updateLighting()
    {
        lighting.intensity = lightCurve.Evaluate((float)(System.DateTime.Now.Minute + System.DateTime.Now.Hour * 60) / 1440);
    }
}