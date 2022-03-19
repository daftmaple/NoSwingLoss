using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Utils;
using NoSwingLossCounter.Configuration;
using TMPro;
using UnityEngine;

namespace NoSwingLossCounter
{
    class NoSwingLossCounter : BasicCustomCounter, INoteEventHandler
    {
        private TMP_Text _leftText;
        private TMP_Text _rightText;
        private TMP_Text _bottomText;

        public override void CounterInit()
        {
            LabelInit();
        }

        private void LabelInit()
        {
            var label = CanvasUtility.CreateTextFromSettings(Settings);
            label.text = "Full Swing Accuracy";
            label.fontSize = 2.5f;

            Vector3 bottomOffset = Vector3.up * -0.2f;
            TextAlignmentOptions leftAlign = TextAlignmentOptions.Top;
            float bottomTextFontSize = 3.5f;

            if (PluginConfig.Instance.separateSaber)
            {
                _leftText = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(-0.3f, -0.25f, 0));
                _leftText.lineSpacing = -26;
                _leftText.text = "100%";
                _leftText.alignment = leftAlign;
                _leftText.fontSize = 2.5f;

                _rightText = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0.3f, -0.25f, 0));
                _rightText.lineSpacing = -26;
                _rightText.text = "100%";
                _rightText.alignment = TextAlignmentOptions.TopLeft;
                _rightText.fontSize = 2.5f;

                bottomOffset = Vector3.up * -0.6f;
                bottomTextFontSize = 3f;
            }

            _bottomText = CanvasUtility.CreateTextFromSettings(Settings, bottomOffset);
            _bottomText.lineSpacing = -26;
            _bottomText.text = FormatBottomText("100%");
            _bottomText.alignment = TextAlignmentOptions.Top;
            _bottomText.fontSize = bottomTextFontSize;
        }

        private string FormatBottomText(string text) => string.Format("({0})", text);

        public void ScoreHandler(int preCut, int postCut, int accCut)
        {
            _bottomText.text = string.Format("{0:N2}", accCut);

            if (PluginConfig.Instance.separateSaber)
            {
                _leftText.text = string.Format("{0:N2}", preCut);
                _rightText.text = string.Format("{0:N2}", postCut);
            }
        }

        public override void CounterDestroy()
        {
        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            ScoreModel.RawScoreWithoutMultiplier(
                info.swingRatingCounter, 
                info.cutDistanceToCenter, 
                out int preCut, 
                out int postCut, 
                out int accCut
            );
            ScoreHandler(preCut, postCut, accCut);
        }

        public void OnNoteMiss(NoteData data)
        {
            
        }
    }

    class NoSwingLossCalculator
    {

    }
}
