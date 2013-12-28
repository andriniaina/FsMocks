namespace Tests

open System
open System.Windows.Forms
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module OutRef =
    let [<Fact>] ``check expected 'out' parameter``() =
        use mock = new FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            o.RefFunction(1, ref "a") |> returns_outref_params [|"b"|]
        }
        let s = ref "a"
        o.RefFunction(1, s)
        Assert.Equal<string>("b", !s)
    let [<Fact>] ``check unexpected 'out' parameter``() =
        Assert.Throws<Rhino.Mocks.Exceptions.ExpectationViolationException>(fun () ->
            use mock = new FsMockRepository()
            let o:AnyInterface = mock.strict []
            mock.define Unordered {
                o.RefFunction(1, ref "a") |> returns_outref_params [|"b"|]
            }
            let s = ref "x"
            o.RefFunction(1, s)
        )

    let [<Fact>] ``check expected 'out' parameter with return value``() =
        use mock = new FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            o.RefFunctionWithReturnValue(1, ref "a") |> returns_outref_params [|"b"|] |> returns 2
        }
        let s = ref "a"
        Assert.Equal(2, o.RefFunctionWithReturnValue(1, s))
        Assert.Equal<string>("b", !s)

