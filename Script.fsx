// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// Note that script files will not be part of the project build.

#r @"packages\RhinoMocks.3.6.1\lib\net\Rhino.Mocks.dll"
#load "Modules.fs"

open Rhino.Mocks.FsWrappers
open Rhino.Mocks.FsWrappers.MockExpectations


// ---------------------------- tests start here 



open System.Collections

(*
let mylist1:IList = Mocks.strict()
let mylist2:IList = Mocks.strict()





[mylist1;mylist2] |> Mocks.startDefinitions
mylist1.Count |> returns 1
mylist2.Count |> returns 2
[mylist1;mylist2] |> Mocks.endDefinitions

printfn "%b" (mylist1.Count=1)
printfn "%b" (mylist2.Count=2)

[mylist1;mylist2] |> Mocks.verifyCalls
*)


//-------------------------------------------------- ordered mock
#r @"packages\RhinoMocks.3.6.1\lib\net\Rhino.Mocks.dll"

#load "Modules.fs"

open Rhino.Mocks.FsWrappers
open Rhino.Mocks.FsWrappers.MockExpectations

let check1 () = 
    let mocks = new Mocks.SimpleMockDefinitionBuilder()
    let o1 = mocks {
        use mylist1:System.Collections.IList = mocks.strict()
        printfn "changing Count"
        mylist1.Count |> returns 1
        printfn "done"
        return mylist1
    }
    let o2 = mocks {
        use mylist2:System.Collections.IList = mocks.strict()
        printfn "changing Count"
        mylist2.Count |> returns 2
        printfn "done"
        return mylist2
    }

    printfn "o2.Count=%i" (o2.Count)
    printfn "o1.Count=%i" (o1.Count)

    //[o1;o2] |> Mocks.verifyCalls
    mocks.verifyAll()


let check2 () =
    let mocks = new Mocks.OrderedMockDefinitionBuilder()
    let l1,l2 = mocks {
        use mylist1:System.Collections.IList = mocks.strict()
        use myDict:System.Collections.IDictionary = mocks.strict()
        printfn "changing Count"
        mylist1.Count |> returns 1
        myDict.Count |> returns 2
        printfn "done"
        return mylist1,myDict
    }

    printfn "l1.Count=%i" (l1.Count)
    printfn "l2.Count=%i" (l2.Count)


    printfn "verifying"
    mocks.verifyAll()
    printfn "verified"
    
let check3 () =
    let builder1 = new Mocks.OrderedMockDefinitionBuilder()
    let l1,l2 = builder1 {
        use mylist1:System.Collections.IList = builder1.strict()
        use myDict:System.Collections.IDictionary = builder1.strict()
        printfn "changing Count"
        mylist1.Count |> returns 1
        myDict.Count |> returns 2
        printfn "done"
        return mylist1,myDict
    }
    
    printfn "l2.Count=%i" (l2.Count)
    printfn "l1.Count=%i" (l1.Count)


    printfn "verifying"
    builder1.verifyAll()
    printfn "verified"