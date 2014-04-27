namespace Tests

open System
open System.Windows.Forms
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module OutRef =
    let [<Fact>] ``check expected 'ref' parameter``() =
        use mock = new FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            ~~ o.RefFunction(1, ref "a", ref 5) |> returns_outref_params [|"b"; 6|] |> end_expectation
        }
        let s1 = ref "a"
        let s2 = ref 5
        o.RefFunction(1, s1, s2)
        Assert.Equal<string>("b", !s1)
        Assert.Equal(6, !s2)

    let [<Fact>] ``check unexpected 'ref' parameter``() =
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            use mock = new FsMockRepository()
            let o:AnyInterface = mock.strict []
            mock.define Unordered {
                ~~ o.RefFunction(1, ref "a", ref 1) |> returns_outref_params [|"b", 2|] |> end_expectation
            }
            o.RefFunction(1, ref "wrong param", ref 1)
        )

    let [<Fact>] ``check expected 'ref' parameter with return value``() =
        use mock = new FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            ~~ o.RefFunctionWithReturnValue(1, ref "a") |> returns_outref_params [|"b"|] |> returns 2 |> end_expectation
        }
        let s = ref "a"
        Assert.Equal(2, o.RefFunctionWithReturnValue(1, s))
        Assert.Equal<string>("b", !s)

