using System;

namespace Tetris
{
    public class GameField
    {
        public int Width { get; }
        public int Height { get; }
        public int[,] Field { get; }
        public GameField(int width, int height)
        {
            Width = width;
            Height = height;
            Field = new int[height, width];
        }
        public bool IsCollision(Figure figure, int offsetX, int offsetY)
        {
            for (int y = 0; y < figure.Size; y++)
            {
                for (int x = 0; x < figure.Size; x++)
                {
                    if (figure.Shape[y, x] == 0) continue;
                    int fx = figure.X + x + offsetX;
                    int fy = figure.Y + y + offsetY;
                    if (fx < 0 || fx >= Width || fy < 0 || fy >= Height)
                        return true;
                    if (Field[fy, fx] == 1)
                        return true;
                }
            }
            return false;
        }
        public void PlaceFigure(Figure figure)
        {
            for (int y = 0; y < figure.Size; y++)
                for (int x = 0; x < figure.Size; x++)
                    if (figure.Shape[y, x] == 1)
                        Field[figure.Y + y, figure.X + x] = 1;
        }
        public int ClearLines()
        {
            int lines = 0;
            for (int y = Height - 1; y >= 0; y--)
            {
                bool full = true;
                for (int x = 0; x < Width; x++)
                    if (Field[y, x] == 0)
                        full = false;
                if (full)
                {
                    for (int yy = y; yy > 0; yy--)
                        for (int x = 0; x < Width; x++)
                            Field[yy, x] = Field[yy - 1, x];
                    for (int x = 0; x < Width; x++)
                        Field[0, x] = 0;
                    lines++;
                    y++;
                }
            }
            return lines;
        }
    }
} 