namespace Tests

open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module RepetitionsTests =
    let [<Fact>] ``1) repetition occurence should default to Once``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear()
        }
        mock.verify (fun() -> list.Clear())

    let [<Fact>] ``2) simple strict mock with repetition=Once``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected once
        }
        mock.verify (fun() -> list.Clear())

    let [<Fact>] ``2b) simple strict mock with repetition=Twice``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected twice
        }
        mock.verify (fun() -> list.Clear(); list.Clear())

    let [<Fact>] ``2c) simple strict mock expected N Times``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected (times 7)
        }
        mock.verify (fun() -> for i in 1..7 do list.Clear())
            
    let [<Fact>] ``3) simple strict mock with repetition=AnyTime called 0 times``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected at_any_moment
        }
        mock.verify (fun() ->())

    let [<Fact>] ``3b) simple strict mock with repetition=AnyTime called N times``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected at_any_moment
        }
        mock.verify (fun() ->
                for i in 0..13 do list.Clear()
            )
            
    let [<Fact>] ``4) simple strict mock with repetition=AtLeastOnce called 0 times should throw an ExpectationViolationException``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected at_least_once
        }
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            mock.verify (fun() ->())
            )
        
    let [<Fact>] ``4b) simple strict mock with repetition=AtLeastOnce called 1 time``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected at_least_once
        }
        mock.verify (fun() -> list.Clear())
        
    let [<Fact>] ``4c) simple strict mock with repetition=AtLeastOnce called N times``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            list.Clear() |> expected at_least_once
        }
        mock.verify (fun() -> list.Clear();list.Clear();list.Clear())

