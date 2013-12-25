namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module ForceImplementation =
    let [<Fact>] ``implement as autoproperty``() =
        let mock = FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            o.VirtualProperty |> implement_as autoproperty
        }
        mock.verify (fun() ->
            o.VirtualProperty <- "value"
            Assert.Equal<string>("value", o.VirtualProperty)
            )
    let [<Fact>] ``force method implementation``() =
        let v = ref ""
        let changeValue () = v:="value"
        let mock = FsMockRepository()
        let o:AnyInterface = mock.strict []
        mock.define Unordered {
            o.DoSomething() |> implement_as (new Action(changeValue))
        }
        mock.verify (fun() ->
            Assert.Equal<string>("", !v)
            o.DoSomething()
            Assert.Equal<string>("value", !v)
            )