namespace Interpolation

module LagrangeInterpolation =
    let private lagrangeBasis (points: Point array) (j: int) (x: float) : float =
        let xj = points.[j].X
        let mutable acc = 1.0
        for m in 0 .. points.Length - 1 do
            if m <> j then
                acc <- acc * (x - points.[m].X) / (xj - points.[m].X)
        acc

    let eval (x: float) (points: Point list) : float option =
        match points |> List.sortBy (fun p -> p.X) with
        | [] -> None
        | sorted ->
            let arr = sorted |> List.toArray
            let n = arr.Length
            let value =
                [ 0 .. n - 1 ]
                |> List.sumBy (fun j -> lagrangeBasis arr j x * arr.[j].Y)
            Some value
