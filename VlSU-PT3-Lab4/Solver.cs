namespace VlSU_PT3_Lab4
{
    public class Solver
    {
        public static double Solve(Func<double,double> f, double x0, double x1)
        {
            double x, fx0 = f(x0), fx1;
            do
            {
                fx1 = f(x1);
                x = x1 - fx1 * (x0 - x1) / (fx0 - fx1);

                x0 = x1; fx0 = fx1;
                x1 = x;
            } while (
                Math.Abs(x0 - x1) > 2 * Double.Epsilon &&
                Math.Abs(x0 / x1 - 1) > 2 * Double.Epsilon &&
                Math.Abs(fx1) > 2 * Double.Epsilon
            );

            return x;
        }
    }
}