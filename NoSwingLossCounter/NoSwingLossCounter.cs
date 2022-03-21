using CountersPlus.Counters.Custom;
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
        private readonly NoSwingLossCalculator calculator;

        public NoSwingLossCounter(
            [Inject] ScoreController scoreController
        )
        {
            this.scoreController = scoreController;
            this.calculator = new NoSwingLossCalculator();
        }

        public override void CounterInit()
        {
            LabelInit();

            scoreController.noteWasCutEvent += NoteWasCutEvent;
            scoreController.noteWasMissedEvent += NoteWasMissedEvent;
        }

        public override void CounterDestroy()
        {
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
            calculator.AddMaxScore(noteData.colorType);
            RefreshText();
        }
    }

    class NoSwingLossCalculator
    {
        private int NoteCountA { get; set; } = 0;
        private int NoteCountB { get; set; } = 0;

        private int MaxScoreA { get; set; } = 0;
        private int MaxScoreB { get; set; } = 0;
        private int ScoreA { get; set; } = 0;
        private int ScoreB { get; set; } = 0;
        private int NoteCount => NoteCountA + NoteCountB;
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
            }

            AddMaxScore(colorType);
        }

        public void AddMaxScore(ColorType colorType)
        {
            int multiplier = 8;

            // Add Note Count
            switch (colorType)
            {
                case ColorType.ColorA:
                    NoteCountA += 1;
                    break;
                case ColorType.ColorB:
                    NoteCountB += 1;
                    break;
            }

            // Only check if NoteCount is less than notecount on FC maximum multiplier
            if (NoteCount < 14)
            {
                if (NoteCount == 1) multiplier = 1;
                else if (NoteCount < 6) multiplier = 2;
                else multiplier = 4;
            }

            int maxScore = 115 * multiplier;

            switch (colorType)
            {
                case ColorType.ColorA:
                    MaxScoreA += maxScore;
                    break;
                case ColorType.ColorB:
                    MaxScoreB += maxScore;
                    break;
            }
        }

        private double DivideNonZero(int dividend, int divisor)
        {
            if (divisor == 0) return 1;
            return ((double)dividend / divisor);
        }
    }
}
