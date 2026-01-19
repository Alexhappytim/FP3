namespace Interpolation

module NewtonInterpolation =
    let private dividedDifferences (points: Point array) : float array =
        let n = Array.length points
        let table = Array2D.create n n 0.0
        
        for i in 0..n - 1 do
            table.[i, 0] <- points.[i].Y
        
        for j in 1..n - 1 do
            for i in 0..n - 1 - j do
                table.[i, j] <- (table.[i + 1, j - 1] - table.[i, j - 1]) / 
                               (points.[i + j].X - points.[i].X)
        
        [| for i in 0 .. n - 1 -> table.[0, i] |]

    let eval (x: float) (points: Point list) : float option =
        match points |> List.sortBy (fun p -> p.X) with
        | [] -> None
        | sortedList ->
            let sorted = sortedList |> List.toArray
            let diffs = dividedDifferences sorted
            let n = sorted.Length
            let mutable result = diffs.[0]
            let mutable term = 1.0

            for i in 1 .. n - 1 do
                term <- term * (x - sorted.[i - 1].X)
                result <- result + diffs.[i] * term

            Some result
