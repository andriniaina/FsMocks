namespace Tests

type AnyInterface =
    interface
        abstract member DoSomething: unit->unit
        abstract member VirtualProperty:string with get,set
        abstract member RefFunction:int*string byref*int byref->unit
        abstract member OutFunction:int->string*int
        abstract member RefFunctionWithReturnValue:int*string byref->int
    end