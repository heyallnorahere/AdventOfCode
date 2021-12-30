namespace AdventOfCode.Structures
{
    public sealed class Map
    {
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            mData = new int[width, height];
        }
        public int this[int x, int y]
        {
            get => mData[x, y];
            set => mData[x, y] = value;
        }
        public int this[Vector position]
        {
            get => mData[position.X, position.Y];
            set => mData[position.X, position.Y] = value;
        }
        public int Width { get; }
        public int Height { get; }
        public Vector Size => (Width, Height);
        public bool IsOutOfBounds(Vector position)
        {
            return position.X < 0 || position.X >= Width ||
                position.Y < 0 || position.Y >= Height;
        }
        private readonly int[,] mData;
    }
}