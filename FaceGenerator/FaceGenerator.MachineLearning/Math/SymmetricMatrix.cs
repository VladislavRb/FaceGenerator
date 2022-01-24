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

        // столбцы меньше чем строки
        //    k      <           m
        // сначала по строкам, потом по столбцам

        //Parallelize potential too
        public (Vector, SquareMatrix) EigenDecomposition()
        {
            var triangle = Elements.LowerTriangle();
            var rotationProduct = Idedntity(N);
            double absMax;

            do
            {
                (int m, int k) = AbsMaxPosition(triangle);
                double xkk = triangle[k][k];
                absMax = System.Math.Abs(triangle[m][k]);

                double fi = Fi(triangle, k, m);
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
                for (int i = m + 1; i < N; i++)
                {
                    triangle[i][k] = triangle[i][k] * cosFi + triangle[i][m] * sinFi;
                    triangle[i][m] = (triangle[i][m] - triangle[i][k] * sinFi) / cosFi;
                }

                for (int i = 0; i < N; i++)
                {
                    rotationProduct[k][i] = rotationProduct[k][i] * cosFi + rotationProduct[m][i] * sinFi;
                    rotationProduct[m][i] = (rotationProduct[m][i] - rotationProduct[k][i] * sinFi) / cosFi;
                }
            }
            while (absMax > 1E-04);

            var diagonalElements = new double[N];
            for (int i = 0; i < N; i++)
            {
                diagonalElements[i] = triangle[i][i];
            }

            var jaggedRotationProduct = new double[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    jaggedRotationProduct[i, j] = rotationProduct[j][i];
                }
            }

            return (new Vector(diagonalElements), new SquareMatrix(jaggedRotationProduct));
        }

        public bool AssertEigenVector(Vector eigenVector, double eigenValue)
        {
            var mv = this * eigenVector;
            var lambdav = eigenValue * eigenVector;

            return System.Math.Abs(lambdav.Length - mv.Length) <= 1e-01;
        }

        private (int, int) AbsMaxPosition(double[][] lowerTriangle)
        {
            var absmax = System.Math.Abs(lowerTriangle[1][0]);
            int maxI = 1, maxJ = 0;

            for (int i = 2; i < N; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    var currentElement = System.Math.Abs(lowerTriangle[i][j]);
                    if (currentElement > absmax)
                    {
                        maxI = i;
                        maxJ = j;
                        absmax = currentElement;
                    }
                }
            }

            return (maxI, maxJ);
        }

        private double Fi(double[][] triangle, int k, int m)
        {
            return 0.5 * System.Math.Atan2(2 * System.Math.Abs(triangle[m][k]), triangle[k][k] - triangle[m][m]);
        }
    }
}
