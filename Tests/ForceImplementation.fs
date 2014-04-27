namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module ForceImplementation =
    let [<Fact>] ``property autoimplementation``() =
        use mock = new FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            ~~ o.VirtualProperty |> implement_as_property
        }
        o.VirtualProperty <- "value"
        Assert.Equal<string>("value", o.VirtualProperty)

    let [<Fact>] ``manual method implementation``() =
        let v = ref ""
        let changeValue () = v:="value"
        use mock = new FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            ~~ o.DoSomething() |> implement_as (new Action(changeValue))
        }

        Assert.Equal<string>("", !v)
        o.DoSomething()
        Assert.Equal<string>("value", !v)