(*
Copyright (c) 2012, Andri Rakotomalala
 All rights reserved.

This software/code is distributed under the BSD license (http://opensource.org/licenses/BSD-3-Clause)

*)


(*
    usage:


*)
namespace FsMocks

module Mocks =

    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints
    

    let strict (repository:MockRepository) (args) : 't =
        args |> Array.ofList |> repository.StrictMock

    let withDefaultValues (repository:MockRepository) (args) : 't when 't : not struct =
        args |> Array.ofList |> repository.DynamicMock

    let withAutoWiring (repository:MockRepository) (args) : 't =
        args |> Array.ofList |> repository.Stub

    let reuseImplementation (repository:MockRepository) (args) : 't when 't : not struct =
        args |> Array.ofList |> repository.PartialMock
    
    [<AbstractClass>]
    type DefinitionBuilderBase() =
        let repo = new MockRepository()
        member x.repository = repo

        member x.verifyAll() = x.repository.VerifyAll()
        member x.strict args = strict x.repository args
        member x.withDefaultValues args = withDefaultValues x.repository args
        member x.withAutoWiring args = withAutoWiring x.repository args
        member x.reuseImplementation args = reuseImplementation x.repository args

    type SimpleMockDefinitionBuilder() =
        inherit DefinitionBuilderBase()

        member x.Using(resource, expr) =
            printfn "calling BackToRecord with object type=%s" (resource.GetType().FullName)
            x.repository.BackToRecord(resource)
            let result = expr resource
            x.repository.Replay(resource)
            printfn "calling Replay with object type=%s" (resource.GetType().FullName)
            result
        member x.Bind(comp, func) =
            func comp
        member x.Return(value) =
            value
        member x.Zero() =
            printfn "zero"



    
    type OrderedMockDefinitionBuilder() as x =
        inherit DefinitionBuilderBase()

        let resources = new System.Collections.ArrayList()
        let mutable started = false
        let recordOrdered resource expr = // starts an ordered mock, evaluate the expression in between, and stops the odered mock
            use recorder = x.repository.Ordered()
            expr resource

        member x.Using(resource, expr) =
            resources.Add(resource) |> ignore
            if (not started) then
                printfn "calling repository.Ordered with object type=%s" (resource.GetType().FullName)
                started <- true
                let result = recordOrdered resource expr
                for resource in resources do x.repository.Replay(resource)
                result
            else
                expr resource
        member x.Bind(comp, func) =
            func comp
        member x.Return(value) =
            started <- false
            value
        member x.Zero() =
            printfn "zero"


module MockExpectations =
    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints

    let returns value (call:'t) = 
        Expect.Call(call).Return(value) |> ignore

    let ignoreArguments (call:'t) =
        Expect.Call(call).IgnoreArguments()
    
    let ignorePropertySetter =
        ignoreArguments

    let constraintArgumentsTo (parameters:AbstractConstraint list) (call:'t) =
        Expect.Call(call).Constraints (Array.ofList(parameters))

    let autoproperty (call:'t) =
        Expect.Call(call).PropertyBehavior()
        
    let onlyWhen (constraints:AbstractConstraint list) (call:'t) =
        Expect.Call(call).Constraints (Array.ofList(constraints))

    (*
    Syntaxe cible : 

    let mock = Mock.strict IList          // strict (lève des exceptions)
    let mock = Mock.withDefaultValues IList         // dynamic (ne lève pas d'exception)
    let mock = Mock.withAutoProperties IList  // generatestub
    let mock = Mock.reuseImplementation Class   // partial mock
    
    mock.Capacity |> autoproperty
    mock.Capacity |> ignorePropertySetter
    mock.doSomething(1) |> returns 2
    mock.doSomething(1) |> ignorearguments |> returns 2
    mock.doSomething(1) |> onlyWhen [Is.Anything()]    |> returns 2

    define ordered mock
    define unordered mock
   
    use mock
     *)