
#r @"packages\FSPowerPack.Core.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.dll"
#r @"packages\FSPowerPack.Linq.Community.3.0.0.0\Lib\Net40\FSharp.PowerPack.Linq.dll"
#r @"C:\Users\Andri\Downloads\Moq.4.0.10827.Final\Moq.4.0.10827\NET35\moq.dll"

open Moq
open System.Collections
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.QuotationEvaluation

let a = ArrayList()
let e = <@ a.Count @>.Compile()()
(*
var mock = new Mock<ILoveThisFramework>();

// WOW! No record/replay weirdness?! :)
mock.Setup(framework => framework.DownloadExists("2.0.0.0"))
    .Returns(true)
    .AtMostOnce();

// Hand mock.Object as a collaborator and exercise it, 
// like calling methods on it...
ILoveThisFramework lovable = mock.Object;
bool download = lovable.DownloadExists("2.0.0.0");

// Verify that the given method was indeed called with the expected value
mock.Verify(framework => framework.DownloadExists("2.0.0.0"));
*)

let mock<'T when 'T: not struct> ()  = new Mock<'T>()// when 'T: not struct
let setup (expr:_ Expr) (mock:'a Mock) =
    mock.Setup(fun (o:'a) -> expr.Compile()())



//let mock = new Mock<System.Collections.ArrayList>()

let listMock = mock<System.Collections.ArrayList>()
listMock.Setup(fun (a:ArrayList) -> a.Count).Returns(1)

//FSharpFunc.ToConverter(fun (m:Mock<System.Collections.ArrayList>) -> m.




