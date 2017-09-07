using System.Linq;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour
{
	AudioSource audioSource;

    public static float[] samplesLeft = new float[512];
    public static float[] samplesRight = new float[512];

    private float[] frequencyBand = new float[8];
    private float[] bandBuffer = new float[8];
    private float[] bufferDecrease = new float[8];    
    private float[] frequencyBandHighest = new float[8];

    //64 bands
    private float[] frequencyBand64 = new float[64];
    private float[] bandBuffer64 = new float[64];
    private float[] bufferDecrease64 = new float[64];
    private float[] frequencyBandHighest64 = new float[64];


    [HideInInspector]
    public static float[] audioBand, audioBandBuffer;

    [HideInInspector]
    public static float[] audioBand64, audioBandBuffer64;

    public static float amplitude, amplitudeBuffer;

    [HideInInspector]
    public static float amplitudeHighest = 0;
    public static float audioProfile;

    public enum channel {Stereo, Left, Right};
    public channel _channel = new channel();

    // Use this for initialization
    void Start ()
	{
        audioBand = new float[8];
        audioBandBuffer = new float[8];

        audioBand64 = new float[64];
        audioBandBuffer64 = new float[64];

        audioSource = GetComponent<AudioSource>();
        AudioProfile();
	}
	
	// Update is called once per frame
	void Update ()
	{
		GetSpectrumAudioSource();

        MakeFrequencyBands();
        MakeFrequencyBands64();

        BandBuffer();
        BandBuffer64();

        CreateAudioBands();
        CreateAudioBands64();

        GetAmplitude();
	}

    void AudioProfile()
    {
        for(int i = 0; i < frequencyBandHighest.Length; i++)
        {
            frequencyBandHighest[i] = audioProfile;
        }
    }

    void GetAmplitude()
    {
        float currentAmplitude = 0;
        float currentAmplitudeBuffer = 0;
        for(int i = 0; i < audioBand.Length; i++)
        {
            currentAmplitude += audioBand[i];
            currentAmplitudeBuffer += audioBandBuffer[i];
        }
        if(currentAmplitude > amplitudeHighest)
        {
            amplitudeHighest = currentAmplitude;
        }
        amplitude = currentAmplitude / amplitudeHighest;
        amplitudeBuffer = currentAmplitudeBuffer / amplitudeHighest;
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (frequencyBand[i] > frequencyBandHighest[i])
            {
                frequencyBandHighest[i] = frequencyBand[i];
            }
            audioBand[i] = (frequencyBand[i] / frequencyBandHighest[i]);
            audioBandBuffer[i] = (bandBuffer[i] / frequencyBandHighest[i]);
        }
    }

    void CreateAudioBands64()
    {
        for (int i = 0; i < 64; i++)
        {
            if (frequencyBand64[i] > frequencyBandHighest64[i])
            {
                frequencyBandHighest64[i] = frequencyBand64[i];
            }
            audioBand64[i] = (frequencyBand64[i] / frequencyBandHighest64[i]);
            audioBandBuffer64[i] = (bandBuffer64[i] / frequencyBandHighest64[i]);
        }
    }

    void GetSpectrumAudioSource()
	{
		audioSource.GetSpectrumData(samplesLeft, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(samplesRight, 1, FFTWindow.Blackman);
    }

    void BandBuffer()
    {
        for (int i = 0; i < 8; ++i)
        {
            if (frequencyBand[i] > bandBuffer[i])
            {
                bandBuffer[i] = frequencyBand[i];
                bufferDecrease[i] = 0.005f;
            }
            else if (frequencyBand[i] < bandBuffer[i])
            {
                bandBuffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f;
            }
        }
    }

    void BandBuffer64()
    {
        for (int i = 0; i < 64; ++i)
        {
            if (frequencyBand64[i] > bandBuffer64[i])
            {
                bandBuffer64[i] = frequencyBand64[i];
                bufferDecrease64[i] = 0.005f;
            }
            else if (frequencyBand64[i] < bandBuffer64[i])
            {
                bandBuffer64[i] -= bufferDecrease64[i];
                bufferDecrease64[i] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        /*
         * 
         * 22050 / 512 = 43 hertz per sample
         * 
         * 20 - 60 hertz
         * 60 - 250 hertz
         * 250 - 500 hertz
         * 500 - 2000 hertz
         * 2000 - 4000 hertz
         * 4000 - 6000 hertz
         * 6000 - 20000 hertz
         * 
         * 0 - 2 = 86 hertz
         * 1 - 4 = 172 hertz - 87-258
         * 2 - 8 = 344 hertz - 239-602
         * 3 - 16 = 688 hertz - 602-1290
         * 4 - 32 = 1376 hertz - 1291-2666
         * 5 - 64 = 2752 hertz - 2667-5418
         * 6 - 128 = 5504 hertz - 5419-10922
         * 7 - 256 = 11008 hertz - 10923-21930
         * 
         * 510 total
         */

        int count = 0;

        for(int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if(i == 7)
            {
                sampleCount += 2;
            }
            for(int j = 0; j < sampleCount; j++)
            {
                if(_channel == channel.Stereo)
                {
                    average += (samplesLeft[count] + samplesRight[count]) * (count + 1);
                }
                else if(_channel == channel.Left)
                {
                    average += samplesLeft[count] * (count + 1);
                }
                else if(_channel == channel.Right)
                {
                    average += samplesRight[count] * (count + 1);
                }
                
                count++;
            }
            average /= count;

            frequencyBand[i] = average * 10;
        }
    }

    void MakeFrequencyBands64()
    {
        int count = 0;
        int sampleCount = 1;
        int power = 0;
        int[] increases = { 16, 32, 40, 48, 56 };

        for (int i = 0; i < 64; i++)
        {
            float average = 0;

            if (increases.Contains(i))
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power) * 2;
                if( power == 3 )
                {
                    sampleCount -= 2;
                }
            }
            for (int j = 0; j < sampleCount; j++)
            {
                if (_channel == channel.Stereo)
                {
                    average += (samplesLeft[count] + samplesRight[count]) * (count + 1);
                }
                else if (_channel == channel.Left)
                {
                    average += samplesLeft[count] * (count + 1);
                }
                else if (_channel == channel.Right)
                {
                    average += samplesRight[count] * (count + 1);
                }

                count++;
            }
            average /= count;

            frequencyBand64[i] = average * 8;
        }
    }
}
