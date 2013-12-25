namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic
open System.Windows.Forms

module reuseImplementation =
    let [<Fact>] ``reuseImplementation``() =
        let mock = FsMockRepository()
        let o:Control = mock.reuseImplementation []
        mock.define Unordered {
            o.ToString() |> returns "Coucou" |> expected twice
        }
        mock.verify (fun() ->
            // verify expectation
            Assert.Equal<string>("Coucou", o.ToString())
            // reuse existing implementation
            let control = new Control()
            o.Controls.Add(control)
            Assert.True(o.Contains(control))
            // verify expectation again
            Assert.Equal<string>("Coucou", o.ToString())
        )

