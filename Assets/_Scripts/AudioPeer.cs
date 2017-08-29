using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour
{
	AudioSource audioSource;

    public static float[] samples = new float[512];
    private static float[] frequencyBand = new float[8];
    private static float[] bandBuffer = new float[8];
    private float[] bufferDecrease = new float[8];
    
    private float[] frequencyBandHighest = new float[8];
    public static float[] audioBand = new float[8];
    public static float[] audioBandBuffer = new float[8];

    // Use this for initialization
    void Start ()
	{
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
	}

    void CreateAudioBands()
    {
        for(int i = 0; i < 8; i++)
        {
            if( frequencyBand[i] > frequencyBandHighest[i] )
            {
                frequencyBandHighest[i] = frequencyBand[i];
            }
            audioBand[i] = (frequencyBand[i] / frequencyBandHighest[i]);
            audioBandBuffer[i] = (bandBuffer[i] / frequencyBandHighest[i]);
        }
    }

	void GetSpectrumAudioSource()
	{
		audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
	}

    void BandBuffer()
    {
        for(int i = 0; i < 8; ++i)
        {
            if(frequencyBand[i] > bandBuffer[i])
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
                average += samples[count] * (count + 1);
                count++;
            }
            average /= count;

            frequencyBand[i] = average * 10;
        }
    }
}
