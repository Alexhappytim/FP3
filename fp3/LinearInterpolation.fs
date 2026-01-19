namespace Interpolation

open System

module LinearInterpolation =
    let interpolateBetween (p1: Point) (p2: Point) (x: float) : float =
        if p2.X = p1.X then
            p1.Y
        else
            p1.Y + (p2.Y - p1.Y) * (x - p1.X) / (p2.X - p1.X)

    let eval (x: float) (points: Point list) : Option<float> =
        let sorted = points |> List.sortBy (fun p -> p.X)
        let segments = sorted |> List.pairwise

        segments
        |> List.tryPick (fun (a, b) ->
            if x >= a.X && x <= b.X then
                Some(interpolateBetween a b x)
            else
                None)
