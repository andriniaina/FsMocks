namespace Tests

open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module ArgumentTests =
    let [<Fact>] ``1) verifying correct arguments``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Add(3)
            list.Add(1)
        }
        mock.verify (fun() -> list.Add(3);list.Add(1))

    let [<Fact>] ``2) verifying wrong arguments should throw an ExpectationViolationException``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Add(1)
        }
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            mock.verify (fun() -> list.Add(20))
            )

    let [<Fact>] ``3) ignored wrong arguments should be accepted``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Add(1) |> ignore_arguments
        }
        mock.verify (fun() -> list.Add(20))

    open Rhino.Mocks.Constraints
    let [<Fact>] ``4) expect argument constraints``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Contains(0) |> only_if_argument [Is.LessThanOrEqual(300)] |> returns false
            list.Contains(0) |> only_if_argument [Is.GreaterThan(300)] |> returns true
        }
        mock.verify (fun() ->
            Assert.False(list.Contains(2))
            Assert.True(list.Contains(20000))
            )



