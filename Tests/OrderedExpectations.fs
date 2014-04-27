namespace Tests

open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module OrderedExpectations =
    let [<Fact>] ``0) simple Ordered expectation``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Ordered {
            list.Add(1) |> end_expectation
            list.Add(2) |> end_expectation
        }
        list.Add(1)
        list.Add(2)

    let [<Fact>] ``0b) simple Ordered expectation, unexpected``() =
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            use mock = new FsMockRepository()
            let list:int IList = mock.strict []
            mock.define Ordered {
                list.Add(1) |> end_expectation
                list.Add(2) |> end_expectation
            }
            list.Add(2)
            list.Add(1)
        )

    let [<Fact>] ``1) 2 Ordered nested in 1 Unordered``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Unordered {
            mock.define Ordered {
                list.Add(1) |> end_expectation
                list.Add(2) |> end_expectation
            }
            mock.define Ordered {
                list.Add(3) |> end_expectation
                list.Add(4) |> end_expectation
            }
        }
        list.Add(1)
        list.Add(2)
        list.Add(3)
        list.Add(4)

    let [<Fact>] ``1b) 2 Ordered nested in 1 Unordered``() =
        // create mocks
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        // mock definition
        mock.define Unordered {
            mock.define Ordered {
                ~~ list.Add(1) |> end_expectation
                ~~ list.Add(2) |> end_expectation
            }
            mock.define Ordered {
                ~~ list.Add(3) |> expected twice |> end_expectation
                ~~ list.Contains(5) |> returns true |> end_expectation
            }
        }

        // run test
        list.Add(3)
        list.Add(3)
        Assert.True(list.Contains(5))
        list.Add(1)
        list.Add(2)
        // verify expectations

    let [<Fact>] ``2) 2 Ordered nested in 1 Unordered, unexpected``() =
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            use mock = new FsMockRepository()
            let list:int IList = mock.strict []
            mock.define Unordered {
                mock.define Ordered {
                    list.Add(1) |> end_expectation
                    list.Add(2) |> end_expectation
                }
                mock.define Ordered {
                    list.Add(3) |> end_expectation
                    list.Add(4) |> end_expectation
                }
            }
            list.Add(1)
            list.Add(3)
            list.Add(2)
            list.Add(4)
        )

    let [<Fact>] ``3) 2 Unordered nested in 1 Ordered``() =
        use mock = new FsMockRepository()
        let list:int IList = mock.strict []
        mock.define Ordered {
            mock.define Unordered {
                list.Add(1) |> end_expectation
                list.Add(2) |> end_expectation
            }
            mock.define Unordered {
                list.Add(3) |> end_expectation
                list.Add(4) |> end_expectation
            }
        }
        list.Add(2)
        list.Add(1)
        list.Add(4)
        list.Add(3)

    let [<Fact>] ``3b) 2 Unordered nested in 1 Ordered, unexpected``() =
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            use mock = new FsMockRepository()
            let list:int IList = mock.strict []
            mock.define Ordered {
                mock.define Unordered {
                    list.Add(1) |> end_expectation
                    list.Add(2) |> end_expectation
                }
                mock.define Unordered {
                    list.Add(3) |> end_expectation
                    list.Add(4) |> end_expectation
                }
            }
            list.Add(4)
            list.Add(3)
            list.Add(2)
            list.Add(1)
        )