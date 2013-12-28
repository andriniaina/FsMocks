namespace Tests

open System
open System.Windows.Forms
open Xunit
open FsMocks.Syntax
open System.Collections.Generic

module emptyImplementation = 
    let [<Fact>] ``emptyImplementation should implement all methods and properties and do nothing``() =
        use mock = new FsMockRepository()
        let o:Control = mock.emptyImplementation []
        mock.define Unordered {
            ()  // no expectation : it's a dynamic mock : everything is allowed, all functions/properties are empty and return default values
        }
        // property set should do nothing
        o.Text <- "u"
        Assert.Null(o.Text)
        // call any virtual method : we should raise no error
        o.ResetText()
        o.Refresh()
        // ...or return a default value
        Assert.Equal(new Drawing.Size(), o.GetPreferredSize(Drawing.Size.Empty))

