namespace Tests

open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module RepetitionsTests =
    let [<Fact>] ``1) repetition occurence should default to Once``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear()
        }
        list.Clear()

    let [<Fact>] ``2) simple strict mock with repetition=Once``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected once
        }
        list.Clear()

    let [<Fact>] ``2b) simple strict mock with repetition=Twice``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected twice
        }
        list.Clear()
        list.Clear()

    let [<Fact>] ``2c) simple strict mock expected N Times``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected (times 7)
        }
        for i in 1..7 do list.Clear()
            
    let [<Fact>] ``3) simple strict mock with repetition=AnyTime called 0 times``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected at_any_moment
        }

    let [<Fact>] ``3b) simple strict mock with repetition=AnyTime called N times``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected at_any_moment
        }
        for i in 0..13 do list.Clear()
            
    let [<Fact>] ``4) simple strict mock with repetition=AtLeastOnce called 0 times should throw an ExpectationViolationException``() =
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            use mock = new FsMockRepository()
            let list:int IList = mock.strict []
            mock.define Unordered {
                list.Clear() |> expected at_least_once
            }
        )
        
    let [<Fact>] ``4b) simple strict mock with repetition=AtLeastOnce called 1 time``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected at_least_once
        }
        list.Clear()
        
    let [<Fact>] ``4c) simple strict mock with repetition=AtLeastOnce called N times``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected at_least_once
        }
        list.Clear()
        list.Clear()
        list.Clear()

