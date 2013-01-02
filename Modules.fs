(*
Copyright (c) 2012, Andri Rakotomalala
 All rights reserved.

This software/code is distributed under the BSD license (http://opensource.org/licenses/BSD-3-Clause)

*)


(*
    usage:
        see Tests.fsx
*)
namespace FsMocks

module Mocks =

    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints
    

    
    [<AbstractClass>]
    type DefinitionBuilderBase() =
    
        let _strict (repository:MockRepository) (args) =
            args |> Array.ofList |> repository.StrictMock

        let _withDefaultValues (repository:MockRepository) (args) =
            args |> Array.ofList |> repository.DynamicMock

        let _withAutoWiring (repository:MockRepository) (args) =
            args |> Array.ofList |> repository.Stub

        let _reuseImplementation (repository:MockRepository) (args) : 't when 't : not struct =
            args |> Array.ofList |> repository.PartialMock

        let repo = new MockRepository()
        member x.repository = repo

        member x.verifyExpectations() = x.repository.VerifyAll()
        member x.strict args = _strict x.repository args
        member x.withDefaultValues args = _withDefaultValues x.repository args
        member x.withAutoWiring args = _withAutoWiring x.repository args
        member x.reuseImplementation args = _reuseImplementation x.repository args

    type SimpleMockDefinitionBuilder() =
        inherit DefinitionBuilderBase()
        
        member x.Bind(resource, expr) =
            printfn "calling BackToRecord with object type=%s" (resource.GetType().FullName)
            x.repository.BackToRecord(resource)
            let result = expr resource
            x.repository.Replay(resource)
            printfn "calling Replay with object type=%s" (resource.GetType().FullName)
            result
        member x.Using = x.Bind
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

        member x.Bind(resource, expr) =
            resources.Add(resource) |> ignore
            if (not started) then
                printfn "calling repository.Ordered with object type=%s" (resource.GetType().FullName)
                started <- true
                let result = recordOrdered resource expr
                for resource in resources do x.repository.Replay(resource)
                result
            else
                expr resource
        member x.Using = x.Bind
        member x.Return(value) =
            started <- false
            value
        member x.Zero() =
            printfn "zero"

[<AutoOpen>]
module FsMocksCommonSyntax =
    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints

    let ignoreArguments (c:'a Interfaces.IMethodOptions) =
        c.IgnoreArguments()
    
    let ignorePropertySetter =
        ignoreArguments

    let constraintArgumentsTo (parameters:AbstractConstraint list) (c:'a Interfaces.IMethodOptions) =
        c.Constraints (Array.ofList(parameters))

    let autoproperty (c:'a Interfaces.IMethodOptions) =
        c.PropertyBehavior()
        
    let onlyWhen (constraints:AbstractConstraint list) (c:'a Interfaces.IMethodOptions) =
        c.Constraints (Array.ofList(constraints))

    type RepeatOptions =
        | AnyTimes
        | Once
        | Twice
        | AtLeastOnce
        | Never
        | Times of int
        
module Syntax1 =
    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints
    
    let is f = f

    let expected repeatOptions call =
        let expectation = Expect.Call<_>(call)
        match repeatOptions with
            | AnyTimes -> expectation.Repeat.Any()
            | Once -> expectation.Repeat.Once()
            | Twice -> expectation.Repeat.Twice()
            | AtLeastOnce -> expectation.Repeat.AtLeastOnce()
            | Never -> expectation.Repeat.Never()
            | Times(i) -> expectation.Repeat.Times(i)
        
    let setup =
        SetupResult.For
    
    let returns (value) (c:'a Interfaces.IMethodOptions) = 
        c.Return(value) |> ignore

module Syntax2 =
    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints

    let once = Once
    let anyTimes = AnyTimes
    let twice = Twice
    let atLeastOnce = AtLeastOnce
    let never = Never
    let times = Times

    let returns (value) repeats (call) = 
        let expectation = Expect.Call<_>(call).Return(value)
        
        match repeats with
            | AnyTimes -> expectation.Repeat.Any()
            | Once -> expectation.Repeat.Once()
            | Twice -> expectation.Repeat.Twice()
            | AtLeastOnce -> expectation.Repeat.AtLeastOnce()
            | Never -> expectation.Repeat.Never()
            | Times(i) -> expectation.Repeat.Times(i)
        |> ignore
