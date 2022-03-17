using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;

namespace NoSwingLossCounter
{
    class Counter : BasicCustomCounter, INoteEventHandler
    {

        public override void CounterDestroy()
        {

        }

        public override void CounterInit()
        {

        }

        public void OnNoteCut(NoteData data, NoteCutInfo info)
        {
            // Bomb
            if (data.colorType == ColorType.None)
                return;

            // TODO: logic here
        }


        public void OnNoteMiss(NoteData data)
        {
            // Do nothing because there is no cut information
            // throw new System.NotImplementedException();
        }

    }
}
