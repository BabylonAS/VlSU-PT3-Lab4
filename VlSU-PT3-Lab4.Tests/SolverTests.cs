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
}