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

            scoreController.scoringForNoteFinishedEvent += ScoringForNoteFinishedEvent;
        }

        public override void CounterDestroy()
        {
            scoreController.scoringForNoteFinishedEvent -= ScoringForNoteFinishedEvent;
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

        private void ScoringForNoteFinishedEvent (ScoringElement scoringElement)
        {
            calculator.AddScore(scoringElement);
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

        public void AddScore(ScoringElement scoringElement)
        {
            NoteData.ScoringType scoringType = scoringElement.noteData.scoringType;
            ColorType colorType = scoringElement.noteData.colorType;
            int multiplier = scoringElement.multiplier;

            if (scoringType == NoteData.ScoringType.BurstSliderElement && PluginConfig.Instance.excludeDottedLink)
            {
                // Exclude dotted link (burst slider element) from calculation
                return;
            }

            // https://stackoverflow.com/a/46409973
            if (scoringElement is GoodCutScoringElement goodCutScoringElement)
            {
                int fullSwingCutScore = 0;

                // BurstSliderHead only cares about preswing and accuracy (total points = 85)
                // BurstSliderElement has 20 points each
                // SliderHead does not care about postswing (total points = 115)
                // SliderTail does not care about preswing (total points = 115)
                switch (scoringType)
                {
                    case NoteData.ScoringType.Normal:
                    case NoteData.ScoringType.SliderHead:
                    case NoteData.ScoringType.SliderTail:
                        fullSwingCutScore = 
                            (100 + goodCutScoringElement.cutScoreBuffer.centerDistanceCutScore) * multiplier;
                        break;
                    case NoteData.ScoringType.BurstSliderHead:
                        if (!PluginConfig.Instance.normaliseArrowedLink)
                        {
                            fullSwingCutScore =
                                (70 + goodCutScoringElement.cutScoreBuffer.centerDistanceCutScore) * multiplier;
                        }
                        else
                        {
                            // Assume postswing exists on chain head since it is treated as normal note
                            fullSwingCutScore =
                                (100 + goodCutScoringElement.cutScoreBuffer.centerDistanceCutScore) * multiplier;
                        }
                        break;
                    case NoteData.ScoringType.BurstSliderElement:
                        fullSwingCutScore = 20 * multiplier;
                        break;
                }

                switch (colorType)
                {
                    case ColorType.ColorA:
                        ScoreA += fullSwingCutScore;
                        break;
                    case ColorType.ColorB:
                        ScoreB += fullSwingCutScore;
                        break;
                }
            }

            AddMaxScore(scoringType, colorType);
        }

        private void AddMaxScore(NoteData.ScoringType scoringType, ColorType colorType)
        {
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

            int multiplier = 8;

            // Only check if NoteCount is less than notecount on FC maximum multiplier
            if (NoteCount < 14)
            {
                if (NoteCount == 1) multiplier = 1;
                else if (NoteCount < 6) multiplier = 2;
                else multiplier = 4;
            }

            int maxScoreOnScoreType = 115;

            switch (scoringType)
            {
                case NoteData.ScoringType.BurstSliderHead:
                    // Max score is 85 if arrowed link is not treated as normal note
                    if (!PluginConfig.Instance.normaliseArrowedLink) maxScoreOnScoreType = 85;
                    break;
                case NoteData.ScoringType.BurstSliderElement:
                    maxScoreOnScoreType = 20;
                    break;
            }

            int maxScore = maxScoreOnScoreType * multiplier;

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
