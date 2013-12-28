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

/// Create a repository :
///    use mock = new FsMockRepository()
/// Create a mock object :
///    let mylist:IList = mock.strict []       // other commands : strict/reuseImplementation/autoProperties/withDefaultValues
/// Create an event raiser :
///    let clickButtonEvent = mock.getEventRaiser (b.Click)
/// Define expectations :
///    mock.define Ordered {   /*mock statements*/   }
/// Verify expectations :
///    mock.verify (fun()->   /*test statements*/   )
///
///  example mock statements :
///    mylist.Add 1 |> expected twice |> only_if_argument [Is.NotNull()] |> returns 1
///    mylist.Add null |> expected once |> throws (new ArgumentException())
///    mylist.Count |> implement_as_property
///    mylist.Item(null) |> for_any_argument_value |> returns 2
///    let button.Click |> subscription expected once
///    
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
        /// <param name="args">Class constructor arguments</param>
        member x.strict args = args |> Array.ofList |> repo.StrictMock
        /// aka PartialMock : Mocking only requested methods: This is available for classes only. It basically means that any non abstract method call will use the original method (no mocking) instead of relying on Rhino Mocks' expectations. You can selectively decide which methods should be mocked.
        /// <param name="args">Class constructor arguments</param>
        member x.reuseImplementation args = args |> Array.ofList |> repo.PartialMock
        /// aka Stub : Create a dynamic mock and call PropertyBehavior on its properties
        /// <param name="args">Class constructor arguments</param>
        member x.autoProperties args = args |> Array.ofList |> repo.Stub
        /// aka DynamicMocks : All method calls during the replay state are accepted. If there is no special handling setup for a given method, a null or zero is returned. All of the expected methods must be called for the object to pass verification.
        /// <param name="args">Class constructor arguments</param>
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

    let ignore_arguments _ =
        LastCall.IgnoreArguments() |> ignore
    let for_any_argument_value =
        ignore_arguments
    let ignore_property_setter =
        ignore_arguments

    /// use any Rhino.Mocks.Constraint.*
    let only_if_argument constraints _ = 
        LastCall.Constraints(Array.ofList(constraints)) |> ignore
        
    let ToAction (f:unit->unit) =
        new Action(f)
        
    let expected occurence _ =
        match occurence with
            | AnyTime -> LastCall.Repeat.Any()
            | Once -> LastCall.Repeat.Once()
            | Twice -> LastCall.Repeat.Twice()
            | AtLeastOnce -> LastCall.Repeat.AtLeastOnce()
            | Times(i) -> LastCall.Repeat.Times(i)
        |> ignore

    let implement_as_property _ =
        LastCall.PropertyBehavior() |> ignore
    let autoproperty = new Action(fun()->())
    let implement_as f _ =
        LastCall.Do(f) |> ignore

    /// same as 'expected' but for events
    /// usual syntax :     event |> subscription expected twice
    let subscription expectation_function occurence event =
        event |> Event.add (fun _ -> ()) |> ignore_arguments |> expectation_function occurence
    
    let returns (value:obj) _ =
        LastCall.Return(value) |> ignore

    let returns_outref_params ([<ParamArray>] values:obj[]) _ =
        if values.Length=1 then LastCall.OutRef(values.[0]) else LastCall.OutRef(values)
            |> ignore

    let throws exceptionInstance _ =
        LastCall.Throw(exceptionInstance) |> ignore

