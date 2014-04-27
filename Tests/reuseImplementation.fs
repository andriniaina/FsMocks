namespace Tests

open System
open Xunit
open FsMocks.Syntax
open System.Collections.Generic
open System.Windows.Forms

module reuseImplementation =
    let [<Fact>] ``reuseImplementation aka partial mock``() =
        use mock = new FsMockRepository()
        let o:Control = mock.reuseImplementation []
        mock.define Unordered {
            ~~ o.ToString() |> expected twice |> returns "Coucou" |> end_expectation
        }
        // verify expectation
        Assert.Equal<string>("Coucou", o.ToString())
        // reuse existing implementation
        let control = new Control()
        o.Controls.Add(control)
        Assert.True(o.Contains(control))
        // verify expectation again
        Assert.Equal<string>("Coucou", o.ToString())

