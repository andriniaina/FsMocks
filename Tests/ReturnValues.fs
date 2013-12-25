namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module ReturnValues =
    let [<Fact>] ``simple strict mock with return value``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Contains(1) |> returns true
        }

        mock.verify (fun() ->
            Assert.True(list.Contains(1))
            )

