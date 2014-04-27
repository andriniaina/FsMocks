namespace Tests

open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module ArgumentTests =
    let [<Fact>] ``1) verifying correct arguments``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Add(3)
            list.Add(1)
        }
        
        list.Add(3)
        list.Add(1)

    let [<Fact>] ``2) verifying wrong arguments should throw an ExpectationViolationException``() =
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            use mock = new FsMockRepository()
            let list:int IList = mock.strict []
            mock.define Unordered {
                list.Add(1)
            }
            list.Add(20)
        )

    let [<Fact>] ``3) ignored wrong arguments should be accepted``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            ~~ list.Add(1) |> ignore_arguments
        }
        
        list.Add(20)

    open Rhino.Mocks.Constraints
    let [<Fact>] ``4) expect argument constraints``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            ~~ list.Contains(0) |> returns false |> only_if_argument [Is.LessThanOrEqual(300)]
            ~~ list.Contains(0) |> returns true |> only_if_argument [Is.GreaterThan(300)]
        }
        Assert.False(list.Contains(2))
        Assert.True(list.Contains(20000))



