namespace Tests

open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module OrderedExpectations =
    let [<Fact>] ``0) simple Ordered expectation``() =
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

    let [<Fact>] ``0b) simple Ordered expectation, unexpected``() =
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

    let [<Fact>] ``1) 2 Ordered nested in 1 Unordered``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            mock.define Ordered {
                list.Add(1) |> expected once
                list.Add(2) |> expected once
            }
            mock.define Ordered {
                list.Add(3) |> expected once
                list.Add(4) |> expected once
            }
        }
        mock.verify (fun() ->
            list.Add(1)
            list.Add(2)
            list.Add(3)
            list.Add(4)
        )

    let [<Fact>] ``1b) 2 Ordered nested in 1 Unordered``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            mock.define Ordered {
                list.Add(1) |> expected once
                list.Add(2) |> expected once
            }
            mock.define Ordered {
                list.Add(3) |> expected once
                list.Add(4) |> expected once
            }
        }
        mock.verify (fun() ->
            list.Add(3)
            list.Add(4)
            list.Add(1)
            list.Add(2)
        )

    let [<Fact>] ``2) 2 Ordered nested in 1 Unordered, unexpected``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            mock.define Ordered {
                list.Add(1) |> expected once
                list.Add(2) |> expected once
            }
            mock.define Ordered {
                list.Add(3) |> expected once
                list.Add(4) |> expected once
            }
        }
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            mock.verify (fun() ->
                list.Add(1)
                list.Add(3)
                list.Add(2)
                list.Add(4)
            )
        )

    let [<Fact>] ``3) 2 Unordered nested in 1 Ordered``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Ordered {
            mock.define Unordered {
                list.Add(1) |> expected once
                list.Add(2) |> expected once
            }
            mock.define Unordered {
                list.Add(3) |> expected once
                list.Add(4) |> expected once
            }
        }
        mock.verify (fun() ->
            list.Add(2)
            list.Add(1)
            list.Add(4)
            list.Add(3)
        )

    let [<Fact>] ``3b) 2 Unordered nested in 1 Ordered, unexpected``() =
        let mock = FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Ordered {
            mock.define Unordered {
                list.Add(1) |> expected once
                list.Add(2) |> expected once
            }
            mock.define Unordered {
                list.Add(3) |> expected once
                list.Add(4) |> expected once
            }
        }
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            mock.verify (fun() ->
                list.Add(4)
                list.Add(3)
                list.Add(2)
                list.Add(1)
            )
        )