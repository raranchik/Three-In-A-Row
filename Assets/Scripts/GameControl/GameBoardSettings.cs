namespace GameControl
{
    public struct GameBoardSettings
    {
        public static readonly int MinX = 10;
        public static readonly int MinY = 10;
        public static readonly int MinCountColors = 2;

        public static readonly int MaxX = 50;
        public static readonly int MaxY = 50;
        public static readonly int MaxCountColors = 5;
        public int X { get; private set; }
        public int Y { get; private set; }
        public int CountColors { get; private set; }

        public GameBoardSettings(in int x = 16, in int y = 10, in int colors = 3)
        {
            X = x;
            Y = y;
            CountColors = colors;
        }

        public static GameBoardSettings GetDefaultSettings()
        {
            return new GameBoardSettings(16, 10, 3);
        }

        public static bool ValidSettings(in GameBoardSettings settings)
        {
            int x = settings.X;
            if (x < MinX || x > MaxX)
                return false;

            int y = settings.Y;
            if (y < MinY || y > MaxY)
                return false;

            int countColors = settings.CountColors;
            if (countColors < MinCountColors || countColors > MaxCountColors)
                return false;

            return true;
        }

    }

}