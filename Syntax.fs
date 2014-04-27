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

module Syntax =
    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints

    type RepeatOptions =
        | AnyTime
        | Once
        | Twice
        | AtLeastOnce
        | Times of int
    type OrderOptions = | Ordered | Unordered

    // lowercase synonyms of RepeatOptions
    let once = Once
    let at_any_moment = AnyTime
    let twice = Twice
    let at_least_once = AtLeastOnce
    let times = Times
    
    type ExpectationBuilder(recorder:IDisposable) =
        let closingExpressionEvent = new Event<_>()
        [<CLIEvent>]
        member x.OnClosingExpression = closingExpressionEvent.Publish
        member x.Zero () = ()
        member x.Delay (f) =
            f() |> ignore
            recorder.Dispose()
            closingExpressionEvent.Trigger()

    type FsMockRepository() =
        let repo = new MockRepository()
        let mutable nestingLevel = 0
        /// aka StrictMock : Only the methods that were explicitly recorded are accepted as valid. This means that any call that is not expected would cause an exception and fail the test. All the expected methods must be called if the object is to pass verification.
        /// <param name="args">Constructor arguments for base class</param>
        member x.strict args = args |> Array.ofList |> repo.StrictMock
        /// aka PartialMock : Mocking only requested methods: This is available for classes only. It basically means that any non abstract method call will use the original method (no mocking) instead of relying on Rhino Mocks' expectations. You can selectively decide which methods should be mocked.
        /// <param name="args">Constructor arguments for base class</param>
        member x.reuseImplementation args = args |> Array.ofList |> repo.PartialMock
        /// aka Stub : Create a dynamic mock and call PropertyBehavior on its properties
        /// <param name="args">Constructor arguments for base class</param>
        member x.autoProperties args = args |> Array.ofList |> repo.Stub
        /// aka DynamicMocks : All method calls during the replay state are accepted. If there is no special handling setup for a given method, a null or zero is returned. All of the expected methods must be called for the object to pass verification.
        /// <param name="args">Constructor arguments for base class</param>
        member x.emptyImplementation args = args |> Array.ofList |> repo.DynamicMock
        /// !! this should be called before mock definitions
        member x.getEventRaiser (evt) =
            evt |> Event.add (fun _ -> ())
            let raiser = LastCall.IgnoreArguments().GetEventRaiser()
            repo.BackToRecordAll() // reset all recorders
            raiser
        member x.define (order:OrderOptions) =
            nestingLevel <- nestingLevel+1
            let builder = new ExpectationBuilder(match order with | Unordered -> repo.Unordered() | Ordered -> repo.Ordered())
            builder.OnClosingExpression |> Event.add (fun _ ->
                nestingLevel<-nestingLevel-1
                if nestingLevel=0 then repo.ReplayAll()
                )
            builder

        interface IDisposable with member x.Dispose() = repo.VerifyAll()

    let ignore_arguments (m:Interfaces.IMethodOptions<_>) =
        m.IgnoreArguments()
    let for_any_argument_value =
        ignore_arguments
    let ignore_property_setter =
        ignore_arguments

    /// use any Rhino.Mocks.Constraint.*
    let only_if_argument constraints (m:Interfaces.IMethodOptions<_>) = 
        m.Constraints(Array.ofList(constraints))
        
    let ToAction (f:unit->unit) =
        new Action(f)

    let end_expectation _ = ()

    let (~~) (result:'a) = Expect.Call(result)
        
    let expected occurence (m:Interfaces.IMethodOptions<_>) =
        let translate_repetition occurence (r:Interfaces.IRepeat<_>) =
            match occurence with
                | AnyTime -> r.Any()
                | Once -> r.Once()
                | Twice -> r.Twice()
                | AtLeastOnce -> r.AtLeastOnce()
                | Times(i) -> r.Times(i)
        m.Repeat |> translate_repetition occurence

    let implement_as_property (m:Interfaces.IMethodOptions<_>) =
        m.PropertyBehavior()
    let autoproperty = new Action(fun()->())
    let implement_as f (m:Interfaces.IMethodOptions<_>) =
        m.Do(f)

    /// same as 'expected' but for events
    /// usual syntax :     event |> subscription expected twice
    let subscription expectation_function occurence event =
        event |> Event.add (fun _ -> ())
        LastCall.IgnoreArguments() |> expectation_function occurence
    
    let returns (value:_) (m:Interfaces.IMethodOptions<_>) =
        m.Return(value)

    let returns_outref_params ([<ParamArray>] values:Object[]) (m:Interfaces.IMethodOptions<_>) =
        if values.Length=1 then m.OutRef(values.[0]) else m.OutRef(values)

    let throws exceptionInstance (m:Interfaces.IMethodOptions<_>) =
        m.Throw(exceptionInstance)

