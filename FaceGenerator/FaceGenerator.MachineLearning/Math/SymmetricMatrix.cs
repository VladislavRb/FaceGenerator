using FaceGenerator.MachineLearning.Extensions;
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

            do
            {
                (int m, int k) = AbsMaxPosition(triangle);
                double xkk = triangle[k][k];
                double xmk = triangle[m][k];
                double xmm = triangle[m][m];
                absMax = System.Math.Abs(xmk);

                (double sinFi, double cosFi) = SinAndCosFrom(xmk, xkk, xmm);

                for (int i = 0; i < k; i++)
                {
                    double xki = triangle[k][i];
                    double xmi = triangle[m][i];

                    triangle[k][i] = xki * cosFi + xmi * sinFi;
                    triangle[m][i] = -xki * sinFi + xmi * cosFi;
                }

                triangle[k][k] = xkk * cosFi * cosFi +
                                 xmm * sinFi * sinFi +
                                 2 * xmk * sinFi * cosFi;
                triangle[m][k] = 0;
                
                for (int i = k + 1; i < m; i++)
                {
                    double xik = triangle[i][k];
                    double xmi = triangle[m][i];

                    triangle[i][k] = xik * cosFi + xmi * sinFi;
                    triangle[m][i] = -xik * sinFi + xmi * cosFi;
                }

                triangle[m][m] = xkk + xmm - triangle[k][k];
                
                for (int i = m + 1; i < N; i++)
                {
                    double xik = triangle[i][k];
                    double xim = triangle[i][m];

                    triangle[i][k] = xik * cosFi + xim * sinFi;
                    triangle[i][m] = -xik * sinFi + xim * cosFi;
                }

                for (int i = 0; i < N; i++)
                {
                    double rik = rotationProduct[i][k];
                    double rim = rotationProduct[i][m];

                    rotationProduct[i][k] = rik * cosFi + rim * sinFi;
                    rotationProduct[i][m] = -rik * sinFi + rim * cosFi;
                }
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
    }
}
