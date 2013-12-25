namespace Tests

open System
open System.Windows.Forms
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module autoProperties = 
    let [<Fact>] ``autoProperties``() =
        let mock = FsMockRepository()
        let o:Control = mock.autoProperties []
        mock.define Unordered {
            ()  // no expectation : it's a dynamic mock : everything is allowed, everything is either an autoproperty or returns a default value
        }
        mock.verify (fun() ->
            // check autoproperty
            o.Text <- "Coucou"
            Assert.Equal<string>("Coucou", o.Text)
            // call any virtual method : we should raise no error
            o.ResetText()
            o.Refresh()
        )

