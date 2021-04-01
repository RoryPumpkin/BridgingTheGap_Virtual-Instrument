using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RDAudioScript : MonoBehaviour
{
    [SerializeField] public Material RDUpdateShader;
    [SerializeField] Material SurfaceShader1;
    [SerializeField] Material SurfaceShader2;
    [SerializeField] Renderer rend;
    [SerializeField] float circleStepSize = 0.01f;
    [SerializeField] float midAmpScalar = 0.5f;
    [SerializeField] float lowAmpScalar = 1f;
    [SerializeField] float Amplitude;
    [SerializeField] float MaxAmplitude;
    [SerializeField] float MinAmplitude;
    [SerializeField] float AvgAmplitude;

    [HideInInspector] public float lerp;
    float scalar;
    float[] amplitudes = new float[120];
    int current;
    string device;
    AudioPeer audioPeer;

    void Start()
    {
        audioPeer = GetComponent<AudioPeer>();
        for(int i = 0; i < 120; i++)
        {
            amplitudes[i] = 0.3f;
        }

        // At start, use the first material
        rend.material = SurfaceShader1;
    }

    // Update is called once per frame
    void Update()
    {
        amplitudes[current] = audioPeer._Amplitude;
        if (current < 119)
        { current++; }
        else { current = 0; }

        AvgAmplitude = 0;
        foreach(float amp in amplitudes)
        {
            AvgAmplitude += amp;
        }
        AvgAmplitude = AvgAmplitude / 120;

        if(AvgAmplitude < 0.22f)
        {
            scalar = lowAmpScalar;
        } else { scalar = midAmpScalar; }

        Amplitude = audioPeer._Amplitude;
        if(Amplitude < MinAmplitude) { MinAmplitude = Amplitude; }
        if(Amplitude > MaxAmplitude) { MaxAmplitude = Amplitude; }

        if (RDUpdateShader.GetFloat("_radius") < audioPeer._Amplitude / AvgAmplitude * scalar)
        {
            RDUpdateShader.SetFloat("_radius", RDUpdateShader.GetFloat("_radius") + circleStepSize * Time.deltaTime);
        } else if (RDUpdateShader.GetFloat("_radius") > audioPeer._Amplitude / AvgAmplitude * scalar)
        {
            RDUpdateShader.SetFloat("_radius", RDUpdateShader.GetFloat("_radius") - circleStepSize * Time.deltaTime);
        }

        // ping-pong between the materials over the duration
        //float lerp = Mathf.PingPong(Time.time, 2) / 2;
        rend.material.Lerp(SurfaceShader1, SurfaceShader2, lerp);
    }
}
