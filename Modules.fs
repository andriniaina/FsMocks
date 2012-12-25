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
    
    let repository = new MockRepository()

    module withConstructorArguments =
        let strict (args) : 't =
            repository.StrictMock<'t>(args |> Array.ofList)

        let withDefaultValues (args) : 't when 't : not struct =
            repository.DynamicMock<'t>(args |> Array.ofList)

        let withAutoWiring (args) : 't =
            repository.Stub<'t>(args |> Array.ofList)

        let reuseImplementation (args) : 't when 't : not struct =
            repository.PartialMock<'t>(args |> Array.ofList)
    
    /// <summary>wrapper autour de StrictMock<> (lève des exceptions)</summary>
    let strict () : 'a =
        withConstructorArguments.strict []
        
    /// <summary>wrapper autour de DynamicMock<> (NE lève PAS d'exceptions)</summary>
    let withDefaultValues () : 'a =
        withConstructorArguments.withDefaultValues []
        
    /// <summary>wrapper autour de Stub<> (branche automatiquement les propriétés et les évènements)</summary>
    let withAutoWiring () : 'a =
        withConstructorArguments.withAutoWiring []
        
    /// <summary>wrapper autour de PartialMock<> (réutilise toute implémentation existante)</summary>
    let reuseImplementation () : 'a =
        withConstructorArguments.reuseImplementation []

    let startDefinitions objects =
        objects |> List.iter (fun o -> repository.BackToRecord(o))
    let endDefinitions objects =
        objects |> List.iter (fun o -> repository.Replay(o))
    let verifyCalls objects =
        objects |> List.iter (fun o -> repository.Verify(o))


module MockOperators =
    /// <summary>
    /// Petit raccourci permettant d'écrire ce genre de code : 
    ///     <code>
    ///     let list = ~~ typeof<IList>
    ///     </code>
    ///</summary>
    (*
    let ( ~~ ) (args) : 'a =
        repository.StrictMock(args)
    *)
    open System
    let ( ~~ ) (t:Type) =
        Convert.ChangeType(Mocks.repository.StrictMock(t), t)
        

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