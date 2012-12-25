(*
Copyright (c) 2012, Andri Rakotomalala
 All rights reserved.

This software/code is distributed under the BSD license (http://opensource.org/licenses/BSD-3-Clause)

*)

namespace Rhino.Mocks.FsWrappers

module Mocks =

    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints
    
    module withConstructorArguments =
        let strict (args) (repository:MockRepository) : 't =
            repository.StrictMock<'t>(args |> Array.ofList)

        let withDefaultValues (args) (repository:MockRepository) : 't when 't : not struct =
            repository.DynamicMock<'t>(args |> Array.ofList)

        let withAutoWiring (args) (repository:MockRepository) : 't =
            repository.Stub<'t>(args |> Array.ofList)

        let reuseImplementation (args) (repository:MockRepository) : 't when 't : not struct =
            repository.PartialMock<'t>(args |> Array.ofList)
    
    /// <summary>wrapper autour de StrictMock (lève des exceptions)</summary>
    let strict (repo:MockRepository)  =
        withConstructorArguments.strict [] repo
        
    /// <summary>wrapper autour de DynamicMock (NE lève PAS d'exceptions)</summary>
    let withDefaultValues () =
        withConstructorArguments.withDefaultValues []
        
    /// <summary>wrapper autour de Stub (branche automatiquement les propriétés et les évènements)</summary>
    let withAutoWiring () =
        withConstructorArguments.withAutoWiring []
        
    /// <summary>wrapper autour de PartialMock (réutilise toute implémentation existante)</summary>
    let reuseImplementation () =
        withConstructorArguments.reuseImplementation []

    [<AbstractClass>]
    type DefinitionBuilderBase() =
        abstract repository:MockRepository with get

        member x.verifyAll() = x.repository.VerifyAll()
        member x.strict2 (args) : 't = withConstructorArguments.strict args x.repository
        member x.withDefaultValues2 (args) : 't when 't : not struct = withConstructorArguments.withDefaultValues args x.repository
        member x.withAutoWiring2 (args) : 't = withConstructorArguments.withAutoWiring args x.repository
        member x.reuseImplementation2 (args) : 't when 't : not struct = withConstructorArguments.reuseImplementation args x.repository
        member x.strict () : 'a = strict x.repository
        member x.withDefaultValues () : 'a = x.withDefaultValues2 []
        member x.withAutoWiring () : 'a = x.withAutoWiring2 []
        member x.reuseImplementation () : 'a = x.reuseImplementation2 []

    type SimpleMockDefinitionBuilder() =
        inherit DefinitionBuilderBase()
        
        let repo = new MockRepository()
        override x.repository = repo

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
        let useOrderedRecorder resource expr = // starts ordered mock, eval, and stops odered mock
            use recorder = x.repository.Ordered()
            expr resource
        
        let repo = new MockRepository()
        override x.repository = repo

        member x.Using(resource, expr) =
            resources.Add(resource) |> ignore
            if (not started) then
                printfn "calling repository.Ordered with object type=%s" (resource.GetType().FullName)
                started <- true
                let result = useOrderedRecorder resource expr
                for resource in resources do x.repository.Replay(resource)
                result
            else
                expr resource
        member x.Bind(comp, func) =
            func comp
        member x.Return(value) =
            started <- false
            value
        member x.Zero() = ()


module MockExpectations =
    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints

    let returns value (call:'t) = 
        Expect.Call<'t>(call).Return(value) |> ignore

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