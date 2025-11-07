using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Transition : MonoBehaviour
{
    [Header("References")]
    public Volume volume;

    [Header("Target Values")]
    public float targetExposure = 5f;
    public float maxBloom = 10f;
    public float minDistortion = -0.5f;
    public float maxChromaticAberration = 1f;
    public float transitionDuration = 2f;

    private ColorAdjustments colorAdjust;
    private Bloom bloom;
    private LensDistortion lensDist;
    private ChromaticAberration chromatic;

    private void Start()
    {
        volume.profile.TryGet(out colorAdjust);
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out lensDist);
        volume.profile.TryGet(out chromatic);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(DoTransition());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReverseTransition());
        }
    }

    private IEnumerator DoTransition()
    {
        float startExposure = colorAdjust.postExposure.value;
        float startBloom = bloom.intensity.value;
        float startDist = lensDist.intensity.value;
        float startDistScale = lensDist.scale.value;
        float startChrom = chromatic.intensity.value;

        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = EaseInOutCubic(time / transitionDuration);

            colorAdjust.postExposure.value = Mathf.Lerp(startExposure, targetExposure, t);
            bloom.intensity.value = Mathf.Lerp(startBloom, maxBloom, t);
            lensDist.intensity.value = Mathf.Lerp(startDist, minDistortion, t);
            lensDist.scale.value = Mathf.Lerp(startDistScale, 0, t);
            chromatic.intensity.value = Mathf.Lerp(startChrom, maxChromaticAberration, t);

            yield return null;
        }

        colorAdjust.postExposure.value = targetExposure;
        bloom.intensity.value = maxBloom;
        lensDist.intensity.value = minDistortion;
        lensDist.scale.value = 0;
        chromatic.intensity.value = maxChromaticAberration;
    }

    private IEnumerator ReverseTransition()
    {
        float startExposure = colorAdjust.postExposure.value;
        float startBloom = bloom.intensity.value;
        float startDist = lensDist.intensity.value;
        float startDistScale = lensDist.scale.value;
        float startChrom = chromatic.intensity.value;

        float time = 0f;

        // Target normal (baseline) values
        float normalExposure = 0f;
        float normalBloom = 0f;
        float normalDistortion = 0f;
        float normalScale = 1f;
        float normalChrom = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = EaseInOutCubic(time / transitionDuration);

            colorAdjust.postExposure.value = Mathf.Lerp(startExposure, normalExposure, t);
            bloom.intensity.value = Mathf.Lerp(startBloom, normalBloom, t);
            lensDist.intensity.value = Mathf.Lerp(startDist, normalDistortion, t);
            lensDist.scale.value = Mathf.Lerp(startDistScale, normalScale, t);
            chromatic.intensity.value = Mathf.Lerp(startChrom, normalChrom, t);

            yield return null;
        }

        colorAdjust.postExposure.value = normalExposure;
        bloom.intensity.value = normalBloom;
        lensDist.intensity.value = normalDistortion;
        lensDist.scale.value = normalScale;
        chromatic.intensity.value = normalChrom;
    }

    private float EaseInOutCubic(float t)
    {
        return t < 0.5f
            ? 4f * t * t * t
            : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
