using FaceGenerator.MachineLearning.Extensions;
using System.Diagnostics;
using static FaceGenerator.MachineLearning.Helpers.NullCheckHelper;
using static FaceGenerator.MachineLearning.Helpers.MathHelper;

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
            var triangle = Elements.LowerTriangle();
            var rotationProduct = Idedntity(N);
            double absMax;

            long absMaxTime = 0;
            long otherTime = 0;
            Stopwatch timer = new Stopwatch();

            do
            {
                timer.Start();
                (int m, int k) = AbsMaxPosition(triangle);
                absMaxTime += timer.ElapsedTicks;
                timer.Stop();
                timer.Reset();

                timer.Start();
                double xkk = triangle[k][k];
                double xmk = triangle[m][k];
                double xmm = triangle[m][m];
                absMax = System.Math.Abs(xmk);

                (double sinFi, double cosFi) = SinAndCosFrom(xmk, xkk, xmm);

                for (int i = 0; i < k; i++)
                {
                    Rotate(triangle, k, i, m, i, cosFi, sinFi);
                }

                triangle[k][k] = xkk * cosFi * cosFi +
                                 xmm * sinFi * sinFi +
                                 2 * xmk * sinFi * cosFi;
                triangle[m][k] = 0;
                
                for (int i = k + 1; i < m; i++)
                {
                    Rotate(triangle, i, k, m, i, cosFi, sinFi);
                }

                triangle[m][m] = xkk + xmm - triangle[k][k];
                
                for (int i = m + 1; i < N; i++)
                {
                    Rotate(triangle, i, k, i, m, cosFi, sinFi);
                }

                for (int i = 0; i < N; i++)
                {
                    Rotate(rotationProduct, i, k, i, m, cosFi, sinFi);
                }

                otherTime += timer.ElapsedTicks;
                timer.Stop();
                timer.Reset();
            }
            while (absMax > 1E-02);

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
                    jaggedRotationProduct[i, j] = rotationProduct[i][j];
                }
            }

            return (new Vector(diagonalElements), new SquareMatrix(jaggedRotationProduct));
        }

        public bool AssertEigenVector(Vector eigenVector, double eigenValue)
        {
            var mv = this * eigenVector;
            var lambdav = eigenVector * eigenValue;

            return System.Math.Abs(lambdav.Length - mv.Length) <= 1;
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

        private void Rotate(double[][] matrix, int ki, int kj, int mi, int mj, double cosFi, double sinFi)
        {
            double xk = matrix[ki][kj];
            double xm = matrix[mi][mj];

            matrix[ki][kj] = xk * cosFi + xm * sinFi;
            matrix[mi][mj] = -xk * sinFi + xm * cosFi;
        }
    }
}
