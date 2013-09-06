

#r @"packages\FSPowerPack.Core.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.dll"
#r @"packages\FSPowerPack.Linq.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.Linq.dll"

#r @"packages\RhinoMocks.3.6.1\lib\net\Rhino.Mocks.dll"
#load "modules.fs"

open Rhino.Mocks
open Rhino.Mocks.Constraints
open System
open FsMocks
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation

let a = <@1@>

a.Eval()

let mock = FsMockRepository()
let (mylist1:System.Collections.IList) = mock.strict []
mock.define Ordered 
    <@
    mylist1.Add "e" |> expected any_times |> only_if_argument [Is.NotNull()] |> returns 1
    mylist1.Clear() |> expected twice
@>

mock.verify
    <@
    let r = mylist1.Add("unknown argument")
    printfn "r=%i" r
    mylist1.Add("another argument") |> ignore
    mylist1.Clear()
    mylist1.Clear()
@>

