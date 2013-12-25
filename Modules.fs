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
///    let mock = FsMockRepository()
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
///    mylist.Count |> implement_autoproperty
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
    
    type RecordBuilder(recorder:IDisposable) =
        member x.Zero () = ()
        member x.Delay (f) =
            f() |> ignore
            recorder.Dispose()

    type FsMockRepository() =
        let repo = new MockRepository()
        /// aka StrictMock : Only the methods that were explicitly recorded are accepted as valid. This means that any call that is not expected would cause an exception and fail the test. All the expected methods must be called if the object is to pass verification.
        member x.strict args = args |> Array.ofList |> repo.StrictMock
        /// aka PartialMock : Mocking only requested methods: This is available for classes only. It basically means that any non abstract method call will use the original method (no mocking) instead of relying on Rhino Mocks' expectations. You can selectively decide which methods should be mocked.
        member x.reuseImplementation args = args |> Array.ofList |> repo.PartialMock
        /// aka Stub : Create a dynamic mock and call PropertyBehavior on its properties
        member x.autoProperties args = args |> Array.ofList |> repo.Stub
        /// aka DynamicMocks : All method calls during the replay state are accepted. If there is no special handling setup for a given method, a null or zero is returned. All of the expected methods must be called for the object to pass verification.
        member x.withDefaultValues args = args |> Array.ofList |> repo.DynamicMock
        
        member x.getEventRaiser (evt) =
            //if repo.IsInReplayMode(o) then failwith "The raiser can be created only when recording"
            evt |> Event.add (fun _ -> ())
            let raiser = LastCall.IgnoreArguments().GetEventRaiser()
            repo.BackToRecordAll() // reset all recorders
            raiser
        member x.define (order:OrderOptions) =
            new RecordBuilder(match order with | Unordered -> repo.Unordered() | Ordered -> repo.Ordered())
        member x.verify (f:unit->unit) = 
            repo.ReplayAll()
            use recorder = repo.Playback()
            f()

    let ignore_arguments _ =
        LastCall.IgnoreArguments() |> ignore
    let for_any_argument_value =
        ignore_arguments
    let ignore_property_setter =
        ignore_arguments

    let only_if_argument constraints _ = 
        LastCall.Constraints(Array.ofList(constraints)) |> ignore
    let autoimplement_property _ =
        LastCall.PropertyBehavior() |> ignore

    let action (f:unit->unit) =
        Expect.Call(new Expect.Action(f)) |> ignore

    let expected occurence _ =
        match occurence with
            | AnyTime -> LastCall.Repeat.Any()
            | Once -> LastCall.Repeat.Once()
            | Twice -> LastCall.Repeat.Twice()
            | AtLeastOnce -> LastCall.Repeat.AtLeastOnce()
            | Times(i) -> LastCall.Repeat.Times(i)
        |> ignore

    let empty_function = new Action( fun()->()) // just a simple helper to mock unit->unit functions
    let autoproperty = new Action( fun()->())
    let implement_as f _ =
        if (autoproperty=f) then autoimplement_property()
        else LastCall.Do(f) |> ignore

    /// same as expected but for events
    let subscription expected_function occurence event =
        event |> Event.add (fun _ -> ()) |> ignore_arguments |> expected_function occurence
    
    let returns (value:obj) _ =
        LastCall.Return(value) |> ignore
    let throws exceptionInstance _ =
        LastCall.Throw(exceptionInstance) |> ignore

