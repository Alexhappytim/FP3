namespace Interpolation

open System

type Point = { X: float; Y: float }

type InterpolationMethod =
    | Linear
    | Lagrange
    | Newton
    | Gauss

type Config =
    { Methods: InterpolationMethod list
      Step: float
      WindowSize: int
      Verbose: bool }

type State =
    { Points: Point list
      LastEmitted: float option }
