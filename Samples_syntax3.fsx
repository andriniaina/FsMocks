

#r @"packages\FSPowerPack.Core.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.dll"
#r @"packages\FSPowerPack.Linq.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.Linq.dll"

#r @"packages\RhinoMocks.3.6.1\lib\net\Rhino.Mocks.dll"
#load "modules.fs"

open Rhino.Mocks
open Rhino.Mocks.Constraints
open System
open FsMocks

let mock = FsMockRepository()
let (mylist1:System.Collections.IList) = mock.strict []
mock.record {
    printfn "inside recorder"
    mylist1.Add "e" |> expected AnyTimes |> constraintArgumentsTo [Is.NotNull()] |> returns 1
    mylist1.Clear() |> expected twice
}


mock.playback {
    let r = mylist1.Add("unknown argument")
    printfn "r=%i" r
    mylist1.Add("another argument") |> ignore
    //should throw an expection : mylist1.Add(null) |> ignore
    mylist1.Clear()
    mylist1.Clear()
}


let mock2 = FsMockRepository()
let (mylist2:System.Collections.IList) = mock2.strict []
mock2.recordQ <@
                    printfn "inside recorder"
                    mylist2.Add "e" |> expected AnyTimes |> constraintArgumentsTo [Is.NotNull()] |> returns 1
                    mylist2.Clear() |> expected twice
@>


mock2.playbackQ <@
                    let r = mylist2.Add("unknown argument")
                    printfn "inside playback r=%i" r
                    mylist2.Add("another argument") |> ignore
                    //should throw an expection : mylist1.Add(null) |> ignore
                    mylist2.Clear()
                    mylist2.Clear()
@>