namespace Interpolation.Tests

open System
open System.Diagnostics
open System.IO
open Xunit

module ConsoleTests =

    let runProgram (args: string) (input: string) : string =
        let workDir = 
            let testAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location
            let testDir = Path.GetDirectoryName(testAssemblyPath)
            Path.GetFullPath(Path.Combine(testDir, "..", "..", "..", ".."))
        
        let psi =
            ProcessStartInfo(
                FileName = "dotnet",
                Arguments = sprintf "run --project fp3 -- %s" args,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workDir
            )

        use proc = new Process()
        proc.StartInfo <- psi
        proc.Start() |> ignore

        proc.StandardInput.Write(input)
        proc.StandardInput.Close()

        let output = proc.StandardOutput.ReadToEnd()
        let _ = proc.StandardError.ReadToEnd()

        proc.WaitForExit(5000) |> ignore

        if not proc.HasExited then
            proc.Kill()

        output

    [<Fact>]
    let ``Linear interpolation outputs results`` () =
        let input = "0;0\n1;1\n2;2\n"
        let output = runProgram "--linear --step 1.0" input
        Assert.Contains("linear:", output)
        Assert.Contains("0 0", output)

    [<Fact>]
    let ``Linear interpolation with custom step`` () =
        let input = "0;0\n1;1\n"
        let output = runProgram "--linear --step 0.5" input
        let lines = output.Trim().Split('\n')
        Assert.True(lines.Length >= 2, sprintf "Expected multiple output lines, got %d" lines.Length)

    [<Fact>]
    let ``Lagrange interpolation works`` () =
        let input = "0;0\n1;1\n2;4\n"
        let output = runProgram "--lagrange --step 1.0" input
        Assert.Contains("lagrange:", output)

    [<Fact>]
    let ``Newton interpolation with window size`` () =
        let input = "0;0\n1;1\n2;4\n3;9\n"
        let output = runProgram "--newton -n 3 --step 1.0" input
        Assert.Contains("newton:", output)

    [<Fact>]
    let ``Multiple methods output all results`` () =
        let input = "0;0\n1;1\n2;2\n"
        let output = runProgram "--linear --lagrange --step 1.0" input
        Assert.Contains("linear:", output)
        Assert.Contains("lagrange:", output)

    [<Fact>]
    let ``Gauss interpolation works`` () =
        let input = "0;0\n1;1\n2;4\n"
        let output = runProgram "--gauss --step 1.0" input
        Assert.Contains("gauss:", output)

    [<Fact>]
    let ``Output format is correct`` () =
        let input = "0;0\n1;1\n"
        let output = runProgram "--linear --step 1.0" input
        let lines = output.Trim().Split('\n')

        Assert.True(
            Array.forall (fun (line: string) -> line.Contains(":") && line.Contains(" ")) lines,
            sprintf "Output format incorrect: %s" output
        )

    [<Fact>]
    let ``Point with whitespace separator works`` () =
        let input = "0 0\n1 1\n2 2\n"
        let output = runProgram "--linear --step 1.0" input
        Assert.Contains("linear:", output)

    [<Fact>]
    let ``Mixed separators work`` () =
        let input = "0;0\n1 1\n2;2\n"
        let output = runProgram "--linear --step 1.0" input
        Assert.Contains("linear:", output)
