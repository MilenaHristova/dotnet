
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 7:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 7:0:0:0
}
.assembly StructsAsArrayElements01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.StructsAsArrayElements01
{
  // Offset: 0x00000000 Length: 0x000007A0
  // WARNING: managed resource file FSharpSignatureData.StructsAsArrayElements01 created
}
.mresource public FSharpOptimizationData.StructsAsArrayElements01
{
  // Offset: 0x000007A8 Length: 0x00000232
  // WARNING: managed resource file FSharpOptimizationData.StructsAsArrayElements01 created
}
.module StructsAsArrayElements01.exe
// MVID: {63DBF1DE-B642-0B3D-A745-0383DEF1DB63}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000016E2B360000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed StructsAsArrayElements01
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class sequential ansi serializable sealed nested public T
         extends [System.Runtime]System.ValueType
         implements class [System.Runtime]System.IEquatable`1<valuetype StructsAsArrayElements01/T>,
                    [System.Runtime]System.Collections.IStructuralEquatable,
                    class [System.Runtime]System.IComparable`1<valuetype StructsAsArrayElements01/T>,
                    [System.Runtime]System.IComparable,
                    [System.Runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field public int32 i
    .method public hidebysig virtual final 
            instance int32  CompareTo(valuetype StructsAsArrayElements01/T obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       33 (0x21)
      .maxstack  5
      .locals init (valuetype StructsAsArrayElements01/T& V_0,
               class [System.Runtime]System.Collections.IComparer V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_000f:  stloc.2
      IL_0010:  ldloc.0
      IL_0011:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0016:  stloc.3
      IL_0017:  ldloc.2
      IL_0018:  ldloc.3
      IL_0019:  cgt
      IL_001b:  ldloc.2
      IL_001c:  ldloc.3
      IL_001d:  clt
      IL_001f:  sub
      IL_0020:  ret
    } // end of method T::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  StructsAsArrayElements01/T
      IL_0007:  call       instance int32 StructsAsArrayElements01/T::CompareTo(valuetype StructsAsArrayElements01/T)
      IL_000c:  ret
    } // end of method T::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [System.Runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       39 (0x27)
      .maxstack  5
      .locals init (valuetype StructsAsArrayElements01/T V_0,
               valuetype StructsAsArrayElements01/T& V_1,
               class [System.Runtime]System.Collections.IComparer V_2,
               int32 V_3,
               int32 V_4)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  StructsAsArrayElements01/T
      IL_0006:  stloc.0
      IL_0007:  ldloca.s   V_0
      IL_0009:  stloc.1
      IL_000a:  ldarg.2
      IL_000b:  stloc.2
      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0012:  stloc.3
      IL_0013:  ldloc.1
      IL_0014:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0019:  stloc.s    V_4
      IL_001b:  ldloc.3
      IL_001c:  ldloc.s    V_4
      IL_001e:  cgt
      IL_0020:  ldloc.3
      IL_0021:  ldloc.s    V_4
      IL_0023:  clt
      IL_0025:  sub
      IL_0026:  ret
    } // end of method T::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       27 (0x1b)
      .maxstack  7
      .locals init (int32 V_0,
               class [System.Runtime]System.Collections.IEqualityComparer V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4     0x9e3779b9
      IL_0007:  ldarg.1
      IL_0008:  stloc.1
      IL_0009:  ldarg.0
      IL_000a:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_000f:  ldloc.0
      IL_0010:  ldc.i4.6
      IL_0011:  shl
      IL_0012:  ldloc.0
      IL_0013:  ldc.i4.2
      IL_0014:  shr
      IL_0015:  add
      IL_0016:  add
      IL_0017:  add
      IL_0018:  stloc.0
      IL_0019:  ldloc.0
      IL_001a:  ret
    } // end of method T::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 StructsAsArrayElements01/T::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method T::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       42 (0x2a)
      .maxstack  4
      .locals init (object V_0,
               valuetype StructsAsArrayElements01/T V_1,
               valuetype StructsAsArrayElements01/T& V_2,
               class [System.Runtime]System.Collections.IEqualityComparer V_3)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  isinst     StructsAsArrayElements01/T
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_0028

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  StructsAsArrayElements01/T
      IL_0013:  stloc.1
      IL_0014:  ldloca.s   V_1
      IL_0016:  stloc.2
      IL_0017:  ldarg.2
      IL_0018:  stloc.3
      IL_0019:  ldarg.0
      IL_001a:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_001f:  ldloc.2
      IL_0020:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0025:  ceq
      IL_0027:  ret

      IL_0028:  ldc.i4.0
      IL_0029:  ret
    } // end of method T::Equals

    .method public hidebysig instance void 
            Set(int32 i) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 StructsAsArrayElements01/T::i
      IL_0007:  ret
    } // end of method T::Set

    .method public hidebysig virtual final 
            instance bool  Equals(valuetype StructsAsArrayElements01/T obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       18 (0x12)
      .maxstack  4
      .locals init (valuetype StructsAsArrayElements01/T& V_0)
      IL_0000:  ldarga.s   obj
      IL_0002:  stloc.0
      IL_0003:  ldarg.0
      IL_0004:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_0009:  ldloc.0
      IL_000a:  ldfld      int32 StructsAsArrayElements01/T::i
      IL_000f:  ceq
      IL_0011:  ret
    } // end of method T::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       30 (0x1e)
      .maxstack  4
      .locals init (object V_0,
               valuetype StructsAsArrayElements01/T V_1)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  isinst     StructsAsArrayElements01/T
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_001c

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  StructsAsArrayElements01/T
      IL_0013:  stloc.1
      IL_0014:  ldarg.0
      IL_0015:  ldloc.1
      IL_0016:  call       instance bool StructsAsArrayElements01/T::Equals(valuetype StructsAsArrayElements01/T)
      IL_001b:  ret

      IL_001c:  ldc.i4.0
      IL_001d:  ret
    } // end of method T::Equals

  } // end of class T

  .method public specialname static valuetype StructsAsArrayElements01/T[] 
          get_a() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     valuetype StructsAsArrayElements01/T[] '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01::a@11
    IL_0005:  ret
  } // end of method StructsAsArrayElements01::get_a

  .property valuetype StructsAsArrayElements01/T[]
          a()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype StructsAsArrayElements01/T[] StructsAsArrayElements01::get_a()
  } // end of property StructsAsArrayElements01::a
} // end of class StructsAsArrayElements01

