using FaceGenerator.MachineLearning.Extensions;
using static FaceGenerator.MachineLearning.Helpers.NullCheckHelper;

namespace FaceGenerator.MachineLearning.Math
{
    public class SymmetricMatrix : SquareMatrix
    {
        public SymmetricMatrix(double[,] elements)
        {
            ThrowIfNull(elements);
            Init(elements);

            for (int i = 0; i < N; i++)
            {
                for (int j = i; j < N; j++)
                {
                    Elements[i, j] = Elements[j, i] = elements[i, j];
                }
            }
        }

        public (Vector, SquareMatrix) EigenDecomposition()
        {
            var n = Elements.Rows();
            var nWithoutOne = n - 1;

            var triangle = Elements.LowerTriangle();

            // столбцы меньше чем строки
            //    k      <           m
            // сначала по строкам, потом по столбцам

            //Parallelize potential too
            var absMax = triangle
                [MaxAmongAbsMaxes(rowMaxes, nWithoutOne) + 1]
                [MaxAmongAbsMaxes(columnMaxes, nWithoutOne)];

            var rotationProduct = new double[n][];
            for (int i = 0; i < n; i++)
            {
                rotationProduct[i] = new double[n];
                rotationProduct[i][i] = 1;
            }

            while (absMax > 1E-02)
            {
                int m = MaxAmongAbsMaxes(rowMaxes, nWithoutOne) + 1; // row index
                int k = MaxAmongAbsMaxes(columnMaxes, nWithoutOne); // column index
                double xkk = triangle[k][k];
                absMax = triangle[m][k];

                double fi = Fi(absMax, k, m);
                double cosFi = System.Math.Cos(fi);
                double sinFi = System.Math.Sin(fi);

                for (int i = 0; i < k; i++)
                {
                    triangle[k][i] = triangle[k][i] * cosFi + triangle[m][i] * sinFi;
                    triangle[m][i] = (triangle[m][i] - triangle[k][i] * sinFi) / cosFi;
                }
                triangle[k][k] = xkk * cosFi * cosFi +
                                 triangle[m][m] * sinFi * sinFi +
                                 2 * absMax * sinFi * cosFi;
                triangle[m][k] = 0;
                for (int i = k + 1; i < m; i++)
                {
                    triangle[i][k] = triangle[i][k] * cosFi + triangle[m][i] * sinFi;
                    triangle[m][i] = (triangle[m][i] - triangle[i][k] * sinFi) / cosFi;
                }
                triangle[m][m] = xkk + triangle[m][m] - triangle[k][k];
                for (int i = m + 1; i < n; i++)
                {
                    triangle[i][k] = triangle[i][k] * cosFi + triangle[i][m] * sinFi;
                    triangle[i][m] = (triangle[i][m] - triangle[i][k] * sinFi) / cosFi;
                }

                for (int i = 0; i < n; i++)
                {
                    rotationProduct[k][i] = rotationProduct[k][i] * cosFi + rotationProduct[m][i] * sinFi;
                    rotationProduct[m][i] = (rotationProduct[m][i] - rotationProduct[k][i] * sinFi) / cosFi;
                }
            }

            var diagonalElements = new double[n];
            for (int i = 0; i < n; i++)
            {
                diagonalElements[i] = triangle[i][i];
            }

            return (new Vector(diagonalElements), RotationProduct.Transpose());
        }

        public bool AssertEigenVector(Vector eigenVector, double eigenValue)
        {
            var mv = this * eigenVector;
            var lambdav = eigenValue * eigenVector;

            return System.Math.Abs(lambdav.Length - mv.Length) <= 1e-03;
        }

        private (int, int) AbsMaxPosition(double[][] triangle, int n)
        {

        }

        private int MaxAmongAbsMaxes(double[] absMaxes, int nWithoutOne)
        {
            var absMax = absMaxes[0];
            var maxI = 0;

            for (int i = 1; i < nWithoutOne; i++)
            {
                if (absMaxes[i] > absMax)
                {
                    absMax = absMaxes[i];
                    maxI = i;
                }
            }

            return maxI;
        }

        private double Fi(double valueWithMaxAbs, int k, int m)
        {
            return 0.5 * System.Math.Atan2(2 * valueWithMaxAbs, Elements[k, k] - Elements[m, m]);
        }
    }
}
