using TobbyBingMaps.Common.Commanding;

namespace TobbyBingMaps.MapGeometry
{
    public static class Commands
    {
        static Commands()
        {
            ClosePopupCommand = new Command("ClosePopup");
            StopDrawingCommand = new Command("StopDrawing");
            EditVectorCommand = new Command("EditVectorCommand");
            LoadBalloonDataCommand = new Command("LoadBalloonDataCommand");
            ItemSelectedCommand = new Command("ItemSelectedCommand");
            PollTileLayerCommand = new Command("PollTileLayerCommand");
        }

        public static Command ClosePopupCommand { get; private set; }
        public static Command StopDrawingCommand { get; private set; }
        public static Command EditVectorCommand { get; private set; }
        public static Command LoadBalloonDataCommand { get; private set; }
        public static Command ItemSelectedCommand { get; private set; }
        public static Command PollTileLayerCommand { get; private set; }
    }
}
