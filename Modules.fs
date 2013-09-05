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

[<AutoOpen>]
module Syntax =
    open System
    open Rhino.Mocks
    open Rhino.Mocks.Constraints
    open Microsoft.FSharp.Quotations
    open Microsoft.FSharp.Linq.QuotationEvaluation

    type RepeatOptions =
        | AnyTimes
        | Once
        | Twice
        | AtLeastOnce
        | Never
        | Times of int

    // lowercase synonyms of RepeatOptions
    let once = Once
    let always = AnyTimes
    let twice = Twice
    let atLeastOnce = AtLeastOnce
    let never = Never
    let times = Times
    
    type RecordBuilder(repo:MockRepository) =
        member x.Run f =
            use recorder = repo.Record()
            printfn "start recording with repo %i" (repo.GetHashCode())
            let result = f()
            printfn "end recording"
            result
        member x.Delay f = f
        member x.Zero () = ()

    type PlaybackBuilder(repo:MockRepository) =
        member x.Run f =
            use player = repo.Playback()
            f()
        member x.Delay f = f
        member x.Zero () = ()
        
    type FsMockRepository() =
        let _repo = new MockRepository()
        /// Only the methods that were explicitly recorded are accepted as valid. This means that any call that is not expected would cause an exception and fail the test. All the expected methods must be called if the object is to pass verification.
        member x.strict args = args |> Array.ofList |> _repo.StrictMock
        /// Mocking only requested methods: This is available for classes only. It basically means that any non abstract method call will use the original method (no mocking) instead of relying on Rhino Mocks' expectations. You can selectively decide which methods should be mocked.
        member x.reuseImplementation args = args |> Array.ofList |> _repo.PartialMock
        /// Create a dynamic mock and call PropertyBehavior on its methods
        member x.autoProperties args = args |> Array.ofList |> _repo.Stub
        /// All method calls during the replay state are accepted. If there is no special handling setup for a given method, a null or zero is returned. All of the expected methods must be called for the object to pass verification.
        member x.withDefaultValues args = args |> Array.ofList |> _repo.DynamicMock

        member x.record = new RecordBuilder(_repo)
        member x.playback = new PlaybackBuilder(_repo)
        
        member x.recordQ (expr:unit Expr) =
            use recorder = _repo.Record()
            expr.Eval()
        member x.playbackQ (expr:unit Expr) = 
            use recorder = _repo.Playback()
            expr.Eval()

    let ignoreArguments _ =
        LastCall.IgnoreArguments() |> ignore
    
    let ignorePropertySetter =
        ignoreArguments

    let constraintArgumentsTo (parameters:AbstractConstraint list) _ =
        LastCall.Constraints(Array.ofList(parameters)) |> ignore

    let implement_autoproperty _ =
        LastCall.PropertyBehavior() |> ignore

    let _map_repeat (r:Interfaces.IRepeat<_>) =
        function
            | AnyTimes -> r.Any()
            | Once -> r.Once()
            | Twice -> r.Twice()
            | AtLeastOnce -> r.AtLeastOnce()
            | Never -> r.Never()
            | Times(i) -> r.Times(i)
    
    let is f = f

    let expected repeats _ =
        let repeat = LastCall.Repeat
        _map_repeat repeat repeats |> ignore
    
    let returns value _ =
        LastCall.Return(value) |> ignore
    let throws repeats exceptionInstance _ =
        LastCall.Throw(exceptionInstance) |> ignore
