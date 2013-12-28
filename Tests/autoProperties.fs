namespace Tests

open System
open System.Windows.Forms
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module autoProperties = 
    let [<Fact>] ``autoProperties/stub get and set, and method calls``() =
        use mock = new FsMockRepository()
        let o:Control = mock.autoProperties []
        mock.define Unordered {
            ()  // no expectation : it's a stub : everything is allowed, all methods are return a default value, all properties are wired as autoproperties
        }
        // check autoproperty
        o.Text <- "Coucou"
        Assert.Equal<string>("Coucou", o.Text)
        // call any virtual method : we should raise no error
        o.ResetText()
        o.Refresh()

