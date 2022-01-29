namespace GameControl
{
    public struct GameBoardSettings
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int CountColors { get; private set; }

        public GameBoardSettings(in int x = 16, in int y = 10, in int colors = 3)
        {
            X = x;
            Y = y;
            CountColors = colors;
        }

    }
}