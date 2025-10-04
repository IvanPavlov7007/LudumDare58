using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UISoftMask;
using Coffee.UISoftMaskInternal;

public class SoftMaskTransparency : MonoBehaviour, ITransparencyController
{
    public float threshold = 0.05f;
    public float Alpha
    {
        get
        {
            if (!initialize())
            {
                throw new UnityException(name + " can find an Alpha-Setting target");
            }
            return softMask.softnessRange.average;
        }
        set
        {
            if (!initialize())
            {
                throw new UnityException(name + " can find an Alpha-Setting target");
            }
            var c = softMask.softnessRange = clampedRange(value, threshold);

        }
    }

    static MinMax01 clampedRange(float alphaMedian, float threshold)
    {
        float min = Mathf.Clamp01(alphaMedian - threshold);
        float max = Mathf.Clamp01(alphaMedian + threshold);
        return new MinMax01(min, max);
    }

    SoftMask softMask;
    private bool initialize()
    {
        if (softMask != null)
            return true;
        softMask = GetComponent<SoftMask>();
        if (softMask == null)
            return false;
        return true;
    }

}

public static partial class Transparency
{
    public static SoftMaskTransparency GetController(SoftMask softMask)
    {
        SoftMaskTransparency result;
        if (!softMask.TryGetComponent<SoftMaskTransparency>(out result))
        {
            result = softMask.gameObject.AddComponent<SoftMaskTransparency>();
        }
        return result;
    }
}