using System;
using System.Collections.Generic;

namespace GameApp.API.Models
{
    public class SudokuHelper
    {
        private int[,] board;
        private int N = 9;
        private int SRN; // Căn bậc 2 của N (là 3)
        private int K;   // Số ô trống cần đục lỗ

        public SudokuHelper(int missingDigits)
        {
            this.K = missingDigits;
            double SRNd = Math.Sqrt(N);
            SRN = (int)SRNd;
            board = new int[N, N];
        }

        public MapSudoku GenerateGame()
        {
            FillValues();

            // Lưu lại bảng đáp án (đã giải full)
            int[,] solution = new int[9, 9];
            Array.Copy(board, solution, board.Length);

            // Đục lỗ (tạo đề bài)
            RemoveKDigits();

            // Chuyển mảng 2 chiều thành mảng 1 chiều cho dễ gửi qua API
            return new MapSudoku
            {
                Solution = Flatten(solution),
                Puzzle = Flatten(board)
            };
        }

        private List<int> Flatten(int[,] arr)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    list.Add(arr[i, j]);
                }
            }
            return list;
        }

        private void FillValues()
        {
            FillDiagonal();
            FillRemaining(0, SRN);
        }

        private void FillDiagonal()
        {
            for (int i = 0; i < N; i = i + SRN)
                FillBox(i, i);
        }

        private bool UnUsedInBox(int rowStart, int colStart, int num)
        {
            for (int i = 0; i < SRN; i++)
                for (int j = 0; j < SRN; j++)
                    if (board[rowStart + i, colStart + j] == num)
                        return false;
            return true;
        }

        private void FillBox(int row, int col)
        {
            int num;
            for (int i = 0; i < SRN; i++)
            {
                for (int j = 0; j < SRN; j++)
                {
                    do
                    {
                        num = RandomGenerator(N);
                    }
                    while (!UnUsedInBox(row, col, num));
                    board[row + i, col + j] = num;
                }
            }
        }

        private int RandomGenerator(int num)
        {
            Random rand = new Random();
            return (int)Math.Floor((double)(rand.NextDouble() * num + 1));
        }

        private bool CheckSafe(int i, int j, int num)
        {
            return (UnUsedInRow(i, num) &&
                    UnUsedInCol(j, num) &&
                    UnUsedInBox(i - i % SRN, j - j % SRN, num));
        }

        private bool UnUsedInRow(int i, int num)
        {
            for (int j = 0; j < N; j++)
                if (board[i, j] == num)
                    return false;
            return true;
        }

        private bool UnUsedInCol(int j, int num)
        {
            for (int i = 0; i < N; i++)
                if (board[i, j] == num)
                    return false;
            return true;
        }

        private bool FillRemaining(int i, int j)
        {
            if (j >= N && i < N - 1)
            {
                i = i + 1;
                j = 0;
            }
            if (i >= N && j >= N)
                return true;
            if (i < SRN)
            {
                if (j < SRN)
                    j = SRN;
            }
            else if (i < N - SRN)
            {
                if (j == (int)(i / SRN) * SRN)
                    j = j + SRN;
            }

            for (int num = 1; num <= N; num++)
            {
                if (CheckSafe(i, j, num))
                {
                    board[i, j] = num;
                    if (FillRemaining(i, j + 1))
                        return true;
                    board[i, j] = 0;
                }
            }
            return false;
        }

        private void RemoveKDigits()
        {
            int count = K;
            while (count != 0)
            {
                int cellId = RandomGenerator(N * N) - 1;
                int i = (cellId / N);
                int j = cellId % 9;
                if (j != 0) j = j - 1;

                if (board[i, j] != 0)
                {
                    count--;
                    board[i, j] = 0; // 0 nghĩa là ô trống
                }
            }
        }
    }

    // Class chứa dữ liệu trả về cho Frontend
    public class MapSudoku
    {
        public List<int> Solution { get; set; } // Đáp án full
        public List<int> Puzzle { get; set; }   // Đề bài (có số 0)
    }
}