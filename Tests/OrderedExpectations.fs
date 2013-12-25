namespace Tests

open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module OrderedExpectations =
    let [<Fact>] ``in order as expected``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Ordered {
            list.Add(1) |> expected once
            list.Add(2) |> expected once
        }
        mock.verify (fun() ->
            list.Add(1)
            list.Add(2)
        )
    let [<Fact>] ``in order different than expected``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Ordered {
            list.Add(1) |> expected once
            list.Add(2) |> expected once
        }
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            mock.verify (fun() ->
                list.Add(2)
                list.Add(1)
            )
        )

    

