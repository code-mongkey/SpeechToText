using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Speech.V1;

namespace SpeechToText
{
    class STT
    {
        public string Trans()
        {
            string ret = "";
            string DEMO_FILE = "audio.wav";
            var speech = SpeechClient.Create();
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 41000,
                AudioChannelCount = 2,
                //LanguageCode = "ko-KR",
                LanguageCode = LanguageCodes.Korean.SouthKorea,
            }, RecognitionAudio.FromFile(DEMO_FILE));

            RecognizeResponse recognizeResponse = new RecognizeResponse();
            

            //var audio = RecognitionAudio.from

            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                    ret = alternative.Transcript;
                }
            }
            return ret;
        }
    }
}
