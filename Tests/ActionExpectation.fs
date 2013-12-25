namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module ActionExpectation =
    let [<Fact>] ``unit->unit expectation helper``() =
        let mock = FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            o.DoSomething |> action
        }
        mock.verify (fun() ->
            o.DoSomething()
            )
    


