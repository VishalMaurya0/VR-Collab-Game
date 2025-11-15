using Autohand;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    [Header("References")]
    public Volume volume;
    public AutoHandPlayer rig;
    public GameObject boundary;

    public List<Rigidbody> rigidbodiesToToggle;

    [Header("Transition Duration")]
    public float transitionDuration = 2f;

    [System.Serializable]
    public class TransitionSettings
    {
        [Range(-5f, 5f)] public float exposure = 0f;
        [Range(0f, 20f)] public float bloom = 0f;
        [Range(-1f, 1f)] public float lensDistortion = 0f;
        [Range(0f, 1f)] public float lensScale = 1f;
        [Range(0f, 1f)] public float chromaticAberration = 0f;
    }

    [Header("Before Transition (start)")]
    public TransitionSettings beforeTransition = new TransitionSettings();

    [Header("After Transition (end)")]
    public TransitionSettings afterTransition = new TransitionSettings();

    private ColorAdjustments colorAdjust;
    private Bloom bloom;
    private LensDistortion lensDist;
    private ChromaticAberration chromatic;

    private void Awake()
    {
        TryFetchOverrides();
        SetToFinalState(afterTransition);
    }

    private void Start()
    {
        // Automatically reverse on load to fade in from the "after" state
        StartCoroutine(ReverseTransition());
    }

    private void TryFetchOverrides()
    {
        if (volume == null)
            volume = GetComponent<Volume>();

        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGet(out colorAdjust);
            volume.profile.TryGet(out bloom);
            volume.profile.TryGet(out lensDist);
            volume.profile.TryGet(out chromatic);
        }
        else
        {
            Debug.LogWarning("Transition: Volume or Volume Profile missing!");
        }
    }

    private void OnEnable()
    {
        TryFetchOverrides();
    }








    private string activeScene;
    private Dictionary<string, Scene> loadedScenes = new Dictionary<string, Scene>();

    public IEnumerator DoTransition(bool returning)
    {
        TryFetchOverrides();
        if (!AreOverridesValid()) yield break;

                rig.maxMoveSpeed = 0f;
                //boundary.SetActive(false);
        //foreach (Rigidbody rb in rigidbodiesToToggle)
        //{
        //    rb.isKinematic = true;
        //}
        yield return LerpSettings(beforeTransition, afterTransition);

        if (GameManager.Instance.playerGO != null)
        {
            if (!returning) {
                //Quaternion originalRotation = rig.transform.rotation; 
                //rig.transform.SetPositionAndRotation(GameManager.Instance.paintingPos[GameManager.Instance.currentPaintingIndex], originalRotation);

                GameManager.Instance.Paintings[GameManager.Instance.toSetPaintingIndex].SetActive(true);
                GameManager.Instance.Room.SetActive(false);
            }
            else
            {
                //Quaternion originalRotation = rig.transform.rotation;
                //rig.transform.SetPositionAndRotation(GameManager.Instance.returningPos[GameManager.Instance.currentPaintingIndex], originalRotation);
                GameManager.Instance.Paintings[GameManager.Instance.toSetPaintingIndex].SetActive(false);
                GameManager.Instance.Room.SetActive(true);
            }
        }
    }






    public IEnumerator ReverseTransition()
    {
        TryFetchOverrides();
        if (!AreOverridesValid()) yield break;

        yield return LerpSettings(afterTransition, beforeTransition);
        boundary.SetActive(true);
        //rig.GetComponent<Rigidbody>().isKinematic = false;
        foreach (Rigidbody rb in rigidbodiesToToggle)
        {
            rb.isKinematic = false;
        }
        rig.maxMoveSpeed = 5f;
    }

    private IEnumerator LerpSettings(TransitionSettings start, TransitionSettings end)
    {
        float time = 0f;
        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = EaseInOutCubic(time / transitionDuration);

            colorAdjust.postExposure.value = Mathf.Lerp(start.exposure, end.exposure, t);
            bloom.intensity.value = Mathf.Lerp(start.bloom, end.bloom, t);
            lensDist.intensity.value = Mathf.Lerp(start.lensDistortion, end.lensDistortion, t);
            lensDist.scale.value = Mathf.Lerp(start.lensScale, end.lensScale, t);
            chromatic.intensity.value = Mathf.Lerp(start.chromaticAberration, end.chromaticAberration, t);

            yield return null;
        }

        // Ensure final exact values are applied
        SetToFinalState(end);
    }

    private void SetToFinalState(TransitionSettings state)
    {
        if (!AreOverridesValid()) return;

        colorAdjust.postExposure.value = state.exposure;
        bloom.intensity.value = state.bloom;
        lensDist.intensity.value = state.lensDistortion;
        lensDist.scale.value = state.lensScale;
        chromatic.intensity.value = state.chromaticAberration;
    }

    private bool AreOverridesValid()
    {
        return (colorAdjust != null && bloom != null && lensDist != null && chromatic != null);
    }

    private float EaseInOutCubic(float t)
    {
        return t < 0.5f
            ? 4f * t * t * t
            : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
