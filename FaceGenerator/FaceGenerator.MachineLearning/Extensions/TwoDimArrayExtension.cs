namespace FaceGenerator.MachineLearning.Extensions
{
    public static class TwoDimArrayExtension
    {
        public static int Rows(this double[,] array)
        {
            return array.GetLength(0);
        }

        public static int Columns(this double[,] array)
        {
            return array.GetLength(1);
        }

        public static double[][] LowerTriangle(this double[,] array)
        {
            var rows = array.Rows();
            var columns = array.Columns();
            
            var n = rows < columns ? rows : columns;

            var triangle = new double[n][];
            for (int i = 0; i < n; i++)
            {
                var rowI = new double[i + 1];
                for (int j = 0; j < i + 1; j++)
                {
                    rowI[j] = array[i, j];
                }

                triangle[i] = rowI;
            }

            return triangle;
        }
    }
}
