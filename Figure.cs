using System;

namespace Tetris
{
    public class Figure
    {
        public int[,] Shape { get; private set; }
        public int Size { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Figure(int[,] shape)
        {
            Size = shape.GetLength(0);
            Shape = new int[Size, Size];
            Array.Copy(shape, Shape, shape.Length);
            X = 0;
            Y = 0;
        }
        public void Rotate()
        {
            int[,] newShape = new int[Size, Size];
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                    newShape[x, Size - 1 - y] = Shape[y, x];
            Shape = newShape;
        }
    }
} 