namespace Tests

type AnyInterface =
    interface
        abstract member DoSomething: unit->unit
        abstract member VirtualProperty:string with get,set
    end