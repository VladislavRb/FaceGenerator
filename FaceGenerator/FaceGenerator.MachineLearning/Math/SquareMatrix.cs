using static FaceGenerator.MachineLearning.Helpers.NullCheckHelper;
using FaceGenerator.MachineLearning.Extensions;
using System;

namespace FaceGenerator.MachineLearning.Math
{
    public class SquareMatrix : Matrix
    {
        protected int N;

        protected SquareMatrix() : base() { }

        public SquareMatrix(double[,] elements) : base()
        {
            ThrowIfNull(elements);
            Init(elements);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Elements[i, j] = elements[i, j];
                }
            }
        }

        public static SquareMatrix operator *(SquareMatrix m1, SquareMatrix m2)
        {
            return new SquareMatrix(ProductAsElements(m1, m2));
        }

        public static SquareMatrix Identity(int n)
        {
            var elements = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                elements[i, i] = 1;
            }

            return new SquareMatrix(elements);
        }

        public SymmetricMatrix AsSymmetric()
        {
            return new SymmetricMatrix(Elements);
        }

        public double[] DiagonalElements()
        {
            var diagonal = new double[N];
            for (int i = 0; i < N; i++)
            {
                diagonal[i] = Elements[i, i];
            }

            return diagonal;
        }

        public double Determinant()
        {
            return DecompositionSum(Elements.Copy());
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
        }

        private double DecompositionSum(double[,] elements)
        {
            var rows = elements.Rows();
            if (rows == 1)
            {
                return elements[0, 0];
            }

            var pivotI = 0;
            while (elements[pivotI, 0] == 0)
            {
                pivotI++;
                if (pivotI == rows)
                {
                    return 0;
                }
            }

            elements.SwapRows(pivotI, 0);
            for (int i = 1; i < rows; i++)
            {
                var coef = - elements[i, 0] / elements[0, 0];
                elements.AddRow(i, 0, coef);
            }

            var result = elements[0, 0] * DecompositionSum(Minor(elements));

            return (pivotI == 0 ? 1 : -1) * result;
        }

        private double[,] Minor(double[,] elements)
        {
            var minorRows = elements.Rows() - 1;
            var minor = new double[minorRows, minorRows];

            for (int i = 0; i < minorRows; i++)
            {
                for (int j = 0; j < minorRows; j++)
                {
                    minor[i, j] = elements[i + 1, j + 1];
                }
            }

            return minor;
        }
    }
}
