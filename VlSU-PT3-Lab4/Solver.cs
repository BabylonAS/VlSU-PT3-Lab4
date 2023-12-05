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

        public static double SolveBisect(Func<double,double> f, double x0, double x1)
        {
            // Удостоверимся, что границы отрезка заданы по возрастанию
            double a = x0, b = x1;
            if (x1 < x0) {
                a = x1;
                b = x0;
            }

            // Проверить случаи, когда одна из границ является корнем
            // функции
            double fa = f(a), fb = f(b);
            if (fa == 0.0)
                return a;
            else if (fb == 0.0)
                return b;

            // Удостоверимся, что функция принимает разные знаки на
            // обоих границах отрезка
            if (Math.Sign(fa) == Math.Sign(fb))
                return Double.NaN;

            // Производим деление отрезков
            double c, fc;
            ushort iterations = 0;
            while (iterations < UInt16.MaxValue)
            {
                // Определяем середину отрезка
                c = a + (b - a) * 0.5;
                fc = f(c);

                if (fc == 0.0 || b - a <= 2 * Double.Epsilon)
                    // Корень найден
                    return c;
                else if (Math.Sign(fa) != Math.Sign(fc))
                    // Корень находится в левом подотрезке
                    b = c;
                else
                {
                    // Корень находится в правом подотрезке
                    a = c;
                    fa = fc;
                }
                iterations++;
            }

            // Выполнено слишком много итераций
            return Double.NaN;
        }
    }
}