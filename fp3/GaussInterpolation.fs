namespace Interpolation

module GaussInterpolation =
    let private lagrangeBasis (j: int) (x: float) (points: Point array) : float =
        let xj = points.[j].X
        let mutable result = 1.0

        for m in 0 .. (Array.length points - 1) do
            if m <> j then
                result <- result * (x - points.[m].X) / (xj - points.[m].X)

        result

    let private findNearestPointIndex (x: float) (points: Point array) : int =
        let mutable minIdx = 0
        let mutable minDiff = abs (x - points.[0].X)

        for i in 1 .. (Array.length points - 1) do
            let diff = abs (x - points.[i].X)

            if diff < minDiff then
                minDiff <- diff
                minIdx <- i

        minIdx

    let eval (x: float) (points: Point list) : float option =
        match points |> List.sortBy (fun p -> p.X) with
        | [] -> None
        | sortedList ->
            let sorted = sortedList |> List.toArray
            let _ = findNearestPointIndex x sorted // kept for potential tuning
            let mutable acc = 0.0

            for i in 0 .. sorted.Length - 1 do
                acc <- acc + lagrangeBasis i x sorted * sorted.[i].Y

            Some acc
