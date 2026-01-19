namespace Interpolation.Tests

open Xunit
open Interpolation

module InterpolationTests =

    let epsilon = 1e-6

    [<Fact>]
    let ``Linear interpolation at point between two values`` () =
        let p1 = { X = 0.0; Y = 0.0 }
        let p2 = { X = 2.0; Y = 4.0 }
        let result = LinearInterpolation.interpolateBetween p1 p2 1.0
        Assert.Equal(2.0, result, 5)

    [<Fact>]
    let ``Linear interpolation at exact point`` () =
        let points = [ { X = 0.0; Y = 0.0 }; { X = 1.0; Y = 1.0 } ]
        let result = LinearInterpolation.eval 0.0 points
        Assert.Equal(Some 0.0, result)

    [<Fact>]
    let ``Linear interpolation out of range returns None`` () =
        let points = [ { X = 0.0; Y = 0.0 }; { X = 1.0; Y = 1.0 } ]
        let result = LinearInterpolation.eval 2.0 points
        Assert.Equal(None, result)

    [<Fact>]
    let ``Lagrange interpolation for two points equals linear`` () =
        let points = [ { X = 0.0; Y = 0.0 }; { X = 1.0; Y = 1.0 } ]
        let x = 0.5
        let linear = LinearInterpolation.eval x points |> Option.get
        let lagrange = LagrangeInterpolation.eval x points |> Option.get
        Assert.True(abs (linear - lagrange) < epsilon, sprintf "Expected %f but got %f" linear lagrange)

    [<Fact>]
    let ``Newton interpolation for exact point`` () =
        let points = [ { X = 0.0; Y = 0.0 }; { X = 1.0; Y = 1.0 }; { X = 2.0; Y = 4.0 } ]
        let result = NewtonInterpolation.eval 0.0 points |> Option.get
        Assert.Equal(0.0, result, 5)

    [<Fact>]
    let ``Gauss interpolation for known points`` () =
        let points = [ { X = 0.0; Y = 0.0 }; { X = 1.0; Y = 1.0 }; { X = 2.0; Y = 4.0 } ]
        let result = GaussInterpolation.eval 1.0 points |> Option.get
        Assert.Equal(1.0, result, 5)

    [<Fact>]
    let ``Lagrange with single point`` () =
        let points = [ { X = 0.0; Y = 5.0 } ]
        let result = LagrangeInterpolation.eval 0.0 points |> Option.get
        Assert.Equal(5.0, result, 5)

    [<Fact>]
    let ``Newton with parabola points y = x^2`` () =
        let points = [ { X = 0.0; Y = 0.0 }; { X = 1.0; Y = 1.0 }; { X = 2.0; Y = 4.0 } ]
        let result = NewtonInterpolation.eval 1.5 points |> Option.get
        Assert.True(abs (result - 2.25) < 0.1, sprintf "Expected ~2.25 but got %f" result)
