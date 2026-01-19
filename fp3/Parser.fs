namespace Interpolation

open System
open System.Globalization

module Parser =
    let parsePoint (line: string) : Option<Point> =
        let normalized = line.Replace(';', ' ').Trim()
        let parts = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries)

        match parts with
        | [| x; y |] ->
            let style = NumberStyles.Float
            let culture = CultureInfo.InvariantCulture
            match Double.TryParse(x, style, culture), Double.TryParse(y, style, culture) with
            | (true, xv), (true, yv) -> Some { X = xv; Y = yv }
            | _ -> None
        | _ -> None

    let parseArgs (args: string array) : Config =
        let mutable methods: InterpolationMethod list = []
        let mutable step = 1.0
        let mutable windowSize = 4
        let mutable verbose = false
        let mutable i = 0

        while i < args.Length do
            match args.[i] with
            | "--linear" ->
                methods <- methods @ [ InterpolationMethod.Linear ]
                i <- i + 1
            | "--lagrange" ->
                methods <- methods @ [ InterpolationMethod.Lagrange ]
                i <- i + 1
            | "--newton" ->
                methods <- methods @ [ InterpolationMethod.Newton ]
                i <- i + 1
            | "--gauss" ->
                methods <- methods @ [ InterpolationMethod.Gauss ]
                i <- i + 1
            | "-n" ->
                if i + 1 < args.Length then
                    match Int32.TryParse args.[i + 1] with
                    | true, n when n > 1 -> windowSize <- n
                    | _ -> ()
                    i <- i + 2
                else
                    i <- i + 1
            | "--step" ->
                if i + 1 < args.Length then
                    let style = NumberStyles.Float
                    let culture = CultureInfo.InvariantCulture
                    match Double.TryParse(args.[i + 1], style, culture) with
                    | true, s when s > 0.0 -> step <- s
                    | _ -> ()
                    i <- i + 2
                else
                    i <- i + 1
            | "-v" | "--verbose" ->
                verbose <- true
                i <- i + 1
            | _ -> i <- i + 1

        let chosen = if List.isEmpty methods then [ InterpolationMethod.Linear ] else methods

        { Methods = chosen
          Step = step
          WindowSize = windowSize
          Verbose = verbose }
