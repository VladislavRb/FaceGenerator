using static FaceGenerator.MachineLearning.Helpers.NullCheckHelper;
using FaceGenerator.MachineLearning.Extensions;
using System;

namespace FaceGenerator.MachineLearning.Math
{
    public class SquareMatrix : Matrix
    {
        protected int N;

        protected SquareMatrix() : base() { }

        public SquareMatrix(double[,] elements)
        {
            ThrowIfNull(elements);
            Init(elements);
        }

        public static SquareMatrix operator *(SquareMatrix m1, SquareMatrix m2)
        {
            return new SquareMatrix(ProductAsElements(m1, m2));
        }

        public static double[][] Idedntity(int n)
        {
            var identity = new double[n][];
            for (int i = 0; i < n; i++)
            {
                identity[i] = new double[n];
                identity[i][i] = 1;
            }

            return identity;
        }

        public override SquareMatrix Transpose()
        {
            return new SquareMatrix(TransposedElements());
        }

        protected void Init(double[,] elements)
        {
            var rows = elements.Rows();
            var columns = elements.Columns();

            N = rows < columns ? rows : columns;
            if (N == 0)
            {
                throw new ArgumentException("Array, representing matrix entries, contains no elements");
            }

            Elements = new double[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Elements[i, j] = elements[i, j];
                }
            }
        }
    }
}
