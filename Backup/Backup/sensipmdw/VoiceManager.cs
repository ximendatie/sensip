using System;
using System.Collections.Generic;
using System.Text;
using SpeechLib;

namespace Sensip
{
    class VoiceManager
    {
        private SpVoice voice;
        private SpeechLib.SpSharedRecoContext objRecoContext;
        private SpeechLib.ISpeechRecoGrammar grammar;
        private string strData = "No recording yet";

        public VoiceManager()
        {
            voice = new SpVoice();
        }

        public void Speak(string txt)
        {
            voice.Speak(txt, SpeechVoiceSpeakFlags.SVSFDefault);
        }
    }
}
