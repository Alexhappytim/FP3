namespace Interpolation

open System

module PointGenerator =
    let generateRange (startX: float) (endX: float) (step: float) : float seq =
        seq {
            let mutable x = startX
            while x <= endX + 1e-9 do
                yield x
                x <- x + step
        }
