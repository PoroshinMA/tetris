using System;
using System.Threading;
using System.Collections.Generic;

namespace Tetris
{
    class Program
    {
        static readonly int fieldWidth = 10;
        static readonly int fieldHeight = 20;
        static readonly int tickMs = 400;
        static readonly ConsoleKey[] controlKeys = { ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.DownArrow, ConsoleKey.UpArrow };
        static readonly List<int[,]> figures = new List<int[,]>
        {
            new int[,] { {1,1,1,1}, {0,0,0,0}, {0,0,0,0}, {0,0,0,0} }, // I
            new int[,] { {1,1}, {1,1} }, // O
            new int[,] { {0,1,0}, {1,1,1}, {0,0,0} }, // T
            new int[,] { {1,1,0}, {0,1,1}, {0,0,0} }, // S
            new int[,] { {0,1,1}, {1,1,0}, {0,0,0} }, // Z
            new int[,] { {1,0,0}, {1,1,1}, {0,0,0} }, // J
            new int[,] { {0,0,1}, {1,1,1}, {0,0,0} }  // L
        };
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("ТЕТРИС\nУправление: ← → ↓ — движение, ↑ — поворот. Esc — выход.\nНажмите любую клавишу для старта...");
                Console.ReadKey(true);
                RunGame();
                Console.WriteLine("\nИгра окончена! Нажмите Enter для новой игры или Esc для выхода.");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape) break;
            }
        }

        static void RunGame()
        {
            var field = new GameField(fieldWidth, fieldHeight);
            Figure current = NextFigure();
            current.X = fieldWidth / 2 - current.Size / 2;
            current.Y = 0;
            int score = 0;
            bool gameOver = false;
            DateTime lastTick = DateTime.Now;
            bool figurePlaced = false;
            Draw(field, current, score, figurePlaced);
            while (!gameOver)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Escape) { gameOver = true; break; }
                    Figure moved = CopyFigure(current);
                    if (key == ConsoleKey.LeftArrow) moved.X--;
                    else if (key == ConsoleKey.RightArrow) moved.X++;
                    else if (key == ConsoleKey.DownArrow) moved.Y++;
                    else if (key == ConsoleKey.UpArrow)
                    {
                        moved.Rotate();
                        if (moved.X < 0) moved.X = 0;
                        if (moved.X + moved.Size > fieldWidth) moved.X = fieldWidth - moved.Size;
                    }
                    if (!field.IsCollision(moved, 0, 0))
                        current = moved;
                    Draw(field, current, score, false);
                }
                if ((DateTime.Now - lastTick).TotalMilliseconds > tickMs)
                {
                    Figure down = CopyFigure(current); down.Y++;
                    if (!field.IsCollision(down, 0, 0))
                    {
                        current = down;
                        Draw(field, current, score, false);
                    }
                    else
                    {
                        field.PlaceFigure(current);
                        int lines = field.ClearLines();
                        score += lines * 100;
                        Draw(field, current, score, true); // рисуем без текущей фигуры
                        current = NextFigure();
                        current.X = fieldWidth / 2 - current.Size / 2;
                        current.Y = 0;
                        if (field.IsCollision(current, 0, 0))
                        {
                            gameOver = true;
                            continue;
                        }
                    }
                    lastTick = DateTime.Now;
                }
                Thread.Sleep(10);
            }
        }

        static Figure NextFigure()
        {
            int[,] shape = figures[rnd.Next(figures.Count)];
            int size = shape.GetLength(0);
            int[,] copy = new int[size, size];
            Array.Copy(shape, copy, shape.Length);
            return new Figure(copy);
        }

        static Figure CopyFigure(Figure f)
        {
            int[,] copy = new int[f.Size, f.Size];
            Array.Copy(f.Shape, copy, f.Shape.Length);
            var fig = new Figure(copy) { X = f.X, Y = f.Y };
            return fig;
        }

        static void Draw(GameField field, Figure current, int score, bool figurePlaced)
        {
            Console.SetCursorPosition(0, 4);
            for (int y = 0; y < field.Height; y++)
            {
                Console.Write("|");
                for (int x = 0; x < field.Width; x++)
                {
                    bool isFigure = false;
                    if (!figurePlaced)
                    {
                        for (int fy = 0; fy < current.Size; fy++)
                            for (int fx = 0; fx < current.Size; fx++)
                                if (current.Shape[fy, fx] == 1 && current.Y + fy == y && current.X + fx == x)
                                    isFigure = true;
                    }
                    if (field.Field[y, x] == 1 || isFigure)
                        Console.Write("■");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine("|");
            }
            Console.WriteLine(new string('-', field.Width + 2));
            Console.WriteLine($"Счёт: {score}");
        }
    }
}