.class private abstract auto ansi sealed '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01
       extends [System.Runtime]System.Object
{
  .field static assembly valuetype StructsAsArrayElements01/T[] a@11
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       35 (0x23)
    .maxstack  4
    .locals init (valuetype StructsAsArrayElements01/T[] V_0,
             valuetype StructsAsArrayElements01/T V_1)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldloc.1
    IL_0003:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Create<valuetype StructsAsArrayElements01/T>(int32,
                                                                                                                                   !!0)
    IL_0008:  dup
    IL_0009:  stsfld     valuetype StructsAsArrayElements01/T[] '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01::a@11
    IL_000e:  stloc.0
    IL_000f:  call       valuetype StructsAsArrayElements01/T[] StructsAsArrayElements01::get_a()
    IL_0014:  ldc.i4.0
    IL_0015:  ldelema    StructsAsArrayElements01/T
    IL_001a:  ldc.i4.s   27
    IL_001c:  call       instance void StructsAsArrayElements01/T::Set(int32)
    IL_0021:  nop
    IL_0022:  ret
  } // end of method $StructsAsArrayElements01::main@

} // end of class '<StartupCode$StructsAsArrayElements01>'.$StructsAsArrayElements01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file D:\code\FS\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net7.0\tests\EmittedIL\Misc\StructsAsArrayElements01_fs\StructsAsArrayElements01.res
