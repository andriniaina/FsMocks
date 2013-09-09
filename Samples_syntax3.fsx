

//#r @"packages\FSPowerPack.Core.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.dll"
//#r @"packages\FSPowerPack.Linq.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.Linq.dll"

#r @"packages\RhinoMocks.3.6.1\lib\net\Rhino.Mocks.dll"
#load "modules.fs"

open Rhino.Mocks
open Rhino.Mocks.Constraints
open System
open FsMocks.Syntax


#r @"System.Windows.Forms"
open System.Windows.Forms



let mock = FsMockRepository()
let b:Button = mock.strict []
let clickButtonEvent = mock.getEventRaiser (b.Click)

mock.define Unordered {
    b.Click |> subscription expected once
    b.ResetText() |> manual_implementation (new Action(fun()-> printfn "reset text!!"))
}
mock.verify (fun () ->
        b.ResetText()
        b.Click |> Event.add (fun arg -> printfn "clicked!!")
        clickButtonEvent.Raise(b, new EventArgs())
)


