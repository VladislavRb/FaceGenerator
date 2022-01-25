namespace FaceGenerator.MachineLearning.Helpers
{
    public static class MathHelper
    {
        private static double Epsilon = 5e-03;

        public static (double, double) SinAndCosFrom(double xmk, double xkk, double xmm)
        {
            int xmkSign = System.Math.Sign(xmk);

            if (System.Math.Abs(xkk - xmm) < Epsilon)
            {
                return (xmkSign, 0);
            }

            double tg2fi = 2 * xmk / (xkk - xmm);
            double cos2fi = 1 / System.Math.Sqrt(1 + tg2fi * tg2fi);
            int sinSign = (xkk > xmm ? 1 : -1) * xmkSign;

            return (sinSign * System.Math.Sqrt(0.5 - 0.5 * cos2fi),
                System.Math.Sqrt(0.5 + 0.5 * cos2fi));
        }
    }
}
