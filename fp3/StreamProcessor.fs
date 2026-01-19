namespace Interpolation

open System

module StreamProcessor =
    let private methodName =
        function
        | InterpolationMethod.Linear -> "linear"
        | InterpolationMethod.Lagrange -> "lagrange"
        | InterpolationMethod.Newton -> "newton"
        | InterpolationMethod.Gauss -> "gauss"

    let private nearestWindow (size: int) (points: Point list) (x: float) : Point list =
        let sorted = points |> List.sortBy (fun p -> p.X)

        if sorted.Length <= size then
            sorted
        else
            let arr = sorted |> List.toArray
            let len = arr.Length

            let idx =
                let mutable k = len - 1

                for i in 0 .. len - 1 do
                    if arr.[i].X >= x && i < k then
                        k <- i

                k

            let half = size / 2

            let start =
                idx - half
                |> fun s -> Math.Max(s, 0)
                |> fun s -> if s + size > len then len - size else s

            arr.[start .. start + size - 1] |> Array.toList

    let private interpolate
        (config: Config)
        (points: Point list)
        (x: float)
        (method_: InterpolationMethod)
        : float option =
        match method_ with
        | InterpolationMethod.Linear -> LinearInterpolation.eval x points
        | InterpolationMethod.Lagrange -> LagrangeInterpolation.eval x points
        | InterpolationMethod.Newton ->
            let window = nearestWindow config.WindowSize points x
            NewtonInterpolation.eval x window
        | InterpolationMethod.Gauss ->
            let window = nearestWindow config.WindowSize points x
            GaussInterpolation.eval x window

    let private emit (config: Config) (state: State) (isEof: bool) : State =
        match state.Points with
        | []
        | [ _ ] -> state
        | pts ->
            let sorted = pts |> List.sortBy (fun p -> p.X)

            let startX =
                match state.LastEmitted with
                | None -> sorted.Head.X
                | Some last -> last + config.Step

            let lastPoint = sorted |> List.last

            let endX =
                if isEof then
                    lastPoint.X + 2.0 * config.Step
                else
                    lastPoint.X - config.Step

            if startX > endX then
                state
            else
                PointGenerator.generateRange startX endX config.Step
                |> Seq.iter (fun xVal ->
                    for m in config.Methods do
                        match interpolate config sorted xVal m with
                        | Some y -> printfn "%s: %.10g %.10g" (methodName m) xVal y
                        | None -> ())

                { state with LastEmitted = Some endX }

    let processInputAndOutput (config: Config) : unit =
        let rec loop (state: State) =
            match Console.In.ReadLine() with
            | null ->
                let _ = emit config state true
                ()
            | line ->
                match Parser.parsePoint line with
                | Some p ->
                    let updatedPoints = (p :: state.Points) |> List.sortBy (fun pt -> pt.X)
                    let newState = emit config { state with Points = updatedPoints } false
                    loop newState
                | None -> loop state

        loop { Points = []; LastEmitted = None }
