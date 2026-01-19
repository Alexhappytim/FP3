open Interpolation

[<EntryPoint>]
let main argv =
    let config = Parser.parseArgs argv
    StreamProcessor.processInputAndOutput config
    0