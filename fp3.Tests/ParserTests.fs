namespace Interpolation.Tests

open Xunit
open Interpolation

module ParserTests =

    [<Fact>]
    let ``Parse valid point`` () =
        let line = "1.5;3.0"
        let result = Parser.parsePoint line

        match result with
        | Some p ->
            Assert.Equal(1.5, p.X)
            Assert.Equal(3.0, p.Y)
        | None -> Assert.True(false, "Expected Some but got None")

    [<Fact>]
    let ``Parse point with whitespace`` () =
        let line = "  1.5 ; 3.0  "
        let result = Parser.parsePoint line
        Assert.NotEqual(None, result)

    [<Fact>]
    let ``Parse invalid point returns None`` () =
        let line = "invalid;data"
        let result = Parser.parsePoint line
        Assert.Equal(None, result)

    [<Fact>]
    let ``Parse point with missing value`` () =
        let line = "1.5;"
        let result = Parser.parsePoint line
        Assert.Equal(None, result)

    [<Fact>]
    let ``Parse args with linear flag`` () =
        let args = [| "--linear"; "--step"; "0.5" |]
        let config = Parser.parseArgs args
        Assert.Equal(0.5, config.Step)
        Assert.Contains(InterpolationMethod.Linear, config.Methods)

    [<Fact>]
    let ``Parse args with default linear`` () =
        let args = [||]
        let config = Parser.parseArgs args
        Assert.Contains(InterpolationMethod.Linear, config.Methods)
        Assert.Equal(1.0, config.Step)

    [<Fact>]
    let ``Parse args with multiple methods`` () =
        let args = [| "--linear"; "--lagrange"; "--step"; "0.7" |]
        let config = Parser.parseArgs args
        Assert.Equal(2, List.length config.Methods)
        Assert.Equal(0.7, config.Step)

    [<Fact>]
    let ``Parse Newton with window size`` () =
        let args = [| "--newton"; "-n"; "5"; "--step"; "0.5" |]
        let config = Parser.parseArgs args
        Assert.Contains(InterpolationMethod.Newton, config.Methods)
        Assert.Equal(5, config.WindowSize)
