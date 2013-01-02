// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// Note that script files will not be part of the project build.

#r @"packages\RhinoMocks.3.6.1\lib\net\Rhino.Mocks.dll"
#load "Modules.fs"

open FsMocks
open FsMocks.Syntax2

//-------------------------------------------------- simple mock
let sample1 () = 
    let mockProvider = new Mocks.SimpleMockDefinitionBuilder()
    let o1 = mockProvider {
        let! (mylist1:System.Collections.IList) = mockProvider.strict []
        
        mylist1.IsReadOnly |> returns anyTimes false

        mylist1.Contains(1) |> returns once false
            
        mylist1.Add(1) |> returns once 1

        mylist1.Add(2) |> returns once 2
        mylist1.Contains(1) |>  returns once true

        return mylist1
    }
    let o2 = mockProvider {
        let! (mylist2:System.Collections.IList) = mockProvider.strict []

        mylist2.Count |> returns once 2

        return mylist2
    }
    
    printfn "o2.Count=%i" (o2.Count)

    printfn "o1.IsReadOnly=%b" (o1.IsReadOnly)

    printfn "o1.Contains(1)=%b" (o1.Contains(1))
    printfn "o1.Contains(1)=%i" (o1.Add(1))
    printfn "o1.Contains(1)=%i" (o1.Add(2))
    printfn "o1.Contains(1)=%b" (o1.Contains(1))
    
    printfn "o1.IsReadOnly=%b" (o1.IsReadOnly)


    mockProvider.verifyExpectations()

sample1()
    
//-------------------------------------------------- ordered mock
let sample2 () =
    let mockProvider = new Mocks.OrderedMockDefinitionBuilder()
    let l1,l2 = mockProvider {
        let! (mylist1:System.Collections.IList) = mockProvider.strict []
        let! (myDict:System.Collections.IDictionary) = mockProvider.strict []

        mylist1.Count |> returns once 1
        myDict.Count |> returns once 2

        return mylist1,myDict
    }

    printfn "l1.Count=%i" (l1.Count)
    printfn "l2.Count=%i" (l2.Count)


    mockProvider.verifyExpectations()
   
sample2()

//-------------------------------------------- this ordered mock should fail
let sample3 () =
    let mockBuilder = new Mocks.OrderedMockDefinitionBuilder()
    let l1,l2 = mockBuilder {
        let! (mylist1:System.Collections.IList) = mockBuilder.strict []
        let! (myDict:System.Collections.IDictionary) = mockBuilder.strict []

        mylist1.Count |> returns once 1
        myDict.Count |> returns once 2

        return mylist1,myDict
    }
    
    printfn "l2.Count=%i" (l2.Count)
    printfn "l1.Count=%i" (l1.Count)

    mockBuilder.verifyExpectations()

sample3()
