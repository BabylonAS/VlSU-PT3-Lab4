using Xunit;

namespace VlSU_PT3_Lab4.Tests;

public class SolverTests
{
    [Fact]
    public void SolveEquation()
    {
        double result = Solver.Solve(x => 2 * x * x - 5, 0, -1);
        Assert.Equal(-Math.Sqrt(2.5), result, 1e-10);
    }

    [Fact]
    public void SolveEquationByBisection()
    {
        Func<double, double> f = x => 0.5*x*x*x - 6*x + 2;

        // Функция имеет три корня в пределах отрезка [-4; 4].
        // Сканируем четыре подотрезка длиной 2 каждый.

        // Эталонные значения получены через библиотеку scipy
        // для Python (функция scipy.optimize.fsolve)
        Assert.Equal(-3.6200758584679065, Solver.SolveBisect(f, -4, -2), 1e-10);
        Assert.Equal(Double.NaN, Solver.SolveBisect(f, -2, 0), 1e-10);
        Assert.Equal(0.3365088035620548, Solver.SolveBisect(f, 0, 2), 1e-10);
        Assert.Equal(3.2835670549058515, Solver.SolveBisect(f, 2, 4), 1e-10);
    }
}