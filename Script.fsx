// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// Note that script files will not be part of the project build.

#r @"packages\RhinoMocks.3.6.1\lib\net\Rhino.Mocks.dll"
#load "Modules.fs"

open Rhino.Mocks.FsWrappers
open Rhino.Mocks.FsWrappers.MockOperators
open Rhino.Mocks.FsWrappers.MockExpectations


// ---------------------------- tests start here 



open System.Collections


let mylist1:IList = Mocks.strict()
let mylist2:IList = Mocks.strict()





[mylist1;mylist2] |> Mocks.startDefinitions
mylist1.Count |> returns 1
mylist2.Count |> returns 2
[mylist1;mylist2] |> Mocks.endDefinitions

printfn "%b" (mylist1.Count=1)
printfn "%b" (mylist2.Count=2)

[mylist1;mylist2] |> Mocks.verifyCalls
