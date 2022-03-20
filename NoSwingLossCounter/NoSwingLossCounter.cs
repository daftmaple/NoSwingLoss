using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Utils;
using NoSwingLossCounter.Configuration;
using TMPro;
using UnityEngine;
using Zenject;

namespace NoSwingLossCounter
{
    class NoSwingLossCounter : BasicCustomCounter
    {
        private TMP_Text _leftText;
        private TMP_Text _rightText;
        private TMP_Text _bottomText;

        private readonly ScoreController scoreController;
        private NoSwingLossCalculator calculator;

        public NoSwingLossCounter([Inject] ScoreController scoreController)
        {
            this.scoreController = scoreController;
        }

        public override void CounterInit()
        {
            LabelInit();
            calculator = new NoSwingLossCalculator();

            scoreController.noteWasCutEvent += NoteWasCutEvent;
            scoreController.noteWasMissedEvent += NoteWasMissedEvent;
        }

        public override void CounterDestroy()
        {
            calculator = null;

            scoreController.noteWasCutEvent -= NoteWasCutEvent;
            scoreController.noteWasMissedEvent -= NoteWasMissedEvent;
        }

        private void LabelInit()
        {
            var label = CanvasUtility.CreateTextFromSettings(Settings);
            label.text = "Full Swing Accuracy";
            label.fontSize = 2.5f;

            Vector3 bottomOffset = Vector3.up * -0.2f;
            TextAlignmentOptions leftAlign = TextAlignmentOptions.Top;
            float bottomTextFontSize = 3.5f;
            string bottomText = FormatToPercentage(1);

            if (PluginConfig.Instance.separateSaber)
            {
                _leftText = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(-0.3f, -0.25f, 0));
                _leftText.lineSpacing = -26;
                _leftText.text = FormatToPercentage(1);
                _leftText.alignment = leftAlign;
                _leftText.fontSize = 2.5f;

                _rightText = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0.3f, -0.25f, 0));
                _rightText.lineSpacing = -26;
                _rightText.text = FormatToPercentage(1);
                _rightText.alignment = TextAlignmentOptions.TopLeft;
                _rightText.fontSize = 2.5f;

                bottomOffset = Vector3.up * -0.6f;
                bottomTextFontSize = 3f;
                bottomText = FormatToPercentageBottomText(1);
            }

            _bottomText = CanvasUtility.CreateTextFromSettings(Settings, bottomOffset);
            _bottomText.lineSpacing = -26;
            _bottomText.text = bottomText;
            _bottomText.alignment = TextAlignmentOptions.Top;
            _bottomText.fontSize = bottomTextFontSize;
        }

        private string FormatToPercentage(double number) 
            => string.Format("{0:N2}%", number * 100);
        private string FormatToPercentageBottomText(double number) 
            => string.Format("({0:N2}%)", number * 100);

        private void RefreshText()
        {
            if (PluginConfig.Instance.separateSaber)
            {
                _leftText.text = FormatToPercentage(calculator.PercentageA);
                _rightText.text = FormatToPercentage(calculator.PercentageB);
                _bottomText.text = FormatToPercentageBottomText(calculator.Percentage);
            }
            else
            {
                _bottomText.text = FormatToPercentage(calculator.Percentage);
            }
        }

        private void NoteWasCutEvent (NoteData noteData, in NoteCutInfo noteCutInfo, int multiplier)
        {
            ScoreModel.RawScoreWithoutMultiplier(
                noteCutInfo.swingRatingCounter,
                noteCutInfo.cutDistanceToCenter,
                out int _,
                out int _,
                out int accCut
            );

            calculator.AddScore(noteData.colorType, multiplier, accCut);
            RefreshText();
        }

        private void NoteWasMissedEvent (NoteData noteData, int multiplier)
        {
            calculator.AddMaxScore(noteData.colorType, multiplier);
            RefreshText();
        }
    }

    class NoSwingLossCalculator
    {
        private int MaxScoreA { get; set; } = 0;
        private int MaxScoreB { get; set; } = 0;
        private int ScoreA { get; set; } = 0;
        private int ScoreB { get; set; } = 0;

        private int Score => ScoreA + ScoreB;
        private int MaxScore => MaxScoreA + MaxScoreB;

        public double PercentageA => DivideNonZero(ScoreA, MaxScoreA);
        public double PercentageB => DivideNonZero(ScoreB, MaxScoreB);
        public double Percentage => DivideNonZero(Score, MaxScore);

        public void AddScore(ColorType colorType, int multiplier, int accCut)
        {
            int fullSwingCutScore = (100 + accCut) * multiplier;

            switch (colorType)
            {
                case ColorType.ColorA:
                    ScoreA += fullSwingCutScore;
                    break;
                case ColorType.ColorB:
                    ScoreB += fullSwingCutScore;
                    break;
                case ColorType.None:
                default:
                    return;
            }

            AddMaxScore(colorType, multiplier);
        }

        public void AddMaxScore(ColorType colorType, int multiplier)
        {
            int maxCutScore = 115 * multiplier;

            switch (colorType)
            {
                case ColorType.ColorA:
                    MaxScoreA += maxCutScore;
                    break;
                case ColorType.ColorB:
                    MaxScoreB += maxCutScore;
                    break;
                case ColorType.None:
                default:
                    return;
            }
        }

        private double DivideNonZero(int dividend, int divisor)
        {
            if (divisor == 0) return 1;
            return ((double)dividend / divisor);
        }
    }
}
