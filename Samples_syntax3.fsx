

//#r @"packages\FSPowerPack.Core.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.dll"
//#r @"packages\FSPowerPack.Linq.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.Linq.dll"
#r @"packages\FSPowerPack.Core.Community.3.0.0.0\Lib\Net20\FSharp.PowerPack.dll"
#r @"packages\FSPowerPack.Linq.Community.3.0.0.0\Lib\Net20\FSharp.PowerPack.Linq.dll"

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
    b.Hide() |> implement_as empty_function |> expected once
    //b.Hide |> action |> expected once
    b.Click |> subscription expected once
    b.AllowDrop |> implement_as autoproperty
    b.PointToClient(Drawing.Point.Empty) |> ignore_arguments |> expected once |> returns (new Drawing.Point(2,3))
}
mock.verify (fun () ->
        b.Hide()
        b.AllowDrop <- true
        b.Click |> Event.add (fun arg -> printfn "clicked!!")
        clickButtonEvent.Raise(b, new EventArgs())
        
        let p = b.PointToClient(new Drawing.Point(5,6))
        printfn "%i" (p.X)
)


open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation

let mock2 = FsMockRepository()
let (mylist1:System.Collections.IList) = mock2.strict []
mock2.define Ordered {
    mylist1.Add "e" |> expected at_any_moment |> returns 1 |> only_if_argument [Is.NotNull()]
    mylist1.Clear() |> expected twice
}

mock2.verify (fun()->
    let r = mylist1.Add("unknown argument")
    printfn "r=%i" r
    mylist1.Add("another argument") |> ignore
    mylist1.Clear()
    mylist1.Clear()
)

