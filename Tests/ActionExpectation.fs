namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic
(*
module ActionExpectation =
    let [<Fact>] ``unit->unit expectation helper``() =
        use mock = new FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            // this syntax is similar to : o.DoSomething() |> expected once
            o.DoSomething |> as_action // 'expected once' is implicit
        }
        mock.verify (fun() ->
            o.DoSomething()
            )
*)